using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToPlayer : MonoBehaviour {

    public Transform target;
    public float smoothTime = 0.3f;
    public float maxDist = 20f;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
        gameObject.transform.position = target.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        Vector3 offset = target.position - transform.position;
        float sqrLen = offset.sqrMagnitude;
        if (sqrLen > maxDist * maxDist)
            gameObject.transform.position = target.transform.position;
        //Vector3 lerpPosCamSmooth = Vector3.Lerp(gameObject.transform.position, target.transform.position, Time.deltaTime * 10);
        //gameObject.transform.position = new Vector3(lerpPosCamSmooth.x, lerpPosCamSmooth.y, 0);
    }
}
