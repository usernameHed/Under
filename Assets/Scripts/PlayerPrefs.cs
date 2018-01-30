using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

[Serializable]
public struct MapsInfos
{
    public int bestHighScore;
    public int succes;
    public bool blocked;
}

//[RequireComponent(typeof(CircleCollider2D))]
public class PlayerPrefs : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public static PlayerPrefs PP;

    public int worldUnlock = 0;                     //id du dernier monde débloqué
    public int mapUnlockId = 0;                     //id du dernier level du monde débloqué

    public int mapUnlockMulti = 0;                  //id du dernier level du mode multi débloqué (mettre au max des le debut ?)


    public int[] lastLevelPlayerId;                 //id du world/level JOUé


    //public string lastLevelPlayed = "None";         //string de la derniere map joué
    //public string nextLevel = "None";               //string de la prochaine map jouable

    //public bool mapNextBlocked = true;
    public bool fromRestart = false;

    public bool mapPrefIsInit = false;
    //public int lastWorldIdPlayed = 0;
    
    //public List<List<MapsInfos>> mapInfosPref = new List<List<MapsInfos>>();
    public List<MapsInfos> mapInfosPref0 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref1 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref2 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref3 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref4 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref5 = new List<MapsInfos>();
    public List<int> worldCountGoldenEggs = new List<int>();
    public int numberOfPref = 6;
    [HideInInspector] public int isWorldUnlocked = -1;              //y-a-t-il un monde qui vient d'être débloqué ?

    //public List<int> bestHighScorePerMaps;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>

    /// <summary>
    /// variable privé serealized
    /// </summary>
    //[SerializeField] private bool tmp;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        if (PP == null)
        {
            DontDestroyOnLoad(gameObject);
            PP = this;
        }
        else if (PP != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        if (!Load())                                                        //chargement des données du joueurs
           Save();                                                         //si ça n'a pas marché, création d'une nouvelle sauvegarde
    }
    #endregion

    #region core script

    /// <summary>
    /// utilisé pour récupéré la bonne maps
    /// </summary>
    public MapsInfos getLevels(int idWorld, int idMap)
    {
        switch (idWorld)
        {
            case 0:
                return (mapInfosPref0[idMap]);
            case 1:
                return (mapInfosPref1[idMap]);
            case 2:
                return (mapInfosPref2[idMap]);
            case 3:
                return (mapInfosPref3[idMap]);
            case 4:
                return (mapInfosPref4[idMap]);
            default:
                return (mapInfosPref5[idMap]);
        }
    }

    /// <summary>
    /// clear all list
    /// </summary>
    public void clearAll()
    {
        mapInfosPref0.Clear();
        mapInfosPref1.Clear();
        mapInfosPref2.Clear();
        mapInfosPref3.Clear();
        mapInfosPref4.Clear();
        mapInfosPref5.Clear();
    }

    public void setLevel(int idWorld, int idMaps, MapsInfos MI)
    {
        switch (idWorld)
        {
            case 0:
                mapInfosPref0[idMaps] = MI;
                break;
            case 1:
                mapInfosPref1[idMaps] = MI;
                break;
            case 2:
                mapInfosPref2[idMaps] = MI;
                break;
            case 3:
                mapInfosPref3[idMaps] = MI;
                break;
            case 4:
                mapInfosPref4[idMaps] = MI;
                break;
            default:
                mapInfosPref5[idMaps] = MI;
                break;
        }
    }

    /// <summary>
    /// utilisé pour récupéré la bonne maps
    /// </summary>
    public List<MapsInfos> getWorlds(int idWorld)
    {
        switch (idWorld)
        {
            case 0:
                return (mapInfosPref0);
            case 1:
                return (mapInfosPref1);
            case 2:
                return (mapInfosPref2);
            case 3:
                return (mapInfosPref3);
            case 4:
                return (mapInfosPref4);
            default:
                return (mapInfosPref5);
        }
    }

    /// <summary>
    /// ici set tout les level fait à 4 étoiles
    /// </summary>
    [ContextMenu("Cheat")]
    private void Cheat()                                                     //Save
    {
        DeleteSave();

        worldUnlock = 5;
        for (int i = 0; i < mapInfosPref0.Count; i++)
        {
            MapsInfos MI;
            MI.bestHighScore = 4; //le temps requis pour faire l'epic
            MI.succes = 4;                  //donne l'epic
            MI.blocked = false;
            mapInfosPref0[i] = MI;
        }
        for (int i = 0; i < mapInfosPref1.Count; i++)
        {
            MapsInfos MI;
            MI.bestHighScore = 4; //le temps requis pour faire l'epic
            MI.succes = 4;                  //donne l'or
            MI.blocked = false;
            mapInfosPref1[i] = MI;
        }
        for (int i = 0; i < mapInfosPref2.Count; i++)
        {
            MapsInfos MI;
            MI.bestHighScore = 4; //le temps requis pour faire l'epic
            MI.succes = 4;                  //donne l'or
            MI.blocked = false;
            mapInfosPref2[i] = MI;
        }
        for (int i = 0; i < mapInfosPref3.Count; i++)
        {
            MapsInfos MI;
            MI.bestHighScore = 4; //le temps requis pour faire l'epic
            MI.succes = 4;                  //donne l'or
            MI.blocked = false;
            mapInfosPref3[i] = MI;
        }
        for (int i = 0; i < mapInfosPref4.Count; i++)
        {
            MapsInfos MI;
            MI.bestHighScore = 4; //le temps requis pour faire l'epic
            MI.succes = 4;                  //donne l'or
            MI.blocked = false;
            mapInfosPref4[i] = MI;
        }

        Save();
    }

    /// <summary>
    /// Sauvegarde
    /// </summary>
    [ContextMenu("Save")]
    public void Save()                                                     //Save
    {
        BinaryFormatter bf = new BinaryFormatter();                         //
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        if (!mapPrefIsInit)
            setupHighScore();
        data.mapUnlockId = mapUnlockId;
        data.mapUnlockMulti = mapUnlockMulti;
        data.lastLevelPlayerId = lastLevelPlayerId;
        data.mapPrefIsInit = mapPrefIsInit;
        //data.lastLevelPlayed = lastLevelPlayed;
        //data.mapNextBlocked = mapNextBlocked;
        //data.nextLevel = nextLevel;
        data.fromRestart = fromRestart;
        //data.mapInfosPref = mapInfosPref;
        data.mapInfosPref0 = mapInfosPref0;
        data.mapInfosPref1 = mapInfosPref1;
        data.mapInfosPref2 = mapInfosPref2;
        data.mapInfosPref3 = mapInfosPref3;
        data.mapInfosPref4 = mapInfosPref4;
        data.mapInfosPref5 = mapInfosPref5;
        //data.lastWorldIdPlayed = lastWorldIdPlayed;
        data.worldUnlock = worldUnlock;
        //setupHighScore();
        setupCountGoldenEggs();

        Debug.Log("création de la nouvelle sauvegarde");
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// chargement
    /// </summary>
    public bool Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();                         //
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open); 
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();


            mapUnlockId = data.mapUnlockId;
            mapUnlockMulti = data.mapUnlockMulti;
            lastLevelPlayerId = data.lastLevelPlayerId;
            //lastLevelPlayed = data.lastLevelPlayed;
            //mapNextBlocked = data.mapNextBlocked;
            //nextLevel = data.nextLevel;
            fromRestart = data.fromRestart;
            //mapInfosPref = data.mapInfosPref;
            mapInfosPref0 = data.mapInfosPref0;
            mapInfosPref1 = data.mapInfosPref1;
            mapInfosPref2 = data.mapInfosPref2;
            mapInfosPref3 = data.mapInfosPref3;
            mapInfosPref4 = data.mapInfosPref4;
            mapInfosPref5 = data.mapInfosPref5;

            mapPrefIsInit = data.mapPrefIsInit;
            //lastWorldIdPlayed = data.lastWorldIdPlayed;
            worldUnlock = data.worldUnlock;
            //setupHighScore();
            setupCountGoldenEggs();
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// setup les high score: il faut que la liste soit de la même
    /// taille que la liste des maps.
    /// </summary>
    void setupHighScore()
    {
        //List<List<MapsInfos>> mapInfosPref

        clearAll();
        numberOfPref = MapPrefs.PP.numberOfWorld;
        for (int i = 0; i < MapPrefs.PP.numberOfWorld; i++)
        {
            //List<MapsInfos> tmpList = new List<MapsInfos>();
            for (int j = 0; j < MapPrefs.PP.getWorlds(i).Count; j++)
            {
                MapsInfos MI;
                MI.bestHighScore = 999999;
                MI.succes = 0;
                if (j == 0)
                    MI.blocked = false;
                else
                    MI.blocked = true;
                getWorlds(i).Add(MI);
                //tmpList.Add(MI);
            }
            //mapInfosPref.Add(tmpList);
            //getWorlds(i) = tmpList;
        }
        mapPrefIsInit = true;
    }

    public bool isLastLevelMap(int idWorld, int idMap)
    {
        if (MapPrefs.PP.getLevels(idWorld, idMap).lastLevelMap)
            return (true);
        return (false);
    }

    public bool isLastMap(int idWorld, int idMap)
    {
        if (MapPrefs.PP.getLevels(idWorld, idMap).lastMap)
            return (true);
        return (false);
    }

    /// <summary>
    /// test si la map [world][level + 1] est bloqué
    /// </summary>
    public bool nextMapBlocked(int idWorld, int idMap)
    {
        if (isLastMap(idWorld, idMap) || isLastLevelMap(idWorld, idMap))
            return (false);
        if (getLevels(idWorld, idMap + 1).blocked)
            return (true);
        return (false);
    }

    /// <summary>
    /// débloque le level
    /// </summary>
    public void unlockTheNextLevel(int idWorld, int idMap)
    {
        if (isLastMap(idWorld, idMap) || isLastLevelMap(idWorld, idMap))
            return;
        MapsInfos MI;

        MI.succes = getLevels(idWorld, idMap + 1).succes;
        MI.bestHighScore = getLevels(idWorld, idMap + 1).bestHighScore;
        MI.blocked = false;

        setLevel(idWorld, idMap + 1, MI);
        //mapInfosPref[idWorld][idMap + 1] = MI;
    }

    public string jumpToSpecificLevel(int idWorld, int idMap)
    {
        return (MapPrefs.PP.getLevels(idWorld, idMap).nameLevel);
    }

    /// <summary>
    /// renvoi le total des golden eggs gagné
    /// </summary>
    /// <returns></returns>
    public int getTotalGoldenEggsWinned()
    {
        int total = 0;
        for (int i = 0; i < worldCountGoldenEggs.Count; i++)
            total += worldCountGoldenEggs[i];
        return (total);
    }

    /// <summary>
    /// remplie la liste des countGoldenEggs (en ajoutant les 1-2-3-4 eggs de chaque map débloqué !)
    /// </summary>
    public void setupCountGoldenEggs()
    {
        worldCountGoldenEggs.Clear();
        for (int i = 0; i < MapPrefs.PP.numberOfWorld; i++)                //setup les 11 mondes (de 0 a 10)
        //for (int i = 0; i < 4; i++)                //setup les 11 mondes (de 0 a 10)
            worldCountGoldenEggs.Add(0);
        //d'abord reset tout a 0;
        /*for (int i = 0; i < MapPrefs.PP.mapsInfosSolo.Count; i++)
        {
            worldCountGoldenEggs.Add(0);
            //worldCountGoldenEggs[MapPrefs.PP.mapsInfosSolo[i].worldId] = 0;
            //worldCountGoldenEggs[]
        }*/
        //pour chaque monde, on essay de trouver les succes correspondant
        for (int i = 0; i < worldCountGoldenEggs.Count; i++)
        {
            if (MapPrefs.PP.numberOfWorld <= i)
                return;
            for (int j = 0; j < MapPrefs.PP.getWorlds(i).Count; j++)
            {
                //if (MapPrefs.PP.mapsInfosSolo[j].worldId == i)          //si l'id du monde est égal à l'index de la list à ajouter
                //{
                worldCountGoldenEggs[i] += getLevels(i, j).succes;//mapInfosPref[j].succes;
                //}
            }
            //worldCountGoldenEggs[MapPrefs.PP.mapsInfosSolo[i].worldId] += ;
            //worldCountGoldenEggs[]
        }
        //worldCountGoldenEggs
    }

    /// <summary>
    /// delete file save
    /// </summary>
    [ContextMenu("delete")]
    public void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");

            //System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program Application.Quit();
            //Application.Quit();
        }
    }

    /// <summary>
    /// reset
    /// </summary>
    [ContextMenu("reset")]
    public void resetAll()
    {
        DeleteSave();               //supprime la save
        mapUnlockId = 0;
        mapUnlockMulti = 0;
        lastLevelPlayerId = new int[2];
        lastLevelPlayerId[0] = 0;
        lastLevelPlayerId[1] = 0;

        //lastLevelPlayed = "None";
        //nextLevel = "None";
        //lastWorldIdPlayed = 0;
        worldUnlock = 0;
        //mapNextBlocked = true;
        mapPrefIsInit = false;
        fromRestart = false;
        //mapInfosPref.Clear();
        clearAll();

        worldCountGoldenEggs.Clear();
        setupHighScore();
        Save();                     //resauvegarde avec les paramettre pa rdefault !
    }

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {

    }
    #endregion
}

