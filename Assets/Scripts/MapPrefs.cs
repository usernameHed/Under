using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;

//[RequireComponent(typeof(CircleCollider2D))]
public class MapPrefs : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Tooltip("list des info du monde 0")] public List<dataMapsSolo> mapsInfosSoloWorlds0 = new List<dataMapsSolo>();
    [Tooltip("list des info du monde 1")] public List<dataMapsSolo> mapsInfosSoloWorlds1 = new List<dataMapsSolo>();
    [Tooltip("list des info du monde 2")] public List<dataMapsSolo> mapsInfosSoloWorlds2 = new List<dataMapsSolo>();
    [Tooltip("list des info du monde 3")] public List<dataMapsSolo> mapsInfosSoloWorlds3 = new List<dataMapsSolo>();
    [Tooltip("list des info du monde 4")] public List<dataMapsSolo> mapsInfosSoloWorlds4 = new List<dataMapsSolo>();
    [Tooltip("list des info du monde 5")] public List<dataMapsSolo> mapsInfosSoloWorlds5 = new List<dataMapsSolo>();
    [FoldoutGroup("Debug")] [Tooltip("nombre de monde")] public int numberOfWorld = 6;

    [Tooltip("Nombre min d'oeuf à avoir pour débloquer le monde")] public List<int> numberEggsToHave = new List<int>();

    [Tooltip("Liste des infos des maps multi")] public List<dataMapsSolo> mapsInfosMulti = new List<dataMapsSolo>();

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private static MapPrefs SS; //singleton privé

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
    static public MapPrefs getSingularity()
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
        setDontDestroyOnLoad();
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {

    }
    #endregion

    #region core script

    /// <summary>
    /// utilisé pour récupéré la bonne maps
    /// </summary>
    public dataMapsSolo getLevels(int idWorld, int idMap)
    {
        switch (idWorld)
        {
            case 0:
                return (mapsInfosSoloWorlds0[idMap]);
            case 1:
                return (mapsInfosSoloWorlds1[idMap]);
            case 2:
                return (mapsInfosSoloWorlds2[idMap]);
            case 3:
                return (mapsInfosSoloWorlds3[idMap]);
            case 4:
                return (mapsInfosSoloWorlds4[idMap]);
            default:
                return (mapsInfosSoloWorlds5[idMap]);
        }
    }

    public List<dataMapsSolo> getWorlds(int idWorld)
    {
        switch (idWorld)
        {
            case 0:
                return (mapsInfosSoloWorlds0);
            case 1:
                return (mapsInfosSoloWorlds1);
            case 2:
                return (mapsInfosSoloWorlds2);
            case 3:
                return (mapsInfosSoloWorlds3);
            case 4:
                return (mapsInfosSoloWorlds4);
            default:
                return (mapsInfosSoloWorlds5);
        }
    }


    /// <summary>
    /// Sauvegarde
    /// </summary>
    [ContextMenu("Save")]
    public void Save()                                                     //Save
    {
        BinaryFormatter bf = new BinaryFormatter();                         //
        FileStream file = File.Create(Application.dataPath + "/Resources/mapInfo.dat");

        MapsPrefData data = new MapsPrefData();
        //data.mapsInfosSolo = mapsInfosSolo;
        data.mapsInfosMulti = mapsInfosMulti;
        data.numberEggsToHave = numberEggsToHave;
        //data.mapsInfosSoloWorlds = mapsInfosSoloWorlds;
        data.mapsInfosSoloWorlds0 = mapsInfosSoloWorlds0;
        data.mapsInfosSoloWorlds1 = mapsInfosSoloWorlds1;
        data.mapsInfosSoloWorlds2 = mapsInfosSoloWorlds2;
        data.mapsInfosSoloWorlds3 = mapsInfosSoloWorlds3;
        data.mapsInfosSoloWorlds4 = mapsInfosSoloWorlds4;
        data.mapsInfosSoloWorlds5 = mapsInfosSoloWorlds5;

        Debug.Log("création de la nouvelle sauvegarde");
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// création des equart til par rapport à tous les temps...
    /// </summary>
    [ContextMenu("Create setup quartil & Eggs To Have if there is nothing")]
    public void CreateEquatTil()
    {

        for (int i = 0; i < numberOfWorld; i++)
        {
            for (int j = 0; j < getWorlds(i).Count; j++)
            {
                getLevels(i, j).mapId = j;

                if (getLevels(i, j).timeEggs2to4.Count > 0 && getLevels(i, j).timeEggs2to4[0] == 0)
                    getLevels(i, j).timeEggs2to4.Clear();
                int eggsCount = 3;
                while (getLevels(i, j).timeEggs2to4.Count < 3)
                {
                    getLevels(i, j).timeEggs2to4.Add(10 * eggsCount);
                    eggsCount--;
                    if (eggsCount < 0)
                        eggsCount = 0;
                }


                //défini le last level
                if (j + 1 >= getWorlds(i).Count)        //si on est sur le dernier de la liste...
                {
                    getLevels(i, j).lastLevelMap = true;
                }
                else
                    getLevels(i, j).lastLevelMap = false;

            }
            //défini le lastMap
            if (i + 1 >= numberOfWorld)
            {
                getLevels(i, getWorlds(i).Count - 1).lastMap = true;
            }
            else
                getLevels(i, getWorlds(i).Count - 1).lastMap = false;
        }

        if (numberEggsToHave.Count > numberOfWorld)
            numberEggsToHave.Clear();
        while (numberEggsToHave.Count < numberOfWorld)
        {
            if (numberEggsToHave.Count == 0)
                numberEggsToHave.Add(1);
            else
                numberEggsToHave.Add(12);
        }

        changeWorldUnlock();
    }

    /// <summary>
    /// change le worldUnlock selon les médailles débloqué dans chaque niveaux
    /// </summary>
    public void changeWorldUnlock()
    {
        Debug.Log("Count numberEggsToHave: " + numberEggsToHave.Count);
        Debug.Log("Count worldCountGoldenEggs: " + PlayerPrefs.getSingularity().worldCountGoldenEggs.Count);
        Debug.Log("value worldUnlock: " + PlayerPrefs.getSingularity().worldUnlock);
        for (int i = 0; i < numberEggsToHave.Count; i++)
        {
            if (!PlayerPrefs.getSingularity())
                Debug.Log("FUCK PLAYERPFREF PP");

            if (PlayerPrefs.getSingularity().getTotalGoldenEggsWinned() >= numberEggsToHave[Mathf.Min(i + 1, numberEggsToHave.Count - 1)]
                && PlayerPrefs.getSingularity().worldUnlock <= i
                && checkIfAllLevelAreUnlock(PlayerPrefs.getSingularity().worldUnlock))    //ici vérifier quand tous les niveaux du monde actuel a été débloqué !
            {
                PlayerPrefs.getSingularity().worldUnlock = i + 1;
                PlayerPrefs.getSingularity().mapUnlockId = 0;
                Debug.Log("on a débloqué le monde: " + (PlayerPrefs.getSingularity().worldUnlock) + " !");
                PlayerPrefs.getSingularity().isWorldUnlocked = PlayerPrefs.getSingularity().worldUnlock;
            }
        }
    }

    /// <summary>
    /// retourne vrai si un monde à tout ses niveau de débloqué
    /// </summary>
    /// <param name="idWorld"></param>
    /// <returns></returns>
    public bool checkIfAllLevelAreUnlock(int idWorld, bool alsoBonus = false)
    {
        int saveI = 0;
        //dès qu'on trouve un niveau bloqué, on part
        for (int i = 0; i < PlayerPrefs.getSingularity().getWorlds(idWorld).Count; i++)
        {
            if (PlayerPrefs.getSingularity().getLevels(idWorld, i).blocked)
            {

                if (!(((idWorld == 1 && mapsInfosSoloWorlds1[i].isBonus)
                       || (idWorld == 2 && mapsInfosSoloWorlds2[i].isBonus)
                       || (idWorld == 3 && mapsInfosSoloWorlds3[i].isBonus)
                       || (idWorld == 4 && mapsInfosSoloWorlds4[i].isBonus)
                       || (idWorld == 5 && mapsInfosSoloWorlds5[i].isBonus)) && !alsoBonus))
               {
                   return (false);
               }
            }

            saveI = i;
        }
        //si le dernier niveau est bloqué, retourne faux ????
        if (PlayerPrefs.getSingularity().getLevels(idWorld, saveI).succes == 0)
        {
            Debug.Log("ici le dernier niveau est toujours bloqué ??");
            return (false);
        }
        return (true);
    }

    /// <summary>
    /// chargement
    /// </summary>
    [ContextMenu("Load")]
    public bool Load()
    {
        if (File.Exists(Application.dataPath + "/Resources/mapInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/Resources/mapInfo.dat", FileMode.Open);
            MapsPrefData data = (MapsPrefData)bf.Deserialize(file);
            file.Close();

            mapsInfosMulti = data.mapsInfosMulti;
            numberEggsToHave = data.numberEggsToHave;
            mapsInfosSoloWorlds0 = data.mapsInfosSoloWorlds0;
            mapsInfosSoloWorlds1 = data.mapsInfosSoloWorlds1;
            mapsInfosSoloWorlds2 = data.mapsInfosSoloWorlds2;
            mapsInfosSoloWorlds3 = data.mapsInfosSoloWorlds3;
            mapsInfosSoloWorlds4 = data.mapsInfosSoloWorlds4;
            mapsInfosSoloWorlds5 = data.mapsInfosSoloWorlds5;

            Debug.Log("chargement duccess !");
            return (true);
        }
        return (false);
    }

    #endregion

    #region unity fonction and ending

    #endregion
}

