using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExternalDLL;

public class Observer : MonoBehaviour
{
    #region public
    public bool debugMode;
    public GameObject hand;
    public Transform picTran;
    public Transform SlideTran;
    public Transform PlayerBoxTran;
    public FrameInfoKey test = new FrameInfoKey(Vector3.zero, new Vector3[5], 0);
    public bool testFlag = false;
    public ParamArgus argus = new ParamArgus();
    public MoviePlayer moviePlayer;
    public MeshRenderer btnPointer;

    public MeshRenderer slidePointer;
    public MeshRenderer graspPointer;

    #endregion

    #region private
    public Viewer viewer;
    private Material mat;
    private Vector3 slideOriginPos;
    private bool isEnterSlide;
    public bool Catched;
    public int prevFingerNum;

    public bool playboxFlag;

    private Vector3 PicTranPos;
    private float offset;

    private Animator animator;
    #endregion

    private Vector3 originCameraPos;

    private Quaternion orignCameraRot;

    private Vector3 cameraPos;


    private bool enterSlide;
    private float enterSlideZPos;
    private float enterSlideXPos;
    



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

        playboxFlag = false;
        
        prevFingerNum = 0;
        hand.SetActive(false);

        

        originCameraPos = Camera.main.transform.localPosition;
        orignCameraRot = Camera.main.transform.localRotation;
        cameraPos = transform.TransformPoint(originCameraPos);

        Vector3 temp = btnPointer.transform.localScale;
        temp.z = argus.MediaPointerZScale;
        btnPointer.transform.localScale = temp;

        temp = btnPointer.transform.localPosition;
        temp.z = -0.5f * argus.MediaPointerZScale;
        btnPointer.transform.localPosition = temp;



        temp = slidePointer.transform.localScale;
        Vector3 vector = argus.SlidePointerScale;
        temp.x = vector.x;
        temp.z = vector.z;
        slidePointer.transform.localScale = temp;

        temp = slidePointer.transform.localPosition;
        temp.z = -0.5f * argus.SlidePointerScale.z;
        slidePointer.transform.localPosition = temp;


        temp = graspPointer.transform.localScale;
        temp.z = 0.2f * argus.GraspPointerZScale;
        graspPointer.transform.localScale = temp;

        temp = graspPointer.transform.localPosition;
        temp.z = -0.1f * argus.GraspPointerZScale;
        graspPointer.transform.localPosition = temp;

        viewer = new Viewer();
        viewer.Start();
    }

    void Update()
    {
        FrameInfoKey key = viewer.OnUpdate(argus.meidaDecimals);
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
            Color c = argus.GetColor(key.center.z);
            mat.SetColor("_Color", c);

            bool isInPic = false;
            bool isCameraMove = true;


            if (center == Vector3.zero)
            {
                mat.SetColor("_OutlineColor", Color.gray);
                return;
            }

            mat.SetColor("_OutlineColor", new Color32(255, 100, 0, 255));
            float z = argus.GetZ(center.z);

            bool prevEnterSlide = enterSlide;
            enterSlide = argus.isInSlide(center.z);


            if (enterSlide)
            {
                float x = argus.GetX(center.x) - argus.GetX(argus.xMid);
                if (enterSlide != prevEnterSlide)
                {
                    enterSlideXPos = argus.BackPos.x;
                    enterSlideZPos = z;
                }

              

              
                if (Catched)
                {
                    transform.position = new Vector3(argus.BackPos.x, argus.BackPos.y, z);
                }
                else
                {
                    transform.position = new Vector3(argus.BackPos.x + x, argus.BackPos.y, enterSlideZPos);
                    SlideTran.transform.localPosition = slideOriginPos + x * 32 * Vector3.right;
                    isCameraMove = false;
                }


                if (debugMode)
                {
                    slidePointer.gameObject.SetActive(true);
                    slidePointer.material.SetColor("_LineColor", Color.red);
                }
 
            }
            else
            {
                if (argus.isInMedia(center.z))
                {

                    transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.MediaMiddle);


                    if (debugMode)
                    {
                        btnPointer.gameObject.SetActive(true);
                        btnPointer.material.SetColor("_LineColor", Color.red);
                    }

                    Media(key);
                }
                //else if (argus.isInBack(center.z))
                //{
                //    //transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.maxZ);
               
               
                //}
                //else if (argus.isInSlideBack(center.z))
                //{

                //    (false);
                //    transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);

                //}

                else if (argus.isInBottom(center.z))
                {
           
                    transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
                    isInPic = true;
                    if (argus.BackHit(viewer.ResultCode))
                    {
                        StartCoroutine(CatchAct(key));
                    }

                    if (Catched)
                    {
                    
                    }

                    if (debugMode)
                    {
                        graspPointer.gameObject.SetActive(true);
                        graspPointer.material.SetColor("_LineColor", Color.red);
                    }
                }
                else
                {
    
                    transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);

                    if (debugMode)
                    {
                        btnPointer.material.SetColor("_LineColor", Color.gray);
                        slidePointer.material.SetColor("_LineColor", Color.gray);
                        graspPointer.material.SetColor("_LineColor", Color.gray);
                    }


                }

            }
           
            if(!debugMode)
            {
                btnPointer.gameObject.SetActive(false);
                slidePointer.gameObject.SetActive(false);
                graspPointer.gameObject.SetActive(false);
            }

            Grasp(key);

            if (argus.BackHit(viewer.ResultCode) && !isInPic)
            {
                StartCoroutine(Recover(key));
            }


            cameraPos = Camera.main.transform.position;
            SetCamera(isCameraMove);
            prevFingerNum = key.fingerNum;
        }


    }

    private void Media(FrameInfoKey key)
    {
        if (argus.MeidaHit(viewer.ResultCode))
        {
            if (!Catched)
            {
                playboxFlag = !playboxFlag;
                if (playboxFlag)
                {
                    moviePlayer.Play(true);
                    moviePlayer.PlayAudio(true);
                }
                else
                {
                    moviePlayer.Stop();
                }
            }
        }
    }

    private void Grasp(FrameInfoKey key)
    {
        if (Catched)
        {
            float t = (PicTranPos.z - picTran.position.z) / argus.Depth;

            picTran.position = new Vector3(PicTranPos.x, PicTranPos.y, transform.position.z + Mathf.Clamp01(1 - 0.25f * t) * offset);
            picTran.localScale = new Vector3(1 + Mathf.Clamp01(2 * t), 1 + Mathf.Clamp01(2 * t), 1);
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
            for (float time = 0; time <= 1.0f; time += 2 * Time.deltaTime)
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
            if (!Camera.main.transform.parent)
            {
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.localPosition = originCameraPos;
                Camera.main.transform.localRotation = orignCameraRot;
                Camera.main.transform.localScale = Vector3.one;
            }

        }
        else
        {
            if (Camera.main.transform.parent)
            {
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localScale = 0.20f * Vector3.one;
                Vector3 vector = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(-20.54565f, vector.y, vector.z);
            }

        }
       

    }

    void OnDestroy()
    {
        if(viewer != null)
         viewer.OnStop();
    }
}