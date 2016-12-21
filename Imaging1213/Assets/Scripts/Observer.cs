using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExternalDLL;

public class Observer : MonoBehaviour
{
    #region public
    public GameObject hand;
    public Transform picTran;
    public Transform SlideTran;
    public Vector3 test = Vector3.zero;
    public bool testFlag = false;
    public ParamArgus argus = new ParamArgus();
    public MoviePlayer moviePlayer;

    #endregion

    #region private
    private Viewer viewer;
    private Material mat;
    private Vector3 slideOriginPos;
    private bool isEnterSlide;

    public int OpCode;

    public bool inGraspArea;

    public VrState state = VrState.Normal;

    private Vector3 PicTranPos;
    private float offset;

    private Animator animator;
    #endregion


    private void Sample(Animation anim)
    {
        anim.Sample();

    }

    void Start()
    {
        mat = hand.GetComponentInChildren<MeshRenderer>().material;
        slideOriginPos = SlideTran.localPosition;
        isEnterSlide = false;
        offset = picTran.position.z - argus.maxZ;
        PicTranPos = picTran.position;
        animator = picTran.GetComponent<Animator>();

        OpCode = 0;

        inGraspArea = false;


        hand.SetActive(false);
        viewer = new Viewer();
        viewer.Start();
    }
    void Update()
    {
        FrameInfoKey key = viewer.OnUpdate();
        if (key != null)
        {
            if (!hand.activeSelf)
            {
                hand.SetActive(true);
            }

            Vector3 center = key.center;
            Debug.LogWarning(center);

            bool flag = key.fingerNum > 0;
            Color c = argus.GetColor(transform.position.z);
            mat.SetColor("_Color", c);

            if (center == Vector3.zero)
            {
                mat.SetColor("_OutlineColor", Color.gray);
                return;
            }

            mat.SetColor("_OutlineColor", new Color32(255, 100, 0, 255));
            float z = argus.GetZ(center.z);
            int sign = key.fingerNum > 0 ? 1 : 0;
            bool translateFlag = ((OpCode & 0x1) ^ sign) == 1;
            if (translateFlag)
            {
                OpCode = ((OpCode << 1) | sign) & 0x07;
            }


            if (argus.isInSlide(z))
            {
                if (OpCode == 5)
                {
                    if(state == VrState.SlideLocked)
                    {
                        state = VrState.Normal;
                    }
                    else
                    {
                        state = VrState.SlideLocked;
                    }
                    OpCode = 0;
                }

             
            }
            else if (argus.isInMedia(z))
            {
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.mediaPos);

                if (OpCode == 5)
                {
                    if (state == VrState.PlayBoxLocked)
                    {
                        state = VrState.Normal;
                    }
                    else
                    {
                        state = VrState.PlayBoxLocked;
                    }
                    OpCode = 0;
                }

               
            }
            else if (argus.isInBack(z))
            {
                if (OpCode == 5)
                {
                    if (state == VrState.Normal)
                    {
                        state = VrState.GraspLocked;
                        StartCoroutine(CatchAct(key));
                    }
                    OpCode = 0;
                }

                inGraspArea = true;
            }
            else
            {
                bool active = z < argus.slidePos + argus.slideScope + 2;
                SlideTran.root.gameObject.SetActive(active);
                inGraspArea = false;
            }



            switch (state)
            {
                case VrState.PlayBoxLocked:
                    Media(true);
                    return;
                case VrState.GraspLocked:
                    if (inGraspArea)
                    {
                        transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.maxZ);
                    }
                    else
                    {
                        if (OpCode == 5)
                        {
                            Grasp(false);
                            state = VrState.Normal;
                            OpCode = 0;
                            return;
                        }
                        Grasp(true);
                    }

                    return;

                case VrState.SlideLocked:
                    float x = argus.GetX(center.x);
                    Debug.LogError(x);
                    transform.position = new Vector3(x, argus.BackPos.y, argus.slidePos);
                    SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
                    SetCamera(false);
                    return;

                case VrState.Normal:

                default:

                    Media(false);
                    Grasp(false);

                    transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
                    SetCamera(true);
                    break;
            }
        }
    }

    private void Media(bool flag)
    {
        if (flag)
        {
            moviePlayer.Play(true);
            moviePlayer.PlayAudio(true);
        }
        else
        {
            moviePlayer.Stop();
        }
    }

    private void Grasp(bool flag)
    {
        if (flag)
        {
            float t = (PicTranPos.z - picTran.position.z) / argus.Depth;
            picTran.position = new Vector3(PicTranPos.x, PicTranPos.y, transform.position.z + (1- 3.03f * t) * offset);
            picTran.localScale = new Vector3(1 +  4.8f * t, 1 + 4.8f * t, 1);
        }
        else
        {
            StartCoroutine(Recover());
        }
    }

    private IEnumerator Recover()
    {
        float time = 0;

        float offset = (PicTranPos - picTran.position).z;
        if(Mathf.Approximately(offset, 0))
        {
            yield break;
        }

        float speed = 100 / offset;
        Vector3 pos = picTran.position;
        while (time <= 1)
        {
            time += speed * Time.deltaTime;
            picTran.position = Vector3.Lerp(pos, PicTranPos, time);
            yield return null;
        }

        picTran.localScale = Vector3.one;
    }

    private IEnumerator CatchAct(FrameInfoKey key)
    {
        Vector3 pos = picTran.position;
        for (float time = 0; time <= 0.6f; time += Time.deltaTime)
        {
            picTran.Translate(0, 0, -1.0f, Space.World);
            picTran.position = Vector3.Lerp(pos, pos - 10 * Vector3.forward, time);
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetCamera(bool hasParent)
    {
        if (hasParent)
        {
            Camera.main.transform.SetParent(transform, true);
            Camera.main.transform.localPosition = new Vector3(39.46f, -18f, 0.53f);
            Camera.main.transform.localRotation = Quaternion.Euler(130.75f, 91.52f, 1.14f);
            Camera.main.transform.localScale = Vector3.one;
        }
        else
        {
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(-21.40f, 4.82f, 2.9f);
            Camera.main.transform.localRotation = Quaternion.Euler(-0.31f, -0.483f, 0);
            Camera.main.transform.localScale = 0.20f * Vector3.one;
        }
       

    }

    void OnDestroy()
    {
        viewer.OnStop();
    }
}