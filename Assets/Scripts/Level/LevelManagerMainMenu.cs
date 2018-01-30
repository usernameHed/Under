using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired.UI.ControlMapper;
using System.Collections.Generic;

public class LevelManagerMainMenu : MonoBehaviour {

    public GameObject exit;
    public Button [] listButtonChoice;
    public Button[] listButtonEscape;
    public Button[] listButtonCampain;
    public GameObject[] listGroupsLevel;
    public GameObject soloCoop;
    public FadeObjectInOut picCanvas;                                   //image principale du menu main Campain (pour l'afficher ou le cacher)
    public GameObject newWorldParticle;                                 //animation quand un nouveau monde est découvert !
    public GameObject AreYouSure;

    /// <summary>
    /// menu du level
    /// </summary>
    [Header("Menu solo")]
    [SerializeField] private CustomButton[] playButtons;
    [SerializeField] private GameObject[] powerMap;
    [SerializeField] private GameObject[] goldenEggs;
    [SerializeField] private GameObject[] goldenEggsText;
    [SerializeField] private GameObject[] goalText;
    [SerializeField] private Text achievement;
    [SerializeField] private GameObject[] playerPanel;
    [SerializeField] private GameObject[] powerPlayerPanel;
    [SerializeField] private GameObject[] powerPlayerPanel1;
    [SerializeField] private GameObject[] powerPlayerPanel2;
    [SerializeField] private Image screenShots;
    [SerializeField] private GameObject[] controls1;
    [SerializeField] private GameObject[] controls2;
    [SerializeField] private GameObject canvasCinematic;
    [SerializeField] private Text totalGoldenEggs;
    [SerializeField] private GameObject playIn3;
    [SerializeField] private Text selectYourWorld;                              //text affiché dans le menuCapain (select your world);
    [SerializeField] private GameObject[] tutoBack;                             //back echape ou B ?
    [SerializeField] private GameObject[] tutoBackLevels;                             //back echape ou B ?
    [SerializeField] private GameObject[] worldunlockedGUI;                             //back echape ou B ?

    [Header("Menu multi")]
    
    [SerializeField] private GameObject MainMenuMulti;
    [SerializeField] private CustomButton playButtonMulti;
    [SerializeField] private CustomButton[] AreYouSureButton;
    [SerializeField] private Image screenShotsMulti;
    [SerializeField]    private bool multi = false;
    [SerializeField] private Text textWaitMulti;
    [HideInInspector] public int multiLocation = 0;
    public List<MovePlayer> listPlayersPanel = new List<MovePlayer>();
    public List<GameObject> listPowerMaps = new List<GameObject>();
    public List<int> antsTeam1 = new List<int>();
    public List<int> antsTeam2 = new List<int>();
    public List<Sprite> picAntsPower = new List<Sprite>();
    public List<GameObject> slotTeam1 = new List<GameObject>();
    public List<GameObject> slotTeam2 = new List<GameObject>();
    public List<GameObject> slotTeam1Bad = new List<GameObject>();
    public List<GameObject> slotTeam2Bad = new List<GameObject>();
    public List<GameObject> panelBadAnts = new List<GameObject>();
    public List<List<int>> multiPlayerList = new List<List<int>>();

    public int multiNumberPowerMap = 0;

    [Header("other")]
    private GameObject GlobalVariableManager;
    [HideInInspector] public PlayerConnected PCC;
    private GlobalVariableManager Global;

    public bool exitActive = false;
    private bool goToNextLevel = false;
    public bool isInMove = false;

    [Space(10)]
    [Header("controller")]
    [SerializeField]    private bool keyboardType = true;
    [SerializeField]    private int joypadConnected = 0;
    [SerializeField]    private int player1Control = 0;
    [SerializeField]    private int player2Control = 5;
    

    ///private;
    
    private int superLocation = 0;
    public int SuperLocation
    {
        get
        {
            return (superLocation);
        }
        set
        {
            if (superLocation != value)
            {
                Debug.Log("superlocation: " + value);
                superLocation = value;
                if (superLocation == 1) //on va vers le mode capain
                {
                    multi = false;
                    setButtonCampainInteractable();     //set les boutons des mondes interactables ou pas selon si ils sont débloqués
                    CMM.playPathToCampain();
                }
                if (superLocation == 2) //on veut aller au multi ! (on fini a 8)
                {
                    multi = true;
                    CMM.dotMove(CMM.listWorld[1], 0);
                }
                else if (superLocation == 3)                                    //on est passé du Main Menu à capain (choix level)
                {
                    isInMoveOrNot(false);                                       //la camera n'est plus en train de bouger, on peut quitter ou revenir en arrière !
                    setButtonCampainInteractable();                             //set les boutons des mondes interactables ou pas selon si ils sont débloqués
                    campainLocation = PlayerPrefs.PP.lastLevelPlayerId[0];      //get le dernie rmonde joué par le joueur !
                    manageNewWordSelection();                                   //selectionne le dernier monde joué
                    
                }
                else if (superLocation == 4)    //on a zoomé sur un monde
                {
                    isInMoveOrNot(false);
                    
                    ////////////////////////////////////
                    //////////////////////////////////// ici on a le level !
                    //CMM.listRealWorld[0].gameObject.SetActive(false);
                    //CMM.listRealWorld[campainLocation].GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeOut = true;
                    //CMM.listRealWorld[campainLocation].GetChild(1).gameObject.SetActive(true);
                    //CMM.listRealWorld[campainLocation].GetChild(1).gameObject.GetComponent<FadeObjectInOut>().fadeOut = true;

                    zoomedOnLevel();
                }
                else if (superLocation == 5)    //on est en train de dezoomer sur les monde a nouveau
                {
                    //listGroupsLevel[campainLocation].SetActive(false);
                    dezoomOnLevel();
                    CMM.moveWorld(-1);  //objet "worlds" !
                    CMM.backToCampainLocation();
                }
                else if (superLocation == 6)    //on est repassé au mode dezoom de choix de maps
                {
                    isInMoveOrNot(false);
                    superLocation = 3;
                }
                else if (superLocation == 8)    //on est arrivé dans le mode multi (on clique sur level: 12)
                {
                    Debug.Log("On est arrivé dans le mode multi");
                    playIn3activated = false;
                    multiIsLoading = false;
                    playIn3.SetActive(false);
                    multiLocation = 0;
                    desctivateAllPanel();
                    CMM.listButtonMutli[PlayerPrefs.PP.mapUnlockMulti].Select();
                    isInMoveOrNot(false);

                    if (Global.backToMainMenu == 5)
                    {
                        CMM.cam.position = CMM.listWorld[1].position;
                        MainMenuMulti.GetComponent<FadeObjectInOut>().changeFade(false);
                        MainMenuMulti.GetComponent<CanvasGroup>().alpha = 1;
                        SuperLocation = 12;
                        Global.backToMainMenu = 0;
                    }
                }
                else if (superLocation == 7)    //ici on est dans le mode campain monde et on veut revenir au menu principale !
                {
                    CMM.moveWorld(-3);    //move to objet "menu" !
                    Debug.Log("ici on est dans le mode campain monde et on veut revenir au menu principale !");
                }
                else if (superLocation == 9)    //ici on est dans le mode multi et on veut revenir au menu principale !
                {
                    CMM.moveWorld(-3);    //move to objet "menu" !
                    Debug.Log("ici on est dans le mode multi et on veut revenir au menu principale !");
                }
                else if (superLocation == 10)
                {
                    Debug.Log("ici on est revenu dans le mode normal au tout début !!!");
                    listButtonChoice[0].enabled = true;
                    listButtonChoice[1].enabled = true;
                    superLocation = 0;
                    isInMoveOrNot(false);
                    StartCoroutine(selectButton(listButtonChoice[ (multi) ? 1 : 0  ]));
                }
                else if (superLocation == 11)   //on a cliqué sur un level en particulier !
                {
                    Debug.Log("ici level");
                    isInMoveOrNot(true);
                    setUpLevelMenu();
                    //StartCoroutine(selectButton(listGroupsLevel[campainLocation - 1].transform.GetChild(1).GetComponent<CustomButton>()));
                }
                else if (superLocation == 12)   //on a cliqué sur un level du multi !
                {
                    Debug.Log("ici level: " + PlayerPrefs.PP.mapUnlockMulti);
                    //setup le screenshots !!!!!
                    screenShotsMulti.sprite = Resources.Load<Sprite>("Screenshot/screen_multi/" + PlayerPrefs.PP.mapUnlockMulti);

                    justeStartState12();
                    activateAllPanel();

                    //CMM.listButtonMutli[PlayerPrefs.PP.mapUnlockMulti].

                    //level: PlayerPrefs.PP.mapUnlockMulti
                }
            }
        }
    }

