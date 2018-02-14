using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour {

    public enum TypeOfMove                                                    //type de pouvoir
    {
        keyboardZQSD = 0,
        joypad1 = 1,
        joypad2 = 2,
        joypad3 = 3,
        joypad4 = 4,
        keyboardARROW = 5,
    }
    public TypeOfMove TOP = TypeOfMove.keyboardZQSD;
    //public GameObject[] panel;
    public bool isActive = false;

    public int location = 0;
    public int Location
    {
        get
        {
            return (location);
        }
        set
        {
            if (location != value)
            {
                
                location = value;
                if (location < 0)
                    location = 0;
                else if (location > 1)
                    location = 1;

                if (location == 0)
                {
                    selectionMain[0].SetActive(true);
                    selectionMain[1].SetActive(false);
                }
                else if (location == 1)
                {
                    selectionMain[0].SetActive(false);
                    selectionMain[1].SetActive(true);
                }
                TWNE.isOk = false;
            }
        }
    }


    public int powerType = 0;
    public int PowerType
    {
        get
        {
            return (powerType);
        }
        set
        {
            if (powerType != value)
            {

                powerType = value;
                checkPowerChange();
                LMMM.actualiseListPlayers();
                checkValidate();
                TWNE.isOk = false;
            }
        }
    }

    public int teamType = 0;
    public int TeamType
    {
        get
        {
            return (teamType);
        }
        set
        {
            if (teamType != value)
            {

                teamType = value;
                checkTeamChange();
                LMMM.actualiseListPlayers();
                checkValidate();

                TWNE.isOk = false;
            }
        }
    }

    public bool lockedTeam = false;
    [SerializeField] private LevelManagerMainMenu LMMM;
    [SerializeField] private GameObject[] selectionMain;
    [SerializeField] private GameObject enablePanel;
    public List<GameObject> powerList = new List<GameObject>();
    public List<GameObject> antsList = new List<GameObject>();
    public List<GameObject> textTeamInfo = new List<GameObject>();
    [SerializeField] private Toggle couronne;
    [SerializeField] private GameObject validate;
    public bool validated = false;
    public bool isLocked = false;
    public GameObject lockedObject;
    

    /// <summary>
    /// private variable
    /// </summary>

    private TimeWithNoEffect TWNE;
    private PlayerConnected PC;
    //private bool changedPowerWithKeyboard = false;

    /// <summary>
    /// start
    /// </summary>
    private void Start()
    {
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        
        //setActivation();
    }

    /// <summary>
    /// change récursivement les pouvoirs 
    /// </summary>
    void checkPowerChange()
    {

        if (powerType < 0)
            powerType = LMMM.multiNumberPowerMap;
        /*else if (powerType > LMMM.multiNumberPowerMap)
        {
            Debug.Log("reset to 0 what ???");
            powerType = 0;
        }*/
            
        if (powerType >= 5)
            powerType = 0;

        if (    (powerType == 1 && !MapPrefs.getSingularity().mapsInfosMulti[PlayerPrefs.getSingularity().mapUnlockMulti].blue)
            ||  (powerType == 2 && !MapPrefs.getSingularity().mapsInfosMulti[PlayerPrefs.getSingularity().mapUnlockMulti].red)
            ||  (powerType == 3 && !MapPrefs.getSingularity().mapsInfosMulti[PlayerPrefs.getSingularity().mapUnlockMulti].yellow)
            ||  (powerType == 4 && !MapPrefs.getSingularity().mapsInfosMulti[PlayerPrefs.getSingularity().mapUnlockMulti].green)
                )
        {
            powerType++;
            checkPowerChange();
            return;
        }

        for (int i = 0; i < powerList.Count; i++)
        {
            powerList[i].SetActive(false);
            antsList[i].SetActive(false);
        }
        if (powerType != 0)
            powerList[powerType].SetActive(true);
        antsList[powerType].SetActive(true);
    }

    /// <summary>
    /// check si le joueur peu valider son jeu
    /// </summary>
    void checkValidate()
    {
        if (powerType != 0 && teamType != 0 && !LMMM.panelBadAnts[teamType - 1].activeSelf)
        {
            validated = true;
            validate.SetActive(true);
        }
        else
        {
            validated = false;
            validate.SetActive(false);
        }
    }

    void checkTeamChange()
    {
        //team = 0 (rien), 1 (bleu), 2 (red)
        if (teamType < 0)
            teamType = 2;
        else if (teamType > 2)
            teamType = 0;
        textTeamInfo[0].SetActive(false);
        textTeamInfo[1].SetActive(false);
        textTeamInfo[2].SetActive(false);
        switch (teamType)
        {
            case 0:
                couronne.isOn = false;
                couronne.interactable = false;
                textTeamInfo[0].SetActive(true);
                break;
            case 1:
                couronne.interactable = true;
                couronne.isOn = true;
                textTeamInfo[1].SetActive(true);
                break;
            case 2:
                couronne.interactable = true;
                couronne.isOn = false;
                textTeamInfo[2].SetActive(true);
                break;
        }
    }

    /// <summary>
    /// change l'activation du player courrant (pour les joypad, s'active si il est activé !)
    /// actualise le panel entier à l'état initiale
    /// </summary>
    public void setActivation()
    {
        PC = LMMM.PC;
        validated = false;
        isLocked = false;
        lockedObject.SetActive(false);
        isActive = PC.playerControllerConnected[getPlayerInt()];
        if (isActive)
        {
            Debug.Log("ici actualise et active" + getPlayerInt());
            gameObject.SetActive(true);
            enablePanel.SetActive(false);
            checkPowerChange();
            checkValidate();
            LMMM.actualiseListPlayers();
            if (getPlayerInt() == 1)
                LMMM.listPlayersPanel[0].desactiveThis();
            else if (getPlayerInt() == 2)
                LMMM.listPlayersPanel[5].desactiveThis();
        }
        else
        {
            desactiveThis();
        }
    }

    public void desactiveThis()
    {
        Debug.Log("ici actualise et desactive" + getPlayerInt());
        gameObject.SetActive(false);
        enablePanel.SetActive(true);
        powerType = 0;
        checkPowerChange();
        TeamType = 0;
        checkTeamChange();
        LMMM.actualiseListPlayers();
    }

    /// <summary>
    /// gère les inputs de chaque joueurs pour le choix des team, de pouvoir, de suivant et précédent
    /// </summary>
    void playerInput()
    {
        playerInputChooseTeamAndPower();
    }

    int getPlayerInt()
    {
        switch (TOP)
        {
            case TypeOfMove.keyboardZQSD:
                return (0);
            case TypeOfMove.joypad1:
                return (1);
            case TypeOfMove.joypad2:
                return (2);
            case TypeOfMove.joypad3:
                return (3);
            case TypeOfMove.joypad4:
                return (4);
            case TypeOfMove.keyboardARROW:
                return (5);
        }
        return (0);
    }

    float getSensitivity()
    {
        int playerTmpType = getPlayerInt();
        if (playerTmpType == 0 || playerTmpType == 5)
            return (0);
        return (0.5f);
    }

    /// <summary>
    /// le player a validé
    /// </summary>
    public void lockedPlayer()
    {
        isLocked = true;
        lockedObject.SetActive(true);
    }

    /// <summary>
    /// le player annule !
    /// </summary>
    public void unlockPlayer()
    {
        isLocked = false;
        lockedObject.SetActive(false);
        if (getPlayerInt() == 1)
            LMMM.multiLocation = 1;
    }

    /// <summary>
    /// get player input choose team and power
    /// </summary>
    void playerInputChooseTeamAndPower()
    {
        if (LMMM.playIn3activated)
            return;
        //si les contrôles ne sont pas bloqué par sa validation
        if (!isLocked)
        {
            if (location == 0)
            {
                if (PC.getPlayer(getPlayerInt()).GetAxis("UIHorizontal") > getSensitivity())
                    PowerType++;
                else if (PC.getPlayer(getPlayerInt()).GetAxis("UIHorizontal") < -getSensitivity())
                    PowerType--;
            }
            else if (location == 1)
            {
                if (PC.getPlayer(getPlayerInt()).GetAxis("UIHorizontal") > getSensitivity())
                    TeamType++;
                else if (PC.getPlayer(getPlayerInt()).GetAxis("UIHorizontal") < -getSensitivity())
                    TeamType--;
            }
            //vertical marche tout le temps
            if (PC.getPlayer(getPlayerInt()).GetAxis("UIVertical") > getSensitivity())
                Location++;
            else if (PC.getPlayer(getPlayerInt()).GetAxis("UIVertical") < -getSensitivity())
                Location--;

            //ajout de la touche A pour valider !
            if (PC.getPlayer(getPlayerInt()).GetButtonDown("UISubmit"))
            {
                if (getPlayerInt() != 0 && getPlayerInt() != 5)
                {
                    if (validated)
                    {
                        lockedPlayer();
                    }
                }
            }
            if (PC.getPlayer(getPlayerInt()).GetButtonDown("UICancel"))
            {
                if (getPlayerInt() != 0 && getPlayerInt() != 5)
                {
                    PowerType = 0;
                    TeamType = 0;
                }
            }
        }
        else
        {
            if (PC.getPlayer(getPlayerInt()).GetButtonDown("UICancel") && LMMM.TWNE.isOk && !LMMM.AreYouSure.activeSelf) //B
            {
                if (getPlayerInt() != 0 && getPlayerInt() != 5)
                {
                    unlockPlayer();
                    LMMM.TWNE.isOk = false;
                }
            }
        }

    }

    // Update is called once per frame
    void Update ()
    {
        if (TWNE.isOk && LMMM.multiLocation == 1 && isActive)
        {
            playerInput();
        }
    }
}
