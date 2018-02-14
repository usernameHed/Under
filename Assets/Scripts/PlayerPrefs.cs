using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;

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
    public int worldUnlock = 0;                     //id du dernier monde débloqué
    public int mapUnlockId = 0;                     //id du dernier level du monde débloqué
    public int mapUnlockMulti = 0;                  //id du dernier level du mode multi débloqué (mettre au max des le debut ?)
    public int[] lastLevelPlayerId;                 //id du world/level JOUé

    public bool fromRestart = false;
    public bool mapPrefIsInit = false;

    public List<MapsInfos> mapInfosPref0 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref1 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref2 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref3 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref4 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref5 = new List<MapsInfos>();
    public List<int> worldCountGoldenEggs = new List<int>();
    public int numberOfPref = 6;
    

    //public List<int> bestHighScorePerMaps;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    [HideInInspector] public int isWorldUnlocked = -1;              //y-a-t-il un monde qui vient d'être débloqué ?

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private static PlayerPrefs SS;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")]
    [Tooltip("Activation de la singularité du script")]
    [SerializeField]
    private bool enableSingularity = false;

    [FoldoutGroup("Debug")]
    [Tooltip("l'objet est-il global inter-scène ?")]
    [SerializeField]
    private bool dontDestroyOnLoad = false;

    #endregion

    #region fonction debug variables
    /// <summary>
    /// retourne une erreur si le timeOpti est inférieur ou égal à 0.
    /// </summary>
    /// <returns></returns>
    private bool debugTimeOpti(float timeOpti)
    {
        if (timeOpti <= 0)
            return (false);
        return (true);
    }

    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    private void testSingularity()
    {
        if (!enableSingularity)
            return;

        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// récupère la singularité (si ok par le script)
    /// </summary>
    /// <returns></returns>
    static public PlayerPrefs getSingularity()
    {
        if (!SS || !SS.enableSingularity)
        {
            Debug.LogError("impossible de récupéré la singularité");
            return (null);
        }
        return (SS);
    }

    /// <summary>
    /// set l'objet en dontDestroyOnLoad();
    /// </summary>
    private void setDontDestroyOnLoad()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        testSingularity();                                                  //set le script en unique ?
        setDontDestroyOnLoad();                                             //set le script Inter-scène ?
        Cursor.visible = false;                                             //cache le curseur de souris
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        if (!Load())                                                        //chargement des données du joueurs
            resetAll();
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

    /// <summary>
    /// change les info d'un level avec MI
    /// </summary>
    /// <param name="idWorld">l'id du monde</param>
    /// <param name="idMaps">l'id du level dans le monde</param>
    /// <param name="MI">les info à mettre dans les lists</param>
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

        data.fromRestart = fromRestart;
        data.mapInfosPref0 = mapInfosPref0;
        data.mapInfosPref1 = mapInfosPref1;
        data.mapInfosPref2 = mapInfosPref2;
        data.mapInfosPref3 = mapInfosPref3;
        data.mapInfosPref4 = mapInfosPref4;
        data.mapInfosPref5 = mapInfosPref5;
        data.worldUnlock = worldUnlock;
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
            fromRestart = data.fromRestart;

            mapInfosPref0 = data.mapInfosPref0;
            mapInfosPref1 = data.mapInfosPref1;
            mapInfosPref2 = data.mapInfosPref2;
            mapInfosPref3 = data.mapInfosPref3;
            mapInfosPref4 = data.mapInfosPref4;
            mapInfosPref5 = data.mapInfosPref5;

            mapPrefIsInit = data.mapPrefIsInit;
            worldUnlock = data.worldUnlock;
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
        clearAll();
        numberOfPref = MapPrefs.getSingularity().numberOfWorld;
        for (int i = 0; i < MapPrefs.getSingularity().numberOfWorld; i++)
        {
            //List<MapsInfos> tmpList = new List<MapsInfos>();
            for (int j = 0; j < MapPrefs.getSingularity().getWorlds(i).Count; j++)
            {
                MapsInfos MI;
                MI.bestHighScore = 999999;
                MI.succes = 0;
                if (j == 0)
                    MI.blocked = false;
                else
                    MI.blocked = true;
                getWorlds(i).Add(MI);
            }
        }
        mapPrefIsInit = true;
    }

    public bool isLastLevelMap(int idWorld, int idMap)
    {
        if (MapPrefs.getSingularity().getLevels(idWorld, idMap).lastLevelMap)
            return (true);
        return (false);
    }

    public bool isLastMap(int idWorld, int idMap)
    {
        if (MapPrefs.getSingularity().getLevels(idWorld, idMap).lastMap)
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
    }

    public string jumpToSpecificLevel(int idWorld, int idMap)
    {
        return (MapPrefs.getSingularity().getLevels(idWorld, idMap).nameLevel);
    }

    /// <summary>
    /// renvoi le total des golden eggs gagné
    /// </summary>
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
        for (int i = 0; i < MapPrefs.getSingularity().numberOfWorld; i++)                //setup les 11 mondes (de 0 a 10)
             worldCountGoldenEggs.Add(0);
        //pour chaque monde, on essay de trouver les succes correspondant
        for (int i = 0; i < worldCountGoldenEggs.Count; i++)
        {
            if (MapPrefs.getSingularity().numberOfWorld <= i)
                return;
            for (int j = 0; j < MapPrefs.getSingularity().getWorlds(i).Count; j++)
            {
                worldCountGoldenEggs[i] += getLevels(i, j).succes;
            }
        }
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
        worldUnlock = 0;
        mapPrefIsInit = false;
        fromRestart = false;
        clearAll();

        worldCountGoldenEggs.Clear();
        setupHighScore();
        Save();                     //resauvegarde avec les paramettre pa rdefault !
    }

    #endregion

    #region unity fonction and ending

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

    public bool mapPrefIsInit = false;

    [Tooltip("Est-ce qu'on viens du mode restart ? (utile dans le menu solo pour relancer le jeu courant")]
    public bool fromRestart;

    [Tooltip("La liste des scores de chaque maps")]
    public List<MapsInfos> mapInfosPref0 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref1 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref2 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref3 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref4 = new List<MapsInfos>();
    public List<MapsInfos> mapInfosPref5 = new List<MapsInfos>();

    [Tooltip("le nombre de golden effs débloqqué par niveau")]
    public List<int> worldCountGoldenEggs = new List<int>();
}