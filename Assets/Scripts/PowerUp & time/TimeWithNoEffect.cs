using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipNUtBT3lNdG95UU0%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipNUtBT3lNdG95UU0
see:
https://drive.google.com/file/d/0Byzet-SVq6ipRWtCRTRwMWVZZ0U/view
*/

/// <summary>
/// Quand La variable IsOk est désactivé, le script réactive cette variable TimeWithNoEffect seconde après
/// (la variable restart permet de réactualisé le temps au cas ou IsOk à déja été désactivé)
/// - utile pour les portals: pour qu'il y ai un temps minimal entre 2 téléportation.
/// Idem pour les powerUp ayant un intervalle entre 2 utilisation.
/// </summary>

public class TimeWithNoEffect : MonoBehaviour
{
    [Range(0, 10f)]             public float timeOpti = 0.1f;           //optimisation du temps
    [Range(0, 300f)]             public float timeWithNoEffect = 1.0f;  //temps d'innactivité
    public string whoUseThis = "nobody";                                //savoir qui utiilise ce script
    public bool isOk = true;                                            //dès que c'est false, commence le timer, et se remet à true quand le timer est fini
    public bool restart = false;                                        //recommence le timer
    public int additionnalTime = 1;                                    //temps d'attente additionnnel

    /// <summary>
    /// variable privé
    /// </summary>

    private float timeToGo;                                             //optimisation
    private bool isOkIsFalse = false;                                   //tmp pour le restart
    private float timeStartWithNoEffect;                                //le temps de fin (détermine la fin du timer)

    /// <summary>
    /// Initialise l'optimisation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }

    private void Update()
    {
        if (Time.fixedTime >= timeToGo)                                             //optimisation du CPU
        {
            if (restart)                                                            //recommence le timer
            {
                isOkIsFalse = false;
                isOk = false;
                restart = false;
            }
            if (!isOk && !isOkIsFalse)                                              //le début du timer
            {
                isOkIsFalse = true;
                timeStartWithNoEffect = Time.fixedTime + (timeWithNoEffect * additionnalTime);
            }
            if (!isOk && isOkIsFalse && Time.fixedTime >= timeStartWithNoEffect)    //la fin du timer
            {
                isOk = true;
                //Debug.Log("ok amazing !");
                additionnalTime = 1;
                isOkIsFalse = false;
            }
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
