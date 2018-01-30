using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fading : MonoBehaviour
{
    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }


    public Texture2D fadeOutTexture;    //texture that will overlay the screen. This can be a black image or loading graphic
    public float fadeSpeed = 0.8f;      //the fading speed

    private int drawDepth = -1000;      //texture order in dray hierarchy: a low number means renders on top;
    public float alpha = 1.0f;         //texture's alpha value (0-1)
    private int fadeDir = -1;           //the direction to fade: in = -1 or out = 1;

    [SerializeField] private Texture2D [] spriteWorld;    //les 6 différents sprite des worlds

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;                                  //fade out/in the alpha value using direction, speed and time to convert operation in second
        alpha = Mathf.Clamp01(alpha);                                                   //force (clamp) the number between 0 & 1

        //set color of GUI
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);            //set the alpha value  
        GUI.depth = drawDepth;                                                          //make the black texture render on top
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);   //draw texture to fit entire screen;
    }

    //set fadeDir -1, 1
    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);             //retur time speed to fade
    }

    public void setupWorld(int worldId)
    {
        if (spriteWorld.Length <= worldId || !spriteWorld[worldId])
            return;
        fadeOutTexture = spriteWorld[worldId];
    }

    /*//called when a level is loaded;
    public void OnLevelWasLoaded()
    {
        //alpha = 1;
        BeginFade(-1);
    }*/

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        BeginFade(-1);
    }
}
