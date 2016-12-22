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
    public bool Catched;
    public int prevFingerNum;

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
        Catched = false;

        prevFingerNum = 0;
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
            if(center != Vector3.zero)
            Debug.Log(center + "   " + key.fingerNum) ;

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
            if (argus.isInSlide(center.z))
            {
                SlideTran.root.gameObject.SetActive(true);
                float x = argus.GetX(center.x);
                transform.position = new Vector3(x, argus.BackPos.y, argus.SlidePos);
                SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
                SetCamera(false);
            }
            else if (argus.isInMedia(center.z))
            {
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.MediaPos);
                Media(key);
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


    }

    private void Media(FrameInfoKey key)
    {
        if(prevFingerNum > 2  && key.fingerNum <= 2)
        {
            moviePlayer.Play(true);
            moviePlayer.PlayAudio(true);
        }
        else if(prevFingerNum <= 2 && key.fingerNum > 2)
        {
            moviePlayer.Stop();
        }
    }

    private void Grasp(FrameInfoKey key)
    {
        if (Catched && key.fingerNum <= 0)
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

            picTran.localScale = Vector3.one;
        }
 
    }

    private IEnumerator CatchAct(FrameInfoKey key)
    {
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
            Camera.main.transform.localPosition = new Vector3(-20.63f, 5.53f, 4.31f);
            Camera.main.transform.localRotation = Quaternion.Euler(-2.03f, 0.21f, 0);
            Camera.main.transform.localScale = 0.20f * Vector3.one;
        }
       

    }

    void OnDestroy()
    {
        if(viewer != null)
         viewer.OnStop();
    }
}