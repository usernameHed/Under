using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialiseGame : MonoBehaviour
{
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    public GlobalVariableManager GBM;                                  //script global pour les data des joueurs


    [Header("solo")]
    [SerializeField]    private GameObject[] parentPlayerSolo;          //parent du joueur 1 et du joueur 2 du mode solo
    /// <summary>
    /// private reference
    /// </summary>
    [Space(10)]
    [Header("private serealized")]
    [SerializeField]    private RectTransform toFade;                      //GUI principale
    
    [SerializeField]    private GameObject[] QueensPrefabs;
    [SerializeField]    private GameObject[] playersPrefabs;
    [SerializeField]    private GameObject trailPlayer;
    [SerializeField]    private GameObject groupTrail;                  //group de trail
    [SerializeField]    private GameObject progressBarGreenPrefabs;     //prefabs des progressBar des verte
    [SerializeField]    private GameObject groupProgressBar;            //objet group des progressBar des verte

    [Space(10)]
    [SerializeField]    private GameObject BigPanelSolo;                //le panel principale du solo
    [SerializeField]    private GameObject BigPanelMulti;               //le panel principale du multi
    [SerializeField]    private GameObject groupQueen;                  //le groupe des queen
    private GameObject SpawnerQueen;                //group spawner des queen
    [SerializeField]    private GameObject groupPlayer;                 //le groupe des players
        public GameObject SpawnerPlayers;              //group spawner des players

    [SerializeField]    private GameObject avatarPlayerCanvas;
    [SerializeField]    private GameObject avatarPlayerRespawn;
    [SerializeField]    private GameObject PanelTeamSoloCanvas;               //panel comportant les 1-4 joueurs pour la partie solo
    [SerializeField]    private GameObject PanelTeam1Canvas;                  //panel comportant les 1-4 joueurs de la team 1
    [SerializeField]    private GameObject PanelTeam2Canvas;                  //panel comportant les 1-4 joueurs de la team 2
    public List<GameObject> PanelTeamRespawn;                  //panel comportant les 1-4 joueurs de la team 2


    [SerializeField]    private Sprite[] OPteam;                        //objective pointer On Screen pour le splayer du mode multi
    [SerializeField]    private Sprite[] OPteamCoop;                        //objective pointer On Screen pour le splayer du mode solo coop
    public Sprite[] picPower;                      //images des 4 pouvoirs (power - 1)
    public Sprite[] picPlayerDead;                 //images des 4 fourmis morte (Dead - 1)
    [HideInInspector] public GameObject levelDataObject;                       //get les info de la map
    [HideInInspector] public LevelData levelDataScript;                       //get les info de la map
    private GameObject groupTuto;                                       //objet groupTuto
    [SerializeField] private GameObject groupPointCam;                  //groupe des points de caméra des joueurs

    /// <summary>
    /// private
    /// </summary>
    private float timeToGo;
    private GameManager GM;                                             //game manager
    private GameObject VariableGlobal;                                  //variable global
    
    private GameObject cam;                                             //objet caméra
    private CameraController CamControl;                                //script cameraController sur l'objet cam
    private SwitchAnts SA;
    private LevelManagerGame LMG;

    private GameObject instanceNewAvatarCanvasTmp;
    private GameObject instanceNewAvatarRespawnTmp;

    private void Awake()
    {
        GM = gameObject.GetComponent<GameManager>();
        GM.allPlayerDead = false;
        VariableGlobal = GameObject.FindGameObjectWithTag("Global");                //récupère la référence de l'objet global
        if (VariableGlobal)
            GBM = VariableGlobal.GetComponent<GlobalVariableManager>();                 //récupère le script du global
        cam = Camera.main.gameObject;                                               //récupère la caméra
        CamControl = cam.GetComponent<CameraController>();                          //récupère la référence du script
        SA = gameObject.GetComponent<SwitchAnts>();
        LMG = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManagerGame>();
        levelDataObject = GameObject.FindGameObjectWithTag("LevelData");
        levelDataScript = levelDataObject.GetComponent<LevelData>();

        //init music pour pouvoir changer de state in-game
        //SoundManager.getSingularity().musicEmitterScript = levelDataObject.GetComponent<FmodEventEmitter>();
        //if (!SoundManager.getSingularity().musicEmitterScript)
            //Debug.LogError("nop");


        SpawnerQueen = levelDataScript.SpawnQueen;
        SpawnerPlayers = levelDataScript.SpawnPlayer;
        groupTuto = levelDataScript.groupTuto;                                      //récupère le groupTuto
    }

    // Use this for initialization
    void Start ()
    {
        timeToGo = Time.fixedTime + timeOpti;
        SetAvatarPlayer();
        getGlobalVariable();
        makeChangeProgressBarMulti();                                               //change le type de progress bar en mode solo ou multi.
        setUpPanelSoloMulti();                                                      //set up les panels du mode solo ou du mode multi

        if (levelDataScript.specialTuto > 0)                                        //si la caméra au début doit être sur quelque chose...
            setUpSpecialTuto();
      
        CamControl.SetStartPositionAndSize();                                       //set la position de la caméra au début du jeu
        SetChronoInfo();
        SetUpGroupTuto();                                                           //setup tout les tuto pours qu'ils aient les bonnes images selon les contrôle du joueur
    }

    /// <summary>
    /// set les avatar de splayer
    /// </summary>
    void SetAvatarPlayer()
    {
        PanelTeamRespawn.Clear();
        PanelTeamRespawn.Add(levelDataScript.respawn[0]);
        PanelTeamRespawn.Add(levelDataScript.respawn[1]);
        PanelTeamRespawn.Add(levelDataScript.respawn[2]);

        GM.TEL.PanelPlayer = PanelTeamRespawn;
    }

    void setUpSpecialTuto()
    {
        int typeOfTuto = levelDataScript.specialTuto;
        if (typeOfTuto == 1)
            CamControl.TWNE.timeWithNoEffect = 6;
        CamControl.TWNE.isOk = false;           //d'abord set la camera a false pour focus la queen
        for (int i = 0; i < GM.TargetsPlayers.Count; i++)
        {
            GM.TargetsPlayers[i].GetComponent<PlayerController>().activeControl = false;
        }
    }

    /// <summary>
    /// setup tout les tuto pours qu'ils aient les bonnes images selon les contrôle du joueur
    /// </summary>
    void SetUpGroupTuto()
    {
        for (int i = 0; i < groupTuto.transform.childCount; i++)
        {
            Transform child = groupTuto.transform.GetChild(i);

            if (!child || !child.gameObject || !child.gameObject.GetComponent<textTuto>() || !child.gameObject.activeSelf)
                continue;
            textTuto TT = groupTuto.transform.GetChild(i).gameObject.GetComponent<textTuto>();
            if (TT.controller)
            {
                if (GM.TargetsPlayers.Count == 0)
                    continue;
                PlayerController Ptmp = GM.TargetsPlayers[0].GetComponent<PlayerController>();
                if (!Ptmp)
                    return;

                int type = Ptmp.nb_player;
                Debug.Log("ici à: " + type);
                if (type == 0)  //zqsd
                    TT.TypeTuto = 0;
                else if (type == 5) //arrow
                    TT.TypeTuto = 1;
                else                //joystick
                    TT.TypeTuto = 2;
            }
        }
    }

    /// <summary>
    /// set les info du chrono
    /// </summary>
    void SetChronoInfo()
    {
        if (!GM.multi)
        {
            GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().setMax(GBM.maxEggs);
            GM.Chrono.GetComponents<ColoredProgressBar>()[0].setMax(GBM.timeMax);
            toFade.anchorMin = new Vector2(0f, 1f);
            toFade.anchorMax = new Vector2(0f, 1f);
            toFade.pivot = new Vector2(0f, 1f);
        }
        else
        {
            GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().setMax(GBM.maxEggs);
            GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().setMax(GBM.maxEggs);
            GM.Chrono.GetComponents<ColoredProgressBar>()[1].setMax(GBM.timeMax);

            toFade.anchorMin = new Vector2(0.5f, 1f);
            toFade.anchorMax = new Vector2(0.5f, 1f);
            toFade.pivot = new Vector2(0.5f, 1f);
        }

    }

    /// <summary>
    /// cree la ou les queens aux bonnes position
    /// </summary>
    void createQueen()
    {
        if (!GM.multi)             //si on est en mode solo
        {
            GameObject instanceNewQueen = Instantiate(QueensPrefabs[0],
                                                    SpawnerQueen.transform.GetChild(0).transform.position,
                                                    SpawnerQueen.transform.GetChild(0).transform.rotation,
                                                    groupQueen.transform
                                                    ) as GameObject;
            //GameObject instanceNewQueen = Instantiate(Resources.Load("Players/Queen", typeof(GameObject))) as GameObject;      //créé une queen

            CamControl.alternativeFocus = SpawnerPlayers.transform.GetChild(0).transform;                                                           //change le focus alternatif du mode solo en le checkpoints !
            CamControl.secondAlternativeFocus = instanceNewQueen.transform;                                                           //change le focus alternatif du mode solo en le checkpoints !
            GM.TEL.initQueenLinks(instanceNewQueen);
        }
        else                                    //si on est en mode  multi
        {
            GameObject instanceNewQueen = Instantiate(QueensPrefabs[1],
                                                        SpawnerQueen.transform.GetChild(1).transform.position,
                                                        SpawnerQueen.transform.GetChild(1).transform.rotation,
                                                        groupQueen.transform
                                                        ) as GameObject;      //créé une queen
            GM.TEL.QueenMulti.Add(instanceNewQueen);

            GameObject instanceNewQueen2 = Instantiate(QueensPrefabs[2],
                                                        SpawnerQueen.transform.GetChild(2).transform.position,
                                                        SpawnerQueen.transform.GetChild(2).transform.rotation,
                                                        groupQueen.transform
                                                        ) as GameObject;      //créé une queen 2
            GM.TEL.QueenMulti.Add(instanceNewQueen2);
        }
        //Destroy(SpawnerQueen);
    }

    /// <summary>
    /// cree le trail des players
    /// </summary>
    /// <param name="player"></param>
    void createTrailOfPlayer(GameObject player)
    {
        GameObject newTrailPlayer = Instantiate(trailPlayer, groupTrail.transform) as GameObject;   //créé un trail pour le player !
        newTrailPlayer.GetComponent<TrailPlayerId>().refPlayer = player;     //cree une reference du trail du joueur

        TrailController TC = player.transform.GetChild(0).gameObject.GetComponent<TrailController>();    //get le trailController du joueur !
        TC.trailPlayer = newTrailPlayer;

        GameObject progressBarGreen = Instantiate(progressBarGreenPrefabs, groupProgressBar.transform) as GameObject;   //créé un trail pour le player !
        progressBarGreen.GetComponent<TrailPlayerId>().refPlayer = player;     //cree une reference du trail du joueur

        progressBarGreen.GetComponent<GuiFollow>().WorldObject = player;            //set le player à follow pour la GUI


        TC.progressBarGreen = progressBarGreen;
        TC.CPB = progressBarGreen.GetComponent<ColoredProgressBar>();
    }

    /// <summary>
    /// cree un player solo
    /// </summary>
    /// <param name="type">type d epouvoir, entre 0 et 4 (0 = white, 4 = flaire)</param>
    /// <param name="indexPlayer"> entre 0 et 1</param>
    /// <param name="instanceNewAvatar">l'avatar du joueur</param>
    /// <param name="countPower">0 ou plus, si c'est 0, c'est le premier de la liste du joueur 1 ou 2 !</param>
    void SoloCreatePlayer(int type, int indexPlayer, ref int countPower)
    {
        GameObject instanceNewPlayer = Instantiate(playersPrefabs[type],
            SpawnerPlayers.transform.GetChild(0).transform.GetChild(indexPlayer).transform.position,
            Quaternion.identity,
            groupPlayer.transform.GetChild(indexPlayer).transform
            ) as GameObject;   //créé un player !
        PlayerController PC = instanceNewPlayer.GetComponent<PlayerController>();                                   //récupère la référence de son PlayerCOntroller
        //instanceNewPlayer.transform.parent = groupPlayer.transform.GetChild(indexPlayer).transform;                 //changer son parent dans le groupPlayer 1 ou 2

        ObjectiveIndicator OP = instanceNewPlayer.GetComponent<ObjectiveIndicator>();                               //get l'OP du player
        if (OP)
        {
            OP.hideOnScreen = true;                                                                                    //permet de visualiser on screen le pointer
            if (GBM.soloAnd2Player)
            {
                Debug.Log("en mode coop, afficher les OP");
                OP.hideOnScreen = false;
                OP.onScreenSprite = OPteamCoop[indexPlayer];                                        //change le sprite de sa team
                OP.isMulti = true;
            }
        }

        //si c'est la fourmis verte, créé les objet nécéssaire pour les trails
        if (type == 4)
        {
            createTrailOfPlayer(instanceNewPlayer);
        }
        else if (type == 3) //si c'est une fourmis jaune, init les portals color
        {
            PC.FP.initColorPortal(false, indexPlayer, -1);
        }
        //active le pouvoir du joueur dans le spawn
        if (type != 0)
        {
            //ici garder en 2 le LOOK
            SpawnerPlayers.transform.GetChild(0).transform.GetChild(2).transform.GetChild(type).transform.GetChild(0).gameObject.SetActive(true);
            SpawnerPlayers.transform.GetChild(0).transform.GetChild(2).transform.GetChild(type).transform.GetChild(1).gameObject.SetActive(false);
        }



        PC.groupPointCam = groupPointCam;   //set le référence du group pointCam pour la caméra...
        PC.nb_team = GBM.playersData[indexPlayer].team;           //change la team selon les data
        PC.nb_player = GBM.playersData[indexPlayer].idPlayer;     //change l'id selon les data (détermine les controles du joueur)
        LMG.idPlayerInGame.Add(PC.nb_player);                       //ajoute l'id dans la liste (liste des contrôles de jeu)

        addAvatarPlayer(instanceNewPlayer, false, PC, type, false);


        PC.indexSolo = indexPlayer;

        //instanceNewPlayer.transform.position = SpawnerPlayers.transform.GetChild(0).transform.GetChild(indexPlayer).transform.position;    //changer sa position au spawn 0 (repulsor)
        //PC.lightPlayer.color = Color.white;

        //l'ajoute à la liste de switchAnts
        if (indexPlayer == 0)
        {
            SA.listPower1.Add(type);
            SA.TargetsPlayerSolo1.Add(instanceNewPlayer);
        }
        else
        {
            SA.listPower2.Add(type);
            SA.TargetsPlayerSolo2.Add(instanceNewPlayer);
        }
        //si c'est le premier player de la liste, l'activer.
        if (countPower == 0)
        {
            SA.PanelPower[indexPlayer].GetComponent<GuiFollow>().WorldObject = instanceNewPlayer;        //change l'objet à suivre de la GUI au joueur
            PC.setRefToOther();                         //set ref à la caméra et au gameManager
            PC.changeSpriteAvatar(picPower[type], picPlayerDead[type]);
            //set le player à la heatmap
            //EventAnalytics.EA.player = instanceNewPlayer;
        }
        else
            instanceNewPlayer.SetActive(false);
        countPower++;
    }

    /// <summary>
    /// create avatar for player (solo or multi)
    /// </summary>
    void createAvatar(int multi)
    {
        instanceNewAvatarCanvasTmp = null;
        instanceNewAvatarRespawnTmp = null;
        Transform posCanvas = PanelTeamSoloCanvas.transform;
        Transform posRespawn = PanelTeamRespawn[0].transform;

        if (multi > 0)  //0 = solo, 1 = team 1, 2 = team 2
        {
            if (multi == 1)                                                                                           //si le joueur est de la team 1
                posCanvas = PanelTeam1Canvas.transform;                                                              //move to panelTeam1
            else if (multi == 2)                                                                                      //si le joueur est de la team 2
                posCanvas = PanelTeam2Canvas.transform;                                                              //move to panelTeam1
            posRespawn = PanelTeamRespawn[multi].transform;
        }

        //créé l'avatar du joueur CANVAS
        GameObject instanceNewAvatarCanvas = Instantiate(avatarPlayerCanvas, transform.position, transform.rotation) as GameObject;  //créé l'avatar du joueur    
        instanceNewAvatarCanvas.transform.SetParent(posCanvas.transform);
        instanceNewAvatarCanvasTmp = instanceNewAvatarCanvas;

        //créé l'avatar du joueur RESPAWN
        GameObject instanceNewAvatarRespawn = Instantiate(avatarPlayerRespawn, transform.position, transform.rotation) as GameObject;  //créé l'avatar du joueur    
        instanceNewAvatarRespawn.transform.SetParent(posRespawn);
        instanceNewAvatarRespawn.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<ColoredProgressBar>().max = GBM.timeRespawn;  //set le temps de respawn
        instanceNewAvatarRespawnTmp = instanceNewAvatarRespawn;
    }

    /// <summary>
    /// créé un avatar des players pour le Canvas ET le respawn
    /// </summary>
    void addAvatarPlayer(GameObject instanceNewPlayer, bool changeSprite, PlayerController PC, int type, bool multi)
    {
        PC.setAvatar(instanceNewAvatarCanvasTmp, instanceNewAvatarRespawnTmp);                //set l'avatar du joueur
        if (!multi)
        {
            instanceNewAvatarCanvasTmp.GetComponent<TrailPlayerId>().refPlayerSolo.Add(instanceNewPlayer);   //ajoute à la liste des joeurs solo (avec X pouvoirs)
            instanceNewAvatarRespawnTmp.GetComponent<TrailPlayerId>().refPlayerSolo.Add(instanceNewPlayer);   //ajoute à la liste des joeurs solo (avec X pouvoirs)
        }
        else
            instanceNewAvatarRespawnTmp.GetComponent<TrailPlayerId>().refPlayer = instanceNewPlayer;   //ajoute à la liste des joeurs solo (avec X pouvoirs)

        if (changeSprite)
        {
            PC.changeSpriteAvatar(picPower[type], picPlayerDead[type]);
        }
    }

    /// <summary>
    /// cree le player solo / avec son amis aux positions des spawns,
    /// avec les bon switch des pouvoirs qui leurs sont autorisé
    /// </summary>
    void createSoloPlayers()
    {
        Debug.Log("ici cree les players du mode solo");
        for (int i = 0; i < GBM.playersData.Count; i++) //parcourt les 1 ou 2 joueurs
        {
            parentPlayerSolo[i].SetActive(true);                                                //active le parent du joueur solo 1 ou 2
            SA.PanelPower[i].SetActive(true);                                                    //active le panel des pouvoirs du joueurs
            //PanelPower[i].GetComponent<GuiFollow>().WorldObject = parentPlayerSolo[i];        //change l'objet à suivre de la GUI au joueur

            createAvatar(0);

            int countPower = 0;                                                                 //permet de connaitre la fourmis à activer
            //pour 1 player, créé autant de fourmis qu'il a de pouvoirs
            if (GBM.playersData[i].white)
                SoloCreatePlayer(0, i, ref countPower);
            if (GBM.playersData[i].repulse)
                SoloCreatePlayer(1, i, ref countPower);              
            if (GBM.playersData[i].attract)
                SoloCreatePlayer(2, i, ref countPower);
            if (GBM.playersData[i].blackhole)
                SoloCreatePlayer(3, i, ref countPower);
            if (GBM.playersData[i].flaire)
                SoloCreatePlayer(4, i, ref countPower);
            
        }
        //Destroy(SpawnerPlayers);
    }

    /// <summary>
    /// cree les 2/4 players du mode multi aux positions des spawns
    /// correspondant
    /// </summary>
    void createMultiPlayers()
    {
        Debug.Log("ici cree les players du mode multi");
        int countTeam1 = 0;
        int countTeam2 = 0;
        for (int i = 0; i < GBM.playersData.Count; i++)
        {
            //GameObject instanceNewPlayer;                                           //cree un nouveau player
            //string namePlayer = "Players/Player1 - repulse";                        //nom par défaut
            int type = 0;                                                      //position initiale dans le tableau de sprite

            //change la position et le nom par rapport à son pouvoirs
            if (GBM.playersData[i].white)
                type = 0;
            if (GBM.playersData[i].repulse)
                type = 1;
            else if (GBM.playersData[i].attract)
                type = 2;
            else if (GBM.playersData[i].blackhole)
                type = 3;
            else if (GBM.playersData[i].flaire)
                type = 4;

            Vector3 positionMulti = new Vector3();
            if (GBM.playersData[i].team == 1)
                positionMulti = SpawnerPlayers.transform.GetChild(GBM.playersData[i].team).transform.GetChild(countTeam1).transform.position;    //changer sa position au spawn 0 (repulsor)
            else if (GBM.playersData[i].team == 2)
                positionMulti = SpawnerPlayers.transform.GetChild(GBM.playersData[i].team).transform.GetChild(countTeam2).transform.position;    //changer sa position au spawn 0 (repulsor)

            GameObject instanceNewPlayer = Instantiate(playersPrefabs[type], positionMulti, Quaternion.identity, groupPlayer.transform) as GameObject;   //créé un player !

            //instanceNewPlayer = Instantiate(Resources.Load(namePlayer, typeof(GameObject))) as GameObject;  //créé un player !
            PlayerController PC = instanceNewPlayer.GetComponent<PlayerController>();


            ObjectiveIndicator OP = instanceNewPlayer.GetComponent<ObjectiveIndicator>();                   //get l'OP du player
            if (OP)
            {
                OP.hideOnScreen = false;                                                                        //permet de visualiser on screen le pointer
                OP.onScreenSprite = OPteam[GBM.playersData[i].team - 1];                                        //change le sprite de sa team
                OP.isMulti = true;                                                                              //dis au OP qu'on est en mutli (pour add up)
            }

            if (type == 4)     //si c'est la 4eme joueur
            {
                createTrailOfPlayer(instanceNewPlayer);
            }

            PC.groupPointCam = groupPointCam;   //set le référence du group pointCam pour la caméra...
            PC.nb_team = GBM.playersData[i].team;           //change la team selon les data

            if (type == 3) //si c'est une fourmis jaune, init les portals color
            {
                PC.FP.initColorPortal(true, -1, PC.nb_team);
            }

            PC.nb_player = GBM.playersData[i].idPlayer;     //change l'id selon les data (détermine les controles du joueur)
            LMG.idPlayerInGame.Add(PC.nb_player);           //ajoute l'id dans la liste (liste des contrôles de jeu)

            createAvatar(GBM.playersData[i].team);          //cree avatar pour le player
            addAvatarPlayer(instanceNewPlayer, true, PC, type, true);    //setup l'avatar

            if (PC.nb_team == 1)
            {
                //instanceNewPlayer.transform.position = SpawnerPlayers.transform.GetChild(GBM.playersData[i].team).transform.GetChild(countTeam1).transform.position;    //changer sa position au spawn 0 (repulsor)
                PC.lightPlayer.color = Color.blue;
            }
            else if (PC.nb_team == 2)
            {
                //instanceNewPlayer.transform.position = SpawnerPlayers.transform.GetChild(GBM.playersData[i].team).transform.GetChild(countTeam2).transform.position;    //changer sa position au spawn 0 (repulsor)
                PC.lightPlayer.color = Color.red;
            }

            PC.setRefToOther();                         //set ref à la caméra et au gameManager
            //compte le nombre de player en team 1 et 2
            if (GBM.playersData[i].team == 1)
                countTeam1++;
            else
                countTeam2++;
        }
        //Destroy(SpawnerPlayers);
    }

    /// <summary>
    /// génère les wallcolliders des joueurs
    /// </summary>
    void setColliderWallsOfPlayer()
    {
        for (int i = 0; i < GM.TargetsPlayers.Count; i++)
        {
            GM.TargetsPlayers[i].GetComponent<PlayerController>().setColliderWallsOther();
        }
    }

    /// <summary>
    /// récupère les variable globales en tout début de scène,
    /// et créé tout ce qu'il faut
    /// </summary>
    void getGlobalVariable()
    {
        GM.multi = GBM.multi;
        GM.soloAnd2Player = GBM.soloAnd2Player;

        createQueen();
        if (GM.multi)
        {
            createMultiPlayers();
            SpawnerPlayers.transform.GetChild(0).gameObject.SetActive(false);
            //Chrono.GetComponent<RectTransform>().localPosition.
        }
        else
        {
            createSoloPlayers();
            SpawnerPlayers.transform.GetChild(1).gameObject.SetActive(false);
            SpawnerPlayers.transform.GetChild(2).gameObject.SetActive(false);
        }
        setColliderWallsOfPlayer();
    }

    /// <summary>
    /// Applique le changement de progressbar
    /// si le jeu est en solo, ou en multi
    /// </summary>
    void makeChangeProgressBarMulti()
    {
        ColoredProgressBar[] CPB = GM.Chrono.GetComponents<ColoredProgressBar>();  //récupère les 2 scripts CPB attaché à l'objet Chrono...
        if (!GM.multi)                                                             //si on est en mode solo, 
        {
            CPB[0].enabled = true;                                              //active le premier script du chrono et désactive le second...
            CPB[1].enabled = false;
        }
        else                                                                    //si on est en mode multi, desactive le premier et active le second...
        {
            CPB[0].enabled = false;
            CPB[1].enabled = true;
        }
    }

    /// <summary>
    /// set up les variables et l'affichage du panel suivant si on est en mode solo, ou multi !
    /// </summary>
    void setUpPanelSoloMulti()
    {
        if (!GM.multi)                                                                                 //si on est en mode solo
        {
            BigPanelSolo.SetActive(true);
            BigPanelMulti.SetActive(false);
        }
        else                                                                                        //on est en mode multi
        {
            BigPanelSolo.SetActive(false);
            BigPanelMulti.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
