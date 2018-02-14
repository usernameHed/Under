using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// détecte une collision 3d avec le joueur, si le joueur entre dans la zone,
/// déplace le spawn player et/ou oeufs à la position "pointSpawn"
/// </summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class Checkpoints : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Tooltip("est-ce que le checkpoints déplace le spawn des joueurs ?")]
    public bool moveSpawnPlayer = true;

    [Tooltip("est-ce que le checkpoints déplace le spawn des oeufs ?")]
    public bool moveSpawnEggs = true;

    [Tooltip("est-ce que c'est le player qui doit activer le checkpoint ?")]
    public bool whenPlayerCross = true;

    [Tooltip("est-ce que c'est les oeufs qui doit activer le checkpoint ?")]
    public bool whenEggsCross = false;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private bool checkpointPassed = false;                                  //le checkpoint est-il atteint ?

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [Tooltip("La position où téléporter les spawn")]
    [SerializeField] private Transform pointSpawn;                                  //information sur les references des spawns
    [SerializeField] private LevelData LD;                                  //information sur les references des spawns

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
    private void newCheckPoints()                                             //nouveau checkpoints atteint
    {
        checkpointPassed = true;
        LD.GM.newCheckpointFind.SetActive(true);
        if (moveSpawnPlayer)
        {
            LD.Spawn.transform.GetChild(0).GetChild(0).gameObject.transform.position = pointSpawn.position;
            Transform look = LD.Spawn.transform.GetChild(0).GetChild(0).GetChild(2);
            look.GetChild(1).gameObject.SetActive(false);
            look.GetChild(2).gameObject.SetActive(false);
            look.GetChild(3).gameObject.SetActive(false);
            look.GetChild(4).gameObject.SetActive(false);
        }            
        if (moveSpawnEggs)                                                      //si le checkpoint déplace le spawner des oeufs...
        {
            LD.Spawn.transform.GetChild(1).gameObject.transform.position = pointSpawn.position;
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
        if (checkpointPassed)
            return;
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (collision.CompareTag("Player") && whenPlayerCross)
        {
            newCheckPoints();
        }
        else if (collision.CompareTag("Eggs") && whenEggsCross)
        {
            newCheckPoints();
        }
    }
    #endregion
}
