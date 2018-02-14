using System.Collections.Generic;
using UnityEngine;

/*
edit:

see:

*/

public class GameManager : MonoBehaviour
{
    [Header("Main")]
    public bool multi = false;                                  //défini si le jeu courrant est du type solo, ou joueur contre joueur
    public bool soloAnd2Player = false;                         //défini s'il y a 2 joueurs pour le mode solo

    [Header("solo")]
    public GameObject ScoreProgressBarSolo;                     //la progress bar pour le mode solo

    [Header("multi")]
    public GameObject ScorePorgressBarTeam1;                    //la progress bar pour la team 1
    public GameObject ScorePorgressBarTeam2;                    //la progress bar pour la team 2

    [Space(10)]
    [Header("Different End Bool")]                              //les différentes fin de jeu possible
    public bool allPlayerDead = false;                          //tout les players sont mort (marche en solo comme en multi)
    public bool timerEnd = false;                               //le temps est fini (marche en multi uniquement)
    public bool queenIsFull = false;                            //variable qui défini si la reine à suffisament d'oeuf pour finir le jeu
    public bool everyEggsAreInfect = false;                     //tout les oeufs ont été comptaminé
    public bool endOfTheWorld = false;                          //c'est la fin du jeu (est défini en solo et multi selon différent paramètre)
    public int winnerTeamMulti = -1;                            //détermine quel team a gagné
    public bool waitForGoldenBall = false;                      //attend le ballon d'or...
    public bool winnedByGoldenBall = false;                     //détermine si on gagne par ballon d'or...

    [Space(10)]
    public GameObject groupPlayer;                              //le groupe des players
    public GameObject groupPoolPlayer;                          //le groupe de la pool des players (pour le mode solo !)
    public GameObject Chrono;                                   //objet chrono
    [HideInInspector] public Timer timerChorno;                 //red du script timer sur le cchrono
    public GameObject newCheckpointFind;                        //affichage "nouveau checkpoint découvert"

    [Space(10)]
    [Header("Debug")]
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    [Range(0, 10f)]    public float timeBeforGoToQueen = 1.5f;
    [Range(0, 10f)]    public float timeBeforGoToQueenEnd = 3f;
    
    public List<GameObject> TargetsPlayers = new List<GameObject>();
    public List<GameObject> TargetsPlayersToRespawn = new List<GameObject>();


    /// <summary>
    /// variable privée
    /// </summary>
    private float timeToGo;
    [HideInInspector] public SwitchAnts SA;
    public GameObject groupEggs;
    [HideInInspector] public GameObject levelManager;
    [HideInInspector] public LevelManagerGame LMG;
    [HideInInspector] public TestEndofLevel TEL;
    [HideInInspector] public InitialiseGame IG;

    private GameObject mainCamera;                                          //reference de la mainCamera
    private CameraController cameraController;                              //ref du script cameraController

    /// <summary>
    /// initialise
    /// </summary>
    private void Awake ()
    {
        timerChorno = Chrono.GetComponent<Timer>();
        groupEggs = GameObject.FindGameObjectWithTag("GroupEggs");                  //récupère le groups des oeufs
        levelManager = GameObject.FindGameObjectWithTag("LevelManager");
        LMG = levelManager.GetComponent<LevelManagerGame>();
        SA = gameObject.GetComponent<SwitchAnts>();
        TEL = gameObject.GetComponent<TestEndofLevel>();
        IG = gameObject.GetComponent<InitialiseGame>();
        IG.levelDataScript.resetGM();
        if (Camera.main)                                                        //s'il n'y a pas de main camera dans la scène, on a un gros problème !
        {
            mainCamera = Camera.main.gameObject;
            cameraController = mainCamera.GetComponent<CameraController>();
        }
    }

