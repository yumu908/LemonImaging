using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Threading;
using UGUIExtension;

namespace ExternalDLL
{
    [Serializable]
    public class Viewer
    {
        [DllImport("HandPosRec2")]
        private static extern IntPtr GetFrontFrame();

        [DllImport("HandPosRec2")]
        private static extern bool StartListener(Callback callback, long duration);


        [DllImport("HandPosRec2", EntryPoint = "Stop")]
        private static extern bool DoStop();

        public delegate void Callback(FrameInfo frameInfo);

        public Queue<FrameInfoKey> queue;
        private Thread thread;
        private bool flag;

        public void Start()
        {
            queue = new Queue<FrameInfoKey>(30);
            flag = true;
            thread = new Thread(ToListener);
            thread.Start();
        }


        public FrameInfoKey OnUpdate()
        {
            if (queue.Count > 0)
            {
               return queue.Dequeue();
            }

            return null;
        }



        private void ToListener()
        {
            while (flag)
            {
                StartListener(Collect, 5000);
                Thread.Sleep(10);
            }
        }

        public void Collect(FrameInfo frameInfo)
        {
            Act direct = Act.ForwardToPlay;
            //if (frameInfo.fingerNum < 0 || frameInfo.center == Vector3.zero)
            //{
            //    return;
            //}

            int size = Marshal.SizeOf(typeof(Point3F));
            Vector3[] points = new Vector3[5];

            for (int i = 0; i < points.Length; i++)
            {
                IntPtr ptr = (IntPtr)((UInt32) frameInfo.fingers + i * size);
                points[i] = (Point3F) Marshal.PtrToStructure(ptr, typeof(Point3F));
            }

            FrameInfoKey key = new FrameInfoKey(frameInfo.center, points, frameInfo.fingerNum);

            queue.Enqueue(key);
        }

        public void OnStop()
        {
            flag = false;
            Thread.Sleep(10);
            if (thread.IsAlive)
            {
                thread.Abort();
            }
        }
       
    }

}

