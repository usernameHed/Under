using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(CircleCollider2D))]
public class MapsDatas : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public GameObject groupMap;                                         //parent qui contient toute les map

    [Tooltip("défini si on est en mode multi ou pas")]
    public bool multi = false;                                          //défini si on est en mode multi ou pas

    //[Tooltip("Liste des infos des maps")]
    
    //public List<dataMapsSolo> dms;
    //public List<List<dataMapsSolo> > worlds;

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
        initialiseMaps();                                                   //d'après les différentes infos, setup les maps !
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
    /// setup les maps:
    /// met les bon pouvoirs sur les maps, 
    /// affiche le message de blocage de maps
    /// </summary>
    private void initialiseMaps()                                             //test
    {
        /*List<dataMapsSolo> tmpPP;
        int max;

        if (!multi)
        {
            max = PlayerPrefs.PP.mapUnlock;
            tmpPP = MapPrefs.PP.mapsInfosSolo;
        }
        else
        {
            max = PlayerPrefs.PP.mapUnlockMulti;
            tmpPP = MapPrefs.PP.mapsInfosMulti;
        }

        
        for (int i = 0; i < tmpPP.Count; i++)
        {
            Transform mapTmp = groupMap.transform.GetChild(i);
            if (i < max)                                          //la la map fait partie des maps unnlock...
            {
                //GUI
                mapTmp.GetComponent<Toggle>().interactable = true;                      //rend la map interactable (le joueur peu y accéder)
                mapTmp.GetChild(1).gameObject.SetActive(true);                          //active l'objet qui montre les pouvoirs de la maps
                //ici activer les bon pouvoirs
                mapTmp.GetChild(1).GetChild(0).gameObject.SetActive(tmpPP[i].white);
                mapTmp.GetChild(1).GetChild(1).gameObject.SetActive(tmpPP[i].blue);
                mapTmp.GetChild(1).GetChild(2).gameObject.SetActive(tmpPP[i].red);
                mapTmp.GetChild(1).GetChild(3).gameObject.SetActive(tmpPP[i].yellow);
                mapTmp.GetChild(1).GetChild(4).gameObject.SetActive(tmpPP[i].green);

                mapTmp.GetChild(2).gameObject.SetActive(true);                          //activer l'objet qui contient le nom de la map
                mapTmp.GetChild(2).GetComponent<Text>().text = tmpPP[i].nameMap;          //attribuer le nom de la map
                mapTmp.GetChild(3).gameObject.SetActive(false);                         //cacher le cadenas
                tmpPP[i].blocked = false;

            }
            else
            {
                groupMap.transform.GetChild(i).GetComponent<Toggle>().interactable = false; //rend la map non atteignable
                mapTmp.GetChild(1).gameObject.SetActive(false);                          //active l'objet qui montre les pouvoirs de la maps
                mapTmp.GetChild(2).gameObject.SetActive(false);                          //cache le nom de la map
                mapTmp.GetChild(3).gameObject.SetActive(true);                          //affiche le cadenas
                tmpPP[i].blocked = true;
            }

        }*/
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