using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipYVhibkU2bXlCd28%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipYVhibkU2bXlCd28
see:
https://drive.google.com/file/d/0Byzet-SVq6ipamRDZW1jNjhta28/view
*/
/// <summary>
/// CollideWithWalls se charge de tester si les objets touche les 4 murs ou le UpTriggerCanvas (le 5ème mur).
/// - Si les joueurs touche/sort des 4 murs: (si l'objet associé à le tag fromPlayer):
///     change isCloseToWalls du joueur à vrai / faux
/// - Si les joueurs / les métaux(non kinematic) touches/sort du 5ème mur
///     (si le collider other à le tag triggersWalls (le 5ème mur)): fade In/out
/// </summary>

public class CollideWithWalls : MonoBehaviour
{
    /// <summary>
    /// variable privé
    /// </summary>

    public GameObject refParent;                      //référence du parent

    /// <summary>
    /// initialise en début de jeu le parent
    /// </summary>
    private void Awake()
    {
        //parent = gameObject.transform.parent.gameObject;
    }

    /// <summary>
    /// Lorsqu'un objet rentre en collision avec le trigger attaché à CollideWithWalls
    /// si l'objet est de type "wall" et que la référencec du parent existe encore: change isCloseToWalls à true
    /// sinon si c'est un objet de type triggerWalls, fade l'objet !
    /// </summary>
    /// <param name="other">objet en collision avec nous</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wall" && refParent)
        {
            //Debug.Log("ici " + gameObject.name + " touche: " + other.name);
            if (refParent.GetComponent<BoolCloseToWalls>())
                refParent.GetComponent<BoolCloseToWalls>().isCloseToWalls = true;
            else
                Debug.LogError("error");
        }
    }

    //idem en inverse du Enter
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall" && refParent)
        {
            //Debug.Log("ici " + gameObject.name + " sort: " + other.name);
            if (refParent.GetComponent<BoolCloseToWalls>())
                refParent.GetComponent<BoolCloseToWalls>().isCloseToWalls = false;
            else
                Debug.LogError("error");
        }
    }

    /// <summary>
    /// détermine le fade de la liste toFade (vrai ou faux)
    /// </summary>
    /// <param name="fade">vrai ou faux</param>
    /*public void fadeObject(bool fade)
    {
        for (int i = 0; i < toFade.Length; i++)
        {
            if (toFade[i].GetComponent<FadeObjectInOut>())
                toFade[i].GetComponent<FadeObjectInOut>().fadeOut = fade;
        }
    }*/
}
