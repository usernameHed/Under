using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class LevelData : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject Spawn;
    public GameObject SpawnPlayer;
    public GameObject SpawnQueen;
    public GameObject SpawnEggs;
    public GameObject groupEggs;
    public GameObject groupTuto;
    public int specialTuto = 0;                                         //défini le type de caméra en début de jeu... -1 normal, 1 tuto 1
    public GameObject[] respawn;

    //public int idMap = 1;
    public bool isThereCinematic = false;
    public bool debug = false;
    public bool goToWhenOver = false;
    private Fading fade;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    [HideInInspector] public GameManager GM;
    [HideInInspector] public bool goToLevel = false;
    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private QuitOnClick QOC;
    private GameObject gameController;
    
    private int maxEggs;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [SerializeField] private GameObject Cinematic;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        QOC = gameObject.GetComponent<QuitOnClick>();
        gameController = GameObject.FindGameObjectWithTag("GameController");
        groupTuto = GameObject.FindGameObjectWithTag("groupTuto");
        fade = gameObject.GetComponent<Fading>();
        if (isThereCinematic && Cinematic)
        {
            Cinematic.SetActive(true);
            Cinematic.transform.GetChild(0).gameObject.GetComponent<AudioSource>().enabled = true;
            Cinematic.transform.GetChild(0).gameObject.GetComponent<PlayVideo>().enabled = true;
        }
            
        if (gameController)
        {
            GM = gameController.GetComponent<GameManager>();
            GM.groupEggs = groupEggs;
        }

        if (respawn.Length == 0)
            Debug.LogError("error");
    }

    /// <summary>
    /// retry to get GM
    /// </summary>
    public void resetGM()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController)
        {
            GM = gameController.GetComponent<GameManager>();
            GM.groupEggs = groupEggs;
        }
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        GameObject[] multipleLevelData = GameObject.FindGameObjectsWithTag("LevelData");
        if (multipleLevelData.Length <= 1 && !debug)
        {
            goToLevel = true;
            goToWhenOver = true;
            if (isThereCinematic)
                QOC.jumpAdditiveScene("5_Game");              //juste après être arrivé dans la scène de la map, passer au jeu, en gardant cette map !
            else
            {
                if (fade)
                {
                    fade.enabled = true;
                    fade.BeginFade(-1);

                }
            }
            //if (!isThereCinematic)                              //si il n'y a pas de cinématique, directement passer au games
            //QOC.jumpAdditiveScene("5_Game");              //juste après être arrivé dans la scène de la map, passer au jeu, en gardant cette map !
        }
        else
        {
            if (debug && multipleLevelData.Length > 1)
                Destroy(gameObject);
        }
    }
    #endregion

    #region core script
    /// <summary>
    /// Initialisation
    /// </summary>

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        if (goToWhenOver && fade.alpha == 0)
        {
            goToWhenOver = false;
            QOC.jumpAdditiveScene("5_Game");              //juste après être arrivé dans la scène de la map, passer au jeu, en gardant cette map !
        }
    }
    #endregion
}
