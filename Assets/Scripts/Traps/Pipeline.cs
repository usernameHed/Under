using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class Pipeline : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject project;
    public GameObject posEnd;
    public float strenghtOfRepulsion = 40f;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>

    /// <summary>
    /// variable privé serealized
    /// </summary>
    //[SerializeField] private bool tmp;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {

    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {

    }
    #endregion

    #region core script
    /// <summary>
    /// Initialisation
    /// </summary>
    private void functionTest()                                             //test
    {

    }
    #endregion

    #region unity fonction and ending

    void ManageTeleport(GameObject other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Eggs") || other.CompareTag("Destructible")
            || other.CompareTag("Destructible")) //projectil  portal
        {
            other.transform.position = project.transform.position;

            Vector3 moveDir = (posEnd.transform.position - project.transform.position).normalized;
            if (other.GetComponent<Rigidbody>())
            {
                other.GetComponent<Rigidbody>().velocity = moveDir * strenghtOfRepulsion * Time.deltaTime;
                other.GetComponent<Rigidbody>().angularVelocity = moveDir * strenghtOfRepulsion * Time.deltaTime;
            }
        }
        else if (other.CompareTag("Pushable"))
        {
            other.transform.position = project.transform.position;
        }

    }

    /// <summary>
    /// collision avec un autre objet 2D
    /// (player ou oeufs)
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ColliderTrail3d") && other.gameObject.GetComponent<TimeWithNoEffect>().isOk)
        {
            other.gameObject.GetComponent<TimeWithNoEffect>().isOk = false;
            //si il n'a pas de TrailCollider, c'est un projectile portal !
            if (!other.gameObject.GetComponent<TrailCollider>())
            {
                ManageTeleport(other.transform.parent.gameObject);
            }
            else
                ManageTeleport(other.gameObject.GetComponent<TrailCollider>().refObject);
        }
    }

    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {

    }
    #endregion
}
