using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class ZonesFreez : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>

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

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// action lorsque le joueur entre dans une zone
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().WalkOnSnow(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().WalkOnSnow(false);
        }
    }
    #endregion
}
