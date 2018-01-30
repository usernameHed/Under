using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipYjl4ODc0ZkFRZm8
see:
https://drive.google.com/file/d/0Byzet-SVq6ipUnJaWlV2WldTOE0/view
*/

public class IsOnScreen : MonoBehaviour
{
    public bool isOnScreen = false;
    [Range(0, 10f)]    public float timeOpti = 1.0f;
    [Range(0, 1f)]    public float margeOfError = 0.3f;

    /// <summary>
    /// variable privé
    /// </summary>

    private float timeToGo;

    /// <summary>
    /// test is l'objet courant est dans l'écran (avec une marge d'erreur)
    /// </summary>
    /// <returns>vrai ou faux</returns>
    bool testIfOutOfRange()
    {
        if (!Camera.main)
            return (false);
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > (0 - margeOfError) && screenPoint.x < (1 + margeOfError) && screenPoint.y > (0 - margeOfError) && screenPoint.y < (1 + margeOfError);
        if (!onScreen)
            return (true);
        return (false);
    }

    // Use this for initialization
    void Start ()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }
	
	//appelle la fonction pour définir si l'objet est dans l'écran
	void testOnScreen()
    {
        isOnScreen = !testIfOutOfRange();
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            testOnScreen();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
