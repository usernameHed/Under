using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewired.UI.ControlMapper;

public class LevelManagerGame : MonoBehaviour
{
    /// <summary>
    /// public
    /// </summary>
    public GameObject winSolo;                          //panel victoire solo
    public GameObject winMulti;                         //panel victoire multi
    public GameObject lose;                             //panel lose
    public GameObject exit;                             //panel exit
    public GameObject gamePadDisconnected;              //panel gamepad
    public bool gamePaused = false;
    public bool gamePausedCinematic = false;            //pause l ejeu en cinematic
    public List<int> idPlayerInGame = new List<int>();
    public int fpsApplication = 60;                                         //fps du jeu
    public bool displayMenuOnce = false;

    /// <summary>
    /// public & hide
    /// </summary>
    [HideInInspector] public PlayerConnected PC;

    /// <summary>
    /// private
    /// </summary>
    private GameObject Global;
    private TimeWithNoEffect TWNE;
    private TimeWithNoEffectGUI TWNEG;
    private GlobalVariableManager GVM;
    private QuitOnClick QOC;

    //private int locationQuit = 0;
    private FPSDisplay FPS;
    private bool joypadTmp = false;

    private GameManager gameManager;
    private GameObject canvasCinematic;
    private PlayVideo PV;
    private int nextType = 0;           //est-ce qu'on est en next level, next level bloqué, next world ou ending ?
    private Fading fade;

    /// <summary>
    /// private & serealize
    /// </summary>
    [Space(10)]
    [Header("private serealized")]
    [SerializeField] private CustomButton[] listButtonWinMulti;
    [SerializeField] private Text textWinMulti;
    [SerializeField] private GameObject[] listLogoWin;
    [SerializeField] private CustomButton[] listButtonLose;
    [SerializeField] private CustomButton[] listButtonEscape;
    [SerializeField] private CustomButton[] listButtonEscapeJoyPad;
    [SerializeField] private GameObject[] listWindowButton;
    [Space(30)]
    [SerializeField] private GameObject GoldenEggs;
    [SerializeField] private GameObject RecordTimes;
    [SerializeField] private GameObject currentTime;
    [SerializeField] private GameObject AdditionalTime;
    [SerializeField] private GameObject[] textButtonRestart;
    [SerializeField] private GameObject[] textButtonNext;


    /// <summary>
    /// init
    /// </summary>
    private void Awake()
    {
        Global = GameObject.FindGameObjectWithTag("Global");
        Application.targetFrameRate = fpsApplication;                      //limite fps to 60
        QualitySettings.vSyncCount = 1;
        

        if (Global)
        {
            GVM = Global.GetComponent<GlobalVariableManager>();
            PC = Global.GetComponent<PlayerConnected>();
            PC.LMG = this;
        }
        QOC = gameObject.GetComponent<QuitOnClick>();
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        TWNEG = gameObject.GetComponent<TimeWithNoEffectGUI>();
        FPS = gameObject.GetComponent<FPSDisplay>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        canvasCinematic = GameObject.FindGameObjectWithTag("CanvasForCinematics");
        fade = gameObject.transform.GetChild(0).GetComponent<Fading>();
        if (canvasCinematic)
        {
            PV = canvasCinematic.transform.GetChild(0).gameObject.GetComponent<PlayVideo>();
            if (!PV.isPlaying)
            {
                fade.enabled = true;
                fade.BeginFade(-1);
            }
        }
        //else


    }

    /// <summary>
    /// init
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        pauseAtStart();                     //met en pause le jeu dès le début si une des manette des joueurs est déconnecté

