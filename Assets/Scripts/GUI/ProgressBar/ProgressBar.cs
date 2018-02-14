using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
[ExecuteInEditMode]
[System.Serializable]
public class ProgressBar : MonoBehaviour {
    private List<OnCompleteListener> listeners = new List<OnCompleteListener>();

    public Image progress;
    public Text text;

    public string addictionalText = "";
    public int max = 100;
    public string afterText = "%";

    [Range(1,10)]
    public float smoothTime = 2;

    protected bool smoothly = false;

    protected int facticalProgress = 0;
    public int currentProgress = 0;
    public int warningTime = 10;

    private bool full = false;
    void Start() {
        progress.fillAmount = 0;
    }
    void Update() {
        calculateProgress();
        notifyListeners();
    }
 
    protected virtual void calculateProgress() {
        if (isMinMax()) return;
        if (smoothly) {
            float fillNeed = currentProgress / (float)max;
            float currentFill = Mathf.Lerp(progress.fillAmount, fillNeed, smoothTime*Time.unscaledDeltaTime);
            progress.fillAmount = currentFill;
            facticalProgress = Mathf.RoundToInt(currentFill * (float)max);
        } else {
            facticalProgress = currentProgress;
            progress.fillAmount = facticalProgress / (float)max;
        }
        text.text = addictionalText + facticalProgress + afterText;
    }
    protected bool isMinMax() {
        if (currentProgress > max) {
            currentProgress = max;
            return true;
        }
        if (currentProgress < 0) {
            currentProgress = 0;
            return true;
        }
        return false;
    }

    public int getProgress() {
        return facticalProgress;
    }
    public void setProgress(int progress) {
        this.smoothly = false;
        currentProgress = progress;
        calculateProgress();
    }
    public void setProgress(int progress, bool smooth) {
        this.smoothly = smooth;
        currentProgress = progress;
    }

    public virtual ProgressBar setColor(Color c) {
        progress.color = c;
        return this;
    }
    public  ProgressBar setTextSize(int size) {
        text.fontSize = size;
        return this;
    }
    public virtual ProgressBar setAfterText(string text) {
        afterText = text;
        return this;
    }
    public ProgressBar setMax(int max) {
        if (max<0) {
            return this;
        }
        this.max = max;
        if (facticalProgress > max) {
            facticalProgress = max;
        }
        if (currentProgress > max) {
            currentProgress = max;
        }
        return this;
    }
    public virtual ProgressBar setAddictionalText(string text) {
        addictionalText = text;
        return this;
    }
    public string getAddictionalText() {
        return addictionalText;
    }
    void notifyListeners() {
        if (facticalProgress == max &&!full) {
            full = true;
            foreach (OnCompleteListener l in listeners) {
                if (l != null) {
                    l.progressBarComplete();
                }
            }
        } else if (facticalProgress<max) {
            full = false;
        }
    }
    public ProgressBar addListener(OnCompleteListener listener) {
        if (!listeners.Contains(listener)) {
            listeners.Add(listener);
        }
        return this;
    }
    public ProgressBar removeListener(OnCompleteListener listener) {
        if (!listeners.Contains(listener)) {
            listeners.Remove(listener);
        }
        return this;
    }
}
