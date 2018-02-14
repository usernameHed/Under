using UnityEngine;

/// <summary>
/// test si l'objet est dans l'écran de la main caméra
/// </summary>
public class IsOnScreen : MonoBehaviour
{
    public bool isOnScreen = false;
    public float timeOpti = 1.0f;
    public float margeOfError = 0.3f;

    private float timeToGo;

    /// <summary>
    /// test is l'objet courant est dans l'écran (avec une marge d'erreur)
    /// </summary>
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

    private void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            testOnScreen();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
