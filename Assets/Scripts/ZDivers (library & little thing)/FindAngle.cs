using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAngle : MonoBehaviour
{
    public GameObject goPlayer;

    void Update()
    {

        Vector3 v3Pos;
        float fAngle;

        if (Input.GetMouseButtonDown(0))
        {
            // Project the mouse point into world space at
            //   at the distance of the player.
            v3Pos = Input.mousePosition;
            v3Pos.z = (goPlayer.transform.position.z - Camera.main.transform.position.z);
            v3Pos = Camera.main.ScreenToWorldPoint(v3Pos);
            v3Pos = v3Pos - goPlayer.transform.position;
            fAngle = Mathf.Atan2(v3Pos.y, v3Pos.x) * Mathf.Rad2Deg;
            if (fAngle < 0.0f) fAngle += 360.0f;
            Debug.Log("1) " + fAngle);

            // Raycast against a mathematical plane in world space
            Plane plane = new Plane(Vector3.forward, goPlayer.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float fDist;
            plane.Raycast(ray, out fDist);
            v3Pos = ray.GetPoint(fDist);
            v3Pos = v3Pos - goPlayer.transform.position;
            fAngle = Mathf.Atan2(v3Pos.y, v3Pos.x) * Mathf.Rad2Deg;
            if (fAngle < 0.0f) fAngle += 360.0f;
            Debug.Log("2) " + fAngle);

            //Convert the player to Screen coordinates
            v3Pos = Camera.main.WorldToScreenPoint(goPlayer.transform.position);
            v3Pos = Input.mousePosition - v3Pos;
            fAngle = Mathf.Atan2(v3Pos.y, v3Pos.x) * Mathf.Rad2Deg;
            if (fAngle < 0.0f) fAngle += 360.0f;
            Debug.Log("3) " + fAngle);
        }
    }
}
