using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

//[RequireComponent(typeof(CircleCollider2D))]
public class EventAnalytics : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public static EventAnalytics EA;
    public GameObject player;
    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private Dictionary<string, object> dict = new Dictionary<string, object>();

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
        if (EA == null)
        {
            DontDestroyOnLoad(gameObject);
            EA = this;
        }
        else if (EA != this)
        {
            Destroy(gameObject);
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
    /// est appelé quand un level est fini
    /// </summary>
    /// <param name="solo"></param>
    /// <param name="idMap"></param>
    /// <param name="timeSucced"></param>
    public void OnLevelCompleted(bool solo, int idWorld, int idMap, int timeSucced)
    {
        Debug.Log("Send to analytics ! solo: " + solo + ", idWorld: " + idWorld + ", idMap:" + idMap + ", timeSucced: " + timeSucced);
        //dict["isSoloMap"] = solo;
        //dict["idMap"] = idMap;
        dict["timeSucced"] = idWorld + "-" + idMap + "_" + timeSucced;
        //Debug.Log(Analytics.CustomEvent("LevelCompleted", dict));
        Analytics.CustomEvent("LevelCompletedTimeSolo", dict);
    }

    /// <summary>
    /// est appelé quand un level est échoué (le panel levelRestart s'affiche) en solo ou multi !
    /// </summary>
    public void displayLevelRestart(string idLevelFailed)
    {
        /*Debug.Log(Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            { "multi", multi }
        }));*/
        Analytics.CustomEvent("levelFailedSolo", new Dictionary<string, object>
        {
            { "idMap", idLevelFailed }
        });
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        if (player)
        {
            //Debug.Log("ici sending to heatmap");
            //HeatmapEvent.Send("PlayerPosition", player.transform.position, Time.timeSinceLevelLoad, PlayerPrefs.PP.lastLevelPlayerId);
        }
            
    }
    #endregion
}