[Serializable]
class MapsPrefData
{
    public List<dataMapsSolo> mapsInfosSoloWorlds0 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosSoloWorlds1 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosSoloWorlds2 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosSoloWorlds3 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosSoloWorlds4 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosSoloWorlds5 = new List<dataMapsSolo>();
    public List<dataMapsSolo> mapsInfosMulti = new List<dataMapsSolo>();
    public List<int> numberEggsToHave = new List<int>();
}

[System.Serializable]
public class dataMapsSolo
{
    [FoldoutGroup("don't touch")]
    [Tooltip("Id du monde auquel il appartient")]
    public int worldId = 1;

    [FoldoutGroup("don't touch")]
    [Tooltip("l'id de la map")]
    public int mapId = 0;

    [FoldoutGroup("don't touch")]
    [Tooltip("Nom exacte de la scène Unity")]
    public string nameLevel = "MapSolo_001";

    [FoldoutGroup("main info")]
    [Tooltip("Nom affiché de la map")]
    public string nameMap = "Name";

    [FoldoutGroup("main info")]
    [Tooltip("cette map est-elle un bonus")]
    public bool isBonus = false;

    [ValidateInput("coopEnabled", "Warning, the power of player 1 & 2 are not correctly setup (if changed, uncheck/check coop button for check again)", InfoMessageType.Warning)]
    [Tooltip("Le jeu peut-il se faire en coop ou pas ? (il doit y avoir 2 pouvoirs d'activer pour cela !)")]
    public bool coop = false;

