using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasAddOne : MonoBehaviour
{
    public void destroyThis()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = pos.y + 0.05f;
        transform.position = pos;
    }
}
