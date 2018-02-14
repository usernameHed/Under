using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestEndofLevel : MonoBehaviour
{
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    public List<GameObject> QueenMulti = new List<GameObject>();// QueenMulti;                                   //référence sur la queen 2 du mode multi

    /// <summary>
    /// private serealized
    /// </summary>
    public GameObject QueenSolo;                                   //référence sur la queen sur mode solo
    
    [HideInInspector]    public List<GameObject> PanelPlayer;                                   //référence sur la queen 2 du mode multi

    [Space(10)]
    [Header("Objects end")]                                                             //affichage de texte de fin de jeu
    [SerializeField]    private GameObject TimeOver;                                    //le temps est terminé
    [SerializeField]    private GameObject GoldenBall;                                  //le ballon d'or ! les 2 équipe en multi sont exéco
    [SerializeField]    private GameObject GoldenBallTeamWin;                           //Une équipe à gagné le ballon d'or !
    [SerializeField]    private GameObject QueenIsFull;                                 //une des reines est pleine
    [SerializeField]    private GameObject QueenIsFullMulti;                            //une des reines en multi est pleine
    [SerializeField]    private GameObject AllEggsInfected;                             //tout les oeufs on été infecté en solo !)
    [SerializeField]    private GameObject AllPlayerDead;                               //tout les joueurs sont mort !
    [SerializeField]    private GameObject LevelFailed;                               //tout les joueurs sont mort !
    [SerializeField]    private GameObject GameEnd;                                     //le jeu est terminé
    [SerializeField]    private GameObject TeamWinByKillingOther;                       //l'une des team est morte
    [SerializeField]    private GameObject QueenHaveEnoughtEggs;                       //l'une des team est morte
    [SerializeField]    private GameObject YouShouldTryHarder;                       //message de la reine quand le joueur a perdu
    [SerializeField]    private GameObject NiceDoneLittleAnts;                       //message de la reine quand le joueur a gagné


    /// <summary>
    /// private
    /// </summary>

    private float timeToGo;
    private GameManager GM;
    private GameObject cam;
    private CameraController CamControl;
    private int tmpScoreTeam1 = -1;
    private int tmpScoreTeam2 = -1;
    private bool isAlreadyOnFinish = false;
    private bool waitForLevelFailed = false;                                            //lorsque le texte de la reine à fini
    private bool winnedByKilling = false;
    private bool isAlreadySave = false;
    private bool allEggsAreGivenTwice = false;                                          //vérifie que le test des oeufs donné à la reine ne se fasse qu'une seul fois
    //private bool multiAndCamera

    [HideInInspector] public bool levelFailedMulti = false;
    [HideInInspector] public bool levelFailedSolo = false;

    /// <summary>
    /// initialise les valeurs
    /// </summary>
    private void Awake()
    {
        GM = gameObject.GetComponent<GameManager>();                                //récupère le gameMaanger
        cam = Camera.main.gameObject;                                               //récupère la caméra
        CamControl = cam.GetComponent<CameraController>();
    }

    // Use this for initialization
    void Start ()
    {
        timeToGo = Time.fixedTime + timeOpti;
        
    }

    public void initQueenLinks(GameObject queen)
    {
        QueenSolo = queen;
        YouShouldTryHarder.GetComponent<GuiFollow>().WorldObject = QueenSolo;       //set la queen au text 
        NiceDoneLittleAnts.GetComponent<GuiFollow>().WorldObject = QueenSolo;       //set la queen au text
    }

    /// <summary>
    /// quand on gagne, les fourmis danse !
    /// </summary>
    /// <param name="winType"></param>
    void LoopPlayer(int winType = 0)
    {
        for (int j = 0; j < GM.TargetsPlayers.Count; j++)
        {
            if (winType == 0 || (winType > 0 && winType == GM.TargetsPlayers[j].GetComponent<PlayerController>().nb_team))
                GM.TargetsPlayers[j].GetComponent<PlayerController>().winned(true);
            else
                GM.TargetsPlayers[j].GetComponent<PlayerController>().winned(false);
        }
        //type = 0 -> solo danse
        //type = 1 -> multi 1 danse
        //type = 2 -> multi 2 danse
    }

    /// <summary>
    /// - Si en mode solo:
    /// Ajoute un oeuf à la reine, augmente de 1 le progressBar
    /// 
    /// - Si en mode multo:
    /// Ajoute ou supprime 1 oeuf à la team 1 ou 2
    /// </summary>
    /// <param name="value">nombre d'oeuf</param>
    /// <param name="type">id de la team (1 ou 2)</param>
    public void addEggsToQueen(int value, int type = 1)
    {
        if (!GM.multi)                 //si on est en mode solo...
        {
            GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().setProgress(GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().currentProgress + value);
            if (GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().getProgress() == GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().max)
            {
                if (!GM.queenIsFull)
                {
                    QueenSolo.GetComponent<QueenController>().winned();
                    LoopPlayer(0);
                }
                    
                GM.queenIsFull = true;
                CamControl.alternativeFocus = null;
            }
                
        }
        else                        //on est en mode multi
        {
            if (type == 1)          //change la team 1
            {
                GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().setProgress(value);
                //si le jeu était en attente du ballon d'or, OU si la team 1 à atteint le max, win !
                if (GM.waitForGoldenBall || GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().getProgress() == GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().max)
                {
                    GM.winnerTeamMulti = 1;
                    if (!GM.queenIsFull)
                    {
                        QueenMulti[0].GetComponent<QueenController>().winned();
                        LoopPlayer(1);
                    }
                        
                    GM.queenIsFull = true;

                    //si l'on a gagné par balon d'or
                    if (GM.waitForGoldenBall)
                        GM.winnedByGoldenBall = true;
                }
            }
            else                    //change la team 2
            {
                GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().setProgress(value);
                //si le jeu était en attente du ballon d'or, OU si la team 2 à atteint le max, win !
                if (GM.waitForGoldenBall || GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().getProgress() == GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().max)
                {
                    GM.winnerTeamMulti = 2;
                    if (!GM.queenIsFull)
                    {
                        QueenMulti[1].GetComponent<QueenController>().winned();
                        LoopPlayer(2);
                    }
                        
                    GM.queenIsFull = true;

                    //si l'on a gagné par balon d'or
                    if (GM.waitForGoldenBall)
                        GM.winnedByGoldenBall = true;
                } 
            }

        }
    }

    /// <summary>
    /// compte le nombre d'eggs coloré de la team 1 et 2 et change leurs score dans les compteurs !
    /// </summary>
    void countPointOfTeamInMulti()
    {
        if (tmpScoreTeam1 == -1 || tmpScoreTeam2 == -1) //change le tmpScore au début au state des chrono progress
        {
            tmpScoreTeam1 = GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().getProgress();
            tmpScoreTeam2 = GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().getProgress();
        }

        int countTeam1 = 0;                                                     //le nombre d'oeuf de la team1
        int countTeam2 = 0;                                                     //le nombre d'oeuf de la team2
        for (int i = 0; i < GM.groupEggs.transform.childCount; i++)                //parcourt chaque oeuf
        {
            EggsController eggs = GM.groupEggs.transform.GetChild(i).GetComponent<EggsController>();
            if (eggs)                                                           //test si cet oeuf existe
            {
                if (eggs.getCurrentPlayer() == 1)                               //si l'oeuf est de la couleurs de la team1
                    countTeam1++;                                               //ajouter 1 à team1
                else if (eggs.getCurrentPlayer() == 2)                          //si l'oeuf est de la cloueur de la team2
                    countTeam2++;                                               //ajouter 1 à team2
            }
        }
        //si le nombre d'oeuf de la team1 ou 2 on changé, les changer dans les scores !
        if (countTeam1 != tmpScoreTeam1)
        {
            addEggsToQueen(countTeam1, 1);  //ajoute 1 oeuf à la reine de la team 1
            tmpScoreTeam1 = countTeam1;
        }
        if (countTeam2 != tmpScoreTeam2)
        {
            addEggsToQueen(countTeam2, 2);  //ajoute 1 oeuf à la reine de la team 1
            tmpScoreTeam2 = countTeam2;
        }
    }

    /// <summary>
    /// test quand les variable multi ou solo levelFailed sont activé !
    /// si oui, afficher dès que necessaire le menu "level failed"
    /// </summary>
    void testWhenStopWhenItFailed()
    {
        //lorsuqe les joueurs sont mort en multi et que le texte "All Player Dead" est fini, on affiche le menu levelFail
        if (levelFailedMulti && !AllPlayerDead.activeSelf)
            GM.LMG.activeLevelFailed();

        //lorsque les joueurs sont mort en solo et que la caméra est arrivé sur la reine, affiche un texte
        if (levelFailedSolo && CamControl.isOnAlternativeFocus && !waitForLevelFailed)
        {
            waitForLevelFailed = true;
            YouShouldTryHarder.SetActive(true);
        }
        //lorsque ce texte est fini, affiche le menu levelFailed
        if (waitForLevelFailed && !YouShouldTryHarder.activeSelf)
        {
            GM.LMG.activeLevelFailed();
        }

        //si on est en multi, que la queen est full, SI ON A GAGNER en tuant les autres
        //et que la caméra est arrivé sur la reine gagnante
        //affiche un text qui sort de la reine
        if (GM.queenIsFull && GM.multi && CamControl.isOnFocusQueen && winnedByKilling && !GM.LMG.displayMenuOnce)
        {
            TeamWinByKillingOther.SetActive(true);
            StartCoroutine(displayMenuMultiSuccesWhenFinish());
        }
        //sinon, on a gagné en ayant le plus d'oeufs !
        else if (GM.queenIsFull && GM.multi && CamControl.isOnFocusQueen && !winnedByKilling && !GM.LMG.displayMenuOnce)
        {
            QueenHaveEnoughtEggs.SetActive(true);
            StartCoroutine(displayMenuMultiSuccesWhenFinish());
        }

        //si on est en solo et que la caméra est arrivé sur la reine
        if (GM.queenIsFull && !GM.multi && CamControl.isOnFocusQueen && !GM.LMG.displayMenuOnce)
        {
            NiceDoneLittleAnts.SetActive(true);
            StartCoroutine(displayMenuSoloSuccesWhenFinish());
        }
    }

    /// <summary>
    /// affiche le menu de fin de jeu solo quand on a gagné
    /// </summary>
    /// <returns></returns>
    IEnumerator displayMenuSoloSuccesWhenFinish()
    {
        yield return new WaitForSeconds(4);
        GM.LMG.activeLevelSoloSuccess();
    }

    /// <summary>
    /// affiche le menu de fin de jeu multi quand une équipe à gagné !
    /// </summary>
    /// <returns></returns>
    IEnumerator displayMenuMultiSuccesWhenFinish()
    {
        yield return new WaitForSeconds(3);
        GM.LMG.activeLevelMultiSuccess();
    }

    //c'est la fin du jeu, est appelé quand en multi tout les joueurs sont mort, OU en solo quand la caméra est arrivé à l'alternative focus !
    public void endOfLevel()
    {
        Debug.Log("ici c'est la vrai fin !");
        
        /*if (GM.endOfTheWorld && QueenSolo.GetComponent<IsOnScreen>() && QueenSolo.GetComponent<IsOnScreen>().isOnScreen)
        {
            TimeOver.SetActive(false);
            QueenIsFull.SetActive(false);
            AllEggsInfected.SetActive(false);
            AllPlayerDead.SetActive(false);
            GameEnd.SetActive(true);
            Debug.Log("ici c'est la vrai fin !");
        }*/
    }

    /// <summary>
    /// désactive tous les objective pointers
    /// </summary>
    public void desactivePointer()
    {
        GameObject co = GameObject.Find("Canvas_Objective");
        if (co)
            co.SetActive(false);
    }

    /// <summary>
    /// est appelé lorsque la partie est fini, parcourt la liste des players
    ///  - Si la variable stop est true, stop le joueur
    ///  - Si la variable stop est false, supprime le joueur de la caméra
    /// </summary>
    /// <param name="stop"></param>
    void desactivePlayers(bool stop = true)
    {
        for (int j = 0; j < GM.TargetsPlayers.Count; j++)
        {
            if (GM.TargetsPlayers[j])
            {
                if (stop)
                    GM.TargetsPlayers[j].GetComponent<PlayerController>().stopEverything();            //stop les joueurs (dans un premier temps)
                else
                    CamControl.deleteToCam(GM.TargetsPlayers[j]);            //ensuite les supprimes de la caméra
            }
        }
    }

    /// <summary>
    /// Désactive tous les objets dans la caméra
    /// </summary>
    void desactiveAllCam()
    {
        CamControl.Targets.Clear();
    }

    /// <summary>
    /// Quand cette fonction est appelé, après timeBeforGoToQueenEnd seconde,
    /// les joueurs se supprime de la caméra, et c'est la fin du jeu (presque)
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitBeforeFinish()
    {
        //wait X seconde
        isAlreadyOnFinish = true;
        yield return new WaitForSeconds(GM.timeBeforGoToQueenEnd);
        Debug.Log("ici la fin du... multi ???");
        if (GM.multi)
            GM.LMG.activeLevelMultiSuccess();
        else
            SaveWin();
    }

    public void SaveWin()
    {
        if (isAlreadySave)
            return;
        Debug.Log("ici Save win ?");
        isAlreadySave = true;

        desactivePlayers(false);                                            //supprime les joueurs de la caméra (il ne restera plus que.. rien, donc la reine par défaut)
        desactiveAllCam();                                                  //désactive tous les objets dans la caméra
        GM.endOfTheWorld = true;                                            //c'esst la fin...
        GM.LMG.activeLevelSoloSuccess();                                    //affiche l'écran de fin !
    }

    /// <summary>
    /// test si il reste des joueurs actif
    /// Les player prennent X seconde pour mourrir, on a besoin de connaitre la valeur
    /// de la variable active
    /// 
    /// Si tout les joueurs sont mort, désactive les timers Respawn des joueurs !
    /// </summary>
    public void testIfAllDead()
    {
        int playerActif = 0;
        for (int j = 0; j < GM.TargetsPlayers.Count; j++)
        {
            if (GM.TargetsPlayers[j] &&
                GM.TargetsPlayers[j].gameObject.activeSelf
                && GM.TargetsPlayers[j].gameObject.GetComponent<PlayerController>()
                && GM.TargetsPlayers[j].gameObject.GetComponent<PlayerController>().active)
                playerActif++;
        }
        if (playerActif == 0)
        {
            //TODO:... enlever ça
            GM.allPlayerDead = true;
            /*for (int j = 0; j < GM.TargetsPlayers.Count; j++)
            {
                GM.TargetsPlayers[j].GetComponent<PlayerController>().avatarPlayer.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
            }*/
        }       
    }

    /// <summary>
    /// test qui a gagné entre les 2 équipes si le temps est écoulé...
    /// </summary>
    bool testWhoWinMulti()
    {
        /*
        //si la team 1 est morte et pas la team 2, direct faire gagner la team 2
        if (GM.numberPlayerLeft(true, true) == 1)                           //si la team 2 est morte
        {
            //team 1 win !
            GM.queenIsFull = true;                                  //une reine a gagné
            GM.winnerTeamMulti = 1;
            TeamWinByKillingOther.GetComponent<GuiFollow>().WorldObject = QueenMulti[0];            //set le focus de la reine
            winnedByKilling = true;
            //TeamWinByKillingOther.SetActive(true);
            Debug.Log("placer le teamKill à la position de la reine");
            return (true);
        }
        else if (GM.numberPlayerLeft(true, true) == 2)                      //si la team 1 est morte...
        {
            //team 2 win !
            GM.queenIsFull = true;                                  //une reine a gagné
            GM.winnerTeamMulti = 2;
            TeamWinByKillingOther.GetComponent<GuiFollow>().WorldObject = QueenMulti[1];            //set le focus de la reine
            winnedByKilling = true;
            //TeamWinByKillingOther.SetActive(true);
            Debug.Log("placer le teamKill à la position de la reine");
            return (true);
        }
        */


        //si la team 1 à la plus de score, OU que toute la team 2 est morte...
        if (GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().getProgress() > GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().getProgress()
            /*|| GM.numberPlayerLeft(true, true) == 1*/)
        {
            GM.winnerTeamMulti = 1;
            CamControl.alternativeFocus = QueenMulti[GM.winnerTeamMulti - 1].transform;    //change le focus alternatif de la caméra à la reine gagnante !
            CamControl.clearTarget();
            desactiveRespawn();
            /*if (GM.numberPlayerLeft(true, true) == 1)
            {
                Debug.Log("ici 1");
                GM.timerEnd = false;                                    //désactive timerEnd (on a fini avec le temps, mais gagné car les autrres joueurs sont mort...)
                //TimeOver.SetActive(false);                              //cache le texte timerOver
                GoldenBall.SetActive(false);                            //cache le texte goldenBall si il a été affiché
                GM.queenIsFull = true;                                  //une reine a gagné
                QueenHaveEnoughtEggs.GetComponent<GuiFollow>().WorldObject = QueenMulti[0];            //set le focus de la reine
                //Debug.Break();
                GM.winnedByGoldenBall = false;                          //on a pas gagné par golderBall !
                return (false);
            }*/
            QueenIsFullMulti.SetActive(true);                                               //victoire en mode multi de la team X
            QueenHaveEnoughtEggs.GetComponent<GuiFollow>().WorldObject = QueenMulti[0];            //set le focus de la reine
            //Debug.Break();
            QueenIsFullMulti.GetComponent<Text>().text = "Team " + (GM.winnerTeamMulti) + " Win !";
            return (true);
        }
        //si la team 2 à la plus de score, OU que toute la team 1 est morte...
        else if (GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().getProgress() > GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().getProgress()
            /*|| GM.numberPlayerLeft(true, true) == 2*/)
        {
            GM.winnerTeamMulti = 2;
            CamControl.alternativeFocus = QueenMulti[GM.winnerTeamMulti - 1].transform;    //change le focus alternatif de la caméra à la reine gagnante !
            CamControl.clearTarget();
            desactiveRespawn();
            /*if (GM.numberPlayerLeft(true, true) == 2)
            {
                Debug.Log("ici 2");
                GM.timerEnd = false;                                    //désactive timerEnd (on a fini avec le temps, mais gagné car les autrres joueurs sont mort...)
                //TimeOver.SetActive(false);                              //cache le texte timerOver
                GoldenBall.SetActive(false);                            //cache le texte goldenBall si il a été affiché
                GM.queenIsFull = true;                                  //une reine a gagné
                QueenHaveEnoughtEggs.GetComponent<GuiFollow>().WorldObject = QueenMulti[1];            //set le focus de la reine
                //Debug.Break();
                GM.winnedByGoldenBall = false;                          //on a pas gagné par golderBall !
                return (false);
            }*/
            QueenIsFullMulti.SetActive(true);                                               //victoire en mode multi de la team X
            QueenHaveEnoughtEggs.GetComponent<GuiFollow>().WorldObject = QueenMulti[1];            //set le focus de la reine
            //Debug.Break();
            QueenIsFullMulti.GetComponent<Text>().text = "Team " + (GM.winnerTeamMulti) + " Win !";
            return (true);
        }
        else    //exéco, on fait le ballon d'or !
        {
            GoldenBall.SetActive(true);
            GM.waitForGoldenBall = true;
            GM.timerChorno.paused = true;                                 //met en pause le timer
            //égalité... ballon d'or !
            return (false);
        }
    }

    /// <summary>
    /// désactive tout les respawn des joueurs
    /// //parcourt le spanel pour trouver chaque respawn pour les désactiver !
    /// </summary>
    public void desactiveRespawn()
    {
        Debug.Log("desactive respawn");
        for (int i = 0; i < PanelPlayer.Count; i++)
        {
            if (PanelPlayer[i])
            {
                for (int j = 0; j < PanelPlayer[i].transform.childCount; j++)
                {
                    PanelPlayer[i].transform.GetChild(j).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// ici test les différentes fin possibles (timer, mort des joueurs, tout les oeuf parasité...
    /// </summary>
    public void testEndOfLevel()
    {
        if (GM.queenIsFull)                                                           //la reine à tout les oeuf qu'il faut pour gagner le jeu, en solo comme en multi !
        {
            if (allEggsAreGivenTwice)                                                   //on ne passe pas ici 2 fois !
                return;
            Debug.Log("all eggs given !");
            

            //si multi, le joueur X gagne
            if (GM.multi)                                                           //si on est en multi, focus la reine gagnante !
            {
                CamControl.alternativeFocus = QueenMulti[GM.winnerTeamMulti - 1].transform;    //change le focus alternatif de la caméra à la reine gagnante !
                CamControl.clearTarget();
                desactiveRespawn();
                GoldenBall.SetActive(false);

                QueenIsFullMulti.SetActive(true);                                               //victoire en mode multi de la team X
                QueenIsFullMulti.GetComponent<Text>().text = "Team " + (GM.winnerTeamMulti) + " Win !";
                QueenHaveEnoughtEggs.GetComponent<GuiFollow>().WorldObject = QueenMulti[GM.winnerTeamMulti - 1];            //set le focus de la reine

                if (GM.winnedByGoldenBall)                                                      //ajoute un texte si on gagne par ballon d'or ! (et qu'il n'y a pas 20/20 d'oeuf !)
                {
                    if ((GM.winnerTeamMulti == 1 && GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().getProgress() < GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().max)
                        || (GM.winnerTeamMulti == 2 && GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().getProgress() < GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().max))
                    {
                        //GoldenBallTeamWin.SetActive(true);
                        Debug.Log("set text golden to the queen with textMeshPro !");
                        //set le texte en parent
                    }
                }
            }
            else
            {
                QueenIsFull.SetActive(true);                                                //affiche le texte victoire en mode solo
                CamControl.clearTarget();
                desactiveRespawn();
            }
                


            desactivePointer();                                                         //désactive les objective pointer...
            desactivePlayers();                                                         //désactive les joueurs inputs...
            GM.timerChorno.paused = true;                                 //met en pause le timer
            if (!isAlreadyOnFinish)
                StartCoroutine(waitBeforeFinish());                                         //attend X seconde avant de finir...

            allEggsAreGivenTwice = true;
        }
        else if (GM.everyEggsAreInfect)                                                    //tout les oeufs sont infecté
        {
            AllEggsInfected.SetActive(true);
            Debug.Log("C'est perdu, tout les oeuf sont contaminé");
            //marche en mode multi et solo
            //la partie est perdu...
            desactivePointer();                                                         //désactive les objective pointer...
            desactivePlayers();                                                         //désactive les joueurs inputs...
            GM.timerChorno.paused = true;                                 //met en pause le timer
            if (!isAlreadyOnFinish)
                StartCoroutine(waitBeforeFinish());                                         //attend X seconde avant de finir...
        }
        else if (GM.timerEnd)                                  //le temps est fini ! on est en mode multi obligé ! (est changé dans ColoredProgressBar)
        {
            Debug.Log("timer end !");
            if (testWhoWinMulti())                                  //test qui gagne entre les 2 équipes
            {
                desactivePointer();
                desactivePlayers();
                GM.timerChorno.paused = true;                                 //met en pause le timer
                TimeOver.SetActive(false);
                Debug.Log("ici une team multi a gagné");
                if (!isAlreadyOnFinish)
                    StartCoroutine(waitBeforeFinish());
                GM.endOfTheWorld = true;
            }
        }
        /*else if (GM.allPlayerDead)                             //tous les joueurs sont mort
        {
            if (!GM.multi)
                return;
                Debug.Log("all player dead");
            GM.timerChorno.paused = true;                                 //met en pause le timer
            GM.endOfTheWorld = true;
            desactivePointer();
            desactiveRespawn();
            if (GM.multi)
            {
                AllPlayerDead.SetActive(true);
                CamControl.enabled = false;
                levelFailedMulti = true;
            }
            else
            {
                levelFailedSolo = true;
                LevelFailed.SetActive(true);
            }
        }*/

    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.fixedTime >= timeToGo && !GM.endOfTheWorld)
        {
            if (GM.multi)
                countPointOfTeamInMulti();                              //compte les scores des joueurs en mode multi
            testIfAllDead();                                            //check si tout les joueurs sont mort
            testEndOfLevel();                                           //test si c'est la fin du jeu
            timeToGo = Time.fixedTime + timeOpti;
        }
        testWhenStopWhenItFailed();
        //testQuenOnScreenEnd();
    }
}
