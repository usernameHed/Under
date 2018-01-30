using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired.UI.ControlMapper;
//using Sirenix.OdinInspector;

/// <summary>
/// description
/// </summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class LevelCallSomeSound : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
	//[FoldoutGroup("folder group")]
	//[Button(Name = "button name")]
	//[Tooltip("information de variable")]
	////[OnValueChanged("value changed ?")]

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
    public void callSound(string sound)
    {
        SoundManager.SS.playSound(sound);
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