    private int location = 0;
    private int levelLocation = 0;
    public int campainLocation = 0;
    private bool debugIsInteractable;
    public bool playIn3activated = false;
    public bool multiIsLoading = false;

    /// <summary>
    /// global
    /// </summary>
    [HideInInspector] public TimeWithNoEffect TWNE;
    private QuitOnClick QOC;
    private GameObject globalObject;
    [HideInInspector] public PlayerConnected PC;
    private CoolMenuManager CMM;

    private void Awake()
    {
        globalObject = GameObject.FindGameObjectWithTag("Global");
        PC = globalObject.GetComponent<PlayerConnected>();
        PC.LMMM = this;
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        QOC = gameObject.GetComponent<QuitOnClick>();
        CMM = gameObject.GetComponent<CoolMenuManager>();
        
        gameObject.GetComponent<Fading>().BeginFade(-1);
        Global = globalObject.GetComponent<GlobalVariableManager>();
    }

    private void Start()
    {
        Cursor.visible = false;

        if (!Global.fromGame)   //reset les valeur SI on ne veins pas de next Level
        {
            Global.soloAnd2Player = false;
            Global.fromMenu = true;
        }
        else
        {
            player1Control = Global.tmpPlayer1Control;
            player2Control = Global.tmpPlayer2Control;
            keyboardType = Global.keyboardType;             //remet le keyboard type sauvegardé de la map précédente (pour garder les contrôles des joueurs Si on est sur joypad + clavier)
        }

        worldUnlock();  /////////////////////////////ici unlock les mondes !!!
        worldMultiSetUp();
        
        listButtonChoice[0].Select();

        HandleNextLevel();
    }

    /// <summary>
    /// selectionne le dernier monde joué
    /// </summary>
    private void manageNewWordSelection()
    {
        if (PlayerPrefs.PP.isWorldUnlocked == -1)                   //si -1, selectionne le dernier monde joué
            StartCoroutine(selectButton(listButtonCampain[campainLocation]));
        else                                                        //sinon: on vient de débloquer un nouveau monde !
        {
            StartCoroutine(selectButton(listButtonCampain[PlayerPrefs.PP.isWorldUnlocked]));    //selectionne le nouveau monde

            //TODO: menu débloqué
            
            
            
            StartCoroutine(reActivateButton(PlayerPrefs.PP.worldUnlock - 1, 3.0f));
            //Vector3 pos = new Vector3(CMM.listRealWorld[PlayerPrefs.PP.isWorldUnlocked].position.x, CMM.listRealWorld[PlayerPrefs.PP.isWorldUnlocked].position.y, -2f);
            //Instantiate(newWorldParticle, pos, Quaternion.identity);



            selectYourWorld.text = "You unlock a new world !";
            PlayerPrefs.PP.isWorldUnlocked = -1;                                                //restart la variable
        }
    }

    /// <summary>
    /// temps d'attente avant l'interaction possible du bouton
    /// </summary>
    /// <param name="button"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator reActivateButton(int button, float time)
    {
        Debug.Log("ici debloque bouton ??");

        worldunlockedGUI[button].SetActive(true);

        CMM.listButtonWorld[button + 1].enabled = false;

        yield return new WaitForSeconds(time);
        CMM.listButtonWorld[button + 1].enabled = true;
        StartCoroutine(selectButton(listButtonCampain[button + 1]));    //selectionne le nouveau monde
        worldunlockedGUI[button].SetActive(false);
        //CMM.listButtonWorld[button].interactable = true;
    }

