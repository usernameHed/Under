using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan_rotator : MonoBehaviour {

    [Tooltip("Gere la vitesse de rotation")]
    public float speed = 200;
    
	
	// Update is called once per frame
	void Update ()
    {
         
    transform.Rotate(new Vector3(0.0f, Time.deltaTime * speed, 0.0f));
	}
}
