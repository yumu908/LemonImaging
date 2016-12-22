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
    public Transform PlayerBoxTran;
    public FrameInfoKey test = new FrameInfoKey(Vector3.zero, new Vector3[5], 0);
    public bool testFlag = false;
    public ParamArgus argus = new ParamArgus();
    public MoviePlayer moviePlayer;

    #endregion

    #region private
    private Viewer viewer;
    private Material mat;
    private Vector3 slideOriginPos;
    private bool isEnterSlide;
<<<<<<< HEAD
    public bool Catched;
    public int prevFingerNum;
=======

    public int OpCode;

    public bool inGraspArea;

    public VrState state = VrState.Normal;
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f

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
<<<<<<< HEAD
        Catched = false;

        prevFingerNum = 0;
=======

        OpCode = 0;

        inGraspArea = false;


>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
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
<<<<<<< HEAD
            if(center != Vector3.zero)
            Debug.Log(center + "   " + key.fingerNum) ;
=======
            Debug.LogWarning(center);
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f

            bool flag = key.fingerNum > 0;
            Color c = argus.GetColor(transform.position.z);
            mat.SetColor("_Color", c);

            bool isInPic = false;

            if (center == Vector3.zero)
            {
                mat.SetColor("_OutlineColor", Color.gray);
                return;
            }

            mat.SetColor("_OutlineColor", new Color32(255, 100, 0, 255));
            float z = argus.GetZ(center.z);
<<<<<<< HEAD
            if (argus.isInSlide(center.z))
            {
                SlideTran.root.gameObject.SetActive(true);
                float x = argus.GetX(center.x);
                transform.position = new Vector3(x, argus.BackPos.y, argus.SlidePos);
                SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
                SetCamera(false);
=======
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

             
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
            }
            else if (argus.isInMedia(center.z))
            {
<<<<<<< HEAD
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.MediaPos);
                Media(key);
=======
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

               
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
            }
            else if (argus.isInBack(center.z))
            {
                //transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.maxZ);
               
               
            }
            else if (argus.isInSlideBack(center.z))
            {
                SlideTran.root.gameObject.SetActive(false);
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
                SetCamera(true);
            }
            
            else if (argus.isInBottom(center.z))
            {
<<<<<<< HEAD
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.maxZ);
                StartCoroutine(CatchAct(key));
                isInPic = true;
            }
            else
            {
                SlideTran.root.gameObject.SetActive(true);
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
                SetCamera(true);

            }

            if(!isInPic)
               Grasp(key);
            prevFingerNum = key.fingerNum;
        }
=======
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

>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f


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
<<<<<<< HEAD
        if(prevFingerNum > 2  && key.fingerNum <= 2)
=======
        if (flag)
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
        {
            moviePlayer.Play(true);
            moviePlayer.PlayAudio(true);
        }
        else if(prevFingerNum <= 2 && key.fingerNum > 2)
        {
            moviePlayer.Stop();
        }
    }

    private void Grasp(bool flag)
    {
<<<<<<< HEAD
        if (Catched && key.fingerNum <= 0)
=======
        if (flag)
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
        {
            float t = (PicTranPos.z - picTran.position.z) / argus.Depth;
            picTran.position = new Vector3(PicTranPos.x, PicTranPos.y, transform.position.z + (1- 2.03f * t) * offset);
            picTran.localScale = new Vector3(1 +  3.8f * t, 1 + 3.8f * t, 1);
        }
        else if (prevFingerNum <= 0 && key.fingerNum > 0)
        {
            StartCoroutine(Recover(key));
        }
    }

    private IEnumerator Recover(FrameInfoKey key)
    {
<<<<<<< HEAD
        if (Catched)
        {
            Catched = false;
            float time = 0;

            float offset = (PicTranPos - picTran.position).z;
            float speed = 100 / offset;
            Vector3 pos = picTran.position;
            while (time <= 1)
            {
                time += speed * Time.deltaTime;
                picTran.position = Vector3.Lerp(pos, PicTranPos, time);
                yield return null;
            }
=======
        float time = 0;

        float offset = (PicTranPos - picTran.position).z;
        if(Mathf.Approximately(offset, 0))
        {
            yield break;
        }
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f

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
<<<<<<< HEAD
        if (!Catched)
        {
            Catched = true;
            Vector3 pos = picTran.position;
            float z = argus.maxZ + (pos.z - argus.maxZ) * 0.60f;
            Vector3 target = new Vector3(pos.x, pos.y, z);
            for (float time = 0; time <= 1.0f; time += Time.deltaTime)
            {
                picTran.position = Vector3.Lerp(pos, target, time);
                yield return new WaitForEndOfFrame();
            }
=======
        Vector3 pos = picTran.position;
        for (float time = 0; time <= 0.6f; time += Time.deltaTime)
        {
            picTran.Translate(0, 0, -1.0f, Space.World);
            picTran.position = Vector3.Lerp(pos, pos - 10 * Vector3.forward, time);
            yield return new WaitForEndOfFrame();
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
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
<<<<<<< HEAD
            Camera.main.transform.localPosition = new Vector3(-20.63f, 5.53f, 4.31f);
            Camera.main.transform.localRotation = Quaternion.Euler(-2.03f, 0.21f, 0);
=======
            Camera.main.transform.localPosition = new Vector3(-21.40f, 4.82f, 2.9f);
            Camera.main.transform.localRotation = Quaternion.Euler(-0.31f, -0.483f, 0);
>>>>>>> c75ddba994dfc6002a055446e6691f4a56a6a79f
            Camera.main.transform.localScale = 0.20f * Vector3.one;
        }
       

    }

    void OnDestroy()
    {
        if(viewer != null)
         viewer.OnStop();
    }
}