using UnityEngine;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipYnBNWVBfcWhhRUU
see:
https://drive.google.com/file/d/0Byzet-SVq6ipbXdHY0lNOHJaMEE/view
*/

[ExecuteInEditMode]
public class DrawSolidArc : MonoBehaviour
{
    [Range(0, 10)]          public float fovRange = 3.0f;
    [Range(0, 360)]         public float fovAngle = 30.0f;
    [Range(-300, 300)]      public float strengthOfAttraction = 20.0f;
}