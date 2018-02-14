using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawSolidArc))]
public class DrawSolidArcEditor : Editor
{
    
    void OnSceneGUI()
    {
        DrawSolidArc t = target as DrawSolidArc;

        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 80, 90, 50));

        GUILayout.EndArea();
        Handles.EndGUI();

        Handles.color = new Color(1, 1, 1, 0.2f);
        Handles.DrawSolidArc(t.transform.position, -t.transform.forward, Quaternion.AngleAxis(90 - t.fovAngle / 2, -t.transform.forward) * -t.transform.right,
                                t.fovAngle, t.fovRange);
    }
}