    /// <summary>
    /// En début de jeu, défini le nombre d'oeuf total, et restant
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }

    /// <summary>
    /// test si un player de l'id ID est en vie
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool isPlayerWithIdAlive(int id)
    {
        for (int i = 0; i < TargetsPlayers.Count; i++)
        {
            if (TargetsPlayers[i].GetComponent<PlayerController>().nb_player == id)
                return (true);
        }
        return (false);
    }

    /// <summary>
    /// supprime un joueur d'un certain id de contrôle
    /// </summary>
    /// <param name="id"></param>
    public void deletePlayerWithId(int id)
    {
        for (int i = 0; i < TargetsPlayers.Count; i++)
        {
            if (TargetsPlayers[i].GetComponent<PlayerController>().nb_player == id)
                TargetsPlayers[i].GetComponent<PlayerController>().destroyThis();
        }
    }

    /// <summary>
    /// ici on a un joueur qui doit être supprimer, et une ZoenEffector
    /// Si cette zoneEffector n'est dans aucun player en vie, renvoyer faux, sinon, frai
    /// </summary>
    /// <param name="playerToDelete"></param>
    /// <param name="zone"></param>
    /// <returns></returns>
    public bool otherPlayerInZone(GameObject playerToDelete, ZoneEffector zone)
    {
        //parcourir tout les player (ants), SI ants != playerToDelete & le zone se trouve dans le ants = true, else faux
        for (int i = 0; i < TargetsPlayers.Count; i++)
        {
            if (playerToDelete != TargetsPlayers[i] && TargetsPlayers[i].GetComponent<PlayerController>().isEffectorOnMyList(zone))
                return (true);
        }
        //ici aucun autre player n'est dans la zone ! on peut la désactiver
        return (false);
    }

    /// <summary>
    /// est appelé par le joueur lorsqu'il est activé (pour gerer le solo et le multi ensemble).
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(GameObject player, bool delete = false)
    {
        //Debug.Log("ici: " + player.name);
        if (TargetsPlayers.IndexOf(player) < 0 && !delete) //si l'objet n'est pas déjà dans l'array !
            TargetsPlayers.Add(player);
        else if (delete)
            TargetsPlayers.Remove(player);
    }

    /// <summary>
    /// parcourt la liste des players active et set leurs colliderWalls
    /// </summary>
    public void setColliderWallsOfTargetsPlayer()
    {
        for (int i = 0; i < TargetsPlayers.Count; i++)
        {
            TargetsPlayers[i].GetComponent<PlayerController>().setColliderWallsOther();
        }
    }

    /// <summary>
    /// Actualise les target joueur du tableau, car si un joueur est mort, il faut le supprimer du tableau !
    /// Si tout les joueurs sont mort, set allPlayerDead to true;
    /// </summary>
    private void ActualizeTarget()
    {
        for (int j = 0; j < TargetsPlayers.Count; j++)
        {
            // If the target isn't active, delete it !
            if (!TargetsPlayers[j] || !TargetsPlayers[j].gameObject.activeSelf)
                TargetsPlayers.Remove(TargetsPlayers[j]);
            else if (cameraController.TWNE.isOk && !TargetsPlayers[j].GetComponent<PlayerController>().activeControl)
                TargetsPlayers[j].GetComponent<PlayerController>().activeControl = true;
        }
    }

    /// <summary>
    /// return le nombre de joueur dans le jeu actif
    /// </summary>
    /// <returns></returns>
    int howMannyPlayer()
    {
        int count = 0;
        for (int i = 0; i < TargetsPlayers.Count; i++)
        {
            if (TargetsPlayers[i].activeSelf)
                count++;
        }
        Debug.Log("player restant:" + count);
        return (count);
    }

    /// <summary>
    /// respawn le player ID (1 ou 2 si solo, 1 à 4 si multi);
    /// </summary>
    /// <param name="id"></param>
    public void respawnPlayer(GameObject refPlayer)
    {
        PlayerController PCtmp = refPlayer.GetComponent<PlayerController>();
        Debug.Log("respawn player: " + PCtmp.nb_player);
        for (int i = 0; i < TargetsPlayersToRespawn.Count; i++)
        {
            if (TargetsPlayersToRespawn[i].GetInstanceID() == refPlayer.GetInstanceID())    //pour le solo quand il y a 2 fourmis dans 1 seul joueur, bug !
            {
                TargetsPlayersToRespawn[i].SetActive(true);
                /*if (!multi)     //si on est en solo
                {
                    //TODO: réafficher le panel switch (le réactiver !)

                    //set la position du spawnPlayerSolo
                    TargetsPlayersToRespawn[i].transform.position = IG.SpawnerPlayers.transform.GetChild(0).transform.GetChild(0).transform.position;
                }
                else
                {*/
                    TargetsPlayersToRespawn[i].transform.position = IG.SpawnerPlayers.transform.GetChild(PCtmp.nb_team).transform.GetChild(0).transform.position;
                /*}*/
                TargetsPlayers.Add(TargetsPlayersToRespawn[i]);                
                TargetsPlayersToRespawn.Remove(refPlayer);
            }
        }
    }
    //même fonction mais qui prend une liste de player (pour le solo)
    public void respawnPlayer(List<GameObject> refPlayers)
    {
        for (int j = 0; j < refPlayers.Count; j++)
        {
            PlayerController PCtmp = refPlayers[j].GetComponent<PlayerController>();
            Debug.Log("respawn player: " + PCtmp.nb_player);
            if (endOfTheWorld)
                return;
            for (int i = 0; i < TargetsPlayersToRespawn.Count; i++)
            {
                if (TargetsPlayersToRespawn[i].GetInstanceID() == refPlayers[j].GetInstanceID())    //si le joueur à respawn estt rouvé dans la liste à respawn !
                {
                    TargetsPlayersToRespawn[i].transform.position = IG.SpawnerPlayers.transform.GetChild(0).transform.GetChild(0).transform.position;
                    TargetsPlayersToRespawn[i].SetActive(true);
                    //set la position du spawnPlayerSolo
                    

                    TargetsPlayers.Add(TargetsPlayersToRespawn[i]);
                    TargetsPlayersToRespawn.Remove(refPlayers[j]);
                    SA.desactiveAllSoloPanel(true, PCtmp.indexSolo);

                    PCtmp.stopGazRespawn();
                }
                //else
                  //  refPlayers[j].SetActive(false);
            }
        }
        
    }

    /// <summary>
    /// ajoute un joueur dans la liste de respawn en toute sécurité sans doublons !
    /// </summary>
    /// <param name="player"></param>
    private void addToTargetRespawn(GameObject player)
    {
        if (!TargetsPlayersToRespawn.Contains(player))
            TargetsPlayersToRespawn.Add(player);
    }

    /// <summary>
    /// appelé quand le joueur est mort, ici gestion solo, et multi
    /// met au pause le timer quand c'est fini (ce temps va devenir le temps record)
    /// </summary>
    /// <param name="player">objet player</param>
    public void playerDeath(GameObject player)
    {
        PlayerController PC = player.GetComponent<PlayerController>();       
        if (PC)
        {
            subRefToOther(PC.getTypePowerPlayer());  //va chercher chaque métaux pour enlever leurs propriété relatif à player..........
        }
        if (!multi)                                                     //si on est en mode solo, y'a plus de jeu !
        {
            SA.desactiveAllSoloPanel(false, PC.indexSolo);
            TEL.testIfAllDead();
            //if (!allPlayerDead)
            //{
                //PC.avatarPlayer.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                addToTargetRespawn(player);
            //}

        }
        else
        {

                //le jeu continue, activer le respawn pour le joueur mort
                //PC.avatarPlayer.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                addToTargetRespawn(player);
        }
    }

    /// <summary>
    /// retourne le nombre de joueur actif dans le groupe de player
    /// Si checkTeam est à true, retourne le plus petit nombre de joueurs dans une des deux team
    /// (renvoi 0 si une team est vide)
    /// </summary>
    /// <returns>nombre de player actif</returns>
    public int numberPlayerLeft(bool checkTeam, bool returnTheTeam = false)
    {
        int team1 = 0;                                                              //nombre de joueur en team1
        int team2 = 0;                                                              //nombre de joueur en team2
        int number = 0;                                                             //nombre de joueur total
        //parcourt la liste des joueurs, sans prendre en compte ceux inactif
        //ou ceux en train de mourrir (ceux encore "présent" mais avec leur variable active à faux)
        for (int i = 0; i < groupPlayer.transform.childCount; i++)      
        {
            GameObject playerActif = groupPlayer.transform.GetChild(i).gameObject;
            if (playerActif && playerActif.activeSelf && playerActif.GetComponent<PlayerController>() && playerActif.GetComponent<PlayerController>().active)
            {
                number++;                                                           //ajoute 1 player
                if (playerActif.GetComponent<PlayerController>().nb_team == 1)      //si il est de la team 1...
                    team1++;                                                        //ajoute 1 à la team 1
                else if (playerActif.GetComponent<PlayerController>().nb_team == 2) //si il est de la team 2...
                    team2++;                                                        //ajoute 1 à la team 2
            }
        }
        if (checkTeam)                                                              //si l'option spécifié dans l'appelle de fonction est true
        {
            if (returnTheTeam)                                                      //si returnTheTeam est vrai, retourne la team victorieuse
            {
                if (team1 > 0 && team2 == 0)                                        //Si il n'y a plus de joueur dans la Team 2, la team 1 gagne !
                    return (1);
                else if (team1 == 0 && team2 > 0)                                   //Si il n'y a plus de joueur dans la Team 1, la team 2 gagne !
                    return (2);
                else if (team1 == 0 && team2 == 0)                                                                //sinon, aucune des 2 gagnes !
                    return (0);
                else
                    return (-1);
            }
            if (team1 == 0 || team2 == 0)                                           //renvoi 0 si une des deux team contient aucun player
                return (0);
        }
        return (number);
    }

    /// <summary>
    /// Lorsque le joueur meurt, enelver toute les références du joueur en remettant le type de particule à -1;
    /// Car les particules des Eggs changes s'il sont controllé par bleu, rouge ou autre...
    /// player = 1 si Repulsor, player = 2 si Attractor
    /// </summary>
    /// <param name="player">le type du player, de 1 à 4</param>
    void subRefToOther(int playerType)
    {
        for (int i = 0; i < groupEggs.transform.childCount; i++)
        {
            EggsController EC = groupEggs.transform.GetChild(i).GetComponent<EggsController>();
            if (EC && EC.getCurrentParticle() == playerType)    //si le type de particule = 0 ou 1 (0 -> repulsor, 1 -> attractor)
            {
                EC.changeCurrentParticle(-1);
            }
        }
    }
	
    /// <summary>
    /// comptabilise tout les Eggs qui sont Kinematic, et retourne ce nombre
    /// </summary>
    /// <returns>nombre d'Eggs kinematics</returns>
    public int getAllKine()
    {
        int k = 0;
        for (int i = 0; i < groupEggs.transform.childCount; i++)
        {
            if (groupEggs.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic)
            {
                k++;
            }
        }
        return (k);
    }

    public void createEclosionEvent()
    {
        Debug.Log("ici effectue une action d'eclosion");
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.fixedTime >= timeToGo && !endOfTheWorld)
        {
            ActualizeTarget();                                          //actualise la liste des joueurs pour vérifier qu'il y a pas de case vide
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
