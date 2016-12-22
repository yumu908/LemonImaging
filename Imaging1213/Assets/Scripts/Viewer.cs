using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Threading;

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
        private bool flag;

        public void Start()
        {
            queue = new Queue<FrameInfoKey>(30);
            flag = true;
            ThreadPool.QueueUserWorkItem(ToListener);
        }


        public FrameInfoKey OnUpdate()
        {
            //while (thread.ThreadState == ThreadState.Stopped)
            //{
            //    thread.Start();
            //}


            if (queue.Count > 0)
            {
               return queue.Dequeue();
            }

            return null;
        }



        private void ToListener(object obj)
        {
            StartListener(Collect, 8000);
            if (flag)
            {
                ThreadPool.QueueUserWorkItem(ToListener);
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
        }
       
    }

}

