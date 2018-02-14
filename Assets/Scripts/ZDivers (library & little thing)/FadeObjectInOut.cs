/*
	FadeObjectInOut.cs
 	Hayden Scott-Baron (Dock) - http://starfruitgames.com
 	6 Dec 2012 
 
	This allows you to easily fade an object and its children. 
	If an object is already partially faded it will continue from there. 
	If you choose a different speed, it will use the new speed. 
 
	NOTE: Requires materials with a shader that allows transparency through color.  
*/

using UnityEngine;
using System.Collections;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipcmtaTS1RODVJb28%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipcmtaTS1RODVJb28
see:
https://drive.google.com/file/d/0Byzet-SVq6ipQWJ2X18xT3h3Skk/view
*/
/// <summary>
/// fade In / out l'objet auquel il est attaché. Si la variable fadeOut change: fade l'objet.
///- attention, si l'objet est un Canvas, isCanvas doit être true
/// </summary>

public class FadeObjectInOut : MonoBehaviour
{
    // publically editable speed
    public bool fadeOut = false;
    
    [Tooltip("required canvas group !")]        public bool isCanvas = false;               //détermine si l'objet est un canvas ou pas !

    [Space(10)]
    [Tooltip("délai au début")]                 public float fadeDelay = 0.0f;              //délai au début avant le fade
    [Tooltip("temps de fade")]                  public float fadeTime = 0.5f;
    public float fadeTimeIn = 0.5f;
    public float fadeTimeOutt = 0.5f;
    public bool fadeInOnStart = false;
    public bool fadeOffOnStart = false;
    public bool fadeOutOnStart = false;
    [Tooltip("Désactive si fade out")]          public bool disableOnFadeOut = false;       //désactive si l'objet est fade out

    // store colours
    private Color[] colors;
    private bool lastFade = false;
    private CanvasGroup CG;
    private bool logInitialFadeSequence = false;

    // allow automatic fading on the start of the scene
    private void Awake()
    {
        if (isCanvas)
            CG = gameObject.GetComponent<CanvasGroup>();
        
    }

    void Start()
    {
        if (fadeOffOnStart && !isCanvas)
        {
            StartCoroutine("FadeSequence2", fadeTime);
        }
    }
    /*IEnumerator Start()
    {
        //yield return null; 
        yield return new WaitForSeconds(fadeDelay);

        if (fadeInOnStart)
        {
            logInitialFadeSequence = true;
            FadeIn();
        }

        if (fadeOutOnStart)
        {
            FadeOut(fadeTime);
        }
    }*/

    IEnumerator waitBeforeFade()
    {
        yield return new WaitForSeconds(fadeDelay);
        if (fadeInOnStart)
        {
            logInitialFadeSequence = true;
            FadeIn();
        }

        if (fadeOutOnStart)
        {
            FadeOut(fadeTime);
        }

        if (fadeOffOnStart && !isCanvas)
        {
            StartCoroutine("FadeSequence2", fadeTime);
        }
            
    }

    private void OnEnable()
    {
        if (isCanvas)
        {
            CG.alpha = 1;
            if (fadeOffOnStart)
                CG.alpha = 0;
        }

        StartCoroutine(waitBeforeFade());
    }

