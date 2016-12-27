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

		public int resultCode;

        public int ResultCode
        {
            get { return resultCode; }
        }

        private Thread thread;

        public void Start()
        {
            queue = new Queue<FrameInfoKey>(30);
            flag = true;
			resultCode = 0;
            thread = new Thread(ToListener);
            thread.Start();
        }


		public FrameInfoKey OnUpdate(int n)
        {
            if (queue.Count > 0)
            {
				FrameInfoKey key = queue.Dequeue();
				int sign = key.fingerNum > 2 ? 1 : 0;
                int translate = (n << 1);
				int code = (1 << translate) - 1;
      
                resultCode = ((resultCode << 1) | sign) & code;
                Debug.LogError(key.fingerNum + "     " + sign + "    " + resultCode);
				return key;
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
            if (frameInfo.fingerNum < 0)
            {
                return;
            }

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
            thread.Abort();
        }
       
    }

}