    /// <summary>
    /// actualise les slot des équipe en mode multi
    /// </summary>
    public void actualiseListPlayers()
    {
        //antsTeam1
        //antsTeam2
        //listPlayersPanel
        //set a faux les fourmis précédentes
        for (int i = 0; i < 3; i++)
            slotTeam1[i].SetActive(false);
        for (int i = 0; i < 3; i++)
            slotTeam2[i].SetActive(false);
        for (int i = 0; i < 3; i++)
            slotTeam1Bad[i].SetActive(false);
        for (int i = 0; i < 3; i++)
            slotTeam2Bad[i].SetActive(false);

        panelBadAnts[0].SetActive(false);
        panelBadAnts[1].SetActive(false);
        //clear les listes des pouvoirs
        antsTeam1.Clear();
        antsTeam2.Clear();
        //parcours les 6 joueurs, et REMPLIE les listes antsTeam1 & 2
        for (int i = 0; i < listPlayersPanel.Count; i++)
        {
            switch (listPlayersPanel[i].TeamType)
            {
                case 1: //SI la team sélectionné est BLEU (team 1)
                    if (listPlayersPanel[i].PowerType != 0 && listPlayersPanel[i].isActive)                 //si le pouvoirs sélectionné n'est pas blanc
                        antsTeam1.Add(listPlayersPanel[i].PowerType);
                    break;
                case 2: //SI la team sélectionné est RED (team 2)
                    if (listPlayersPanel[i].PowerType != 0 && listPlayersPanel[i].isActive)                 //si le pouvoirs sélectionné n'est pas blanc
                        antsTeam2.Add(listPlayersPanel[i].PowerType);
                    break;
            }
        }

        //affiche les bonnes fourmis sur le panel selon les listes
        for (int i = 0; i < antsTeam1.Count; i++)       //la team 1 (BLUE)
        {
            if (i < 3)                                  //si c'est inférieur a 3, on est dans les 3 premier slot correct
            {
                slotTeam1[i].SetActive(true);
                slotTeam1[i].GetComponent<Image>().sprite = picAntsPower[antsTeam1[i]];
            }
            else                                        //on est dans les mauvais slot
            {
                slotTeam1Bad[i - 3].SetActive(true);
                slotTeam1Bad[i - 3].GetComponent<Image>().sprite = picAntsPower[antsTeam1[i - 3]];
                panelBadAnts[0].SetActive(true);
            }
        }
        for (int i = 0; i < antsTeam2.Count; i++)
        {
            if (i < 3)                                  //si c'est inférieur a 3, on est dans les 3 premier slot correct
            {
                slotTeam2[i].SetActive(true);
                slotTeam2[i].GetComponent<Image>().sprite = picAntsPower[antsTeam2[i]];
            }
            else                                        //on est dans les mauvais slot
            {
                slotTeam2Bad[i - 3].SetActive(true);
                slotTeam2Bad[i - 3].GetComponent<Image>().sprite = picAntsPower[antsTeam2[i - 3]];
                panelBadAnts[1].SetActive(true);
            }
        }


    }

    /// <summary>
    /// menu multi 
    /// </summary>
    void justeStartState12()
    {
        multiLocation = 1;
        playButtonMulti.Select();
        AreYouSure.SetActive(false);
    }

    /// <summary>
    /// débloque les mondes ?
    /// </summary>
    public void worldUnlock()
    {
        
        MapPrefs.PP.changeWorldUnlock();
    }

    void worldMultiSetUp()
    {
        for (int i = 0; i < MapPrefs.PP.mapsInfosMulti.Count; i++)
        {
            CMM.listButtonMutli[i].gameObject.GetComponent<SetUpTheRightUIMulti>().setUpWhenWorldIsActive(i);
        }
    }

