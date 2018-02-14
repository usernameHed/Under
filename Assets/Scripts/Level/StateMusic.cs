using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class StateMusic : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Tooltip("désactive une fois passé ?")]
    public bool desactiveAfterPass = true;

    [Tooltip("état de la musique")]
    public int state = 0;

    [Tooltip("trigger quand le joueur passe ?")]
    public bool triggerPlayer = true;


    [Tooltip("trigger quand les oeufs passent ?")]
    public bool triggerEggs = false;

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
    /// fonction qui change la state de la musique
    /// - si desactiveAfterPass, désactiver le trigger
    /// </summary>
    private void CollideStateMusic(GameObject refObj)                                             //nouveau checkpoints atteint
    {
        SoundManager.GetSingleton.MusicState = state;
        if (desactiveAfterPass)
            gameObject.SetActive(false);
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// action lorsque le joueur entre dans une zone (ou les oeufs)
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (triggerPlayer && collision.CompareTag("Player"))
        {
            CollideStateMusic(collision.gameObject);
        }
        else if (triggerEggs && collision.CompareTag("Eggs"))
        {
            CollideStateMusic(collision.gameObject);
        }
    }
    #endregion
}