        pauseGameIfCinematicEnabled();
    }

    /// <summary>
    /// test si une cinématique est entrain d'être joué
    /// </summary>
    void pauseGameIfCinematicEnabled()
    {
        if (gameManager.IG.levelDataScript.isThereCinematic)           //s'il y a une cinématique, alors attendre qu'elle soit fini avant de commencer à jouer !
        {
            pauseGameCinematic();
        }
        else                                            //sinon, on peut détruire le canvas de chargement, et commencer le jeu !
        {
            //pauseGameCinematic();
            //StartCoroutine(destroyCanvas(2));
            Destroy(canvasCinematic);
        }
    }

    IEnumerator destroyCanvas(float time)
    {
        yield return new WaitForSeconds(time);
        pauseGameCinematic();
        Destroy(canvasCinematic);
    }

    /// <summary>
    /// pause le jeu quand il y a une cinematic
    /// </summary>
    void pauseGameCinematic()
    {
        if (!gamePausedCinematic)
        {
            gamePausedCinematic = true;
            Time.timeScale = 0;
            FPS.enabled = false;
        }
        else
        {
            gamePausedCinematic = false;
            Time.timeScale = 1;
            FPS.enabled = true;
        }
    }

    /// <summary>
    /// pause or unpause game (en affichant l'affihcage d'exit ou des manette);
    /// </summary>
    public void pauseGame(bool joypad)
    {
        if (displayMenuOnce)                      //si le level est perdu, ne met pas en pause et n'affiche pas de menu exit/joypad deconnecté
            return;
        joypadTmp = joypad;
        //if (isConnected && !gamePaused)
        //  return;
        if (joypad && exit.activeSelf && gamePaused)
        {
            gamePaused = false;
            exit.SetActive(false);
        }
            

        if (!gamePaused)
        {
            gamePaused = true;
            //locationQuit = 0;
            if (!joypad)
            {
                exit.SetActive(true);

                listButtonEscape[0].gameObject.SetActive(false);
                listButtonEscape[0].gameObject.SetActive(true);
                StartCoroutine(selectButton(listButtonEscapeJoyPad[0]));
                StartCoroutine(selectButton(listButtonEscape[0]));
            }
            else
            {
                gamePadDisconnected.SetActive(true);
                StartCoroutine(selectButton(listButtonEscape[0]));
                StartCoroutine(selectButton(listButtonEscapeJoyPad[0]));
                //listButtonEscapeJoyPad[0].Select();
            }
            TWNE.isOk = false;
            Time.timeScale = 0;
            FPS.enabled = false;
        }
        else
        {
            gamePaused = false;
            if (!joypad)
                exit.SetActive(false);
            else
                gamePadDisconnected.SetActive(false);
            Time.timeScale = 1;
            FPS.enabled = true;
        }
    }

    /// <summary>
    /// appelé pour activer le menu levelFailed
    /// </summary>
    public void activeLevelFailed()
    {
        if (displayMenuOnce)      //cette fonction est appelé qu'une fois !
            return;
        EventAnalytics.EA.displayLevelRestart(PlayerPrefs.PP.lastLevelPlayerId[0] + "-" + PlayerPrefs.PP.lastLevelPlayerId[1]);

        displayMenuOnce = true;
        lose.SetActive(true);
        //listButtonLose[0].Select();
        StartCoroutine(selectButton(listButtonLose[0]));
        Time.timeScale = 0;
        FPS.enabled = false;
    }

    /// <summary>
    /// sauvegarde, gère la sauvegarde des temps et laffichage des étoiles etc...
    /// </summary>
    public void HandleStatsVictorySolo()
    {
        //si le résutat n'est pas déja sauvegardé
        //if (MapPrefs.PP.mapsInfosSolo[PlayerPrefs.PP.lastLevelPlayerId].timeHighScore.IndexOf(gameManager.Chrono.GetComponents<ColoredProgressBar>()[0].currentProgress) < 0)
        //{
            MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).timeHighScore.Add(gameManager.Chrono.GetComponents<ColoredProgressBar>()[0].currentProgress);
            MapPrefs.PP.Save();
        //}
        EventAnalytics.EA.OnLevelCompleted(true, PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1], gameManager.Chrono.GetComponents<ColoredProgressBar>()[0].currentProgress);
    }

    /// <summary>
    /// change le bouton next level / next wolrd / ending
    /// </summary>
    void changeBetweenNextLevelAndNextWorld()
    {
        if (PlayerPrefs.PP.isLastMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))    //si c'est la dernière map, change le bouton en "ending" (qui mène à une cinématique)
        {
            listWindowButton[0].SetActive(false);
            listWindowButton[1].SetActive(false);
            listWindowButton[2].SetActive(true);
            nextType = 3;
        }
        else if (PlayerPrefs.PP.isLastLevelMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))    //si c'est la dernière map d'un monde, change le bouton en "next World" (qui mène au menu)
        {
            listWindowButton[0].SetActive(false);
            listWindowButton[1].SetActive(true);
            listWindowButton[2].SetActive(false);
            nextType = 2;
            
        }
        else                                            //sinon, le bouton "next level" mène au level suivant !
        {
            listWindowButton[0].SetActive(true);
            listWindowButton[1].SetActive(false);
            listWindowButton[2].SetActive(false);
            nextType = 1;
        }
    }

    //[Button("highLight")]
    IEnumerator selectButton(CustomButton butt)
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log("select button ???????????");
        //StartCoroutine(selectButton2(butt));
        //if (!butt.Select)
        butt.Select();
    }

    /*IEnumerator selectButton2(CustomButton butt)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log("select button2 ???????????");
        StartCoroutine(selectButton3(butt));
    }
    IEnumerator selectButton3(CustomButton butt)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log("select button3 ???????????");
        butt.Select();
    }*/

    /// <summary>
    /// selectionne le bouton restart OU nextLevel, selon les 3 window groups
    /// </summary>
    /// <param name="restart"></param>
    void activeTheRightButton(bool restart)
    {
        if (nextType == 0 || nextType == 1) //window group 1 (next level
        {
            if (restart)
            {
                Debug.Log("select restart button ?");
                StartCoroutine(selectButton(listWindowButton[0].transform.GetChild(0).gameObject.GetComponent<CustomButton>()));
                //selectButton(listWindowButton[0].transform.GetChild(0).gameObject.GetComponent<CustomButton>());
            }
                
            else
            {
                Debug.Log("select next button");
                StartCoroutine(selectButton(listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<CustomButton>()));
                //selectButton(listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<CustomButton>());
                //listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<Button
                //listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<CustomButton>().Select();   //bouton next
                //listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<Button>().
            }
             

            if (nextType == 0)
            {
                listWindowButton[0].transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
                textButtonNext[0].SetActive(true);
            }
            else
                textButtonNext[1].SetActive(true);

        }
        else if (nextType == 2) //windowGroup 2 (next world)
        {
            if (restart)
                StartCoroutine(selectButton(listWindowButton[1].transform.GetChild(0).gameObject.GetComponent<CustomButton>()));
            else
                StartCoroutine(selectButton(listWindowButton[1].transform.GetChild(1).gameObject.GetComponent<CustomButton>()));
            textButtonNext[2].SetActive(true);
        }  
        else                    //window groupe 3 (ending)
        {
            if (restart)
                StartCoroutine(selectButton(listWindowButton[2].transform.GetChild(0).gameObject.GetComponent<CustomButton>()));
            else
                StartCoroutine(selectButton(listWindowButton[2].transform.GetChild(1).gameObject.GetComponent<CustomButton>()));
            textButtonNext[3].SetActive(true);
        }
    }

    /// <summary>
    /// affiche si on a fait un high score !
    /// </summary>
    void showHighScore(bool newScore, MapsInfos MI, int currentScore)
    {
        //set la différence entre l'ancien temps et le nouveau
        //(en mettant verrt ou rouge si on gagne ou perd du temps
        //appliquer un fade out ! (ou pas ?)

        
        //si on a encore jamais fais de high score... quitter !!!
        if (MI.bestHighScore == 999999)
        {
            //on a pas fait de high
            AdditionalTime.SetActive(false);
            return;
        }

        int scoreFinal = currentScore - MI.bestHighScore;
        if (scoreFinal >= 0)
            AdditionalTime.GetComponent<Text>().text = "+" + (scoreFinal) + " sec";
        else
            AdditionalTime.GetComponent<Text>().text = (scoreFinal) + " sec";

        if (MI.bestHighScore < currentScore)
            AdditionalTime.GetComponent<Text>().color = Color.red;
        else
            AdditionalTime.GetComponent<Text>().color = Color.green;
        AdditionalTime.SetActive(true);

    }

    /// <summary>
    /// déblock le next level
    /// Ainsi que les potentiel map bonus
    /// </summary>
    void HandleUnlockNextLevel()
    {

    }

    /// <summary>
    /// affiche/cache les oeufs qui'l faut
    /// </summary>
    void changeGoldenEggs()
    {
        int currentScore = gameManager.Chrono.GetComponents<ColoredProgressBar>()[0].currentProgress + gameManager.Chrono.GetComponents<ColoredProgressBar>()[0].addToFractionalPorgress;
        int success = 1;    //succes à 1: on a au moins le bronze arrivé la
        bool newEggs = false;   //est-ce qu'on a gagné un nouvelle oeufs dans ce jeu ?
        bool newScore = false;  //est-ce qu'on a fait un high score ?

        //affiche le temps courrant
        currentTime.GetComponent<Text>().text = "Time: " + currentScore + " sec";

        //détermine à quel niveau de succes nouvellement effectué
        //active le bon oeufs
        // - 1 = bronz
        // - 2 = silver
        // - 3 = gold
        // - 4 = epic
        for (int i = 0; i < MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).timeEggs2to4.Count; i++)
        {
            if (currentScore <= MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).timeEggs2to4[i])
                success++;
        }

        //création d'un nouveau MI (information de la map courrante)
        MapsInfos MI;

        MI.succes = PlayerPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).succes;//success;
        MI.bestHighScore = PlayerPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).bestHighScore;// currentScore;
        MI.blocked = PlayerPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).blocked; //map bloqué...

        //est-ce qu'on a gagné un nouvelle oeufs dans ce jeu ?
        if (success > PlayerPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).succes)
        {
            newEggs = true;
            MI.succes = success;
        }
        else
        {
            success = MI.succes;
        }


        //est-ce qu'on a fait un high score ?
        if (currentScore < PlayerPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]).bestHighScore)
        {
            newScore = true;
            showHighScore(newScore, MI, currentScore);
            MI.bestHighScore = currentScore;
        }
        else
            showHighScore(newScore, MI, currentScore);




        //sauvegarde les temps et succes réalisé sur la map (ou garde les anciens)
        PlayerPrefs.PP.setLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1], MI);
        //PlayerPrefs.PP.mapInfosPref[PlayerPrefs.PP.lastLevelPlayerId[0]][PlayerPrefs.PP.lastLevelPlayerId[1]] = MI;

        //si on a PAS gagné de Golden Eggs, MAIS qu'on a un nouveau record !
        if (!newEggs && !newScore)
            RecordTimes.transform.GetChild(0).gameObject.SetActive(true);           //set "nothing"
        else if (!newEggs && newScore)
            RecordTimes.transform.GetChild(1).gameObject.SetActive(true);           //set "new score"

        //ici lance la musique de victoire
        SoundManager.SS.MusicWin = true;

        //compare les succès pour un cas particulier pour chaque médailles:
        switch (success)
        {
            case 1:
                GoldenEggs.transform.GetChild(0).gameObject.SetActive(true);        //set bronz
                if (newEggs)
                {
                    RecordTimes.transform.GetChild(2).gameObject.SetActive(true);   //set le text du bronz
                }
                    
                break;
            case 2:
                GoldenEggs.transform.GetChild(0).gameObject.SetActive(true);        //set bronz
                GoldenEggs.transform.GetChild(1).gameObject.SetActive(true);        //set solver
                if (newEggs)
                {
                    RecordTimes.transform.GetChild(3).gameObject.SetActive(true);   //set le texte du silver
                }
                    
                break;
            case 3:
                GoldenEggs.transform.GetChild(0).gameObject.SetActive(true);        //set bronz
                GoldenEggs.transform.GetChild(1).gameObject.SetActive(true);        //set le silver
                GoldenEggs.transform.GetChild(2).gameObject.SetActive(true);        //set le gold
                if (newEggs)
                {
                    RecordTimes.transform.GetChild(4).gameObject.SetActive(true);   //set le text du gold
                }
                    
                break;
            case 4:
                GoldenEggs.transform.GetChild(0).gameObject.SetActive(true);        //set bronz
                GoldenEggs.transform.GetChild(1).gameObject.SetActive(true);        //set le silver
                GoldenEggs.transform.GetChild(2).gameObject.SetActive(true);        //set le gold
                GoldenEggs.transform.GetChild(3).gameObject.SetActive(true);        //set le epic
                if (newEggs)
                {
                    RecordTimes.transform.GetChild(5).gameObject.SetActive(true);   //set le text du epic
                }
                    
                break;
        }

        //si la prochaine map est nextLevel
        if (nextType == 1)
        {
            //si c'est une map bonus
            if (MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).isBonus)    //si la map suivante est une map bonus, check la bonus + la suivante !
            {
                if (success < MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 2).succesNeededForUnloclk)
                {
                    //nextType
                    Debug.Log("ICI ON A PAS REUSSI");
                    nextType = 0;   //impossible de passer au level suivant !
                }
            }
            else
            {
                Debug.Log("ici le test, pas de bonus");
                if (success < MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).succesNeededForUnloclk)
                {
                    //nextType
                    Debug.Log("ICI ON A PAS REUSSI");
                    nextType = 0;   //impossible de passer au level suivant !
                }
            }
        }

        //s'il reste encore des golden eggs à déverouiller, change le text
        //en "play again for the next"
        //sinon, "play agin to boost your current score"
        if (success < 3)
        {
            textButtonRestart[0].SetActive(true);
            textButtonRestart[1].SetActive(false);
            //sélectionne le boutton restart, car on peut faire un meilleur temps ! (parmis les 3 windowGroup)
            activeTheRightButton(false);
        }
        else
        {
            textButtonRestart[0].SetActive(false);
            textButtonRestart[1].SetActive(true);
            //active le bouton next level (on a les 3 étoiles !)
            activeTheRightButton(false);
        }



        Debug.Log("---------- debug next map deblocked ---------------");
        Debug.Log("worldunlock: " + PlayerPrefs.PP.worldUnlock);
        Debug.Log("mapUnlockId: " + PlayerPrefs.PP.mapUnlockId);
        Debug.Log("PlayerPrefs.PP.lastLevelPlayerId[0]" + PlayerPrefs.PP.lastLevelPlayerId[0]);
        Debug.Log("PlayerPrefs.PP.lastLevelPlayerId[1]" + PlayerPrefs.PP.lastLevelPlayerId[1]);
        Debug.Log("nextmapblocekd: " + PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]));
        Debug.Log("nextType: " + nextType);

        //si la map n'est pas la dernière...
        if (!PlayerPrefs.PP.isLastLevelMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))
        {
            //si la map suivante est une map bonnus...
            if (MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).isBonus)    //si la map suivante est une map bonus, check la bonus + la suivante !
            {
                unlockNextLevelBonus(success);
                //unlockNextLevel(1);
            }
            else
                unlockNextLevel();
        }

        

        //si le niveau suivant est un nouveau monde, on check si on l'a débloqué ?
        if (newEggs)
        {
            PlayerPrefs.PP.setupCountGoldenEggs();
            MapPrefs.PP.changeWorldUnlock();
        }

        PlayerPrefs.PP.Save();
    }

    /// <summary>
    /// débloque la map suivante
    /// </summary>
    void unlockNextLevel(int isBonus = 0)
    {
        //si la prochaine map du monde peut être débloqué
        if (PlayerPrefs.PP.worldUnlock == PlayerPrefs.PP.lastLevelPlayerId[0]
            && PlayerPrefs.PP.mapUnlockId == PlayerPrefs.PP.lastLevelPlayerId[1] + isBonus
            && nextType != 0
            && PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + isBonus))
        {
            Debug.Log("ICI SAVE QUE LA PROCHAINE MAP EST DEBLOQUE WOHOOO");
            PlayerPrefs.PP.mapUnlockId += 1;                                              //débloque la suite des map solo !
            //TODO: si on a pas débloqué le bonus en meme temps, probleme ???

            PlayerPrefs.PP.unlockTheNextLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + isBonus);
        }
    }
    void unlockNextLevelBonus(int success)
    {
        int tmpMapUnlock = PlayerPrefs.PP.mapUnlockId;
        bool alreadyJumped = true;
        //ici, d'abord tester si on a débloqué la map bonus + 1
        Debug.Log("ici nextLevel bonus: secces: " + success);
        //si la prochaine map du monde peut être débloqué
        if (PlayerPrefs.PP.worldUnlock == PlayerPrefs.PP.lastLevelPlayerId[0]
            && tmpMapUnlock == PlayerPrefs.PP.lastLevelPlayerId[1]
            && nextType != 0
            && PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1))
        {
            Debug.Log("ICI débloque la map après le bonus !");
            alreadyJumped = false;
            PlayerPrefs.PP.mapUnlockId += 2;                                              //débloque 2 (fait un bon de 1)
            //TODO: si on a pas débloqué le bonus en meme temps, probleme ???

            PlayerPrefs.PP.unlockTheNextLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1);
        }

        //si on n'est pas passé par le déblocage de la map suivante, et qu'on l'a déja débloqué en faite...
        if (alreadyJumped
            && !PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1)
            && PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1])
            && nextType != 0
            )
        {
            Debug.Log("ici  on test si maintenant on a réussi le niveau bonus...");
            if (success >= MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).succesNeededForUnloclk)
            {
                Debug.Log("ici YOUPI, on débloque le niveua bonus qu'on avait pas débloqué avant !");
                //ici on débloque le bonus, ok, donc +1 seulement.
                //PlayerPrefs.PP.mapUnlockId += 1;                                              //débloque la suite des map solo !

                PlayerPrefs.PP.unlockTheNextLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]);
            }
        }

        //////////////////////////////maintenant qu'on a débloqué (ou non) la map bonus + 1, regarder le bonus
        //ici pourquoi je passe pas ?

        else if (PlayerPrefs.PP.worldUnlock == PlayerPrefs.PP.lastLevelPlayerId[0]
            && tmpMapUnlock == PlayerPrefs.PP.lastLevelPlayerId[1]
            && nextType != 0
            && PlayerPrefs.PP.nextMapBlocked(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))
        {
            Debug.Log("ici on a réussi le niveau bonus...");
            if (success >= MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).succesNeededForUnloclk)
            {
                Debug.Log("ICI SAVE QUE LA PROCHAINE MAP EST DEBLOQUE WOHOOO");
                //ici on débloque le bonus, ok, donc +1 seulement.
                //PlayerPrefs.PP.mapUnlockId += 1;                                              //débloque la suite des map solo !

                PlayerPrefs.PP.unlockTheNextLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]);
            }
        }
    }

    /// <summary>
    /// appelé pour activer le menu succes
    /// </summary>
    public void activeLevelSoloSuccess()
    {
        if (displayMenuOnce)      //cette fonction est appelé qu'une fois !
            return;
        displayMenuOnce = true;

        changeBetweenNextLevelAndNextWorld();       //change le bouton next level/next world/ending

        changeGoldenEggs();                         //affiche/cache les oeufs qui'l faut

        PlayerPrefs.PP.fromRestart = false;

        //GoldenEggs


        winSolo.SetActive(true);
        HandleStatsVictorySolo();
    }

    /// <summary>
    /// appelé pour activer le win multi
    /// </summary>
    public void activeLevelMultiSuccess()
    {
        if (displayMenuOnce)      //cette fonction est appelé qu'une fois !
            return;
        displayMenuOnce = true;
        winMulti.SetActive(true);
        listButtonWinMulti[0].Select();
        Debug.Log("winner: " + gameManager.winnerTeamMulti);
        if (gameManager.winnerTeamMulti == 1)
        {
            textWinMulti.text = "Blue win !";
            listLogoWin[0].SetActive(true);
            listLogoWin[1].SetActive(false);
            listLogoWin[2].SetActive(false);
        }
        else if (gameManager.winnerTeamMulti == 2)
        {
            textWinMulti.text = "Red win !";
            listLogoWin[0].SetActive(false);
            listLogoWin[1].SetActive(true);
            listLogoWin[2].SetActive(false);
        }
    }

    /// <summary>
    /// victoire ! aller à la scène suivante !
    /// </summary>
    public void jumpNextScene()
    {
        Time.timeScale = 1;
        FPS.enabled = true;
        if (PlayerPrefs.PP.isLastMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))                                        //ending
        {
            //ici on lance la cinématique de fin de jeu
            Debug.Log("ici on lance la cinématique de fin de jeu");
            QOC.jumpToScene("X_Ending");
            return;
        }

        if (quitIfFromUnity())
            return;

        /*else if (PlayerPrefs.PP.nextLevel == "None")
        {
            Debug.Log("none, pas de nextScene quand on lance depuis la scène game directement");
            //QOC.Quit();
            //return;
        }*/
        //on va à nextLevel OU nextWorld, dans les 2 cas c'est géré dans le menu solo
        // -> soit c'est next level autorisé, et on passe directement au niveau suivant
        // -> soit c'est next world, et on va dans le menu Solo "classiquement"
        GVM.fromGame = true;
        handleCorectQuitForMainMenu();
        QOC.jumpToSceneWithFade("1_MainMenu");
    }

    /// <summary>
    /// fait en sorte que lors du retour au menu on soit à la bonne place
    /// </summary>
    void handleCorectQuitForMainMenu()
    {
        if (!GVM.multi)
        {
            GVM.backToMainMenu = 2; //back to menu, retourner au level joué
            if (PlayerPrefs.PP.lastLevelPlayerId[0] == 0)
                GVM.backToMainMenu = 1; //back to menu monde (comme si on avait fini un monde, et non un level)
        }
        else
        {
            GVM.backToMainMenu = 3; //back to menu, retourner au level joué
        }
    }

    bool quitIfFromUnity()
    {
        if (!GVM.fromMenu)
        {
            Debug.Log("NOP, lancé depuis la scène game directement");
            QOC.Quit();
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// restart la scène courante !
    /// </summary>
    public void restartScene()
    {
        Debug.Log("ici what ? restart !");
        pauseGame(false);
        Time.timeScale = 1;
        if (quitIfFromUnity())
            return;
        GVM.fromGame = false;
        PlayerPrefs.PP.fromRestart = true;
        if (!GVM.multi)
            QOC.jumpToSceneWithFade(PlayerPrefs.PP.jumpToSpecificLevel(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]));
        else
            QOC.jumpToSceneWithFade(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].nameLevel);
    }

    /// <summary>
    /// trouve les joueurs toujrous en vie mais avec leurs manette déconnecté, et les détruit !
    /// </summary>
    public void playButExecutePlayer()
    {
        //Debug.Log("playButExecutePlayer, cherche les joueur en vie avec leur manette deco (PC)");
        //if ()
        for (int i = 1; i <= 4; i++)
        {
            if (!PC.playerControllerConnected[i] && gameManager.isPlayerWithIdAlive(i))
                gameManager.deletePlayerWithId(i);
        }

        pauseGame(true);
    }

    /// <summary>
    /// si l'id du joueur est de 1 à 4, ou que le joueur de la manette est déconnecté
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool testIfIdIsPlayer(int id)
    {
        if (id == 0 || id == 5 || !idPlayerInGame.Contains(id))
            return (false);

        //test si le joueur est encore en vie
        if (!gameManager.isPlayerWithIdAlive(id))
            return (false);

        return (true);
    }

    /// <summary>
    /// Pause le jeu au démarage si l'un des joueurs n'a pas la manette connecté
    /// </summary>
    void pauseAtStart()
    {
        for (int i = 1; i <= 4; i++)
        {
            if (!PC.playerControllerConnected[i] && gameManager.isPlayerWithIdAlive(i))
            {
                pauseGame(true);
                return;
            }
        }
    }

    /// <summary>
    /// in game, afficher et bloquer le jeu si une manette est déconnecté
    /// </summary>
    public void handleDisconnect(int id, bool isConnected)
    {
        if (!testIfIdIsPlayer(id) || isConnected || gamePadDisconnected.activeSelf)
            return;
        pauseGame(true);
        //gamePadDisconnected.SetActive(!isConnected);
    }

    /// <summary>
    /// retourne au menu solo ou multi
    /// </summary>
    public void backToMenu()
    {
        if (quitIfFromUnity())
            return;

        Time.timeScale = 1;
        GVM.fromGame = false;

        handleCorectQuitForMainMenu();
        
        QOC.jumpToSceneWithFade("1_MainMenu");
    }

    /// <summary>
    /// gère les inputs UI pour le menu pause
    /// </summary>
    void testForEndQuicly()
    {
        if (gameManager.endOfTheWorld)
            return;
        if (PC.BAll() && (gameManager.TEL.levelFailedMulti || gameManager.TEL.levelFailedSolo))
        {
            Invoke("activeLevelFailed", 1);
            //activeLevelFailed();
        }
        else if (PC.BAll() && gameManager.queenIsFull && gameManager.multi)
        {
            Invoke("activeLevelMultiSuccess", 1);
        }
        else if (PC.BAll() && gameManager.queenIsFull && !gameManager.multi)
        {
            gameManager.TEL.testEndOfLevel();
            Debug.Log("ici solo fuck ????");
            gameManager.TEL.SaveWin();
            
            Invoke("activeLevelSoloSuccess", 1);
        }
    }

    /// <summary>
    /// utilise OnGUI pour exécuter du code quand le jeu est en pause !!! :D :D
    /// </summary>
    private void OnGUI()
    {
        if ((PV && !PV.isPlaying && gamePausedCinematic)
            || (gamePausedCinematic && PC.exitAll()))
        {
            TWNE.isOk = false;
            //Debug.Log("ici quand le jeu reprend ???");
            pauseGameCinematic();
            PV.stopVideo();
            Destroy(canvasCinematic);
        }
        if (PC.exitAll() && gamePaused && TWNEG.isOk /* && !displayMenuOnce && !gameManager.TEL.levelFailedMulti && !gameManager.TEL.levelFailedSolo && !gameManager.queenIsFull*/)
        {
            //StartCoroutine(getOutOfPause());
            TWNEG.isOk = false;
            pauseGame(false);
            //Debug.Log("le jeu reprend ????");
            /*joypadTmp = false;
            pauseGame(joypadTmp);
            TWNE.isOk = false;*/
        }
    }

    /*IEnumerator getOutOfPause()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log("le jeu reprend ????");
        pauseGame(false);
        TWNE.isOk = false;
    }*/

    /// <summary>
    /// update
    /// </summary>
    private void Update()
    {
        //action retour du joueur 0 ou 1 (clavier ou joystick 1)
        if (PC.exitAll() && !gamePausedCinematic && TWNE.isOk && TWNEG.isOk && !displayMenuOnce && !gameManager.TEL.levelFailedMulti && !gameManager.TEL.levelFailedSolo && !gameManager.queenIsFull)
        {
            //Debug.Log("on passe ici quand on met pause ??");
            joypadTmp = false;
            pauseGame(joypadTmp);
            TWNE.isOk = false;
            TWNEG.isOk = false;
        }

        testForEndQuicly();
    }
}
