using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ProgressBarAppearing : MonoBehaviour {
  
    Vector3 targetScale;
    public Vector3 targetScaleP;
    public float appearTime = 1f;
    public float appearTimeColor = 1f;
    public Image animatedBackground;
    public Image animatedProgress;
    public bool animateFill = false;
    public bool animateProg = false;
    public bool animateColor = false;
    public bool animateSize = false;
    public Color fromColor;
    public Color toColor;
    private float controlTime;
    private float controlTimeProg;
    void Start () {
        if (animateSize) {               
           targetScale = animatedBackground.transform.localScale;
           animatedBackground.transform.localScale = targetScale / 4;        
            if (animatedProgress != null && animateProg) {
                targetScaleP = animatedProgress.transform.localScale;
                animatedProgress.transform.localScale = targetScaleP / 4;
            }
        }
        if (animateColor) {
            animatedBackground.color = fromColor;
        }
        if (animateFill) {
            animatedBackground.fillAmount = 0f;
        }
    }

	void Update () {
        calculateSize();
        calculateFill();
        calculateColor();     
        if (animationsOver()) {
            Destroy(this);
        }
    }
    private void calculateSize() {
      
        if (animateSize) {
            if (animatedProgress != null && animateProg) {
                if (animatedProgress.transform.localScale.x < targetScaleP.x) {
                    animatedProgress.transform.localScale += targetScaleP / appearTime * Time.deltaTime;
                } else {
                    animateProg = false;
                }
            }
            if (animatedBackground.transform.localScale.x < targetScale.x) {
                animatedBackground.transform.localScale += targetScale / appearTime * Time.deltaTime;
            } else {
                animateSize = false || animateProg;
            }
        }
    }
    private void calculateFill() {
        if (animateFill) {
            animatedBackground.fillAmount += 1.0f / appearTime * Time.deltaTime;
            if (animatedBackground.fillAmount == 1f) {
                animateFill = false;
            }
        }
    }
    private void calculateColor() {
        if (animateColor) {
            if (controlTime < 1) {
                controlTime += Time.deltaTime / appearTimeColor;
            } else {
                animateColor = false;
            }
            animatedBackground.color = Color.Lerp(fromColor, toColor, controlTime);
        }
    }
    private bool animationsOver() {
        return !(animateSize || animateColor || animateFill || animateColor);
    }
}

