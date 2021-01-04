using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Animator cameraControl;

    public int cameraID;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            passCameraID();
        }
    }

    private void passCameraID()
    {
        UIManager.instance.fadeout();
        cameraControl.SetInteger("CameraID", cameraID);
    }
}
