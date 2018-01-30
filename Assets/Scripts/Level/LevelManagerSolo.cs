using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerSolo : MonoBehaviour
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
    public GameObject Chargement;                                              //panel UI de chargement qui cache tout le reste
    public int mainLocation = 0;
    public int mapLocation = 0;
    public Image backgroudMap;
    public Image backgroudTeam;
    public Color colorActive;
    public Color colorInactive;
    public Color colorValidate;
    public Toggle[] panelSoloOrCoop;                                //tableau comportant le panel solo et coop
    //objet à activer/désactiver
    public GameObject toActiveSolo;
    public GameObject toDesactiveSolo;
    public GameObject toActiveCoop;
    public GameObject toDesactiveCoop;
    public GameObject modeBlocked;

    [Space(10)]
    public Image[] chooseTab;                                     //onglet "choose map"
    public Color colorTabActive;
    public Color colorTabInactive;

    [Space(10)]
    public GameObject soloKeyboard;
    public GameObject soloJoypad;
    public GameObject coop2keyboard;
    public GameObject coopKeyboardJoypad;
    public GameObject coopJoypadJoypad;
    public bool keyboardType = true;

    /// <summary>
    /// private serealized field
    /// </summary>
    [SerializeField] private GameObject ChargementPicture;

    /// <summary>
    /// variable privé
    /// </summary>
    public GameObject[] panelJoueur;                                           //panel du joueur 1  et 2
    private List<int> powerMapAccepted = new List<int>();                       //liste des pouvoirs accepté par la map
    private List<int> powerPlayer1 = new List<int>();                           //pouvoir du joueur 1
    private List<int> powerPlayer2 = new List<int>();                           //pouvoir du joueur 1
    [HideInInspector]
    public TimeWithNoEffect TWNE;
    private GameObject GlobalVariableManager;
    [HideInInspector]
    public PlayerConnected PC;
    [HideInInspector]
    public GlobalVariableManager Global;
    private int prevMapLocation = -1;
    private int mapValid = 0;
    private int prevMainLocation = -1;
    private int locationCoop = 0;
    private int joypadConnected = 0;
    private bool goToNextLevel = false;

    /// <summary>
    /// initialisation
    /// </summary>
    private void Awake()
    {
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        GlobalVariableManager = GameObject.FindGameObjectWithTag("Global");
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
        prevMapLocation = -1;                                                   //oblige à actualiser la location courrante
        fillMapList();                                                          //remplie la liste des maps, et highlight la dernière map débloqué
        Global.multi = false;
        if (!Global.fromGame)   //reset les valeur SI on ne veins pas de next Level
        {
            Global.soloAnd2Player = false;
            Global.fromMenu = true;
        }
        Global.powerMapAccepted = powerMapAccepted;                             //lie la liste du global à la liste courrante d'ici
        Cursor.visible = false;
        checkJoyPadInteractable();
        //PC.LMS = gameObject.GetComponent<LevelManagerSolo>();
        //PC.LMM = null;
        PlayerPrefs.PP.fromRestart = false;

        HandleNextLevel();                                                      //test si on viens du game (pour next level) ou pas
    }

    /// <summary>
    /// gère si on doit directement passer au level suivant, (si on arrive depuis le game) ou pas.
    /// </summary>
    void HandleNextLevel()
    {
        if (Global.fromGame)                        //si on viens du game (nextLevel)
        {
            Global.fromGame = false;                //remetre à false (on en a plus besoin)

            //test si la prochaine map permet le next level, si non, annuler et
            //faire comme si on viens d'arriver dans le menu
            if (PlayerPrefs.PP.isLastLevelMap(PlayerPrefs.PP.lastLevelPlayerId[0], PlayerPrefs.PP.lastLevelPlayerId[1]))
            {
                Debug.Log("on a cliqué sur next world depuis le jeu !! fair eun truck ?");
                Global.soloAnd2Player = false;
                Global.fromMenu = true;
                return;
            }
                

            joypadConnected = howManyJoypadAreConnected();          //compte combien de joypad sont connecté

            Chargement.SetActive(true);             //afficher le chargement
            goToNextLevel = true;                   //désactiver tout opération sur le menu

            

            mapLocation = PlayerPrefs.PP.lastLevelPlayerId[1] + 1;             //set la nouvelle mapLocation (pour le justPlay) à l'ancienne + 1 (la prochaine map !)
            changeMapFocus();                                               //change le map focus manuellement

            keyboardType = Global.keyboardType;             //remet le keyboard type sauvegardé de la map précédente (pour garder les contrôles des joueurs Si on est sur joypad + clavier)

            //essaye de garder le mode coop SI la map suivante le permet !
            if (!(Global.soloAnd2Player && powerMapAccepted.Count > 1 && MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], mapLocation).coop))
                Global.soloAnd2Player = false;
            else
            {
                //si on garde le coop, set les bon pouvoirs aux 2 joueurs par rapport aux pouvoir accepté par la map
                //pouvoir accepté par la map qui on été actualisé lorsqu'on a changé mapLocation juste en haut)
                changeIndexPlayer();
                if (Global.soloAnd2PlayerSwitchPower)       //si la dernière fois on avais switch...
                    switchPlayer1and2();                    //le refaire pour garder les mêes pouvoirs dans les joueurs
                
            }

            justPlay();
        }
    }

    /// <summary>
    /// test si le mode keyboard est zqsd ou arrow
    /// </summary>
    void changeKeyboard()
    {
        if (PC.keyboardZqsd() && !keyboardType)
        {
            keyboardType = true;
            soloKeyboard.transform.GetChild(0).transform.gameObject.SetActive(true);
            soloKeyboard.transform.GetChild(1).transform.gameObject.SetActive(false);

            coopKeyboardJoypad.transform.GetChild(0).transform.GetChild(0).transform.gameObject.SetActive(true);
            coopKeyboardJoypad.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
        }
        else if (PC.keyboardArrow() && keyboardType)
        {
            keyboardType = false;
            soloKeyboard.transform.GetChild(0).transform.gameObject.SetActive(false);
            soloKeyboard.transform.GetChild(1).transform.gameObject.SetActive(true);

            coopKeyboardJoypad.transform.GetChild(0).transform.GetChild(0).transform.gameObject.SetActive(false);
            coopKeyboardJoypad.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// reset mainLocation pour actualiser les mannettes
    /// </summary>
    public void checkJoyPadInteractable()
    {
        prevMainLocation = -2;
    }

    /// <summary>
    /// switch les pouvoirs des joueurs 1 et 2
    /// puis actualise la GUI
    /// </summary>
    public void switchPlayer1and2()
    {
        Debug.Log("switch...........");
        if (powerMapAccepted.Count <= 1)
            return;
        List<int> powerTmp = powerPlayer1;
        powerPlayer1 = powerPlayer2;
        powerPlayer2 = powerTmp;
        Global.soloAnd2PlayerSwitchPower = !Global.soloAnd2PlayerSwitchPower;
        actualisePlayerGUISprite();
    }

    /// <summary>
    /// change les pouvoirs des joueurs en fonctions des pouvoirs autorisé par la map nouvellement sélectionné !
    /// </summary>
    void changeIndexPlayer()
    {
        Debug.Log("change les pouvoirs des joueurs en fonctions des pouvoirs autorisé par la map nouvellement sélectionné !");
        powerPlayer1.Clear();
        powerPlayer2.Clear();
        powerPlayer1.Add(powerMapAccepted[0]);
        if (powerMapAccepted.Count == 2)                                        //s'il n'y a que 2 pouvoirs dans la map, active le premier pouvoir dans la team 1, et le deuxième dans la team 2 !
            powerPlayer2.Add(powerMapAccepted[1]);                              //pour le joueur 2, active le pouvoirs se trouvant dans l'index 1 de powerAccepted                        
        else if (powerMapAccepted.Count == 3)                                  //s'il y a 2 pouvoirs, en donner 2 au joueur 1, et 1 ou joueur 2 !
        {
            powerPlayer2.Add(powerMapAccepted[1]);
            powerPlayer1.Add(powerMapAccepted[2]);
        }
        else if (powerMapAccepted.Count == 4)                                  //s'il y a 4 pouvoirs, donner 2 et 2 au deux joueurs !
        {
            powerPlayer2.Add(powerMapAccepted[1]);
            powerPlayer1.Add(powerMapAccepted[2]);
            powerPlayer2.Add(powerMapAccepted[3]);
        }
        actualisePlayerGUISprite();                                               //actualise la GUI des joueurs 1 et 2
    }

    /// <summary>
    /// change les sprites des panel 1 et 2 des joueurs, selon les valeurs de powerPlayer1 et powerPlayer2;
    /// </summary>
    void actualisePlayerGUISprite()
    {
        Debug.Log("change les sprites des panel 1 et 2 des joueurs, selon les valeurs de powerPlayer1 et powerPlayer2");
        //panelJoueur à l'index 0 correspond au panel du joueur 1
        //son child 0 est le premier objet contenant l'image du pouvoir 1
        //son child 1 est le deuxieme objet contenant l'image du pouvoir 2
        panelJoueur[0].transform.GetChild(0).GetComponent<Image>().sprite = picPower[powerPlayer1[0]];          //change son sprite à l'index 0 de powerPlayer1
        if (powerPlayer1.Count == 1)                                                                            //si le joueur 1 n'a qu'un pouvoir, 
            panelJoueur[0].transform.GetChild(1).gameObject.SetActive(false);                                   //désactiver le 2eme sprite
        else
        {
            panelJoueur[0].transform.GetChild(1).gameObject.SetActive(true);                                    //active l'objet du second pouvoir
            panelJoueur[0].transform.GetChild(1).GetComponent<Image>().sprite = picPower[powerPlayer1[1]];      //change son sprite à l'index 1 de powerPlayer1
        }

        //panelJoueur à l'index 1 correspond au panel du joueur 2
        //son child 0 est le premier objet contenant l'image du pouvoir 1
        //son child 1 est le deuxieme objet contenant l'image du pouvoir 2
        panelJoueur[1].transform.GetChild(0).GetComponent<Image>().sprite = picPower[powerPlayer2[0]];          //change son sprite à l'index 0 de powerPlayer2
        if (powerPlayer2.Count == 1)                                                                            //si le joueur 2 n'a qu'un pouvoir, 
            panelJoueur[1].transform.GetChild(1).gameObject.SetActive(false);                                   //désactiver le 2eme sprite
        else
        {
            panelJoueur[1].transform.GetChild(1).gameObject.SetActive(true);                                    //active l'objet du second pouvoir
            panelJoueur[1].transform.GetChild(1).GetComponent<Image>().sprite = picPower[powerPlayer2[1]];      //change son sprite à l'index 1 de powerPlayer2
        }
    }

    /// <summary>
    /// compte le nombre de joypad connecté
    /// </summary>
    /// <returns></returns>
    int howManyJoypadAreConnected()
    {
        int joypadConnected = 0;
        for (int i = 1; i < PC.playerControllerConnected.Count - 1; i++)
        {
            if (PC.playerControllerConnected[i])
                joypadConnected++;
        }
        return (joypadConnected);
    }

    /// <summary>
    /// fonction qui ajoute une map à la liste des map.
    /// en donnant les 4 pouvoirs possibles pour la map.
    /// </summary>
    public void addMapPower(bool white,  bool repu, bool attra, bool black, bool flai)
    {
        mapsTmp.white = white;
        mapsTmp.repulse = repu;
        mapsTmp.attract = attra;
        mapsTmp.blackhole = black;
        mapsTmp.flaire = flai;
        Debug.Log("add " + white + " " + repu + " " + attra + " " + black + " " + flai);
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
        Debug.Log(indexMap);
        for (int i = 0; i < mapsData.Count; i++)
            Debug.Log("map data:" + i);

        if (mapsData[indexMap].white)
            powerMapAccepted.Add(0);
        if (mapsData[indexMap].repulse)
            powerMapAccepted.Add(1);                                                    //si repulse: ajoute "0" à la liste !
        if (mapsData[indexMap].attract)
            powerMapAccepted.Add(2);
        if (mapsData[indexMap].blackhole)
            powerMapAccepted.Add(3);
        if (mapsData[indexMap].flaire)
            powerMapAccepted.Add(4);

        if (joypadConnected >= 2 && powerMapAccepted.Count > 1 && MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], indexMap).coop)       //si il y a présente de 2+ manette
        {
            panelSoloOrCoop[0].isOn = false;
            panelSoloOrCoop[1].isOn = true;
        }
        else
        {
            panelSoloOrCoop[0].isOn = true;
            panelSoloOrCoop[1].isOn = false;
        }
    }

    /// <summary>
    /// remplie la liste des maps
    /// </summary>
    public void fillMapList()
    {
        Debug.Log("old");
        Debug.Break();
        /*
        mapsData.Clear();                                                                       //clear la liste pour en créé une nouvelle
        mapValid = 0;
        for (int i = 0; i < MapPrefs.PP.mapsInfosSolo.Count; i++)                                 //pour chaque map de la liste...
        {
            if (!MapPrefs.PP.mapsInfosSolo[i].blocked          //si la map n'est pas bloqué...
                )        //et que son 2 eme enfant s'appelle "power"     
            {
                mapValid++;         //ajouter aux map valide 1

                addMapPower(MapPrefs.PP.mapsInfosSolo[i].white, MapPrefs.PP.mapsInfosSolo[i].blue, MapPrefs.PP.mapsInfosSolo[i].red, MapPrefs.PP.mapsInfosSolo[i].yellow, MapPrefs.PP.mapsInfosSolo[i].green);                   //ajoute la map à la liste, avec les 1 - 4 pouvoirs que la map accepte !
            }
            //repulseTmp = attractTmp = blackholeTmp = flaireTmp = false;
        }
        mapLocation = mapValid - 1;
        if (mapLocation < 0)
            mapLocation = 0;
       */
    }

    /////////////////////////////////////////////////////////////////// fill player //////////////////////////////////////////////////////////
    /// <summary>
    /// mode solo, qu'un seule player de team 1, avec ses pouvoirs selon la map !
    /// </summary>
    public void addSoloPlayer()
    {
        Debug.Log("ici ajoute 1 player (on est en mode solo !)");
        int tmpIdControll = 0;
        tmpIdControll = (joypadConnected == 0) ? 0 : 1;             //si aucun joypad, control 0: keyboard, sinon, control 1: le joypad 1 !
        
        //si les contrôles sont le clavier, et que le type est ARROW, changer 0 en 5 !
        if (tmpIdControll == 0 && !keyboardType)
        {
            tmpIdControll = 5;
        }

        //créé un joueur de la team 1, avec les pouvoirs que la map accepte
        Global.addPlayer(mapsData[indexMap].white, mapsData[indexMap].repulse, mapsData[indexMap].attract, mapsData[indexMap].blackhole, mapsData[indexMap].flaire, 1, tmpIdControll);
    }

    /// <summary>
    /// dans le mode solo avec 2 joueurs, ajoute les 2 joueurs selon  powerPlayer1 et powerPlayer2;
    /// </summary>
    public void add2playerSolo()
    {
        Debug.Log("ici on est en mode solo2, on ajoute les 2 players avec leur pouvoirs selon powerPlayer1/2");
        int tmpIdControll1 = 0;         //contrôle du joueur 1: clavier zqsd
        int tmpIdControll2 = 5;         //contrôle du joueur 2: clavier arrow
        if (joypadConnected == 1)       //si UN joypad est connecté
        {
            tmpIdControll1 = 1;         //le joueur 1 est au joypad 1
            tmpIdControll2 = 0;         //le joueur 2 est au clavier zqsd
            if (!keyboardType)
                tmpIdControll2 = 5;     //si l'utilisateur touche le clavier, c'est lui qui est pris !
        }
        else if (joypadConnected > 1)   //si 2 ou plus de joypas sont connecté
        {
            tmpIdControll1 = 1;         //le joueur 1 est au joyapd 1
            tmpIdControll2 = 2;         //le joueur 1 est au joyapd 2
        }
        bool whiteTmp = false;
        bool repulseTmp = false;
        bool attractTmp = false;
        bool blackholeTmp = false;
        bool flaireTmp = false;
        for (int i = 0; i < powerPlayer1.Count; i++)
        {
            switch (powerPlayer1[i])                                //Switch du nom du pouvoir
            {
                case 0:
                    whiteTmp = true;
                    break;
                case 1:
                    repulseTmp = true;
                    break;
                case 2:
                    attractTmp = true;
                    break;
                case 3:
                    blackholeTmp = true;
                    break;
                case 4:
                    flaireTmp = true;
                    break;
            }
        }
        Global.addPlayer(whiteTmp, repulseTmp, attractTmp, blackholeTmp, flaireTmp, 1, tmpIdControll1);                      //ajoute le joueur de la team 1 avec ses 1 ou 2 pouvoir !
        whiteTmp = repulseTmp = attractTmp = blackholeTmp = flaireTmp = false;                         //reset les variables pour le prochain joueur !
        for (int i = 0; i < powerPlayer2.Count; i++)
        {
            switch (powerPlayer2[i])                                //Switch du nom du pouvoir
            {
                case 0:
                    whiteTmp = true;
                    break;
                case 1:
                    repulseTmp = true;
                    break;
                case 2:
                    attractTmp = true;
                    break;
                case 3:
                    blackholeTmp = true;
                    break;
                case 4:
                    flaireTmp = true;
                    break;
            }
        }
        Global.addPlayer(whiteTmp, repulseTmp, attractTmp, blackholeTmp, flaireTmp, 1, tmpIdControll2);                      //ajoute le joueur de la team 1 avec ses 1 ou 2 pouvoir !
    }

    /// <summary>
    /// remplie la lsite des joueurs, relativ à la map !!
    /// </summary>
    public void fillPlayerList()
    {
        Debug.Log("ici le gros fillPlayerList");
        Global.playersData.Clear();
        if (!Global.soloAnd2Player)
            addSoloPlayer();                                                           //cree juste un joueur, relatif à la map !
        else
            add2playerSolo();
    }

    void setIfNextMapIsBlocked()
    {
        /*if (mapLocation + 1 >= MapPrefs.PP.mapsInfosSolo.Count || MapPrefs.PP.mapsInfosSolo[mapLocation + 1].blocked)
        {
            PlayerPrefs.PP.mapNextBlocked = true;
        }
        else
            PlayerPrefs.PP.mapNextBlocked = false;*/
    }

    /// <summary>
    /// test si la prochaine map est possible en format "next"
    /// -> si on peut depui sla map courrante de passer à la suivante depuis le jeu
    /// </summary>
    /// <returns></returns>
    bool testIfNextMapIsPossible()
    {
        /*//Si la prochaine map n'est pas la dernière, et qu'elle est du même monde Id, ont autorise le nextLevel
        if (mapLocation + 1 < MapPrefs.PP.mapsInfosSolo.Count && MapPrefs.PP.mapsInfosSolo[mapLocation].worldId == MapPrefs.PP.mapsInfosSolo[mapLocation + 1].worldId)
            return (true);*/
        return (false);
    }

    /// <summary>
    /// lorsque le joueur clique sur play.
    /// </summary>
    public void justPlay()
    {
        /*ChargementPicture.SetActive(true);                                          //affiche l'icon de chargement
        fillPlayerList();                                                           //remplie la liste des joueurs
        Global.maxEggs = MapPrefs.PP.mapsInfosSolo[mapLocation].maxEggs;                               //set le maxEggs à donner à la reine
        Global.timeMax = MapPrefs.PP.mapsInfosSolo[mapLocation].timeMax;                               //set le timerMax (multi), (solo à revoir)
        Global.timeRespawn = MapPrefs.PP.mapsInfosSolo[mapLocation].timeRespawn;                       //set le temps de respawn des joueurs
        Global.idMap = mapLocation + 1;                                             //set l'id de la map
        Global.lastMap = MapPrefs.PP.mapsInfosSolo[mapLocation].lastMap;                               //set si c'est la dernière maps du jeu ! (qui suit une cinématique particulière)
        setIfNextMapIsBlocked();                                                    //détermine si la map suivante est bloqué ou pas
        Global.keyboardType = keyboardType;                                         //sauvegarde si on a switch de type de keyboard

        Global.displayValue();                                                      //debug display value

        PlayerPrefs.PP.lastLevelPlayerId = mapLocation;                             //set l'id de la map qui va jouer
        PlayerPrefs.PP.lastLevelPlayed = MapPrefs.PP.mapsInfosSolo[mapLocation].nameLevel;             //set le nom de la map qui va jouer

        //si on autorise le next level, OK, sinon, nop
        if (testIfNextMapIsPossible())
            PlayerPrefs.PP.nextLevel = MapPrefs.PP.mapsInfosSolo[mapLocation + 1].nameLevel;
        else
            PlayerPrefs.PP.nextLevel = "None";

        PlayerPrefs.PP.Save();                                                      //sauvegarder les données
        QOC.jumpToScene(MapPrefs.PP.mapsInfosSolo[mapLocation].nameLevel);                             //sauter à la scènes*/
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
                mapLocation--;
            //joypad 1, ou clavier ZQSD ou clavier arrow UP
            else if (PC.getPlayer(0).GetAxis("UIHorizontal") > 0 || PC.getPlayer(5).GetAxis("UIHorizontal") > 0 || PC.getPlayer(1).GetAxis("UIHorizontal") > 0.5f
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
                if (joypadConnected >= 2)       //si il y a présente de 2+ manette
                {
                    panelSoloOrCoop[0].isOn = false;
                    panelSoloOrCoop[1].isOn = true;
                }
                TWNE.isOk = false;
            }
        }
        if (mainLocation == 1 && TWNE.isOk)                                             //on est dans le choix du mode
        {
            //soloOrMultiLocation
            //clavier zqsd, joystick ou clavier arrow
            if (PC.getPlayer(0).GetAxis("UIHorizontal") < 0 || PC.getPlayer(5).GetAxis("UIHorizontal") < 0 || PC.getPlayer(1).GetAxis("UIHorizontal") < -0.5f)
            {
                locationCoop = 0;
                panelSoloOrCoop[0].isOn = true;
                panelSoloOrCoop[1].isOn = false;
                Global.soloAnd2Player = false;
                TWNE.isOk = false;
            }
            //joypad 1, ou clavier ZQSD ou clavier arrow UP
            else if ((PC.getPlayer(0).GetAxis("UIHorizontal") > 0 || PC.getPlayer(5).GetAxis("UIHorizontal") > 0 || PC.getPlayer(1).GetAxis("UIHorizontal") > 0.5f)
                        && (powerMapAccepted.Count > 1 && MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], mapLocation).coop))
   //TODO ???
            {
                locationCoop = 1;
                panelSoloOrCoop[0].isOn = false;
                panelSoloOrCoop[1].isOn = true;
                Global.soloAnd2Player = true;
                TWNE.isOk = false;
            }
             
            //action OK du clavier ou de la mannette
            if (PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit"))
            {
                if (!Global.soloAnd2Player)             //si c'est le mode solo, ok
                    mainLocation = 2;
                else                                    //si c'est le mode coop...
                {
                    if (powerMapAccepted.Count <= 1)    //bloquer si il y a moin d'un pouvoirs dans la map
                        Debug.Log("map à 1 pouvoir, désactivé !");
                    else
                        mainLocation = 2;
                }
                TWNE.isOk = false;
            }
        }
        if (mainLocation == 2 && TWNE.isOk)             //si on est dans le mode
        {
            //droite et gauche permet de switch les pouvoirs
            if (locationCoop == 1 && powerMapAccepted.Count > 1 && (PC.getPlayer(0).GetAxis("UIHorizontal") != 0 || PC.getPlayer(5).GetAxis("UIHorizontal") != 0 || PC.getPlayer(1).GetAxis("UIHorizontal") < -0.5f) || PC.getPlayer(1).GetAxis("UIHorizontal") > 0.5f)
            {
                TWNE.isOk = false;
                switchPlayer1and2();
            }

            if (PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit") || PC.getPlayer(2).GetButtonDown("UISubmit"))
            {
                TWNE.isOk = false;
                justPlay();
            }
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
    /// active ou désactive les bon objets du menu
    /// </summary>
    /// <param name="active"></param>
    void SwitchDisplaySoloOrCoop(bool active)
    {
        toActiveSolo.SetActive(active);
        toActiveCoop.SetActive(active);
        toDesactiveSolo.SetActive(!active);
        toDesactiveCoop.SetActive(!active);
    }

    void activeSolo(bool active)
    {
        toActiveSolo.SetActive(active);
        toDesactiveSolo.SetActive(!active);
    }

    void activeCoop(bool active)
    {
        toActiveCoop.SetActive(active);
        toDesactiveCoop.SetActive(!active);
    }

    /// <summary>
    /// lorsqu'on active SOLO, active les bon objets
    /// </summary>
    void soloActiveAll()
    {
        activeSolo(true);
        activeCoop(false);
        if (joypadConnected == 0)
        {
            soloKeyboard.SetActive(true);
            soloJoypad.SetActive(false);
        }
        else
        {
            soloKeyboard.SetActive(false);
            soloJoypad.SetActive(true);
        }
    }

    /// <summary>
    /// lorsqu'on active coop, active les bon objets
    /// </summary>
    void coopActiveAll()
    {
        activeSolo(false);
        activeCoop(true);
        if (joypadConnected == 0)                       //0 joypad connecté, on fait zqsd vs arrow
        {
            coop2keyboard.SetActive(true);
            coopKeyboardJoypad.SetActive(false);
            coopJoypadJoypad.SetActive(false);
        }
        else if (joypadConnected == 1)                  //il y a 1 joypad connecté, on fait mannette 1 vs zqsd ou arrow !
        {
            coop2keyboard.SetActive(false);
            coopKeyboardJoypad.SetActive(true);
            coopJoypadJoypad.SetActive(false);
        }
        else
        {
            coop2keyboard.SetActive(false);
            coopKeyboardJoypad.SetActive(false);
            coopJoypadJoypad.SetActive(true);
        }
    }

    /// <summary>
    /// désactive tout les objet de coop/solo
    /// </summary>
    void desactiveSoloAndCoopAll()
    {
        soloKeyboard.SetActive(false);
        soloJoypad.SetActive(false);
        coop2keyboard.SetActive(false);
        coopKeyboardJoypad.SetActive(false);
        coopJoypadJoypad.SetActive(false);
    }

    /// <summary>
    /// change le focus principale (map/choix de team/play)
    /// </summary>
    private void changeMainFocus()
    {
        if (mainLocation != prevMainLocation)                       //change si il a changé
        {
            joypadConnected = howManyJoypadAreConnected();          //compte combien de joypad sont connecté
            switch (mainLocation)
            {
                case 0:                                             //si les joueurs sont dans la sélection de map
                    backgroudMap.color = colorActive;
                    backgroudTeam.color = colorInactive;
                    activeSolo(false);
                    activeCoop(false);
                    chooseTab[0].color = colorTabActive;
                    chooseTab[1].color = colorTabInactive;
                    modeBlocked.SetActive(false);

                    //if (joypadConnected >= 2)
                    //panelSoloOrCoop[0].isOn = true;                 //on et off les toggles
                    //panelSoloOrCoop[1].isOn = false;
                    break;
                case 1:
                    if (powerMapAccepted.Count <= 1 || !MapPrefs.PP.getLevels(PlayerPrefs.PP.lastLevelPlayerId[0], mapLocation).coop)                    //si la map n'autorise qu'une seul pouvoir...
                    {
                        modeBlocked.SetActive(true);                    //affiche la GUI de blocage
                        locationCoop = 0;                               //change la location à 0 (le mode solo)
                        panelSoloOrCoop[0].isOn = true;                 //on et off les toggles
                        panelSoloOrCoop[1].isOn = false;
                        Global.soloAnd2Player = false;                  //change la variable global
                    }
                    else
                    {
                        modeBlocked.SetActive(false);                   //blockage du coop desactivé
                        changeIndexPlayer();                                //change les pouvoirs des joueurs 1 et 2, + la GUI

                        if (joypadConnected >= 2)       //si il y a présente de 2+ manette
                        {
                            locationCoop = 1;
                            panelSoloOrCoop[0].isOn = false;
                            panelSoloOrCoop[1].isOn = true;
                            Global.soloAnd2Player = true;
                        }
                    }


                    desactiveSoloAndCoopAll();
                    backgroudMap.color = colorValidate;
                    backgroudTeam.color = colorActive;
                    activeSolo(false);
                    activeCoop(false);
                    chooseTab[0].color = colorTabInactive;
                    chooseTab[1].color = colorTabActive;
                    break;
                case 2:
                    chooseTab[1].color = colorTabInactive;
                    if (locationCoop == 0)
                        soloActiveAll();
                    else
                        coopActiveAll();
                    break;
            }
            prevMainLocation = mainLocation;
            TWNE.isOk = false;
        }
    }

    /// <summary>
    /// action quand on appruis sur echap ou B
    /// </summary>
    private void quitAction()
    {
        //action retour du joueur 0 ou 1 (clavier ou joystick 1)
        if (PC.getPlayer(0).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel"))
        {
            switch (mainLocation)
            {
                case 0:
                    gameObject.GetComponent<QuitOnClick>().jumpToScene("1_MainMenu");   //on revient au menu précédent
                    break;
                case 1:
                    mainLocation = 0;
                    break;
                case 2:
                    mainLocation = 1;
                    break;
            }
        }
    }

    /// <summary>
    /// main update
    /// </summary>
    private void Update()
    {
        if (TWNE.isOk && !goToNextLevel)                              //limite les inputs pour pas aller trop vite && si ce n'est pas le mode nextLevel
        {
            playerInput();                          //get player input
            changeMapFocus();                       //change le focus de la map si elle a changé
            changeMainFocus();                      //change le focus principale (map/choix de team/play)
            changeKeyboard();
            quitAction();
        }
    }
}
