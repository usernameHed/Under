using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailDeletePoints : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public int index = 0;
    public float timeToWait = 0.0f;
    public float timeToAdd = 1.0f;
    public float timeToSub = 5.0f;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    [HideInInspector] public TrailController TC;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private LineRenderer line;                                                  //line
    private bool startDelete = false;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    //[SerializeField] private EdgeCollider2D col;                                //collider

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        line = gameObject.GetComponent<LineRenderer>();                     //line
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        InvokeRepeating("deletePoint", TC.startTime, TC.repeateTime);             //supprime les points après un certain temps
    }
    #endregion

    #region core script
    /// <summary>
    /// Supprime le premier points de la trail !
    /// </summary>
    private void deletePoint()                                             //test
    {
        //Debug.Log(timeToWait);
        if (timeToWait > 0)
        {
            timeToWait -= timeToSub;
            if (timeToWait < 0)
                timeToWait = 0;
            return;
        }
        startDelete = true;
        //premièrement, update le lineRender et collider dans l'objet unity
        //line.po
        if (gameObject.transform.childCount > 0 && gameObject.transform.GetChild(0).gameObject.GetComponent<TrapsController>())                                    //si le trail à un enfant, c'est l'attract ! le supprimer quand le trail commence à disparaitre !
            Destroy(gameObject.transform.GetChild(0).gameObject);

        //update les listes sauvegardé dans les données
        if (TC.deletePoint(index, 0))
        {
            line.positionCount = TC.listTrails[index].points.Count;
            for (int i = 0; i < TC.listTrails[index].points.Count; i++)
            {
                line.SetPosition(i, TC.listTrails[index].points[i]);
            }
        }
        else
        {
            CancelInvoke();
            Destroy(gameObject);
        }
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        if (!startDelete && TC.listTrails[index].points.Count > 10)                     //active le miniAttract créé lorque le trail commence à se créer !
        {
            if (gameObject.transform.childCount > 0)                                    //si le trail à un enfant, c'est l'attract ! le supprimer quand le trail commence à disparaitre !
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log("active mini attract !");
            startDelete = true;
        }
    }
    #endregion
}
