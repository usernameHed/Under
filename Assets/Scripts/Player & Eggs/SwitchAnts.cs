using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchAnts : MonoBehaviour
{
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    public GameObject[] PanelPower;                                         //GUI panel des pouvoirs des joueurs 1 et 2 du mode solo
    public GameObject[] groupOfAllSolo;                                     //objet qui contient les X fourmis du joueurs
    public List<bool> panelDisplayed = new List<bool>();                    //list des statue d'affichage du panel 1 et 2
    public List<int> typeSwitch = new List<int>();                          //list des statue pouvoir du joueur 1 et 2
    public List<int> listPower1 = new List<int>();                          //list des pouvoirs du joueur 1
    public List<int> listPower2 = new List<int>();                          //list des pouvoirs du joueur 2
    public List<GameObject> TargetsPlayerSolo1 = new List<GameObject>();    //list des players du solo 1
    public List<GameObject> TargetsPlayerSolo2 = new List<GameObject>();    //list des players du solo 2

    /// <summary>
    /// private serialized
    /// </summary>

    [SerializeField]    private GameObject groupEggs;
    [SerializeField]    private GameObject groupParticleSwitch;

    /// <summary>
    /// private
    /// </summary>
    private GameObject variableGlobal;                                      //référence sur les data global
    private PlayerConnected PC;                                             //ref des manettes connecté
    private GameManager GM;
    private TimeWithNoEffect TWNE;                                          //no effect sur le changement de pouvoirs
    private InitialiseGame IG;
    public List<Transform> tmpTransformPlayer = new List<Transform>();      //sauvegarge temporaire de la fourmis précédente
    private float timeToGo;
    private List<PlayerController> lastPowerDesactivated = new List<PlayerController>();

    //gaz
    private int tmpNumberOfOtorizedGaz;     //sauvegarde le nombre de la fourmis précédente
    private bool tmpIsAlreadySlowedByGaz = false;
    private int tmpNumberOfTimzGazed = 0;
    private List<ZoneEffector> zoneTmpSwitch = new List<ZoneEffector>();

    /// <summary>
    /// init
    /// </summary>
    private void Awake()
    {
        variableGlobal = GameObject.FindGameObjectWithTag("Global");
        if (variableGlobal)
            PC = variableGlobal.GetComponent<PlayerConnected>();
        GM = gameObject.GetComponent<GameManager>();
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        IG = gameObject.GetComponent<InitialiseGame>();
    }

    /// <summary>
    /// start
    /// </summary>
    private void Start()
    {
        if (GM.multi)
            return;
        timeToGo = Time.fixedTime + timeOpti;
        panelDisplayed.Add(false);                  //set le panel 1 à false (caché)
        panelDisplayed.Add(false);                  //set le panel 2 à false (caché)
        lastPowerDesactivated.Add(null);              //set le dernier pouvoir désactivé
        lastPowerDesactivated.Add(null);              //set le dernier pouvoir désactivé
        //set le pouvoirs initiale du joueur 1, et 2 s'il existe
        if (GM.TargetsPlayers.Count > 0)
        {
            typeSwitch.Add(GM.TargetsPlayers[0].GetComponent<PlayerController>().getTypePowerPlayer());
            //Debug.Log("power of player 1: " + GM.TargetsPlayers[0].GetComponent<PlayerController>().getTypePowerPlayer());
            tmpTransformPlayer.Add(null);
        }            
        if (GM.TargetsPlayers.Count > 1)
        {
            typeSwitch.Add(GM.TargetsPlayers[1].GetComponent<PlayerController>().getTypePowerPlayer());
            //Debug.Log("power of player 2: " + GM.TargetsPlayers[1].GetComponent<PlayerController>().getTypePowerPlayer());
            tmpTransformPlayer.Add(null);
        }
            
        configurePanel();                           //configure les panels des joueurs en désactivants les bon pouvoirs, et en mettant ON celui courrant
    }

    /// <summary>
    /// reset les panel lors de l'activation d'un joueur
    /// </summary>
    public void resetPanel(int player)
    {
        PanelPower[player].GetComponent<CanvasGroup>().alpha = 0;
    }

    /// <summary>
    /// configure les panels des joueurs en désactivants les bon pouvoirs, et en mettant ON celui courrant
    /// </summary>
    void configurePanel()
    {
        if (typeSwitch.Count == 1)                  //s'il n'y a qu'un joueur, supprimer le panel 2
            PanelPower[1].SetActive(false);
        int activeFirstPower = 0;
        for (int i = 0; i < listPower1.Count; i++)
        {
            if (listPower1[i] == 0)                     //si le pouvoir est blanc, il n'est pas dans le switch !
                continue;
            if (activeFirstPower == 0)
                PanelPower[0].transform.GetChild(listPower1[i] - 1).GetComponent<Toggle>().isOn = true;
            PanelPower[0].transform.GetChild(listPower1[i] - 1).GetComponent<Toggle>().interactable = true;
            activeFirstPower++;
        }
        activeFirstPower = 0;
        for (int j = 0; j < listPower2.Count; j++)
        {
            if (listPower2[j] == 0)                     //si le pouvoir est blanc, il n'est pas dans le switch !
                continue;
            if (activeFirstPower == 0)
                PanelPower[1].transform.GetChild(listPower2[j] - 1).GetComponent<Toggle>().isOn = true;
            PanelPower[1].transform.GetChild(listPower2[j] - 1).GetComponent<Toggle>().interactable = true;
            activeFirstPower++;
        }
    }

    /// <summary>
    /// dans le mode solo, désactive le panel
    /// //ET désactive le parent qui contient toute les fourmis
    /// </summary>
    public void desactiveAllSoloPanel(bool active, int indexPlayer)
    {
        PanelPower[indexPlayer].SetActive(active);
        //groupOfAllSolo[indexPlayer].SetActive(false);
        //Destroy(PanelPower[indexPlayer]);
    }

    void switchAvatar(GameObject ants, int typePower)
    {
        ants.GetComponent<PlayerController>().changeSpriteAvatar(IG.picPower[typePower], IG.picPlayerDead[typePower]);
    }

    /// <summary>
    /// cherche quel fourmis doit être activé ou désactivé
    /// </summary>
    /// <param name="indexPlayer"></param>
    /// <param name="typePower"></param>
    void findTheRightAnts(int indexPlayer, int typePower, bool active)
    {
        if (indexPlayer == 0)
        {
            for (int i = 0; i < listPower1.Count; i++)
            {
                if (listPower1[i] == typePower)
                {
                    //ici le bon player 1 à désactiver
                    activeAnts(TargetsPlayerSolo1[i], active, indexPlayer, typePower);
                    if (active)
                    {
                        PanelPower[indexPlayer].GetComponent<GuiFollow>().WorldObject = TargetsPlayerSolo1[i];
                        switchAvatar(TargetsPlayerSolo1[i], typePower); 
                    }
                    return;
                }
            }
        }
        else
        {
            //Debug.Log("iciiii");
            for (int i = 0; i < listPower2.Count; i++)
            {
                if (listPower2[i] == typePower)
                {
                    //ici le bon player 2 à activer
                    activeAnts(TargetsPlayerSolo2[i], active, indexPlayer, typePower);
                    if (active)
                    {
                        PanelPower[indexPlayer].GetComponent<GuiFollow>().WorldObject = TargetsPlayerSolo2[i];
                        switchAvatar(TargetsPlayerSolo2[i], typePower);
                    }
                    return;
                }
            }
        }
    }

    /// <summary>
    /// retourne vrai si le joueur courant est dans l'état congelé !
    /// </summary>
    /// <returns></returns>
    bool isCurrentPlayerSlow(int indexPlayer, int typePower)
    {
        if (indexPlayer == 0)
        {
            for (int i = 0; i < listPower1.Count; i++)
            {
                if (listPower1[i] == typePower)
                {
                    return (TargetsPlayerSolo1[i].GetComponent<PlayerController>().isSlow);
                }
            }
        }
        else
        {
            //Debug.Log("iciiii");
            for (int i = 0; i < listPower2.Count; i++)
            {
                if (listPower2[i] == typePower)
                {
                    return (TargetsPlayerSolo2[i].GetComponent<PlayerController>().isSlow);
                }
            }
        }
        return (false);
    }

    /// <summary>
    /// fonction appelé dans le mode solo lors de l'activation d'une fourmis
    /// elle désactive les pouvoirs des oeufs du joueur précédement sélectionné
    /// </summary>
    void resetEggsForPlayer(int indexPlayer)
    {
        //lastPowerDesactivated[indexPlayer].releaseAllEggs();
        for (int i = 0; i < lastPowerDesactivated[indexPlayer].eggsArray.Count; i++)
        {
            if (lastPowerDesactivated[indexPlayer].eggsArray[i])
            {
                Debug.Log("ici !");
                lastPowerDesactivated[indexPlayer].eggsArray[i].transform.gameObject.GetComponent<EggsController>().StopBeingControlled();
            }
        }
        lastPowerDesactivated[indexPlayer].eggsArray.Clear();
    }

    //si le player d'avant était un portals, désactiver tout les portals !!
    void desactiveAllPortals(int indexPlayer)
    {
        FirePortals FP = lastPowerDesactivated[indexPlayer].gameObject.transform.GetChild(0).gameObject.GetComponent<FirePortals>();
        if (FP)
        {
            Debug.Log("ici le destroyLinkPortal switch ???");
            //FP.resetAll();
        }

    }

    /// <summary>
    /// active ou désactive l'objet ants
    /// </summary>
    /// <param name="ants">fourmis à activer ou désactiver</param>
    /// <param name="active">activer ou non</param>
    /// <param name="indexPlayer">le joueur 1 ou 2</param>
    /// <param name="typePower">type de pouvoir, de 1 à 4</param>
    void activeAnts(GameObject ants, bool active, int indexPlayer, int typePower)
    {
        PlayerController PCtmp = ants.GetComponent<PlayerController>();
        if (!PCtmp)
            return;
        if (!active)
        {
            tmpTransformPlayer[indexPlayer] = ants.transform;
            PCtmp.stopEverything();

            tmpNumberOfOtorizedGaz = PCtmp.numberOfOtorizedGaz;
            tmpIsAlreadySlowedByGaz = PCtmp.isAlreadySlowedByGaz;
            tmpNumberOfTimzGazed = PCtmp.numberOfTimzGazed;

            GM.AddPlayer(ants, true);       //delete le player de la liste
            lastPowerDesactivated[indexPlayer] = PCtmp;
            desactiveAllPortals(indexPlayer);               //reset les portals du joueur précédent si c'étais un mec à portals......

            //ici sauvegarde la lsite des zoneeffector, pour ensuite la vider de l'ant a désactiver, et la mettre 
            //dans la ants a activer
            zoneTmpSwitch = PCtmp.getListZoneEffector();
            PCtmp.zoneCamClear();
            ants.SetActive(false);


        }
        else
        {
            if (tmpTransformPlayer[indexPlayer] == null)
                return;
            resetEggsForPlayer(indexPlayer);                 //reset all eggs du player précédemment désactivé

            lastPowerDesactivated[indexPlayer] = null;

            //ici, reset la list des nouvelle ants, set l'ancien liste au nouveau ants, et reset le tmp
            PCtmp.zoneCamClear();
            PCtmp.setListZoneEffector(zoneTmpSwitch);
            zoneTmpSwitch.Clear();

            ants.SetActive(true);

            //get l'autre player pour appeller setColliderWalls sur lui
            ants.transform.position = tmpTransformPlayer[indexPlayer].position;
            ants.transform.rotation = tmpTransformPlayer[indexPlayer].rotation;

            //cree effet de particule du panel
            //active particule effect !
            groupParticleSwitch.transform.GetChild(typePower - 1).gameObject.transform.position = ants.transform.position;
            groupParticleSwitch.transform.GetChild(typePower - 1).gameObject.SetActive(false);
            groupParticleSwitch.transform.GetChild(typePower - 1).gameObject.SetActive(true);

            GM.AddPlayer(ants);             //ajoute le player
            PCtmp.restartEverything();
            PCtmp.setGazWhenSwitch(tmpNumberOfOtorizedGaz, tmpIsAlreadySlowedByGaz, tmpNumberOfTimzGazed);


            GM.setColliderWallsOfTargetsPlayer();
        }
        
    }

    /// <summary>
    /// swithc de fourmis
    /// </summary>
    void switchAnts(int indexPlayer, int typePower)
    {
        
        //désactive la fourmis précédente...
        //désactive la fourmis de l'index: indexPlayer et de pouvoir: typeSwitch[indexPlayer]
        PanelPower[indexPlayer].transform.GetChild(typeSwitch[indexPlayer] - 1).GetComponent<Toggle>().isOn = false;
        findTheRightAnts(indexPlayer, typeSwitch[indexPlayer], false);
        //Debug.Log("desactive ants of power: " + typeSwitch[indexPlayer]);

        //active la nouvelle fourmis
        typeSwitch[indexPlayer] = typePower;
        PanelPower[indexPlayer].transform.GetChild(typeSwitch[indexPlayer] - 1).GetComponent<Toggle>().isOn = true;
        findTheRightAnts(indexPlayer, typeSwitch[indexPlayer], true);
        //Debug.Log("active ants of power: " + typeSwitch[indexPlayer]);
    }

    /// <summary>
    /// affiche/cache le panel, et détermine si on peu switch de pouvoirs
    /// </summary>
    /// <param name="indexPlayer">fourmis 1 ou 2</param>
    /// <param name="typePower">type de pouvoirs à changer</param>
    public void SwitchPanel(int indexPlayer, int typePower)
    {
        //affiche le panel
        if (!panelDisplayed[indexPlayer])
        {
            panelDisplayed[indexPlayer] = true;

            PanelPower[indexPlayer].GetComponent<FadeObjectInOut>().fadeOut = false;
            if (PanelPower[indexPlayer].GetComponent<TimeWithNoEffect>().isOk)
                PanelPower[indexPlayer].GetComponent<TimeWithNoEffect>().isOk = false;
            else
                PanelPower[indexPlayer].GetComponent<TimeWithNoEffect>().restart = true;
        }

        //change le pouvoirs si il peut le faire
        if (typePower != typeSwitch[indexPlayer]
            && TWNE.isOk
            && PanelPower[indexPlayer].transform.GetChild(typePower - 1).GetComponent<Toggle>().IsInteractable()        //ne pas effectuer 2 fois le même changement si c'est avec le même pouvoirs
            && !isCurrentPlayerSlow(indexPlayer, typePower))
        {
            TWNE.isOk = false;
            switchAnts(indexPlayer, typePower);
        }
    }

    /// <summary>
    /// cache le panel si il a été activé
    /// </summary>
    void hidePanel()
    {
        for (int i = 0; i < panelDisplayed.Count; i++)                  //parcourt la liste (0 ou 1)
        {
            //si le pnale est affiché, testé si les input du joueur sont PAS pressé pour cacher...
            if (panelDisplayed[i] && PanelPower[i].GetComponent<TimeWithNoEffect>().isOk)
            {
                if (GM.groupPlayer.transform.childCount == 0 || GM.groupPlayer.transform.GetChild(i).childCount == 0)           //si y'a plus de joueurs de la team 0 ou 1...
                {
                    PanelPower[i].SetActive(false);                     //cacher le panel, et passer au suivant
                    continue;
                }
                //Debug.Log("ici: player de type: " + i);
                //récupère nb_player du player pour connaitre son type de contrôle.
                int nb_player = GM.groupPlayer.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<PlayerController>().nb_player;
                if (PC.getPlayer(nb_player).GetAxisRaw("Switch Vertical") == 0
                    || PC.getPlayer(nb_player).GetAxisRaw("Switch Horizontal") == 0)
                {
                    panelDisplayed[i] = false;                                      //set false
                    PanelPower[i].GetComponent<FadeObjectInOut>().fadeOut = true;  //cacher le panel
                }
            }
        }
    }

    /// <summary>
    /// update
    /// </summary>
    private void Update()
    {
        if (Time.fixedTime >= timeToGo && !GM.multi)
        {
            hidePanel();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
