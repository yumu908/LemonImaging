using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class ParamArgus
{
    /// <summary>
    /// U3D 中手套的前后边界位置
    /// </summary>
    public Vector3 FrontPos;
    public Vector3 BackPos;

    /// <summary>
    /// 深度相机 前后左右边界
    /// </summary>
    public float left;
    public float right;
    public float front;
    public float back;

    // 滑块Z轴活动范围
    //[Range(0, 100)]
    //public float backScope;

    //间隔
    public float margin;

    // 手套 z方向 分界点 separate0 > separate1 > separate2;
    public float maxZ
    {
        get { return BackPos.z; }
    }

    public float minZ
    {
        get { return FrontPos.z; }
    }

    public Color frontColor = Color.green;

    public float mediaPos;
    public float mediaScope;
    public Color mediaColor = Color.blue;


    public float slidePos;
    public float slideScope;
    public Color slideColor = Color.yellow;

    public Color backColor = Color.cyan;
  

    //U3D中对应的X最大和最小值
    public float maxX;
    public float minX;

    [Range(1, 3)]
    public int meidaDecimals = 1;

    [Range(1, 3)]
    public int backDecimals = 1;



    public float xMid
    {
        get
        {
            return (right + left) / 2;
        }
    }

    public float Depth
    {
        get { return maxZ - minZ; }
    }

    public float dScope
    {
         get { return (maxZ - minZ) /(back - front); }
    }

    public float dIntercept
    {
        get { return maxZ - dScope * back; }
    }

    public float wScope
    {
        get { return (maxX - minX) / (right - left); }
    }

    public float wIntercept
    {
        get { return maxX - wScope * right; }
    }

    public Color GetColor(float z)
    {

        if (z < front)
        {
            return frontColor;
        }
        else if (z < mediaPos)
        {
            float t = (z - front) / (mediaPos - front);
            return Color.Lerp(frontColor, mediaColor, t);
        }
        else if (z < slidePos)
        {
            float t = (z - mediaPos) / (slidePos - mediaPos);
            return Color.Lerp(mediaColor, slideColor, t);
        }
        else if (z < back)
        {
            float t = (z - slidePos)/(back - slidePos);
            return Color.Lerp(slideColor, backColor, t);
        }
        else
        {
            return backColor;
        }
      
    }


    public float GetZ(float z)
    {
        float result = dScope * z + dIntercept;
        return  Mathf.Clamp(result, minZ, maxZ);
    }

    public float GetX(float x)
    {
        float result = wScope * x + wIntercept;
        return Mathf.Clamp(result , minX, maxX);
    }

    //public bool isInBack(float z)
    //{
    //    return z <= back && z >= back - backScope;
    //}

    public bool isInSlide(float z)
    {
        return z >= slidePos - slideScope && z < slidePos;
    }

    public bool isInSlideBack(float z)
    {
        return z >= slidePos && z < back - margin;
    }

    public bool isInBottom(float z)
    {
        return z >= back - margin;
    }

    public bool isInMedia(float z)
    {
        return z >= mediaPos - mediaScope && z <= mediaPos;
    }


    public float MediaPos
    {
        get
        {
            return GetZ(mediaPos);
        }
    }

    public float SlidePos
    {
        get
        {
            return GetZ(slidePos);
        }
    }

    public bool MeidaHit(int code)
    {
        int opt = 1 << meidaDecimals - 1;
        return code == (opt << meidaDecimals);
    }

    public bool BackHit(int code)
    {
        int opt = 1 << backDecimals - 1;
        return code == (opt << backDecimals);
    }


    public float MediaPointerZScale
    {
        get { return 0.1f * (GetZ(mediaPos) - GetZ(mediaPos - mediaScope)); }
    }

    public Vector3 SlidePointerCenter
    {
        get
        {
            return new Vector3(FrontPos.x, FrontPos.y, GetZ(slidePos - 0.5f * slideScope));
        }
    }

    public Vector3 SlidePointerScale
    {
        get
        {
            float xScale = GetX(right) - GetX(left);
            float zScale = (GetZ(slidePos) - GetZ(slidePos - slideScope));
            return new Vector3(30 * xScale, 1, 2 * zScale);
        }
    }


    public float GraspPointerZScale
    {
        get
        {
            return GetZ(back) - GetZ(back - margin);
        }
    }

    public float MediaMiddle
    {
        get
        {
            return  0.5f * (GetZ(mediaPos) + GetZ(mediaPos - mediaScope));
        }

    }
}