    //[Tooltip("La map est-elle bloqué ? NE PAS TOUCHER CETTE CASE, ELLE SE GERE TOUTE SEUL AVEC LA PROGRESSION")]
    //public bool blocked = false;

    [FoldoutGroup("power map")]
    [Tooltip("Pouvoir blanc (pas de pouvoirs)")]
    public bool white = false;

    [FoldoutGroup("power map")]
    [Tooltip("Pouvoir bleu (repulse)")]
    public bool blue = true;

    [FoldoutGroup("power map")]
    [Tooltip("Pouvoir red (attire)")]
    public bool red = false;

    [FoldoutGroup("power map")]
    [Tooltip("Pouvoir jaune (portals)")]
    public bool yellow = false;

    [FoldoutGroup("power map")]
    [Tooltip("Pouvoir green (flaire)")]
    public bool green = false;

    /// <summary>
    /// power if coop
    /// </summary>
    [FoldoutGroup("power player 1 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower0", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir blanc (pas de pouvoirs)")]
    public bool white1 = false;

    [FoldoutGroup("power player 1 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower1", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir bleu (repulse)")]
    public bool blue1 = false;

    [FoldoutGroup("power player 1 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower2", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir red (attire)")]
    public bool red1 = false;

    [FoldoutGroup("power player 1 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower3", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir jaune (portals)")]
    public bool yellow1 = false;

    [FoldoutGroup("power player 1 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower4", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir green (flaire)")]
    public bool green1 = false;


    [FoldoutGroup("power player 2 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower0", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir blanc (pas de pouvoirs)")]
    public bool white2 = false;

    [FoldoutGroup("power player 2 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower1", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir bleu (repulse)")]
    public bool blue2 = false;

    [FoldoutGroup("power player 2 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower2", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir red (attire)")]
    public bool red2 = false;

    [FoldoutGroup("power player 2 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower3", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir jaune (portals)")]
    public bool yellow2 = false;

    [FoldoutGroup("power player 2 (coop)")]
    [EnableIf("coop")]
    [ValidateInput("ValidatePower4", "this power isn't in the power list of the map !", InfoMessageType.Error)]
    [Tooltip("Pouvoir green (flaire)")]
    public bool green2 = false;


    [FoldoutGroup("main info")]
    [Tooltip("Nom Maximum d'oeuf à donner à la reine")]
    public int maxEggs = 20;

    [FoldoutGroup("main info")]
    [Tooltip("Le temps max pour finir le niveau")]
    public int timeMax = 180;

    [FoldoutGroup("main info")]
    [Tooltip("Le temps de respawn des joueurs quand ils meurent")]
    public int timeRespawn = 20;

    [FoldoutGroup("don't touch")]
    [Tooltip("List des hight score du level")]
    public List<int> timeHighScore;

    [FoldoutGroup("main info")]
    [Tooltip("Les temps à effectuer pour gagner, dans l'ordre de haut en bas: argent, or, epic")]
    public List<int> timeEggs2to4;

    [FoldoutGroup("main info")]
    [Tooltip("Quel est l'amount de succès qu'il faut avoir sur la map précédente ? (argent (1) ? or (2) ? epic (3) ?)")]
    public int succesNeededForUnloclk = 1;

    [FoldoutGroup("don't touch")]
    [Tooltip("est-ce la dernière map du monde ?")]
    public bool lastLevelMap = false;

    [FoldoutGroup("don't touch")]
    [Tooltip("est-ce que c'est la dernière map du jeu ?")]
    public bool lastMap = false;

    /// <summary>
    /// valide ou non les pouvoirs de coop (les pouvoirs des joueurs 1 et 2 doivent faire partie
    /// des pouvoirs des maps)
    /// </summary>
    private bool ValidatePower0(bool power)
    {
        if (!white && power && coop)
            return (false);
        return (true);
    }
    private bool ValidatePower1(bool power)
    {
        if (!blue && power && coop)
            return (false);
        return (true);
    }
    private bool ValidatePower2(bool power)
    {
        if (!red && power && coop)
            return (false);
        return (true);
    }
    private bool ValidatePower3(bool power)
    {
        if (!yellow && power && coop)
            return (false);
        return (true);
    }
    private bool ValidatePower4(bool power)
    {
        if (!green && power && coop)
            return (false);
        return (true);
    }
    //setup la coop
    private bool coopEnabled(bool coop)
    {
        if (coop && ( (white && !white1 && !white2)
            || (blue && !blue1 && !blue2)
            || (red && !red1 && !red2)
            || (yellow && !yellow1 && !yellow2)
            || (green && !green1 && !green2)))
            return (false);
        return (true);
    }

}
