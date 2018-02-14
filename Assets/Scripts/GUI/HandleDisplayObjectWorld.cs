using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired.UI.ControlMapper;
using UnityEngine.UI;

public class HandleDisplayObjectWorld : MonoBehaviour
{
    public Text numberEggsText;
    public GameObject activeGui;
    public GameObject desactiveGui;
    public Text[] infoLock;
    public Color greenColor;
    public Color redColor;
    public CustomButton thisButton;
    public FadeObjectInOut fadePanel;

    public enum TypeWorld
    {
        CENTRE = 0,
        WORLD = 1,
    }
    public TypeWorld TW = TypeWorld.WORLD;

    /// <summary>
    /// change le nombre d'oeufs
    /// </summary>
    public void changeNumberEggs(int numberEggs)
    {
        numberEggsText.text = "x" + numberEggs;
    }

    /// <summary>
    /// selectionne si on est désacctivé
    /// </summary>
    public void OnSelect()
    {
        if (!thisButton.IsInteractable())
        {
            fadePanel.fadeOut = false;
        }
    }

    /// <summary>
    /// deselectionne si on est désacctivé
    /// </summary>
    public void OnDeselect()
    {
        if (!thisButton.IsInteractable())
        {
            fadePanel.fadeOut = true;
        }
    }


    /// <summary>
    /// change les couleurs des objectifs du mondes
    /// </summary>
    /// <param name="finishLevel">tous les niveaux du monde précédent sont-il fini ?</param>
    /// <param name="gotAllEggs">Le nombre suffisant de goldenEggs est atteint ?</param>
    public void changeColorTextLocked(bool finishLevel, bool gotAllEggs, int eggs)
    {
        if (TW == TypeWorld.CENTRE)
            return;
        if (finishLevel)
            infoLock[0].color = greenColor;
        else
            infoLock[0].color = redColor;

        if (gotAllEggs)
            infoLock[1].color = greenColor;
        else
            infoLock[1].color = redColor;
        infoLock[1].text = "- Get " + eggs + " eggs in total";
    }

    /// <summary>
    /// le monde est-il activé ou pas ?
    /// </summary>
    public void enableWorld(bool active)
    {
        if (TW == TypeWorld.CENTRE)
        {
            activeGui.SetActive(true);
            return;
        }
            
        if (active)
        {
            activeGui.SetActive(true);
            desactiveGui.SetActive(false);
        }
        else
        {
            activeGui.SetActive(false);
            desactiveGui.SetActive(true);
        }

    }
}
