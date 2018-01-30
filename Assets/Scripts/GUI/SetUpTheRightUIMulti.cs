using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired.UI.ControlMapper;

public class SetUpTheRightUIMulti : MonoBehaviour
{
    public List<GameObject> powerAnts = new List<GameObject>();
    /// <summary>
    /// setup les ui à l'intérieur quand le monde est activé
    /// -- set si c'est bloqué ou non
    /// -- set les golden eggs gagné
    /// </summary>
    public void setUpWhenWorldIsActive(int idLevel)
    {
        powerAnts[0].SetActive(MapPrefs.PP.mapsInfosMulti[idLevel].white);
        powerAnts[1].SetActive(MapPrefs.PP.mapsInfosMulti[idLevel].blue);
        powerAnts[2].SetActive(MapPrefs.PP.mapsInfosMulti[idLevel].red);
        powerAnts[3].SetActive(MapPrefs.PP.mapsInfosMulti[idLevel].yellow);
        powerAnts[4].SetActive(MapPrefs.PP.mapsInfosMulti[idLevel].green);
    }
}
