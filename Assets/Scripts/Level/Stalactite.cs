using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired.UI.ControlMapper;
//using Sirenix.OdinInspector;

/// <summary>
/// description
/// </summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class Stalactite : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    //[FoldoutGroup("folder group")]
    //[Button(Name = "button name")]
    //[Tooltip("information de variable")]
    ////[OnValueChanged("value changed ?")]
    public float speedFallDown = 5f;

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
    [SerializeField] private GameObject particleWhite;          //particule de destruction de l'objet

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
    /// lorsque le stalactite tombe, il pointe vers le bas
    /// </summary>
    private void fallDown()
    {
        
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// lorsque le stalactite est en collision avec un autre objet, il se détruit la plupart du temps
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (!(collision.transform.CompareTag("Pushable") && collision.gameObject.GetComponent<Stalactite>()))
        {
            destroyThis();
        }
    }

    public void destroyThis()
    {
        if (!gameObject)
            return;
        Vector3 pos = transform.position;
        pos += gameObject.transform.up * 0.8f;
        Instantiate(particleWhite, pos, transform.rotation, transform.parent);
        Instantiate(particleWhite, pos, transform.rotation, transform.parent);
        Instantiate(particleWhite, pos, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        fallDown();                                                         //pointe vers le bas
    }
    #endregion
}
