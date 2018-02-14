using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GuiFollow : MonoBehaviour
{
    public GameObject WorldObject;                                  //this is your object that you want to have the UI element hovering over
    public float rectifyX = 0.5f;                                  //met la GUI au bon endroit
    public float rectifyY = 0.5f;                                  //met la GUI au bon endroit
    public bool destroyIfNoWolrdObject = true;                     //détruit l'objet si il n'a pas de worldObject

    /// <summary>
    /// private
    /// </summary>
    [SerializeField]    private RectTransform UI_Element;           //this is the ui element
    private RectTransform CanvasRect;                               //first you need the RectTransform component of your canvas
    private GameObject Canvas;                                      //canvas
    private Camera Cam;                                             //main camera

    void Start()
    {
        Canvas = GameObject.FindGameObjectWithTag("mainCanvas");
        if (Canvas)
            CanvasRect = Canvas.GetComponent<RectTransform>();
        Cam = Camera.main;
    }

    void Update()
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        if (!WorldObject && destroyIfNoWolrdObject)
        {
            Destroy(gameObject);
            return;
        }
        if (!WorldObject)
            return;

        if (!CanvasRect)
        {
            Canvas = GameObject.FindGameObjectWithTag("mainCanvas");
            if (Canvas)
                CanvasRect = Canvas.GetComponent<RectTransform>();
            Cam = Camera.main;
        }

        Vector2 ViewportPosition = Cam.WorldToViewportPoint(WorldObject.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * rectifyX)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * rectifyY)));

        //now you can set the position of the ui element
        UI_Element.anchoredPosition = WorldObject_ScreenPosition;
    }
}