    /// <summary>
    /// gère si on doit directement passer au level suivant, (si on arrive depuis le game) ou pas.
    /// </summary>
    void HandleNextLevel()
    {
        if (Global.fromGame)                        //si on viens du game (nextLevel)
        {
            

            //test si la prochaine map permet le next level, si non, annuler et
            //faire comme si on viens d'arriver dans le menu
            if (PlayerPrefs.PP.isLastLevelMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))
            {
                Debug.Log("on a cliqué sur next world depuis le jeu !! fair eun truck ?");
                Global.soloAnd2Player = false;
                Global.fromMenu = true;
                Global.fromGame = false;
                Global.backToMainMenu = 1;
                handleRelocateInMenu();
                return;
            }
            canvasCinematic.SetActive(true);

            howManyJoypadAreConnected();          //compte combien de joypad sont connecté

            //Chargement.SetActive(true);             //afficher le chargement
            goToNextLevel = true;                   //désactiver tout opération sur le menu

            if (MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1] + 1).isBonus)
            {
                PlayerPrefs.PP.lastLevelPlayerId[1]++;
            }

            campainLocation = PlayerPrefs.PP.lastLevelPlayerId[0];
            levelLocation = PlayerPrefs.PP.lastLevelPlayerId[1] + 2;
            //mapLocation = PlayerPrefs.PP.lastLevelPlayerId[1] + 1;             //set la nouvelle mapLocation (pour le justPlay) à l'ancienne + 1 (la prochaine map !)
            //changeMapFocus();                                               //change le map focus manuellement

            

            /*//essaye de garder le mode coop SI la map suivante le permet !
            if (!(Global.soloAnd2Player && powerMapAccepted.Count > 1 && MapPrefs.PP.mapsInfosSoloWorlds[PlayerPrefs.PP.lastLevelPlayerId[0]][mapLocation].coop))
                Global.soloAnd2Player = false;
            else
            {
                //si on garde le coop, set les bon pouvoirs aux 2 joueurs par rapport aux pouvoir accepté par la map
                //pouvoir accepté par la map qui on été actualisé lorsqu'on a changé mapLocation juste en haut)
                changeIndexPlayer();
                if (Global.soloAnd2PlayerSwitchPower)       //si la dernière fois on avais switch...
                    switchPlayer1and2();                    //le refaire pour garder les mêes pouvoirs dans les joueurs

            }*/

            justPlay();
        }
        else
        {
            canvasCinematic.SetActive(false);
            handleRelocateInMenu();
        }
            
    }

    /// <summary>
    /// repositione la caméra sur le monde / level d'avant !
    /// </summary>
    void handleRelocateInMenu()
    {
        setButtonCampainInteractable();

        
        //ici set la caméra au bon endroit (SI on viens du game du genre "next world" ou "menu"
        if (Global.backToMainMenu == 1) //on a cliqué sur next world, aller au menu monde et select le monde actuel !
        {
            Debug.Log("ici focus monde: " + PlayerPrefs.PP.lastLevelPlayerId[0]);
            //Debug.Break();
            CMM.cam.position = CMM.listWorld[2].position;
            campainLocation = PlayerPrefs.PP.lastLevelPlayerId[0];
            SuperLocation = 3;
            Global.backToMainMenu = 0;            
            for (int i = 0; i < CMM.listRealWorld.Count; i++)
            {
                if (i != 0)
                {
                    //CMM.listRealWorld[i].GetChild(1).GetChild(0).gameObject.GetComponent<FadeObjectInOut>().enabled = false;
                    //CMM.listRealWorld[i].GetChild(1).GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;                  
                }
                    
            }
        }
        else if (Global.backToMainMenu == 2)    //on a cliqué sur "menu" (back to menu), aller au monde + level joué d'avant !
        {
            Debug.Log("ici focus level: " + PlayerPrefs.PP.lastLevelPlayerId[1] + " du monde " + PlayerPrefs.PP.lastLevelPlayerId[0]);
            //Debug.Break();
            CMM.cam.position = CMM.listWorld[3 + PlayerPrefs.PP.lastLevelPlayerId[0]].position;
            campainLocation = PlayerPrefs.PP.lastLevelPlayerId[0];
            levelLocation = PlayerPrefs.PP.lastLevelPlayerId[1];
            CMM.listButtonWorld[PlayerPrefs.PP.lastLevelPlayerId[0]].GetComponent<FadeObjectInOut>().changeFade(true);
            CMM.listButtonWorld[PlayerPrefs.PP.lastLevelPlayerId[0]].GetComponent<CanvasGroup>().alpha = 0;

            CMM.listRealWorld[PlayerPrefs.PP.lastLevelPlayerId[0]].GetChild(1).GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeOut = false;
            CMM.listRealWorld[PlayerPrefs.PP.lastLevelPlayerId[0]].GetChild(1).GetChild(0).gameObject.GetComponent<FadeObjectInOut>().fadeReset();
            SetTuto(false);
            SuperLocation = 4;
            
        }
        else if (Global.backToMainMenu == 3)    //on est retourné au menu depuis le mode multi
        {
            Debug.Log("ici focus le menu multi");
            
            Global.backToMainMenu = 5;
            SuperLocation = 8;
            //
            //CMM.listButtonMutli[PlayerPrefs.PP.mapUnlockMulti].
            //Debug.Break();
            //Global.backToMainMenu = 0;
        }
    }

    /// <summary>
    /// on a cliqué sur un level, prépare le menu !
    /// </summary>
    void setUpLevelMenu()
    {
        Debug.Log("idworld: " + campainLocation + ", idlevel: " + (levelLocation - 1));
        if (levelLocation == 0)
            levelLocation = 1;
        screenShots.sprite = Resources.Load<Sprite>("Screenshot/screen_" + campainLocation + "/" + (levelLocation - 1));
        //id world: campainLocation
        //id level: levelLocation - 1
        //powerMap.transform.Ge
        powerMap[0].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).white);
        powerMap[1].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue);
        powerMap[2].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red);
        powerMap[3].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow);
        powerMap[4].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green);

        goalText[0].GetComponent<Text>().text = "win";
        goalText[1].GetComponent<Text>().text = MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).timeEggs2to4[0] + " sec";
        goalText[2].GetComponent<Text>().text = MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).timeEggs2to4[1] + " sec";

        goalText[0].GetComponent<CanvasGroup>().alpha = 1f;
        goalText[1].GetComponent<CanvasGroup>().alpha = 1f;
        goalText[2].GetComponent<CanvasGroup>().alpha = 1f;
        goldenEggs[0].SetActive(false);
        goldenEggs[1].SetActive(false);
        goldenEggs[2].SetActive(false);
        goldenEggs[3].SetActive(false);
        goldenEggsText[0].SetActive(true);
        goldenEggsText[1].SetActive(true);
        goldenEggsText[2].SetActive(true);

        if (PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).succes > 0)
        {
            goalText[0].GetComponent<CanvasGroup>().alpha = 0.3f;
            goldenEggs[0].SetActive(true);
            goldenEggsText[0].SetActive(false);
        }

        if (PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).succes > 1)
        {
            goalText[1].GetComponent<CanvasGroup>().alpha = 0.3f;
            goldenEggs[1].SetActive(true);
            goldenEggsText[1].SetActive(false);
        }
            
        if (PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).succes > 2)
        {
            goalText[2].GetComponent<CanvasGroup>().alpha = 0.3f;
            goldenEggs[2].SetActive(true);
            goldenEggsText[2].SetActive(false);
        }            
        if (PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).succes > 3)
            goldenEggs[3].SetActive(true);
        if (PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).bestHighScore == 999999)
            achievement.text = "Temps record : none";
        else
            achievement.text = "Temps record : " + PlayerPrefs.PP.getLevels(campainLocation, levelLocation - 1).bestHighScore + " sec";

        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop)
        {
            playButtons[1].interactable = true;
            howManyJoypadAreConnected();
            Debug.Log("joypad connected: " + joypadConnected);
            if (joypadConnected >= 2)
                StartCoroutine(selectButton(playButtons[1]));
            //ici définir les pouvoirs des 2 joueurs
            powerPlayerPanel1[0].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue1);
            powerPlayerPanel1[1].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red1);
            powerPlayerPanel1[2].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow1);
            powerPlayerPanel1[3].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green1);

            powerPlayerPanel2[0].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue2);
            powerPlayerPanel2[1].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red2);
            powerPlayerPanel2[2].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow2);
            powerPlayerPanel2[3].SetActive(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green2);
        }
        else
            playButtons[1].interactable = false;

        defineControlAndPower();    //défini les contrôles
    }

    /// <summary>
    /// compte le nombre de joypad connecté
    /// </summary>
    /// <returns></returns>
    void howManyJoypadAreConnected()
    {
        joypadConnected = 0;
        for (int i = 1; i < PC.playerControllerConnected.Count - 1; i++)
        {
            if (PC.playerControllerConnected[i])
                joypadConnected++;
        }
    }

    public void defineControlAndPower()
    {
        if (Global.fromGame)    //les control sont déja assigné si on a fait un next level !!!
            return;

        howManyJoypadAreConnected();    //compte le nombre de joypad connecté
        if (joypadConnected == 0)       //s'il n'y a aucun joypad connecté
        {
            //set le joueur 1 à keyboardType principalement utilisé
            controls1[0].SetActive(false);
            controls1[1].SetActive(keyboardType);
            controls1[2].SetActive(!keyboardType);
            player1Control = (keyboardType) ? 0 : 5;
            if (campainLocation == 0)
                return;

            if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop)
            {
                controls1[0].SetActive(false);
                controls1[1].SetActive(keyboardType);
                controls1[2].SetActive(!keyboardType);
                player1Control = (keyboardType) ? 0 : 5;

                controls2[0].SetActive(false);
                controls2[1].SetActive(!keyboardType);
                controls2[2].SetActive(keyboardType);
                player2Control = (keyboardType) ? 5 : 1;
            }
        }
        else if (joypadConnected == 1)
        {
            controls1[0].SetActive(true);
            controls1[1].SetActive(false);
            controls1[2].SetActive(false);
            player1Control = 1;

            if (campainLocation == 0)
                return;

            if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop)
            {
                controls2[0].SetActive(false);
                controls2[1].SetActive(keyboardType);
                controls2[2].SetActive(!keyboardType);
                player2Control = (keyboardType) ? 0 : 5;
            }
        }
        else
        {
            controls1[0].SetActive(true);
            controls1[1].SetActive(false);
            controls1[2].SetActive(false);
            player1Control = 1;

            if (campainLocation == 0)
            {
                //setCoop(true);
                return;
            }
                

            if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop)
            {
                controls2[0].SetActive(true);
                controls2[1].SetActive(false);
                controls2[2].SetActive(false);
                player2Control = 2;
            }
        }
    }

    /// <summary>
    /// select
    /// </summary>
    public void focusOnSolo()
    {
        //defineControlAndPower(true);
        //playerPanel[0].SetActive(true);
        playerPanel[1].SetActive(false);
        playerPanel[2].SetActive(false);
        powerPlayerPanel[0].SetActive(false);

    }

    /// <summary>
    /// select
    /// </summary>
    public void focusOnMulti()
    {
        //defineControlAndPower();
        if (!playButtons[1].IsInteractable())
        {
            playerPanel[2].SetActive(true);
            return;
        }
        playerPanel[2].SetActive(false);
        //playerPanel[0].SetActive(true);
        playerPanel[1].SetActive(true);
        powerPlayerPanel[0].SetActive(true);
    }

    /// <summary>
    /// fonction appelé quand un joypad s'est connecté/déconnecté
    /// </summary>
    public void joypadActualized()
    {
        Debug.Log("ici new connection/deconnection");
        defineControlAndPower();
        if (multi && superLocation == 12)
        {
            for (int i = 0; i < listPlayersPanel.Count; i++)
            {
                listPlayersPanel[i].setActivation();
            }
        }
    }

    /// <summary>
    /// at start
    /// </summary>
    public void activateAllPanel()
    {
        //défini combien de pouvoir il y a sur la maps
        multiNumberPowerMap = 0;
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].blue)
            multiNumberPowerMap++;
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].red)
            multiNumberPowerMap++;
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].yellow)
            multiNumberPowerMap++;
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].green)
            multiNumberPowerMap++;

        listPowerMaps[0].SetActive(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].white);
        listPowerMaps[1].SetActive(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].blue);
        listPowerMaps[2].SetActive(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].red);
        listPowerMaps[3].SetActive(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].yellow);
        listPowerMaps[4].SetActive(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].green);

        for (int i = 0; i < listPlayersPanel.Count; i++)
        {
            listPlayersPanel[i].enabled = true;
            listPlayersPanel[i].setActivation();
        }
        actualiseListPlayers();
    }

    /// <summary>
    /// at start
    /// </summary>
    public void desctivateAllPanel()
    {
        for (int i = 0; i < listPlayersPanel.Count; i++)
        {
            listPlayersPanel[i].enabled = false;
        }
    }

    /// <summary>
    /// on viens de zoomer sur le level
    /// </summary>
    void zoomedOnLevel()
    {
        picCanvas.changeFade(true);


        listGroupsLevel[campainLocation - 1].SetActive(true);

        //setup le premier
        listGroupsLevel[campainLocation - 1].transform.GetChild(0).gameObject.GetComponent<SetUpTheRightUI>().setUpWhenWorldIsActive(campainLocation, -1);
        for (int j = 0; j < MapPrefs.PP.getWorlds(campainLocation).Count; j++)
        {
            listGroupsLevel[campainLocation - 1].transform.GetChild(j + 1).gameObject.GetComponent<SetUpTheRightUI>().setUpWhenWorldIsActive(campainLocation, j);
        }

        //setup le dernier
        listGroupsLevel[campainLocation - 1].transform.GetChild(13).gameObject.GetComponent<SetUpTheRightUI>().setUpWhenWorldIsActive(campainLocation, -1);

        //selectionne le premier niveau, pas le dernier débloqué ? - id wolrd - id level (le premier)
        if (Global.backToMainMenu == 2) //si on vient d'un level et qu'on a fait back...
        {
            StartCoroutine(selectButton(listGroupsLevel[campainLocation - 1].transform.GetChild(levelLocation + 1).GetComponent<CustomButton>()));
            Global.backToMainMenu = 0;
        }
        else if (PlayerPrefs.PP.worldUnlock == campainLocation)
            StartCoroutine(selectButton(listGroupsLevel[campainLocation - 1].transform.GetChild(PlayerPrefs.PP.mapUnlockId + 1).GetComponent<CustomButton>()));
        else
            StartCoroutine(selectButton(listGroupsLevel[campainLocation - 1].transform.GetChild(1).GetComponent<CustomButton>()));
    }

    public void dezoomOnLevel()
    {
        listGroupsLevel[campainLocation - 1].SetActive(false);
        picCanvas.changeFade(false);
    }

    /// <summary>
    /// set les boutons des mondes interactables ou pas selon si ils sont débloqués
    /// </summary>
    void setButtonCampainInteractable()
    {
        Debug.Log("debloque boutton des mondes débloqué !");
        Debug.Log("wordlUnlock: " + PlayerPrefs.PP.worldUnlock);
        //change le total des goldenEggs (le texte)
        totalGoldenEggs.text = "Total:\n" + PlayerPrefs.PP.getTotalGoldenEggsWinned();

        for (int i = 0; i < listButtonCampain.Length; i++)
        {
            HandleDisplayObjectWorld HDW = listButtonCampain[i].GetComponent<HandleDisplayObjectWorld>();

             HDW.changeNumberEggs(PlayerPrefs.PP.worldCountGoldenEggs[i]);
             


            if (i <= PlayerPrefs.PP.worldUnlock)
            {
                Debug.Log("ici debloque world ?");
                listButtonCampain[i].interactable = true;
                HDW.enableWorld(true);
            }  
            else
            {
                listButtonCampain[i].interactable = false;
                HDW.enableWorld(false);
                HDW.changeColorTextLocked(MapPrefs.PP.checkIfAllLevelAreUnlock(Mathf.Max(i - 1, 0)), (PlayerPrefs.PP.getTotalGoldenEggsWinned() >= MapPrefs.PP.numberEggsToHave[i]), MapPrefs.PP.numberEggsToHave[i]);
            }

        }

        SetTuto(true);
    }

    /// <summary>
    /// set les tuto "back Space" ou "back B"
    /// </summary>
    public void SetTuto(bool main)
    {
        howManyJoypadAreConnected();
        if (joypadConnected > 0)
        {
            if (main)
            {
                tutoBack[0].SetActive(false);
                tutoBack[1].SetActive(true);
            }
            else
            {
                tutoBackLevels[0].SetActive(false);
                tutoBackLevels[1].SetActive(true);
            }
        }
        else
        {
            if (main)
            {
                tutoBack[0].SetActive(true);
                tutoBack[1].SetActive(false);
            }
            else
            {
                tutoBackLevels[0].SetActive(true);
                tutoBackLevels[1].SetActive(false);
            }
        }
    }

    /// <summary>
    /// est appelé lorsque un bouton est surligné (main menu) (à l'appelle de la fonction .Select() du changeFocus(), ou quand la souris passe dessus)
    /// </summary>
    /// <param name="highlight">le boutton à surrligner</param>
    public void changeHighlighted(int highlight)
    {
        location = highlight;
    }


    /// <summary>
    /// est appelé lorsque un bouton est surligné (menu multi)
    /// </summary>
    /// <param name="highlight">le boutton à surrligner</param>
    public void changeHighlightedLevelMulti(int highlight)
    {
        PlayerPrefs.PP.mapUnlockMulti = highlight;
    }

    /// <summary>
    /// appelé lorsqu'un level est select()
    /// </summary>
    /// <param name="highlight"></param>
    public void changeHighlightedLevel(int highlight)
    {
        if (Global.fromGame)
            return;

        levelLocation = highlight;
    }

    /// <summary>
    /// appelé lorsque qu'un bouton de la campagne est surligné
    /// </summary>
    /// <param name="hightlight"></param>
    public void changeCampainLocation(int hightlight)
    {
        campainLocation = hightlight;
    }

    /// <summary>
    /// input du joueur en mode multi;
    /// </summary>
    private void playerInputMulti()
    {
        //on est dans le menu multi (blue vs red), le joueur 1 veut quitter (et on est dans le mode multiLocation = 1)
        if (superLocation == 12 && TWNE.isOk && multiLocation == 1 && !playIn3activated
            && (/*PC.getPlayer(1).GetButtonDown("UICancel") ||*/ PC.getPlayer(-1).GetButtonDown("Escape") || PC.getPlayer(1).GetButtonDown("Start") || (PC.getPlayer(1).GetButtonDown("UICancel") && !listPlayersPanel[1].isLocked) ))
        {
            TWNE.isOk = false;
            //si on est le player 1 et qu'on a validé une action dans le meu mutli
            /*if (PC.getPlayer(1).GetButtonDown("UICancel") && listPlayersPanel[1].isLocked)
            {
                //multiLocation = 3;
                return;
            }*/
            Debug.Log("ici on quite (on affiche êtes vous sure ?)");
            AreYouSure.SetActive(true);
            //AreYouSureNo.Select();
            AreYouSureButton[0].Select();
            StartCoroutine(selectButton(AreYouSureButton[1]));
            multiLocation = 2;
        }
        //on est dans le menu multi (blue vs red), le joueur 1 veut ANULLER le quitter (et on est dans le mode multiLocation = 2)
        else if (superLocation == 12 && multiLocation == 2 && !playIn3activated
            && (PC.getPlayer(1).GetButtonDown("UICancel") || PC.getPlayer(-1).GetButtonDown("Escape")))
        {
            //AreYouSure.SetActive(false);
            Debug.Log("ici on annule le etevoussure");
            GoBackToMaps(false);

        }
        else if (superLocation == 12 && multiLocation == 1 && (PC.getPlayer(-1).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit")))
        {
            if (isReadyToPlay() && !playIn3activated)
            {
                //si oui... JustPlay avec les 2 équipes !

                //active les claviers s'il sont dans la games
                if (listPlayersPanel[0].isActive && listPlayersPanel[0].validated)
                    listPlayersPanel[0].lockedObject.SetActive(true);
                else
                    listPlayersPanel[0].gameObject.SetActive(false);

                if (listPlayersPanel[5].isActive && listPlayersPanel[5].validated)
                    listPlayersPanel[5].lockedObject.SetActive(true);
                else
                    listPlayersPanel[5].gameObject.SetActive(false);

                playIn3.SetActive(true);
                playIn3activated = true;
                Debug.Log("ici on active le multi ! justPlay !");
                textWaitMulti.text = "Play in 3...";
                StartCoroutine(chronoMultiPlay());
            }
        }
        else if (superLocation == 12 && multiLocation == 1 && playIn3activated && !multiIsLoading && (PC.getPlayer(-1).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel")))
        {
            StopCoroutine("chronoMultiPlay");
            textWaitMulti.text = "Play in 3...";
            playIn3.SetActive(false);
            playIn3activated = false;
        }
    }

    IEnumerator chronoMultiPlay()
    {
        yield return new WaitForSeconds(1);
        if (!playIn3activated)
            yield break;
        textWaitMulti.text = "Play in 2...";
        yield return new WaitForSeconds(1);
        textWaitMulti.text = "Play in 1...";
        if (!playIn3activated)
            yield break;
        yield return new WaitForSeconds(1);
        if (!playIn3activated)
            yield break;
        textWaitMulti.text = "Play in 0...";
        yield return new WaitForSeconds(1);
        textWaitMulti.text = "Loading";
        justPlayMulti();
    }

    /// <summary>
    /// la team est-elle prête ?
    /// </summary>
    /// <returns></returns>
    bool isReadyToPlay()
    {
        multiPlayerList.Clear();
        if (panelBadAnts[0].activeSelf || panelBadAnts[1].activeSelf
            || antsTeam1.Count == 0 || antsTeam2.Count == 0)
        {

            //nop
            return (false);
        }

        for (int i = 0; i < 6; i++) //parcourt les 6 potentiel player
        {
            List<int> tmpPlayerData = new List<int>();
            //si le player est activé avec sa team/pouvoir validé
            if (listPlayersPanel[i].isActive && listPlayersPanel[i].validated)
            {
                //si ce n'est pas un clavier, il a besoin d'être locked
                if (i != 0 && i != 5 && !listPlayersPanel[i].isLocked)
                {
                    
                    Debug.Log("locked what  NNN: " + i);
                    return (false);
                    //listPlayersPanel[i].setActivation();
                    //continue;
                }
                    

                //ici on a un player qui joue !
                
                tmpPlayerData.Add(listPlayersPanel[i].PowerType);   //ajoute le pouvoir du joueur !
                tmpPlayerData.Add(listPlayersPanel[i].TeamType);    //ajoute la team du joueur !
                tmpPlayerData.Add(i);                               //ajoute les contrôle du joueur !
                multiPlayerList.Add(tmpPlayerData);
            }
        }
        //listPlayersPanel[0]
        //test si les 2 teams sont remplie
        //qu'il n'y a pas une équipe vide
        //qu'il n'y a pas une équipe trop remplie
        //test si toutes les manettes sont validé
        return (true);
    }

    /// <summary>
    /// change 
    /// </summary>
    private void changeKeyboard()
    {
        if (PC.keyboardZqsd() && !keyboardType)
        {
            keyboardType = true;
            defineControlAndPower();
        }
        else if (PC.keyboardArrow() && keyboardType)
        {
            keyboardType = false;
            defineControlAndPower();
        }
    }

    public void isInMoveOrNot(bool move)
    {
        isInMove = move;
    }

    /// <summary>
    /// gère les inputs clavier ou joypad du joueurs
    /// </summary>
    private void playerInput()
    {
        changeKeyboard();
        /*if (superLocation == 1)
        {
            playerInputSolo();
        }
        if (superLocation == 2)
        {
            playerInputMulti();
        }*/
        if (!isInMove && PC.exitAll(true))   //toute les manette peuvent quitter quand elle veulent avec start
        {
            displayMenuExit(true);
        }
        //pour le clavier, c'est seulement dans le menu principale !
        if (superLocation == 0 && (PC.exitAll() || PC.getPlayer(1).GetButtonDown("UICancel")))
        {
            displayMenuExit(true);
        }
        //ici on veut revenir au MainMenu ! (depuis le mode solo
        if (!isInMove && superLocation == 3 && (PC.getPlayer(1).GetButtonDown("UICancel") || PC.getPlayer(-1).GetButtonDown("Escape")))
        {
            isInMoveOrNot(true);
            SuperLocation = 7;
        }
        //ici on veut revenir au MainMenu ! (depuis le mode solo
        if (superLocation == 8 && (PC.getPlayer(1).GetButtonDown("UICancel") || PC.getPlayer(-1).GetButtonDown("Escape")))
        {
            isInMoveOrNot(true);
            SuperLocation = 9;
        }

        if (superLocation == 4 && (PC.getPlayer(1).GetButtonDown("UICancel") || PC.getPlayer(-1).GetButtonDown("Escape")))
        {
            isInMoveOrNot(true);
            dezoomTo5();    //on revient dans le dezzoom des mondes
        }

        if (superLocation == 11 && (PC.getPlayer(1).GetButtonDown("UICancel") || PC.getPlayer(-1).GetButtonDown("Escape")))
        {
            //dezoomTo5();    //on revient dans le dezzoom des mondes
            isInMoveOrNot(true);
            backToCHoiceLevel();
        }
    }

    /// <summary>
    /// on avait affiché le pop-up du niveau, et on a fait back ou annuler !
    /// </summary>
    public void backToCHoiceLevel()
    {
        Debug.Log("ici ???: " + levelLocation);
        soloCoop.GetComponent<FadeObjectInOut>().changeFade(true);
        StartCoroutine(selectButton(listGroupsLevel[campainLocation - 1].transform.GetChild(levelLocation).GetComponent<CustomButton>()));
        superLocation = 4;
        isInMoveOrNot(false);
    }

    /// <summary>
    /// on était sur un monde, et on dezoom sur les mondes
    /// </summary>
    public void dezoomTo5()
    {
        SuperLocation = 5;  //on revient dans le dezzoom des mondes
    }

    //[Button("highLight")]
    public IEnumerator selectButton(Button butt)
    {
        if (!butt.interactable)
        {
            butt.Select();
        }
        else
        {
            //debugIsInteractable = butt.interactable;
            butt.interactable = false;
            yield return new WaitForEndOfFrame();
            Debug.Log("select button ???????????");
            //if (!butt.Select)
            butt.interactable = true;
            butt.Select();
            StartCoroutine(selectButton2(butt));
        }
    }

    IEnumerator selectButton2(Button butt)
    {
        butt.interactable = false;
        yield return new WaitForEndOfFrame();
        Debug.Log("select button ???????????");
        //if (!butt.Select)
        butt.interactable = true;
        butt.Select();
        //butt.interactable = debugIsInteractable;
    }

    /// <summary>
    /// input du player quand le menu qui est affiché
    /// </summary>
    private void escapeMenuInput()
    {
        //retour avec joystick1
        if (PC.exitAll() || PC.BAll())
        {
            displayMenuExit(false);
        }
    }

    /// <summary>
    /// zoom sur le level selectionné !
    /// </summary>
    public void zoomOnLevel()
    {
        soloCoop.GetComponent<FadeObjectInOut>().changeFade(true);
        CMM.dotMove(listGroupsLevel[campainLocation - 1].transform.GetChild(levelLocation).transform, 3);
    }

    /// <summary>
    /// set coop depuis le bouton
    /// </summary>
    public void setCoop(bool coop)
    {
        if (coop)
        {
            if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop) //si on autorise le coop, ok
                Global.soloAnd2Player = true;
            else
                Global.soloAnd2Player = false;
        }
        else
            Global.soloAnd2Player = false;
    }

    /// <summary>
    /// Enlève le menu "go back to map"
    /// </summary>
    public void GoBackToMaps(bool yes)
    {
        if (yes)
        {
            AreYouSure.SetActive(false);
            MainMenuMulti.GetComponent<FadeObjectInOut>().changeFade(true);
            
            SuperLocation = 8;
        }
        else
        {
            //idem que SuperLo = 12, mais comme on est déjà a 12...
            justeStartState12();
        }
    }

    /// <summary>
    /// play le modemulti !
    /// </summary>
    public void justPlayMulti()
    {
        multiIsLoading = true;
        Debug.Log("ici lance le jeu !");

        Global.maxEggs = MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].maxEggs;           //set le maxEggs à donner à la reine
        Global.timeMax = MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].timeMax;           //set le timerMax (multi), (solo à revoir)
        Global.timeRespawn = MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].timeRespawn;   //set le temps de respawn des joueurs
        Global.multi = true;

        //reset les anciens choix
        Global.powerMapAccepted.Clear();
        Global.playersData.Clear();
        //pouvoir accepté par la map
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].white)
            Global.powerMapAccepted.Add(0);
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].blue)
            Global.powerMapAccepted.Add(1);
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].red)
            Global.powerMapAccepted.Add(2);
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].yellow)
            Global.powerMapAccepted.Add(3);
        if (MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].green)
            Global.powerMapAccepted.Add(4);

        bool bluePower = false;
        bool redPower = false;
        bool yellowPower = false;
        bool greenPower = false;

        //parcourt les X joueurs
        for (int i = 0; i < multiPlayerList.Count; i++)
        {
            bluePower = false;
            redPower = false;
            yellowPower = false;
            greenPower = false;
            if (multiPlayerList[i][0] == 1)
                bluePower = true;
            else if (multiPlayerList[i][0] == 2)
                redPower = true;
            else if (multiPlayerList[i][0] == 3)
                yellowPower = true;
            else if (multiPlayerList[i][0] == 4)
                greenPower = true;

            Global.addPlayer(false, bluePower, redPower, yellowPower, greenPower, multiPlayerList[i][1], multiPlayerList[i][2]);   //créé un joueur de la team 1, repulsor
        }
        //ajoute les X players
        
        //Global.addPlayer(false, false, false, false, true, 2, 2);   //créé un joueur de la team 2, attractor

        PlayerPrefs.PP.Save();                                                                                  //sauvegarder les données
        QOC.jumpToSceneWithFade(MapPrefs.PP.mapsInfosMulti[PlayerPrefs.PP.mapUnlockMulti].nameLevel);         //sauter à la scènes
    }

    /// <summary>
    /// juste play !
    /// </summary>
    public void justPlay()
    {
        Debug.Log("idworld: " + campainLocation + ", idlevel: " + (levelLocation - 1));

        //id world: campainLocation
        //id level: levelLocation - 1
        if (campainLocation == 0 && (!Global.fromGame || levelLocation == 0))
        {
            levelLocation = 1;
            howManyJoypadAreConnected();    //compte le nombre de joypad connecté
            if (joypadConnected >= 2)
            {
                setCoop(true);
                player2Control = 2;
            }
        }

        defineControlAndPower();

        fillPlayerList();         //remplie la liste des joueurs

        Global.maxEggs = MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).maxEggs;           //set le maxEggs à donner à la reine
        Global.timeMax = MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).timeMax;           //set le timerMax (multi), (solo à revoir)
        Global.timeRespawn = MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).timeRespawn;   //set le temps de respawn des joueurs
        Global.keyboardType = keyboardType;                                                                     //sauvegarde si on a switch de type de keyboard
        Global.multi = false;


        Global.displayValue();                                                                                  //debug display value

        PlayerPrefs.PP.lastLevelPlayerId[0] = campainLocation;                                                  //set l'id de la map qui va jouer
        PlayerPrefs.PP.lastLevelPlayerId[1] = levelLocation - 1;                                                //set l'id de la map qui va jouer


        PlayerPrefs.PP.Save();                                                                                  //sauvegarder les données

        Global.tmpPlayer1Control = player1Control;
        Global.tmpPlayer2Control = player2Control;

        if (Global.fromGame)    //on vient de next level, on jump au next level SANS fade
        {
            Global.fromGame = false;                //remetre à false (on en a plus besoin)
            QOC.jumpToScene(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).nameLevel);         //sauter à la scènes
        }
        else
           QOC.jumpToSceneWithFade(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).nameLevel);         //sauter à la scènes
    }

    /// <summary>
    /// remplie la lsite des joueurs, relativ à la map !!
    /// </summary>
    public void fillPlayerList()
    {
        Debug.Log("ici le gros fillPlayerList");
        Global.powerMapAccepted.Clear();
        Global.playersData.Clear();
        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).white)
            Global.powerMapAccepted.Add(0);
        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue)
            Global.powerMapAccepted.Add(1);
        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red)
            Global.powerMapAccepted.Add(2);
        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow)
            Global.powerMapAccepted.Add(3);
        if (MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green)
            Global.powerMapAccepted.Add(4);

        if (!multi)
        {
            //si on est tout seul
            if (!MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).coop || !Global.soloAnd2Player)
            {
                Global.soloAnd2Player = false;
                Global.addPlayer(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).white,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green,
                    1, player1Control);
            }
            //si on est en coop
            else
            {
                if (Global.soloAnd2Player)  //si on a choisi de faire du coop
                {
                    //ici répartir les pouvoirs merde !
                    Global.addPlayer(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).white1,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue1,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red1,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow1,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green1,
                    1, player1Control);

                    Global.addPlayer(MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).white2,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).blue2,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).red2,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).yellow2,
                    MapPrefs.PP.getLevels(campainLocation, levelLocation - 1).green2,
                    1, player2Control);
                }
            }
        }
        //
    }

    /// <summary>
    /// fonction appelé lorsque le menu exit s'affiche/s'enlève
    ///  - si s'affiche: 
    ///     - timer false
    ///     - selection le boutton quitter, et change la location du menu quit à 0
    ///  - si s'enlève:
    ///     - remet la selection sur le bouton quitter
    /// </summary>
    public void displayMenuExit(bool active)
    {
        //Debug.Log("ici fuck");
        if (!TWNE.isOk)
            return;
        TWNE.isOk = false;

        if (active)
        {
            exit.SetActive(true);
            StartCoroutine(selectButton(listButtonEscape[1]));
            //listButtonEscape[1].Select();
            //locationQuit = 0;
        }
        else
        {
            exit.SetActive(false);

            if (superLocation == 0) //si on est dans le menu principal
            {
                StartCoroutine(selectButton(listButtonChoice[location]));
            }
            else if (superLocation == 3)    //si on est dans le mode campain
            {
                StartCoroutine(selectButton(listButtonCampain[campainLocation]));
            }
            else if (superLocation == 4)    //on est zoomé sur un monde
            {
                Debug.Log("on est zoomé sur un monde");
                backToCHoiceLevel();
                //aa
            }
            else if (superLocation == 8)    //on est a la sélection des levels du modes multi
            {
                //changeHighlightedLevelMulti(PlayerPrefs.PP.mapUnlockMulti);
                CMM.listButtonMutli[PlayerPrefs.PP.mapUnlockMulti].Select();
            }
            else if (superLocation == 11)
            {
                howManyJoypadAreConnected();
                if (joypadConnected >= 2)
                    StartCoroutine(selectButton(playButtons[1]));
                else
                    StartCoroutine(selectButton(playButtons[0]));
            }
            /*if (saveLocation == -1)
                listButtonChoice[listButtonChoice.Length - 1].Select();
            else
            {
                listButtonChoice[saveLocation].Select();
            }*/
        }
    }

    private void Update()
    {
        if (TWNE.isOk && !goToNextLevel)
        {
            if (!exit.activeSelf)
            {
                if (superLocation != 12)
                    playerInput();                          //input du joueur qui va switch avec manette ou clavier
                else
                    playerInputMulti();
                //changeFocus();                          //vérifie le changement du focus
                exitActive = false;
            }
            else
            {
                escapeMenuInput();
                exitActive = true;
            }
        }
    }
}
