using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHandler : MonoBehaviour
{
    public static MouseHandler instance;
    private Vector2 mousePos;

    [SerializeField] private LayerMask canFocus;

    [SerializeField] private bool hideCursor = false;
    [SerializeField] private Texture cursorImage;

    public Vector2 MousePos
    {
        get { return mousePos; }
        set { mousePos = value; }
    }

    void Awake()
    {
        instance = this;

        mousePos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Tracks mouse pos
        MousePos = new Vector2(worldPoint.x, worldPoint.y);    //Stores it in the MousePos variable
 
        if (PlayerStates.instance.currentConverstaionState == PlayerConverstaionStates.IN_CONVERSATION)
        {
            hideCursor = false;
        }
        else
        {
            hideCursor = true;
        }

        if (hideCursor == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    //private void OnGUI()
    //{
    //    // these are not actual positions but the change between last frame and now
    //    float h = 1000 * Input.GetAxisRaw("Joystick X") * Time.deltaTime;
    //    float v = 1000 * Input.GetAxisRaw("Joystick Y") * Time.deltaTime;

    //    // add the changes to the actual cursor position
    //    mousePos.x += h;
    //    mousePos.y += v;

    //    GUI.DrawTexture(new Rect(mousePos.x, Screen.height - mousePos.y, 10, 10), cursorImage);
    //}


}
