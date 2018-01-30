using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired.UI.ControlMapper;

public class SetUpTheRightUI : MonoBehaviour {

    public enum TypeLevel
    {
        PREVWORLD = 1,
        LEVEL = 2,
        NEXTWORLD = 3
    }
    public TypeLevel TL = TypeLevel.LEVEL;

    [SerializeField] private GlobalVariableManager GVM;
    [SerializeField] private GameObject central;
    [SerializeField] private GameObject[] centralChild;
    [SerializeField] private CustomButton customBut;
    [SerializeField] private GameObject picBlocked;
    [SerializeField] private FadeObjectInOut powerAnts;
    [SerializeField] private Text panelLocked;
    [SerializeField] private FadeObjectInOut fadePanel;
    private int idWorld;
    private int idLevel;

    void setTextLock(int idW, int idL)
    {
        string typeEggs = "BRONZE";
        if (idL == 0)
            idL = 1;
        if (MapPrefs.PP.getLevels(idW, idL).succesNeededForUnloclk == 2)
            typeEggs = "SILVER";
        else if (MapPrefs.PP.getLevels(idW, idL).succesNeededForUnloclk == 3)
            typeEggs = "GOLD";
        else if (MapPrefs.PP.getLevels(idW, idL).succesNeededForUnloclk == 4)
            typeEggs = "EPIC";
        panelLocked.text = "Get the " + typeEggs + "\nin the previous map";
    }

    /// <summary>
    /// setup les ui à l'intérieur quand le monde est activé
    /// -- set si c'est bloqué ou non
    /// -- set les golden eggs gagné
    /// </summary>
	public void setUpWhenWorldIsActive(int idW, int idL)
    {
        idWorld = idW;
        idLevel = idL;
        //Debug.Log("setup les ui à l'intérieur quand le monde " + idWorld + " est activé, level " + idLevel);

        if (TL == TypeLevel.PREVWORLD)
        {
            customBut.interactable = true;
            fadePanel.fadeOut = true;
            return;
        }
        else if (TL == TypeLevel.NEXTWORLD)
        {
            if (PlayerPrefs.PP.worldUnlock > idW)
            {
                
                fadePanel.gameObject.SetActive(true);
                fadePanel.fadeOut = true;
                customBut.interactable = true;
            }
            else
            {
                customBut.interactable = false;
                fadePanel.gameObject.SetActive(false);
            }
                
            return;
        }


        setTextLock(idW, idL);

        if ((idW == 1 && PlayerPrefs.PP.mapInfosPref1[idL].blocked)
            || (idW == 2 && PlayerPrefs.PP.mapInfosPref2[idL].blocked)
            || (idW == 3 && PlayerPrefs.PP.mapInfosPref3[idL].blocked)
            || (idW == 4 && PlayerPrefs.PP.mapInfosPref4[idL].blocked)
            || (idW == 5 && PlayerPrefs.PP.mapInfosPref5[idL].blocked)
            || PlayerPrefs.PP.worldUnlock == idW && PlayerPrefs.PP.mapUnlockId < idL)
        //si on est sur le dernier monde unlock, et que le level est plus loins que le dernier monde unlock...
        //if (PlayerPrefs.PP.worldUnlock == idW && PlayerPrefs.PP.mapUnlockId < idL)
        {
            central.SetActive(false);
            customBut.interactable = false;
            picBlocked.SetActive(true);
        }
        else
        {
            central.SetActive(true);
            customBut.interactable = true;
            picBlocked.SetActive(false);
            for (int i = 0; i < PlayerPrefs.PP.getLevels(idWorld, idLevel).succes; i++)
            {
                centralChild[i].SetActive(true);
                
            }
            for (int j = PlayerPrefs.PP.getLevels(idWorld, idLevel).succes; j < 4; j++)
            {
                centralChild[j].SetActive(false);
                
            }
            powerAnts.transform.GetChild(0).gameObject.SetActive(MapPrefs.PP.getLevels(idWorld, idLevel).white);
            powerAnts.transform.GetChild(1).gameObject.SetActive(MapPrefs.PP.getLevels(idWorld, idLevel).blue);
            powerAnts.transform.GetChild(2).gameObject.SetActive(MapPrefs.PP.getLevels(idWorld, idLevel).red);
            powerAnts.transform.GetChild(3).gameObject.SetActive(MapPrefs.PP.getLevels(idWorld, idLevel).yellow);
            powerAnts.transform.GetChild(4).gameObject.SetActive(MapPrefs.PP.getLevels(idWorld, idLevel).green);
        }

        if (TL == TypeLevel.LEVEL)
        {
            powerAnts.gameObject.SetActive(customBut.interactable);
        }
    }

    /// <summary>
    /// lors de la selection du bouton
    /// </summary>
    public void OnSelect()
    {
        if (TL == TypeLevel.PREVWORLD || TL == TypeLevel.NEXTWORLD)
        {
            return;
        }
        if ((idWorld == 1 && PlayerPrefs.PP.mapInfosPref1[idLevel].blocked)
            || (idWorld == 2 && PlayerPrefs.PP.mapInfosPref2[idLevel].blocked)
            || (idWorld == 3 && PlayerPrefs.PP.mapInfosPref3[idLevel].blocked)
            || (idWorld == 4 && PlayerPrefs.PP.mapInfosPref4[idLevel].blocked)
            || (idWorld == 5 && PlayerPrefs.PP.mapInfosPref5[idLevel].blocked)
            || PlayerPrefs.PP.worldUnlock == idWorld && PlayerPrefs.PP.mapUnlockId < idLevel)
        {
            //blocked
            powerAnts.gameObject.SetActive(false);
            fadePanel.fadeOut = false;
        }
        else
        {
            //map débloqué
            if (!powerAnts.gameObject.activeSelf)
                powerAnts.gameObject.SetActive(true);
            //powerAnts.fadeOut = false;
        }
            
    }

    public void OnDeselect()
    {
        if (TL == TypeLevel.PREVWORLD || TL == TypeLevel.NEXTWORLD)
        {
            return;
        }
        fadePanel.fadeOut = true;
        //powerAnts.fadeOut = true;
    }
}
