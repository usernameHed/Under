using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcesBetweenEggs : MonoBehaviour
{
    [Range(0, 10)]
    public float fovRange = 3.0f;
    [Range(-300, 300)]
    public float strengthOfAttraction = 20.0f;
    [Range(0, 0.1f)]
    public float timeOpti = 0.1f;
    [Range(1.0f, 100.0f)]
    public float maxVelocity = 10.0f;

    /// <summary>
    /// 
    /// </summary>

    private int layerType = 8;
    private int layerMask = 1 << 8; //select layer 8 (metallica and colider)
    private Rigidbody Rb;
    private float timeToGo;
    private Collider[] gravityColliders;
    private int maxTab = 30;                    //max objectà trigger
    private EggsController ECthis;

    private void Awake()
    {
        Rb = gameObject.GetComponent<Rigidbody>();
        ECthis = gameObject.GetComponent<EggsController>();
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        gravityColliders = new Collider[maxTab];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, fovRange);
    }

    private void ApplyAttraction()//TODO: RAM RAM RAM !!
    {
        Vector3 here = transform.position; // get player position...
        layerType = gameObject.layer;
        gameObject.layer = 0;

        int otherObject = Physics.OverlapSphereNonAlloc(here, fovRange, gravityColliders, layerMask);
        if (otherObject > maxTab)
            otherObject = maxTab;

        for (int i = 0; i < otherObject; i++)
        {
            Collider hitCollider = gravityColliders[i];
            if (hitCollider.tag == "Eggs")   // if it's an eggs...
            {
                EggsController ECTmp = hitCollider.GetComponent<EggsController>();
                if (ECTmp && ECTmp.isKinematicSave && !ECthis.isKinematicSave
                    && !ECthis.isGreenControlled
                    && !ECthis.stopControlGreen)
                    StartCoroutine(ECthis.waitToSetKinematic());
                Vector3 forceDirection = transform.position - hitCollider.transform.position;
                
                hitCollider.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection * Rb.mass);
                
            }
        }

        gameObject.layer = layerType;
    }

    private void testVelocityMax()
    {
        Rb.velocity = Vector3.ClampMagnitude(Rb.velocity, maxVelocity);
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            ApplyAttraction();
            testVelocityMax();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
