using System;
using UnityEngine;
using System.Collections;

namespace ExternalDLL
{
    [Serializable]
    public class FrameInfoKey
    {
        public Vector3 center;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst=5)]
        public Vector3[] fingers;
        public int fingerNum;

        public FrameInfoKey(Vector3 center, Vector3[] fingers, int fingerNum)
        {
            this.center = center;
            this.fingers = fingers;
            this.fingerNum = fingerNum;
        }
    }
}

