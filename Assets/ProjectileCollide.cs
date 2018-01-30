using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollide : MonoBehaviour
{
    private ProjectileScript PS;

    private void Start()
    {
        PS = transform.parent.gameObject.GetComponent<ProjectileScript>();
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.tag);
        if (collision.transform.CompareTag("Platform"))
        {
            PS.hasCollided = true;
            /*Debug.Log("ici au moin s????");
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white, 10f);
            }*/
            PS.refPlayer.projectileHit(gameObject.transform.position, collision.gameObject, collision.contacts[0].normal);
            PS.destroyThis();
        }
    }

}
