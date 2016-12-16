using UnityEngine;
using System.Collections;
using UGUIExtension;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollisonChecker : MonoBehaviour
{

    private float prev = 2;
    public Transform Parent;


    private bool enterSlide;
    private bool enterPlayBtn;

    private bool enterStopBtn;




	// Use this for initialization
	void Start ()
	{

	    enterPlayBtn = false;
	    enterStopBtn = false;
	    enterSlide = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.tag == "SlidePanel")
        //{
        //    player.SetFlag(true);
        //}
    }


    void OnCollisionStay(Collision other)
    {
       
    }


    void OnCollisionExit(Collision other)
    {
        //if (other.gameObject.tag == "SlidePanel")
        //{
        //    player.SetFlag(false);
        //}
    }


    void OnTriggerEnter(Collider other)
    {
        var player = GameObject.FindGameObjectWithTag("MoviePlayer").GetComponent<MoviePlayer>();
        switch (other.gameObject.tag)
        {
            case "SlidePanel":
                enterSlide = true;
                break;
            case "PlayBtn":
                enterPlayBtn = true;
                break;
            case "StopBtn":
                enterStopBtn = true;
                break;
            default:
                break;
        }

        prev = Parent.position.x;
    }


    void OnTriggerStay(Collider other)
    {
        //Debug.LogWarning(other.gameObject.name);
        //var player = GameObject.FindGameObjectWithTag("MoviePlayer").GetComponent<MoviePlayer>();
        //switch (other.gameObject.tag)
        //{
        //    case "SlidePanel":
        //        var comp = other.gameObject.GetComponent<ImageSlidePanel>();
        //        comp.VrDrag(Parent.position.x - prev, 5);
        //        break;
        //    case "PlayBtn":
        //        player.Play();
        //        break;
        //    case "StopBtn":
        //        player.Stop();
        //        break;

        //    case "AudioPlay":
        //        player.Stop();
        //        break;

        //    default:
        //        break;
        //}

        prev = Parent.position.x;
    }


    void OnTriggerExit(Collider other)
    {

        prev = Parent.position.x;
    }
}
