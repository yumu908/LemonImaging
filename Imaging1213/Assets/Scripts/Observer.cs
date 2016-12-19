using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExternalDLL;
using UnityEditor;

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
    private bool Catched;
    private bool playFlag;
    private Vector3 PicTranPos;
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

        PicTranPos = picTran.position;
        Catched = false;
        playFlag = false;
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
            Debug.Log(center);

            bool flag = key.fingerNum > 0;
            Color c = argus.GetColor(transform.position.z);
            mat.SetColor("_Color", c);

            if (center == Vector3.zero)
            {
                mat.SetColor("_OutlineColor", Color.gray);
                return;
            }

            mat.SetColor("_OutlineColor", new Color32(255, 100, 0, 255));

            //if (isEnterSlide)
            //{
            //    float x = argus.GetX(center.x);
            //    transform.position = new Vector3(x, argus.BackPos.y, argus.BackPos.z);
            //    SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
            //    SetCamera(false);
            //    isEnterSlide = argus.isInBack(center.z);
            //}
            //else
            //{
            //    float z = argus.GetZ(center.z);
            //    if (argus.isInBack(center.z))
            //    {
            //        isEnterSlide = true;
            //        float x = argus.GetX(center.x);
            //        transform.position = new Vector3(x, argus.BackPos.y, argus.BackPos.z); 
            //        SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
            //        SetCamera(false);
            //    }
            //    else if(argus.isInLast(center.z))
            //    {
            //        isEnterSlide = false;
            //        transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
            //        SetCamera(true);
            //    }
            //    else if (center.z <= argus.front)
            //    {
                    
            //    }

            //    if (transform.position.z <= argus.minZ)
            //    {
            //        transform.position = argus.FrontPos;
            //    }

            //    if (argus.isInMiddle(transform.position.z))
            //    {
            //      // Media(key);
            //    }

            //}

            float z = argus.GetZ(center.z);
            if (argus.isInSlide(z))
            {
                float x = argus.GetX(center.x);
                transform.position = new Vector3(x, argus.BackPos.y, argus.slidePos);
                SlideTran.transform.localPosition = slideOriginPos + transform.position.x * 20 * Vector3.right;
                SetCamera(false);
            }
            else if (argus.isInMedia(z))
            {
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.mediaPos);
                Media(key);
            }
            else if (argus.isInBack(z))
            {
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, argus.maxZ);

                if (Catched)
                {
                    Catched = key.fingerNum <= 0;
                }
                else
                {
                    Catched = key.fingerNum > 0;
                }
            }
            else
            {
                bool active = z <= argus.slidePos + 1;
                SlideTran.root.gameObject.SetActive(active);
                transform.position = new Vector3(argus.FrontPos.x, argus.FrontPos.y, z);
        //        canvasRect.sizeDelta = canvasOriginSize;
                SetCamera(true);
            }

            Debug.LogError(Catched);
            Grasp(key);
            //  Grasp(key);
        }


    }

    private void Media(FrameInfoKey key)
    {
        if (playFlag)
        {
            playFlag = key.fingerNum <= 0;
        }
        else
        {
            playFlag = key.fingerNum > 0;
        }
 
        if (playFlag)
        {
            moviePlayer.Play(true);
            moviePlayer.PlayAudio(true);
        }
        else
        {
            moviePlayer.Stop();
        }
        //switch (key.fingerNum)
        //{
        //    case 1:
        //        moviePlayer.Play(true);
        //        break;
        //    case 2:
        //        moviePlayer.Play(false);
        //        break;
        //    case 3:
        //        moviePlayer.Stop();
        //        break;
        //    case 4:
        //        moviePlayer.PlayAudio(true);
        //        break;
        //    case 5:
        //        moviePlayer.PlayAudio(false);
        //        break;

        //}
    }

    private void Grasp(FrameInfoKey key)
    {
        if (Catched)
        {
            Catched = key.fingerNum <= 0;
        }

        if (Catched)
        {
            picTran.position = Camera.main.transform.position + argus.margin*Vector3.forward;
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
        float speed =  80 / offset;
        while (time <= 1)
        {
            time += speed * Time.deltaTime;
            picTran.position = Vector3.Lerp(picTran.position, PicTranPos, time);
            yield return null;
        }
        
    }

    private void SetCamera(bool hasParent)
    {
        if (hasParent)
        {
            Camera.main.transform.SetParent(transform, true);
            Camera.main.transform.localPosition = new Vector3(57.78f, -9.52f, 1.73f);
            Camera.main.transform.localRotation = Quaternion.Euler(131.78f, 88.8f, -0.90f);
            Camera.main.transform.localScale = Vector3.one;
        }
        else
        {
            Camera.main.transform.SetParent(null);
            Camera.main.transform.localPosition = new Vector3(-20.8f, 7.465f, 0);
            Camera.main.transform.localRotation = Quaternion.Euler(0.724f, 1.308f, 0);
            Camera.main.transform.localScale = 0.20f * Vector3.one;
        }
       

    }

    void OnDestroy()
    {
        viewer.OnStop();
    }
}