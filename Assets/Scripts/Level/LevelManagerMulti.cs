using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerMulti : MonoBehaviour
{
    public GameObject groupMap;                                                 //parent qui contient toute les map
    public int indexMap = 0;                                                    //l'index de la map choisi

    [SerializeField]
    public Sprite[] picPower;                                                  //référence des 4 Sprites des 4 pouvoirs;
    public struct MapData                                                       //structure de la map
    {
        public bool white;
        public bool repulse;
        public bool attract;
        public bool blackhole;
        public bool flaire;
    }
    public MapData mapsTmp;
    public List<MapData> mapsData = new List<MapData>();                       //liste des players
    [Space(10)]
    [Header("variable menu")]
    public int mainLocation = 0;
    public int mapLocation = 0;
    public Image backgroudMap;
    public Image backgroudTeam;
    public Image backgroudPlay;
    public Color colorActive;
    public Color colorInactive;
    public Color colorValidate;
    public GameObject[] playerInQueue;
    public GameObject[] panel;
    public GameObject panelAntsBlue;
    public GameObject panelAntsRed;
    public Sprite[] picPowerPlayerTab;
    public Sprite[] iconAntsPower;
    [Space(10)]
    public Image[] ChooseTab;
    public Color colorTabActive;
    public Color colorTabActiveBlue;
    public Color colorTabActiveRed;
    public Color colorTabInactive;
    public Color colorTabInactiveBlue;
    public Color colorTabInactiveRed;
    [Space(10)]
    [Header("debug")]
    public GameObject playerJoypad1;
    public GameObject backgroundForTuto;
    public GameObject chargement;
    /// <summary>
    /// variable privé
    /// </summary>
    private List<int> powerMapAccepted = new List<int>();                       //liste des pouvoirs accepté par la map
    [HideInInspector] public TimeWithNoEffect TWNE;
    private GameObject GlobalVariableManager;
    private QuitOnClick QOC;
    [HideInInspector] public PlayerConnected PC;
    [HideInInspector] public GlobalVariableManager Global;
    private int prevMapLocation = -1;
    private int mapValid = 0;
    private int prevMainLocation = -1;
    private bool isGamePlayable = false;

    /// <summary>
    /// initialisation
    /// </summary>
    private void Awake()
    {
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        GlobalVariableManager = GameObject.FindGameObjectWithTag("Global");
        QOC = gameObject.GetComponent<QuitOnClick>();
        if (GlobalVariableManager)
        {
            PC = GlobalVariableManager.GetComponent<PlayerConnected>();
            Global = GlobalVariableManager.GetComponent<GlobalVariableManager>();
        }
    }

    /// <summary>
    /// start
    /// </summary>
    private void Start()
    {
        fillMapList();                                                          //remplie la liste des maps, et highlight la dernière map débloqué
        Global.multi = true;
        Global.fromMenu = true;
        Global.powerMapAccepted = powerMapAccepted;                             //lie la liste du global à la liste courrante d'ici
        Cursor.visible = false;
        checkJoyPadInteractable();
        resetAllPower();
        //PC.LMS = null;
        //PC.LMM = gameObject.GetComponent<LevelManagerMulti>();
    }

    /// <summary>
    /// reset tout les sprite et pouvoir des joueurs à -1
    /// </summary>
    void resetAllPower()
    {
        Debug.Log("change les pouvoirs ?");
        for (int i = 0; i < playerInQueue.Length; i++)
        {
            playerInQueue[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
            //playerInQueue[i].GetComponent<MovePlayer>().prevPowerType = -1;
        }
    }

    /// <summary>
    /// check si les manettes sont connecté, et active ou désactive les joypads
    /// </summary>
    public void checkJoyPadInteractable()
    {
        for (int i = 0; i < PC.playerControllerConnected.Count; i++)
        {
            if (PC.playerControllerConnected[i])
                switchOffPlayerTab(i, true);
            else
                switchOffPlayerTab(i, false);
        }
        //disableKeyboardIfJoyPadConnected();                                 //désactive les joueurs clavier si il y a assez de joypad !
        refreshPanel();
        testForPlay();
    }

    /// <summary>
    /// active ou désactive l'onglet du joueur index
    /// </summary>
    /// <param name="index"></param>
    void switchOffPlayerTab(int index, bool active)
    {
        if (active)
        {
            playerInQueue[index].transform.GetChild(0).GetComponent<Toggle>().interactable = true;
            playerInQueue[index].SetActive(true);
            playerInQueue[index].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = picPowerPlayerTab[playerInQueue[index].GetComponent<MovePlayer>().powerType - 1];
        }
        else
        {
            playerInQueue[index].transform.GetChild(0).GetComponent<Toggle>().interactable = false;
            playerInQueue[index].transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            playerInQueue[index].transform.SetParent(panel[1].transform);
            playerInQueue[index].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
        }
    }

    /// <summary>
    /// désactive les joueurs clavier si il y a 3 ou 4 mannette de connecté !
    /// </summary>
    void disableKeyboardIfJoyPadConnected()
    {
        int joypadConnected = 0;
        for (int i = 1; i < PC.playerControllerConnected.Count - 1; i++)
        {
            if (PC.playerControllerConnected[i])
                joypadConnected++;
        }
        if (joypadConnected == 3)
            switchOffPlayerTab(5, false);
        else if (joypadConnected == 4)
        {
            switchOffPlayerTab(0, false);
            switchOffPlayerTab(5, false);
        }            
    }

    /// <summary>
    /// fonction qui ajoute une map à la liste des map.
    /// en donnant les 4 pouvoirs possibles pour la map.
    /// </summary>
    public void addMapPower(bool repu, bool attra, bool black, bool flai)
    {
        mapsTmp.repulse = repu;
        mapsTmp.attract = attra;
        mapsTmp.blackhole = black;
        mapsTmp.flaire = flai;
        Debug.Log("add " + repu + " " + attra + " " + black + " " + flai);
        mapsData.Add(mapsTmp);
    }

    /// <summary>
    /// fonction qui est appelé lorsque l'on change de map
    /// juste change l'index de la map par rapport au toggle ON dans la liste des childCOunt.
    /// vite et remplie à nouveau powerAccepted avec la liste d'index que la map accepte grace à la liste mapsData
    /// (celle-ci ayant été initialisé au début du jeu)
    /// </summary>
    public void changeIndexMap()
    {
        indexMap = mapLocation;                                                                   //assigne l'index de la map courrante
        powerMapAccepted.Clear();                                                       //clear la liste des pouvoirs autorisé de la map
        if (mapsData[indexMap].repulse)
            powerMapAccepted.Add(0);                                                    //si repulse: ajoute "0" à la liste !
        if (mapsData[indexMap].attract)
            powerMapAccepted.Add(1);
        if (mapsData[indexMap].blackhole)
            powerMapAccepted.Add(2);
        if (mapsData[indexMap].flaire)
            powerMapAccepted.Add(3);
    }

    /// <summary>
    /// remplie la liste des maps
    /// </summary>
    public void fillMapList()
    {
    }

    /////////////////////////////////////////////////////////////////// fill player //////////////////////////////////////////////////////////
    /// <summary>
    /// remplie la lsite des joueurs, relativ à la map !!
    /// </summary>
    public void fillPlayerList()
    {
        Debug.Log("ici le gros fillPlayerList");
        Global.playersData.Clear();
        MovePlayer MP;
        for (int i = 0; i < panel[0].transform.childCount; i++)
        {
            MP = panel[0].transform.GetChild(i).GetComponent<MovePlayer>();
            //si le joueur n'est pas un joueur clavier
            Global.addPlayer((MP.powerType == 0), (MP.powerType == 1), (MP.powerType == 2), (MP.powerType == 3), (MP.powerType == 4), 1, (int)MP.TOP);
        }
        //team rouge vérification
        for (int j = 0; j < panel[2].transform.childCount; j++)
        {
            MP = panel[2].transform.GetChild(j).GetComponent<MovePlayer>();
            //si le joueur n'est pas un joueur clavier
            Global.addPlayer((MP.powerType == 0), (MP.powerType == 1), (MP.powerType == 2), (MP.powerType == 3), (MP.powerType == 4), 2, (int)MP.TOP);
        }
    }

    /// <summary>
    /// lorsque le joueur clique sur play.
    /// </summary>
    public void justPlay()
    {
        fillPlayerList();
        Global.displayValue();
        //PlayerPrefs.PP.lastLevelPlayerId[1] = MapPrefs.PP.mapsInfosMulti[mapLocation].nameLevel;
        PlayerPrefs.PP.lastLevelPlayerId[1] = MapPrefs.PP.mapsInfosMulti[mapLocation].mapId;
        //if (mapLocation < MD.dms.Count && MD.dms[mapLocation].worldId == MD.dms[mapLocation + 1].worldId)
        //  PlayerPrefs.PP.nextLevel = MD.dms[mapLocation + 1].nameLevel;
        PlayerPrefs.PP.Save();
        QOC.jumpToScene(MapPrefs.PP.mapsInfosMulti[mapLocation].nameLevel);
        //QOC.jumpToScene("5_Game");
    }

    //////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////    interaction menu    ///////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// gère les inputs clavier ou joypad du joueurs
    /// </summary>
    private void playerInput()
    {
        if (mainLocation == 0 && TWNE.isOk)                                          //si on est dans le choix des map
        {
            //mapLocation
            //clavier zqsd, joystick ou clavier arrow
            if (PC.getPlayer(0).GetAxis("UIHorizontal") < 0 || PC.getPlayer(5).GetAxis("UIHorizontal") < 0 || PC.getPlayer(1).GetAxis("UIHorizontal") < -0.5f)
            //if (Input.GetAxisRaw("Horizontal1") < -0.9f || Input.GetAxisRaw("Horizontal5") == -1 || Input.GetAxisRaw("Horizontal6") == -1)       
                mapLocation--;
            //joypad 1, ou clavier ZQSD ou clavier arrow UP
            else if (PC.getPlayer(0).GetAxis("UIHorizontal") > 0 || PC.getPlayer(5).GetAxis("UIHorizontal") > 0 || PC.getPlayer(1).GetAxis("UIHorizontal") > 0.5f
            //else if ((Input.GetAxisRaw("Horizontal1") > 0.9f || Input.GetAxisRaw("Horizontal5") == 1 || Input.GetAxisRaw("Horizontal6") == 1)
                        && mapLocation < mapValid - 1)
                mapLocation++;
            //restreint le focus courrant au nombre de boutton présent dans la liste (le focus ne peu pas dépasser sur une map innexistant)
            mapLocation = (mapLocation >= mapValid) ? mapValid - 1 : mapLocation;
            mapLocation = (mapLocation < 0) ? 0 : mapLocation;

            //action OK du clavier ou de la mannette
            if (PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit"))
            {
                //groupMap.transform.GetChild(mapLocation).GetComponent<Toggle>().isOn = true;
                mainLocation = 1;
            }
        }
        if (mainLocation == 1 && TWNE.isOk)
        {
            if ((PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit"))
                   && isGamePlayable)
            {
                chargement.SetActive(true);
                mainLocation = 2;
                TWNE.isOk = false;
                justPlay();
            }
            //Debug.Log("ici tout le temps ?");
        }
    }

    /// <summary>
    /// change le focus si il a été changé quelque part ailleur
    /// </summary>
    private void changeMapFocus()
    {
        if (mapLocation != prevMapLocation)
        {
            groupMap.transform.GetChild(mapLocation).GetComponent<Toggle>().isOn = true;
            prevMapLocation = mapLocation;
            changeIndexMap();
            TWNE.isOk = false;
        }
    }

    /// <summary>
    /// change le focus principale (map/choix de team/play)
    /// </summary>
    private void changeMainFocus()
    {
        if (mainLocation != prevMainLocation)
        {
            switch (mainLocation)
            {
                case 0:                                             //si les joueurs sont dans la sélection de map
                    backgroudMap.color = colorActive;
                    backgroudTeam.color = colorInactive;
                    backgroudPlay.color = colorInactive;
                    setPlayTutoActive(false);                       //le jeu n'est plus jouable
                    ChooseTab[0].color = colorTabActive;
                    ChooseTab[1].color = colorTabInactiveBlue;
                    ChooseTab[2].color = colorTabInactiveRed;
                    break;
                case 1:
                    backgroudMap.color = colorValidate;
                    backgroudTeam.color = colorActive;
                    backgroudPlay.color = colorInactive;
                    ChooseTab[0].color = colorTabInactive;
                    ChooseTab[1].color = colorTabActiveBlue;
                    ChooseTab[2].color = colorTabActiveRed;
                    resetAllPower();                                //reset les pouvoirs des players
                    testForPlay();                                  //test si le jeu est jouable !
                    break;
                case 2:
                    backgroudMap.color = colorValidate;
                    backgroudTeam.color = colorValidate;
                    backgroudPlay.color = colorActive;
                    break;
            }
            prevMainLocation = mainLocation;
            TWNE.isOk = false;
        }
    }

    /// <summary>
    /// affiche/cache les icon des fourmis représentant les joueurs
    /// de chaque panel, et les color selon le type de fourmis choisis
    /// </summary>
    public void refreshPanel()
    {
        MovePlayer MP;
        int i = -1;
        while (++i < panel[0].transform.childCount)
        {
            MP = panel[0].transform.GetChild(i).GetComponent<MovePlayer>();
            panelAntsBlue.transform.GetChild(i).gameObject.SetActive(true);
            panelAntsBlue.transform.GetChild(i).GetComponent<Image>().sprite = iconAntsPower[MP.powerType - 1];
        }
        --i;
        while (++i < 3)
            panelAntsBlue.transform.GetChild(i).gameObject.SetActive(false);
        i = -1;
        while (++i < panel[2].transform.childCount)
        {
            MP = panel[2].transform.GetChild(i).GetComponent<MovePlayer>();
            panelAntsRed.transform.GetChild(i).gameObject.SetActive(true);
            panelAntsRed.transform.GetChild(i).GetComponent<Image>().sprite = iconAntsPower[MP.powerType - 1];
        }
        --i;
        while (++i < 3)
            panelAntsRed.transform.GetChild(i).gameObject.SetActive(false);
    }

    /// <summary>
    /// affiche ou cache le texte tuto / play
    /// change la valeur isGamePlayable qui défini si on peut jouer ou non !
    /// </summary>
    /// <param name="active"></param>
    private void setPlayTutoActive(bool active)
    {
        isGamePlayable = active;
        backgroundForTuto.transform.GetChild(0).gameObject.SetActive(!active);
        backgroundForTuto.transform.GetChild(1).gameObject.SetActive(active);
    }

    /// <summary>
    /// fonction qui est appelé au bon moment quand on appuis sur Entrer ou A pour valider le jeu pour jouer
    /// test si tout les joueurs sont validé (sauf les joueurs clavier) pour passer au jeu
    /// </summary>
    public void testForPlay()
    {
        //compte s'il y a au moin 1 joueur dans chaque team
        //compte s'il n'y a pas plus de 4 joueurs
        //test pour chaque joueur joystick s'il sont validé
        int teamBlue = panel[0].transform.childCount;
        int teamRed = panel[2].transform.childCount;
        if (teamBlue == 0 || teamRed == 0 || teamBlue + teamRed > 6)    //changé de 4 à 6
        {
            setPlayTutoActive(false);       //affiche le tuto
            return;                         //quitte
        }

        MovePlayer MP;
        //team bleu vérification
        for (int i = 0; i < teamBlue; i++)
        {
            MP = panel[0].transform.GetChild(i).GetComponent<MovePlayer>();
            //si le joueur n'est pas un joueur clavier
            if (MP && (int)MP.TOP != 0 && (int)MP.TOP != 5 && !MP.lockedTeam)
            {
                setPlayTutoActive(false);       //affiche le tuto
                return;                         //non plus ! y'a un joueur qui n'a pas validé !
            }
                
        }
        //team rouge vérification
        for (int j = 0; j < teamRed; j++)
        {
            MP = panel[2].transform.GetChild(j).GetComponent<MovePlayer>();
            //si le joueur n'est pas un joueur clavier
            if (MP && (int)MP.TOP != 0 && (int)MP.TOP != 5 && !MP.lockedTeam)
            {
                setPlayTutoActive(false);       //affiche le tuto
                return;                         //non plus ! y'a un joueur qui n'a pas validé !
            }
        }
        //ici c'est ok pour le mode play !
        setPlayTutoActive(true);                //affiche le texte "vous pouvez jouer !"
    }

    /// <summary>
    /// action quand on appruis sur echap ou B
    /// </summary>
    private void quitAction()
    {
        //action retour du joueur 0 ou 1 (clavier ou joystick 1)
        if (PC.getPlayer(0).GetButtonDown("UICancel") || (PC.getPlayer(1).GetButtonDown("UICancel") && !playerInQueue[1].GetComponent<MovePlayer>().lockedTeam))
        {
            switch (mainLocation)
            {
                case 0:
                    gameObject.GetComponent<QuitOnClick>().jumpToScene("1_MainMenu");   //on revient au menu précédent
                    break;
                case 1:
                    mainLocation = 0;
                    resetAllPower();
                    break;
                case 2:
                    mainLocation = 1;
                    break;
            }
        }
    }

    private void Update()
    {
        if (TWNE.isOk)                              //limite les inputs pour pas aller trop vite
        {
            playerInput();                          //get player input
            changeMapFocus();                       //change le focus de la map si elle a changé
            changeMainFocus();                      //change le focus principale (map/choix de team/play)
            quitAction();
        }
        //resetFocusIfClickedWithMouse();             //refocus si missclique
    }
}
