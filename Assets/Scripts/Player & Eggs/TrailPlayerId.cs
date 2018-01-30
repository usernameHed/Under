using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class TrailPlayerId : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject refPlayer;                                            //référence du joueur
    public List<GameObject> refPlayerSolo = new List<GameObject>();                                  //reférence DES joueurs (en solo avec X pouvoirs)

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
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {

    }
    #endregion
}