[Serializable]
class PlayerData
{
    [Tooltip("La progression du joueur en mode solo: l'id de la dernière map débloqué")]
    public int mapUnlockId;

    [Tooltip("La progression du joueur en mode multi: l'id de la dernière map débloqué")]
    public int mapUnlockMulti;

    [Tooltip("Le dernier monde unlock")]
    public int worldUnlock = 0;

    [Tooltip("La dernière map joué (l'id)/ la map en court de jeu")]
    public int [] lastLevelPlayerId;

    //[Tooltip("La dernière map joué (le nom de la scène) / la map en court de jeu")]
    //public string lastLevelPlayed;

    //[Tooltip("L'id du dernier monde joué")]
    //public int lastWorldIdPlayed;

    //[Tooltip("La map suivante est-elle bloqué ?")]
    //public bool mapNextBlocked;

    //[Tooltip("Le nom de la scène de la prochaine map jouable (si applicable !)")]
    //public string nextLevel;

    public bool mapPrefIsInit = false;

    [Tooltip("Est-ce qu'on viens du mode restart ? (utile dans le menu solo pour relancer le jeu courant")]
    public bool fromRestart;

    [Tooltip("La liste des scores de chaque maps")]
    //public List<List<MapsInfos>> mapInfosPref = new List<List<MapsInfos>>();
    public List<MapsInfos> mapInfosPref0 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref1 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref2 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref3 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref4 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref5 = new List<MapsInfos>();

    [Tooltip("le nombre de golden effs débloqqué par niveau")]
    public List<int> worldCountGoldenEggs = new List<int>();
}