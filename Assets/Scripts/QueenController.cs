using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
edit:
https://www.draw.io/#G0Byzet-SVq6ipYWRITDlUaGdYZGs
see:
https://drive.google.com/file/d/0Byzet-SVq6ipbmJXZjJxUmZsV1E/view
*/
/// <summary>
/// Active son objectivePointer Dès qu'elle a été vue une première fois.
/// Dès qu'un oeuf entre en collision, le détruit, et ajoute le score au joueurs
/// </summary>

public class QueenController : MonoBehaviour
{
    [Range(0, 10.0f)]    public float timeOpti = 1.0f;  //optimisation
    public bool isAlreadySee = false;
    public int nb_team = 0;
    private bool winnedIntern = false;

    /// <summary>
    /// variable privé
    /// </summary>
    [SerializeField] private Animator anim;

    private float timeToGo;
    private GameObject gameController;
    private GameManager gameManager;
    private TestEndofLevel testEndOfLevel;

    /// <summary>
    /// initialise le gameController pour pouvoir l'utiliser
    /// </summary>
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController)
        {
            gameManager = gameController.GetComponent<GameManager>();
            testEndOfLevel = gameController.GetComponent<TestEndofLevel>();
        }
    }

    /// <summary>
    /// initialise l'optimisation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }

    /// <summary>
    /// Si l'objet est un oeuf,
    /// désactiver sa SphereCollider (debug ?)
    /// Ajouter au GameManager le score et la trahison du player
    ///     -> récupére dans EggsController la team
    ///     -> récupère dans EggsController le score (1, 6 ou 12)
    ///     -> l'envoyer au gameManager avec les fonctions addToScore et addTreasonToTeam
    /// Détruire l'oeuf
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        EggsController EC = other.GetComponent<EggsController>();
        if (other.tag == "Eggs" && EC)
        {
            if (!gameManager.multi)                         //Si on est en mode solo
            {
                other.GetComponent<SphereCollider>().enabled = false;
                testEndOfLevel.addEggsToQueen(1);              //ajoute 1 oeuf à la reine
                if (!winnedIntern)
                    anim.SetBool("getEggs", true);
                EC.destroyThis(true);                       //détruire l'oeuf (appeler l'a méthode de destruction de l'oeuf !
            }
            else                                            //sinon, on est en mode multi
            {
                if (!EC.addNotTwice)
                {
                    //gameManager.addEggsToQueen(1, nb_team);  //ajoute 1 oeuf à la reine de la bonne team
                    EC.changeCurrentPlayer(nb_team);         //change la couleur de l'oeuf par rapport à la team de la reine (1->bleu ou 2->rouge !)
                    if (!winnedIntern)
                        anim.SetBool("getEggs", true);
                    EC.addNotTwice = true;  
                }
            }
        }
    }

    /// <summary>
    /// lorsqu'on gagne, la reine danse !
    /// </summary>
    public void winned()
    {
        winnedIntern = true;
        anim.SetBool("win", true);
        anim.Play("Idle3");
    }

    /// <summary>
    /// lorsqu'un oeuf sort (en mode multi), il n'est plus dans la liste
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (!gameManager || !gameManager.multi || winnedIntern)                          //ce code ne marche qu'en multijoueur, en solo, les oeuf ne peuvent pas "sortir" vu qu'il sont détruit...
            return;
        EggsController EC = other.GetComponent<EggsController>();
        if (other.tag == "Eggs" && EC)
        {
            //gameManager.addEggsToQueen(-1, nb_team);     //ajoute 1 oeuf à la reine de la bonne team            
            EC.changeCurrentPlayer(0);                                                  //change la couleur de l'oeuf à blanc
            EC.addNotTwice = false;
        }
    }

    /// <summary>
    /// Si la rien est à l'écran,
    /// set isAlreadySee = vrai,
    /// et active l'objectiveIndicator
    /// </summary>
    private void testActivePointer()
    {
        if (gameObject.GetComponent<IsOnScreen>() && gameObject.GetComponent<ObjectiveIndicator>() && gameObject.GetComponent<IsOnScreen>().isOnScreen)
        {
            gameObject.GetComponent<ObjectiveIndicator>().enabled = true;
            isAlreadySee = true;
        }
    }

    private void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            if (!isAlreadySee)
                testActivePointer();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
