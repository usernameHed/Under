using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class TrapsObjectKiller : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public bool killDestructible = true;

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
    /// trigger enter des oeufs ou des joueurs
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().destroyThis();
        }
        else if (other.CompareTag("Eggs"))
        {
            Debug.Log("trapKiller ?");
            other.GetComponent<EggsController>().destroyThis();
        }
        else if (other.CompareTag("Destructible"))
        {
            if (killDestructible && other.GetComponent<ProjectileScript>())
                other.GetComponent<ProjectileScript>().destroyThis();
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
