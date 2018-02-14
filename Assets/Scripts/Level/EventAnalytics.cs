using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using Sirenix.OdinInspector;

//[RequireComponent(typeof(CircleCollider2D))]
public class EventAnalytics : MonoBehaviour                                   //commentaire
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
    private static EventAnalytics SS;
    private Dictionary<string, object> dict = new Dictionary<string, object>();

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")]
    [Tooltip("Activation de la singularité du script")]
    [SerializeField]
    private bool enableSingularity = false;

    [FoldoutGroup("Debug")]
    [Tooltip("l'objet est-il global inter-scène ?")]
    [SerializeField]
    private bool dontDestroyOnLoad = false;

    #endregion

    #region fonction debug variables
    /// <summary>
    /// retourne une erreur si le timeOpti est inférieur ou égal à 0.
    /// </summary>
    /// <returns></returns>
    private bool debugTimeOpti(float timeOpti)
    {
        if (timeOpti <= 0)
            return (false);
        return (true);
    }

    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    private void testSingularity()
    {
        if (!enableSingularity)
            return;

        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// récupère la singularité (si ok par le script)
    /// </summary>
    /// <returns></returns>
    static public EventAnalytics getSingularity()
    {
        if (!SS || !SS.enableSingularity)
        {
            Debug.LogError("impossible de récupéré la singularité");
            return (null);
        }
        return (SS);
    }

    /// <summary>
    /// set l'objet en dontDestroyOnLoad();
    /// </summary>
    private void setDontDestroyOnLoad()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        testSingularity();                                                  //set le script en unique ?
        setDontDestroyOnLoad();                                             //set le script Inter-scène ?
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
        dict["timeSucced"] = idWorld + "-" + idMap + "_" + timeSucced;
        Analytics.CustomEvent("LevelCompletedTimeSolo", dict);
    }

    /// <summary>
    /// est appelé quand un level est échoué (le panel levelRestart s'affiche) en solo ou multi !
    /// </summary>
    public void displayLevelRestart(string idLevelFailed)
    {
        Analytics.CustomEvent("levelFailedSolo", new Dictionary<string, object>
        {
            { "idMap", idLevelFailed }
        });
    }
    #endregion

    #region unity fonction and ending

    #endregion
}