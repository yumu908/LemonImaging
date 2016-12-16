using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;



[StructLayout(LayoutKind.Sequential)]
struct Point3F
{
    [MarshalAs(UnmanagedType.R4)]
    public float x;

    [MarshalAs(UnmanagedType.R4)]
    public float y;

    [MarshalAs(UnmanagedType.R4)]
    public float z;
}

public class TestDll : MonoBehaviour {

    [DllImport("hand_detector")]
    private static extern IntPtr hd_init(string config_file);


    [DllImport("hand_detector")]
    private static extern int hd_get_fingers(IntPtr handle, IntPtr ir_img, IntPtr depth_img, int w, int h,
                                              ref Point3F hand_ceneter, Point3F[] fingertips_list, int f_list_len);

    [DllImport("hand_detector")]
    private static extern bool hd_release(IntPtr handle);

    // Use this for initialization
    void Start () {
	
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnGUI()
    {
        if (GUILayout.Button("Invoke C++ Dll"))
        {
            IntPtr ptr = hd_init(null);

            Point3F hand_center = new Point3F();
            Point3F[] fingertips = new Point3F[5];
            IntPtr fingersPtr = Marshal.UnsafeAddrOfPinnedArrayElement(fingertips, 0);

            IntPtr ir_img = Marshal.AllocHGlobal(2); //IntPtr.Zero;
            IntPtr depth_img = Marshal.AllocHGlobal(2);  //IntPtr.Zero;
            int fingerNum = hd_get_fingers(ptr, ir_img, depth_img, 224, 171, ref hand_center, fingertips, 5);
            Debug.Log("fingernum : " + fingerNum);

            // 释放
            Marshal.FreeHGlobal(ir_img);
            Marshal.FreeHGlobal(depth_img);

            Debug.Log(hd_release(ptr));

   
          
        }
    }
}


