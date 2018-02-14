using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : MonoBehaviour                                  //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject refObject;                            //reference de l'oeuf

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

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        refObject = gameObject.transform.parent.gameObject;
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {

    }
    #endregion

    #region core script

    #endregion

    #region unity fonction and ending

    /// <summary>
    /// détecte les collisions avec les trails
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Trail"))
        {
            other.gameObject.GetComponent<ForceTrail>().BothTriggerStay(refObject);
        }
    }

    /// <summary>
    /// détecte les collisions avec les trails
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Trail"))
        {
            other.gameObject.GetComponent<ForceTrail>().BothTriggerStay(refObject);
        }
    }

    /// <summary>
    /// Si l'oeuf sort de la range de collision, reset ses propriété !
    /// </summary>
    /// <param name="hit"></param>
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Trail"))
        {
            other.gameObject.GetComponent<ForceTrail>().TriggerType(true, refObject);
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
