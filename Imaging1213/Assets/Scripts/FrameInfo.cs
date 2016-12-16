using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ExternalDLL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point3F
    {
	    public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(Point3F v)
        {
            Vector3 vector = new Vector3();
            vector.x = v.x;
            vector.y = v.y;
            vector.z = v.z;
            return vector;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameInfo
    {
        public Point3F center;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst=5)]
        //public Point3F[] fingers;

        public IntPtr fingers;
        public int fingerNum;
    }
}
