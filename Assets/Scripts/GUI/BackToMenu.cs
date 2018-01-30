using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class BackToMenu : MonoBehaviour                                   //commentaire
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
    private GameObject Global;
    private PlayerConnected PC;
    private QuitOnClick QOC;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [SerializeField] private PlayVideo PV;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        Global = GameObject.FindGameObjectWithTag("Global");
        QOC = gameObject.GetComponent<QuitOnClick>();
        if (Global)
        {
            PC = Global.GetComponent<PlayerConnected>();
        }
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
        if (PC && PC.exitAll())
        {
            PV.stopVideo();
            PV.isPlaying = false;
            QOC.ActivateScene();
        }
    }
    #endregion
}
