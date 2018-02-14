using UnityEngine;
using System.Collections.Generic;

public class SpawnerEggs : MonoBehaviour
{
    private GameObject SpawnerEgg;
    public GameObject groupPoolEggs;                            //le groupe de la pool des oeufs
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;

    /// <summary>
    /// private
    /// </summary>
    private TimeWithNoEffect TWNE;
    private float timeToGo;
    private GameObject groupEggs;
    private GameManager GM;

    [SerializeField] private GameObject eggsPrefabs;                                //prefabs de l'oeuf

    // Use this for initialization
    private void Awake()
    {
        //groupEggs = GameObject.FindGameObjectWithTag("GroupEggs");                  //récupère le groups des oeufs
        GM = gameObject.GetComponent<GameManager>();                                //récupère le gameMaanger
        groupEggs = GM.groupEggs;
        SpawnerEgg = GM.IG.levelDataScript.SpawnEggs;                               //get le spawnderEggs
        TWNE = SpawnerEgg.GetComponent<TimeWithNoEffect>();                        //récupère le script du temps
        replaceFalseEggs();                                                         //remplace tout les spawner des oeufs par des vrais oeufs
    }


    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }

    /// <summary>
    /// remplace tout les spawner des oeufs par des vrais oeufs
    /// </summary>
    void replaceFalseEggs()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < groupEggs.transform.childCount; i++)
        {
            positions.Add(groupEggs.transform.GetChild(i).transform.position);
            Destroy(groupEggs.transform.GetChild(i).gameObject);
            //Debug.Log(positions[i]);
        }
        for (int i = 0; i < positions.Count; i++)
        {
            createEggs(positions[i]);
        }

    }

    /// <summary>
    /// cree un nouvelle oeuf
    /// - si Transform eggsInactive, active le premier oeuf de la pool
    /// </summary>
    void createEggs()
    {
        GameObject instanceNewEgg = Instantiate(eggsPrefabs);
        instanceNewEgg.transform.SetParent(groupEggs.transform);                //change son parent, pour mettre l'oeuf avec les autres dans le groupEggs
        instanceNewEgg.transform.position = SpawnerEgg.transform.position;     //changer sa position à la position du spawner 
    }
    void createEggs(Vector3 position)
    {
        GameObject instanceNewEgg = Instantiate(eggsPrefabs);
        instanceNewEgg.transform.SetParent(groupEggs.transform);                //change son parent, pour mettre l'oeuf avec les autres dans le groupEggs
        instanceNewEgg.transform.position = position;     //changer sa position à la position du spawner 
    }
    void createEggs(Transform eggsInactive)
    {
        eggsInactive.SetParent(groupEggs.transform);                            //change son parent, pour mettre l'oeuf avec les autres dans le groupEggs
        eggsInactive.transform.position = SpawnerEgg.transform.position;       //changer sa position à la position du spawner
        eggsInactive.gameObject.SetActive(true);                                //active l'oeufs !
        eggsInactive.GetComponent<SphereCollider>().enabled = true;             //réactive son collider !
    }

    /// <summary>
    /// S'il n'y a pas assez d'oeuf restant pour "remplir" la reine,
    /// Spawner un nouvel oeuf toutes les X secondes
    /// </summary>
    void testForSpawn()
    {
        int countEggs = groupEggs.transform.childCount/* - GM.getAllKine()*/;                      //récupère le nombre d'oeuf actuel, en enlevant ceux qui sont kinetic
        if ((!GM.multi && countEggs < GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().max - GM.ScoreProgressBarSolo.GetComponent<ColoredProgressBar>().getProgress())
            || (GM.multi && countEggs < ((GM.ScorePorgressBarTeam1.GetComponent<ColoredProgressBar>().max + GM.ScorePorgressBarTeam2.GetComponent<ColoredProgressBar>().max) * 0.6f)))                  //si les oeuf restant sont inférieur aux oeuf total à donner (pour le mode solo ET multi...)
        {
            if (TWNE && TWNE.isOk)                                                          //si isOk est vrai...
            {
                TWNE.isOk = false;                                                           //le mettre, à faux, il va se remettre à vrai dans X seconde !
                if (groupPoolEggs.transform.childCount > 0)                                  //si il y a des oeufs inactifs dans la pool, les utilisers !
                {
                    Transform eggsInactive = groupPoolEggs.transform.GetChild(0);           //récupère le premier enfant
                    if (!eggsInactive.gameObject.GetComponent<EggsController>().dead[(int)eggsInactive.gameObject.GetComponent<EggsController>().SOE].GetComponent<ParticleSystem>().IsAlive())
                        //if (eggsInactive.gameObject.GetComponent<EggsController>().TWNE.isOk)   //le réactive seulement si il n'a pas été mort y'a pas longtemps !
                        createEggs(eggsInactive);
                    else                                                                    //sinon, on créé un nouvel oeuf simplement
                        createEggs();
                }
                else                                                                //sinon, il faut créé un nouvelle oeuf !
                    createEggs();
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.fixedTime >= timeToGo)
        {
            testForSpawn();                                             //check si il faut créé un nouvell oeuf
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