    // check the alpha value of most opaque object
    float MaxAlpha()
    {
        float maxAlpha = 0.0f;
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererObjects)
        {
            maxAlpha = Mathf.Max(maxAlpha, item.material.color.a);
        }
        return maxAlpha;
    }

    IEnumerator FadeSequenceCanvas(float fadingOutTime)
    {
        if (CG)
        {
            CG.alpha += fadingOutTime / 10;
            while (CG.alpha > 0 && CG.alpha < 1)
            {
                yield return new WaitForSeconds(fadeTime / 100.0f);
                CG.alpha += fadingOutTime / 50;                
            }
            if (disableOnFadeOut)
                gameObject.SetActive(false);
        }
    }

    // fade sequence
    IEnumerator FadeSequence(float fadingOutTime)
    {
        // log fading direction, then precalculate fading speed as a multiplier
        bool fadingOut = (fadingOutTime < 0.0f);
        float fadingOutSpeed = 1.0f / fadingOutTime;

        // grab all child objects
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }

        // make all objects visible
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].enabled = true;
        }


        // get current max alpha
        float alphaValue = MaxAlpha();


        // This is a special case for objects that are set to fade in on start. 
        // it will treat them as alpha 0, despite them not being so. 
        if (logInitialFadeSequence && !fadingOut)
        {
            alphaValue = 0.0f;
            logInitialFadeSequence = false;
        }

        // iterate to change alpha value 
        while ((alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut))
        {
            alphaValue += Time.deltaTime * fadingOutSpeed;
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
                newColor.a = Mathf.Min(newColor.a, alphaValue);
                newColor.a = Mathf.Clamp(newColor.a, 0.0f, 1.0f);
                rendererObjects[i].material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // turn objects off after fading out
        if (fadingOut)
        {
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                rendererObjects[i].enabled = false;
            }
        }


        Debug.Log("fade sequence end : " + fadingOut);
    }

    // fade sequence
    IEnumerator FadeSequence2(float fadingOutTime)
    {
        // log fading direction, then precalculate fading speed as a multiplier
        bool fadingOut = (fadingOutTime < 0.0f);
        float fadingOutSpeed = 1.0f / fadingOutTime;

        // grab all child objects
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }

        // make all objects visible
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].enabled = true;
        }


        // get current max alpha
        float alphaValue = MaxAlpha();


        // This is a special case for objects that are set to fade in on start. 
        // it will treat them as alpha 0, despite them not being so. 
        if (logInitialFadeSequence && !fadingOut)
        {
            alphaValue = 0.0f;
            logInitialFadeSequence = false;
        }

        // iterate to change alpha value 
        while ((alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut))
        {
            alphaValue += Time.deltaTime * fadingOutSpeed;
            alphaValue = 1.0f;
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
                //newColor.a = Mathf.Min(newColor.a, alphaValue);
                //newColor.a = Mathf.Clamp(newColor.a, 1.0f, 1.0f);
                newColor.a = 0.0f;
                rendererObjects[i].material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // turn objects off after fading out
        if (fadingOut)
        {
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                rendererObjects[i].enabled = false;
            }
        }


        Debug.Log("fade sequence end : " + fadingOut);
    }

    public void fadeReset()
    {
        StopAllCoroutines();
        // grab all child objects
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }

        // make all objects visible
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].enabled = true;
        }


        for (int i = 0; i < rendererObjects.Length; i++)
        {
            Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
            //newColor.a = Mathf.Min(newColor.a, alphaValue);
            //newColor.a = Mathf.Clamp(newColor.a, 1.0f, 1.0f);
            newColor.a = 1.0f;
            rendererObjects[i].material.SetColor("_Color", newColor);
        }
    }


    void FadeIn()
    {
        FadeIn(fadeTime);
    }

    void FadeOut()
    {
        FadeOut(fadeTime);
    }

    void FadeIn(float newFadeTime)
    {
        StopAllCoroutines();
        if (!isCanvas)
            StartCoroutine("FadeSequence", newFadeTime);
        else
            StartCoroutine("FadeSequenceCanvas", fadeTimeIn);
    }

    void FadeOut(float newFadeTime)
    {
        StopAllCoroutines();
        if (!isCanvas)
            StartCoroutine("FadeSequence", -newFadeTime);
        else
            StartCoroutine("FadeSequenceCanvas", -fadeTimeOutt);
    }

    public void changeFade(bool fade)
    {
        fadeOut = fade;
    }

    public void changeFade()
    {
        fadeOut = !fadeOut;
    }

    void Update()
    {
        if (lastFade != fadeOut)
        {
            if (fadeOut)
                FadeOut();
            else
                FadeIn();
            lastFade = fadeOut;
        }
    }


}