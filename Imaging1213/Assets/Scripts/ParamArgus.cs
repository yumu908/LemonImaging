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
    [Range(0, 100)]
    public float backScope;

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

    public float xMid
    {
        get
        {
            return (right + left) / 2;
        }
    }

    public float Depth
    {
        get { return back - front; }
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

        if (z < minZ)
        {
            return frontColor;
        }
        else if (z < mediaPos)
        {
            float t = (z - minZ) /(mediaPos - minZ);
            return Color.Lerp(frontColor, mediaColor, t);
        }
        else if (z < slidePos)
        {
            float t = (z - mediaPos) / (slidePos - mediaPos);
            return Color.Lerp(mediaColor, slideColor, t);
        }
        else if (z < maxZ)
        {
            float t = (z - slidePos)/(maxZ - slidePos);
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
        float result = wScope * (x - xMid) + wIntercept;
        return Mathf.Clamp(result - BackPos.x, minX, maxX);
    }

    public bool isInBack(float z)
    {
        return  z <= maxZ && z >= maxZ - backScope;
    }

    public bool isInSlide(float z)
    {
        return z >= slidePos - slideScope && z <= slidePos;
    }

    public bool isInMedia(float z)
    {
        return z >= mediaPos - mediaScope && z <= mediaPos;
    }

    
}
