using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class TrapsRotate : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    public float turnSpeed = 100;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private float timeToGo;

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
        timeToGo = Time.fixedTime + timeOpti;
    }
    #endregion

    #region core script
    /// <summary>
    /// Initialisation
    /// </summary>
    private void RotateTraps()                                             //test
    {
        transform.Rotate(Vector3.forward * turnSpeed * Time.deltaTime);
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        if (Time.fixedTime >= timeToGo)
        {
            RotateTraps();                                                  //rotate l'objet
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
    #endregion
}
