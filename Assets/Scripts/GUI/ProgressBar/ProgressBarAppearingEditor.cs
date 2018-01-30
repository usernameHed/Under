#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
[CustomEditor(typeof(ProgressBarAppearing))]
[System.Serializable]
public class ProgressBarAppearingEditor : Editor {
    public override void OnInspectorGUI() {
        var script = target as ProgressBarAppearing;        
        script.appearTime = EditorGUILayout.FloatField("Appearing time", script.appearTime);
        script.appearTime = Mathf.Clamp(script.appearTime, 0.2f, 5f);
        script.animateSize = GUILayout.Toggle(script.animateSize, "Animate size");
        script.animateFill = GUILayout.Toggle(script.animateFill, "Animate fillamount of background?");
        if (script.animateSize||script.animateFill) {
            script.animatedBackground = (Image)EditorGUILayout.ObjectField("Animated Background:", script.animatedBackground, typeof(Image), true);    
            script.animateProg = GUILayout.Toggle(script.animateProg, "Animate progress appearing?");
            if (script.animateProg) {
                script.animatedProgress = (Image)EditorGUILayout.ObjectField("Animated Progress:", script.animatedProgress, typeof(Image), true);
            }
        } else {
            script.animateProg = false;
        }
     
        script.animateColor = GUILayout.Toggle(script.animateColor, "Animate color");
        if (script.animateColor) {
            script.appearTimeColor = EditorGUILayout.FloatField("Appearing time color", script.appearTimeColor);
            script.appearTimeColor = Mathf.Clamp(script.appearTimeColor, 0.7f, 5f);
            script.fromColor = EditorGUILayout.ColorField("From color", script.fromColor);
            script.toColor = EditorGUILayout.ColorField("To color", script.toColor);
        }
        EditorUtility.SetDirty(script);
    }
}
#endif