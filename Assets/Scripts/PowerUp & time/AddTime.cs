using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTime : MonoBehaviour
{
    public int timeToAdd = 10;
    public bool autoDestroy = true;
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;

    // Use this for initialization
    [Header("debug")]
    private GameObject chrono;

    /// <summary>
    /// variable privée
    /// </summary>

    private float timeToGo;
    private Vector3 scaleBase = new Vector3(0.3f, 0.3f, 0.3f);
    private Vector3 scaleMin = new Vector3(0.2f, 0.2f, 0.2f);
    private bool notOk = false;
    private GameObject gameManager;
    private int multi = -1;                                                 //dans le mode solo, on réduit le temps avant que le parasite éclos

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        if (gameManager)
            chrono = gameManager.GetComponent<GameManager>().Chrono;
    }

    private void Start()
    {
        if (gameManager && gameManager.GetComponent<GameManager>().multi)                  //si on est en mode multi, ajouter du temps
            multi = 1;
    }

    /// <summary>
    /// recherche à nouveau le timer
    /// </summary>
    void debugChrono()
    {
        if (!gameManager)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            if (gameManager)
            {
                chrono = gameManager.GetComponent<GameManager>().Chrono;
                if (gameManager.GetComponent<GameManager>().multi)                  //si on est en mode multi, ajouter du temps
                    multi = 1;
            }  
        }
    }

    /// <summary>
    /// ajouter au timer
    /// </summary>
    void AddToChrono()
    {
        if (gameObject.GetComponent<TimeWithNoEffect>())
            gameObject.GetComponent<TimeWithNoEffect>().isOk = false;
        notOk = true;
        if (!chrono)            //debug
            debugChrono();
        if (chrono && chrono.GetComponent<Timer>())
            chrono.GetComponent<Timer>().addTime(timeToAdd * multi);
    }

    void ChangeWhenWaiting(bool swap)
    {
        if (swap)
            gameObject.transform.localScale = scaleMin;
        else
            gameObject.transform.localScale = scaleBase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (autoDestroy)
            {
                AddToChrono();
                Destroy(gameObject);
            }
            else
            {
                if (gameObject.GetComponent<TimeWithNoEffect>() && gameObject.GetComponent<TimeWithNoEffect>().isOk)
                {
                    ChangeWhenWaiting(true);
                    AddToChrono();
                }
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.fixedTime >= timeToGo)
        {
            if (notOk && gameObject.GetComponent<TimeWithNoEffect>() && gameObject.GetComponent<TimeWithNoEffect>().isOk)
                ChangeWhenWaiting(false);
            transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
