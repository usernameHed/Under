using System.Collections.Generic;
using UnityEngine;

public class GlobalVariableManager : MonoBehaviour
{
    public bool multi = false;                                                  //désigne si le jeu est en mode multijoueur ou solo
    public bool soloAnd2Player = false;                                         //désigne si le jeu est en mode solo et qu'il y a un ou deux joueur
    public bool soloAnd2PlayerSwitchPower = false;                              //si on est en coop, est-ce que dans le menu on a switch les pouvoirs ?
    public bool keyboardType = true;                                            //sauvegarde le type de keyboard (arrow ou clavier)
    public bool fromMenu = false;                                               //si c'est true, ça viens du menu, sinon, ça viens du game (pour tester directement depuis le jeu sans passer par le menu);
    public bool fromGame = false;                                               //si c'est true, c'est pour le next level, on passe de game à menuSolo à game
    public int maxEggs = 20;
    public int timeMax = 180;
    public int timeRespawn = 20;                                                  //temps de respawn des joueur
    public int backToMainMenu = 0;

    //public bool lastMap = false;                                                //est-ce que c'est la dernière map ?
    //public int idMap = 1;
    public int tmpPlayer1Control = 0;
    public int tmpPlayer2Control = 5;

    public struct PlayerData                                                    //structure du joueur
    {
        public bool white;
        public bool repulse;
        public bool attract;
        public bool blackhole;
        public bool flaire;
        public int team;
        public int idPlayer;
    }
    public PlayerData playersTmp;

    public List<int> powerMapAccepted = new List<int>();                      //liste des pouvoirs accepté par la map
    public List<PlayerData> playersData = new List<PlayerData>();              //liste des maps, /!\ uniquement une a choisir

    private void Awake()
    {
        Cursor.visible = false;
        if (!fromMenu)
            fillOnGameOnUnityGamePlayOnly();
        /*else
        {
            GameObject[] globals = GameObject.FindGameObjectsWithTag("Global");
            if (globals.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(transform.gameObject);
        }
        Debug.Log("ici cree un globalVariableManager, une fois stp !");*/
    }

    /// <summary>
    /// Ajoute un joueur avec les pouvoir qu'il peut avoir,
    /// ainsi que l'index de sa team
    /// </summary>
    public void addPlayer(bool white, bool repu, bool attra, bool black, bool flai, int team, int idPlayer)
    {
        playersTmp.white = white;
        playersTmp.repulse = repu;
        playersTmp.attract = attra;
        playersTmp.blackhole = black;
        playersTmp.flaire = flai;
        playersTmp.team = team;
        playersTmp.idPlayer = idPlayer;
        playersData.Add(playersTmp);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Ces fonctions sont appelé uniquement lorsque je lance play depuis unity dans la game.
    /// <summary>
    /// fonction appelé une fois quand je lance le jeu sans passer par le menu !
    /// remplie juste ce qu'i faut pour tester les 4 joueurs avec les 4 pouvoirs
    /// </summary>
    void fillOnGameOnUnityGamePlayOnly()
    {
        powerMapAccepted.Add(1);                    //ajoute le pouvoir Repulse (1) accepté par la map
        powerMapAccepted.Add(2);                    //ajoute le pouvoir Attract (2) accepté par la map
        if (!multi)                                 //si on est en mode solo
        {
            if (!soloAnd2Player)
                addPlayer(false, true, true, true, true, 1, 1);   //créé le joueur, en lui donnant les 4 pouvoirs (et le joystick 1)
            else
            {
                addPlayer(false, true, true, true, true, 1, 1);   //créé le joueur 1 du solo, en lui donnant 2 pouvoirs (et le joystick 1)
                addPlayer(false, true, true, true, true, 1, 2);   //créé le joueur 2 du solo, en lui donnant 2 pouvoirs (et le joystick 2)
            }
        }
        else
        {
            addPlayer(false, false, false, true, false, 1, 1);   //créé un joueur de la team 1, repulsor
            addPlayer(false, false, false, true, false, 2, 2);   //créé un joueur de la team 2, attractor
            addPlayer(false, false, false, true, false, 1, 3);   //créé un joueur de la team 1, repulsor
            addPlayer(false, false, false, true, false, 2, 4);   //créé un joueur de la team 2, attractor
        }
    }

    /// <summary>
    /// affiche les valeurs avant de jouer
    /// </summary>
    public void displayValue()
    {
        Debug.Log("map power: ");
        for (int i = 0; i < powerMapAccepted.Count; i++)
        {
            if (powerMapAccepted[i] == 0)
                Debug.Log("power white");
            if (powerMapAccepted[i] == 1)
                Debug.Log("power repulse");
            if (powerMapAccepted[i] == 2)
                Debug.Log("power attract");
            if (powerMapAccepted[i] == 3)
                Debug.Log("power blackhole");
            if (powerMapAccepted[i] == 4)
                Debug.Log("power flaire");
        }

        Debug.Log("players:");
        if (!multi && !soloAnd2Player)
        {
            Debug.Log("mode solo ! control: " + playersData[0].idPlayer);
            if (playersData[0].white)
                Debug.Log("power white");
            if (playersData[0].repulse)
                Debug.Log("power repulse");
            if (playersData[0].attract)
                Debug.Log("power attract");
            if (playersData[0].blackhole)
                Debug.Log("power blackhole");
            if (playersData[0].flaire)
                Debug.Log("power flaire");
        }
        else if (!multi && soloAnd2Player)
        {
            Debug.Log("mode solo with friends !");
            Debug.Log("players 1, control: " + playersData[0].idPlayer);
            if (playersData[0].white)
                Debug.Log("power white");
            if (playersData[0].repulse)
                Debug.Log("power repulse");
            if (playersData[0].attract)
                Debug.Log("power attract");
            if (playersData[0].blackhole)
                Debug.Log("power blackhole");
            if (playersData[0].flaire)
                Debug.Log("power flaire");
            Debug.Log("players 2, control: " + playersData[1].idPlayer);
            if (playersData[1].white)
                Debug.Log("power white");
            if (playersData[1].repulse)
                Debug.Log("power repulse");
            if (playersData[1].attract)
                Debug.Log("power attract");
            if (playersData[1].blackhole)
                Debug.Log("power blackhole");
            if (playersData[1].flaire)
                Debug.Log("power flaire");
        }
        else
        {
            Debug.Log("mode multi !");
            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i].white)
                    Debug.Log("player: " + i + ": power white, control: " + playersData[i].idPlayer);
                if (playersData[i].repulse)
                    Debug.Log("player: " + i + ": power repulse, control: " + playersData[i].idPlayer);
                if (playersData[i].attract)
                    Debug.Log("player: " + i + ": power attract, control: " + playersData[i].idPlayer);
                if (playersData[i].blackhole)
                    Debug.Log("player: " + i + ": power blackholle, control: " + playersData[i].idPlayer);
                if (playersData[i].flaire)
                    Debug.Log("player: " + i + ": power flaire, control: " + playersData[i].idPlayer);
            }
        }
    }
}
