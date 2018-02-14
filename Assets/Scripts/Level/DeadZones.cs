using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class DeadZones : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Tooltip("est-ce que on détruit les players ?")]
    public bool killPlayers = true;

    [Tooltip("est-ce que on détruit les oeufs ?")]
    public bool killEggs = true;

    [Tooltip("est-ce que on détruit les pushables (Rochers) ?")]
    public bool killPushable = true;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable

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
    private void TryToKill(GameObject refObj)                                             //nouveau checkpoints atteint
    {
        if (refObj.CompareTag("Player") && killPlayers)
        {
            refObj.GetComponent<PlayerController>().destroyThis();
        }
        else if (refObj.CompareTag("Eggs") && killEggs)
        {
            Debug.Log("dead zone ");
            refObj.GetComponent<EggsController>().destroyThis();
        }
        else if (refObj.CompareTag("Pushable") && killPushable)
        {
            refObj.GetComponent<Pushable>().destroyThis();
        }
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// action lorsque le joueur entre dans une zone
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (collision.CompareTag("Eggs")
            || collision.CompareTag("Player")
                || collision.CompareTag("Pushable"))
        {
            TryToKill(collision.gameObject);
        }
    }
    #endregion
}
