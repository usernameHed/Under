using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool paused = false;
    public bool isChronoRespawn = false;
    /// <summary>
    /// 
    /// </summary>

    private bool isStoped = false;
    private GameObject gameManager;
    private int multi = 1;                                                          //variable qui va définir si on ajoute ou enlève 1 si on est en multi;
    private int indexScriptCPB = 0;                                                 //index du script ColoredProgressBar, en tre 0 ou 1
    private ColoredProgressBar[] CPB;
    private bool tmpStop = false;
    private bool alreadyEnabled = false;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        CPB = gameObject.GetComponents<ColoredProgressBar>();
    }

    // Use this for initialization
    void Start()
    {
        if (gameManager.GetComponent<GameManager>().multi)                          //si on est en mode multi, définir multi = -1;
        {
            multi = -1;
            indexScriptCPB = 1;                                                     //change l'index à 1 pour récupérer le 2ème script
        }
        if (isChronoRespawn)
        {
            indexScriptCPB = 0;
            multi = 1;
        }
        StartCoroutine(keepSubstract());
    }

    public void addTime(int time)
    {
        CPB[indexScriptCPB].setProgress(CPB[indexScriptCPB].getProgress() + time, true);
    }

    public void setTime(int time)
    {
        /*if (time == -1) //alors ça vient de GameManager, on veut mettre le compteur à Warning time si au dessus, OU laisser si en dessous
        {
            int progress = gameObject.GetComponent<ColoredProgressBar>().getProgress();
            int warningTime = gameObject.GetComponent<ColoredProgressBar>().warningTime;
            Debug.Log("progress: " + progress + ", warning: " + warningTime);
            if (progress > warningTime)
                gameObject.GetComponent<ColoredProgressBar>().setProgress(warningTime, true);
            return;
        }*/
        CPB[indexScriptCPB].setProgress(time, true);
    }

    /// <summary>
    ///  - Si mode solo: ajoute 1 (multi = 1);
    ///  - Si mode multi: soustrait 1 (multi = -1);
    /// </summary>
    /// <returns></returns>
    IEnumerator keepSubstract()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            if (!isStoped)
            {
                
                CPB[indexScriptCPB].setProgress(CPB[indexScriptCPB].getProgress() + multi, true);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void pauseForXSecond(float second)
    {
        tmpStop = isStoped;
        isStoped = true;
        StartCoroutine(pauseCoroutineForXSecond(second));
    }

    //freez le timer pendant X second
    IEnumerator pauseCoroutineForXSecond(float second)
    {
        yield return new WaitForSeconds(second);
        isStoped = tmpStop;
    }

    private void OnEnable()
    {
        if (alreadyEnabled)
        {
            StopAllCoroutines();
            StartCoroutine(keepSubstract());
        }
    }

    // Update is called once per frame
    void Update ()
    {
		if (paused && !isStoped)
        {
            StopAllCoroutines();
            isStoped = true;
        }
        if (!paused && isStoped)
        {
            StopAllCoroutines();
            StartCoroutine(keepSubstract());
            
            isStoped = false;
        }

        //////////////////////////////////// respawn
        if (!paused)
        {
            if (isChronoRespawn)
            {
                //if (keepSubstract.)
                if (CPB[indexScriptCPB].getProgress() == CPB[indexScriptCPB].max)
                {
                    alreadyEnabled = true;
                    //si le refPlayer existe, on est en mode multi, avec qu'un seul player
                    if (gameObject.transform.parent.parent.gameObject.GetComponent<TrailPlayerId>().refPlayer)
                        gameManager.GetComponent<GameManager>().respawnPlayer(gameObject.transform.parent.parent.gameObject.GetComponent<TrailPlayerId>().refPlayer);
                    //sinon, on est en mode solo, avec +r fourmis pour 1 joueur
                    else
                        gameManager.GetComponent<GameManager>().respawnPlayer(gameObject.transform.parent.parent.gameObject.GetComponent<TrailPlayerId>().refPlayerSolo);
                    CPB[indexScriptCPB].setProgress(0);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
