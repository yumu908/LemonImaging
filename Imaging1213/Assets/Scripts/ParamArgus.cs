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


    // 手套 z方向 分界点 separate0 > separate1 > separate2;
    public float maxZ
    {
        get { return BackPos.z; }
    }

    public Color color0 = Color.green;

    public float midZ;


    public Color color1 = Color.blue;

    // 滑块Z轴活动范围
    [Range(0, 100)]
    public float midZScope;

    public float minZ
    {
        get { return FrontPos.z; }
    }

    public Color color2 = Color.yellow;

    //U3D中对应的X最大和最小值
    public float maxX;
    public float minX;


    public float xMid
    {
        get { return (left + right)/2; }
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
            return color0;
        }
        else if (z < midZ)
        {
            float t = (z - minZ) /(midZ - minZ);
            return Color.Lerp(color0, color1, t);
        }
        else if (z < maxZ)
        {
            float t = (z - midZ)/(maxZ - midZ);
            return Color.Lerp(color1, color2, t);
        }
        else
        {
            return color2;
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
        return  z <= back && z >= back - backScope;
    }

    public bool isInMiddle(float z)
    {
        return z > midZ - midZScope && z <= midZ;
    }
}
