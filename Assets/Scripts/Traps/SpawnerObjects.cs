using UnityEngine;
using System.Collections;

public class SpawnerObjects : MonoBehaviour
{
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;           //Optimisation des fps

    [Tooltip("Le tableau contenan tout les objets à spawn, au hasard")]
    public GameObject[] hazard;

    [Tooltip("position dévié aléatoirement en X")]
    public float xDeviation = 0f;

    [Tooltip("position dévié aléatoirement en Y")]
    public float yDeviation = 0f;

    [Tooltip("position de spawn")]
    public Transform spawnValues;

    [Tooltip("Combien d'objet par waves ?")]
    public int hazardCount;

    [Tooltip("temps d'attendre entre chaque éléments dans la wave")]
    public float spawnWait;

    [Tooltip("temps d'attendre avant la première waves")]
    public float startWait;

    [Tooltip("temps d'attendre entre chaque waves")]
    public float waveWait;

    [Tooltip("est-ce qu'on rotate les objets ?")]
    public bool setRotation = true;

    [Tooltip("est-ce qu'on ajoute une extra force aux ojets ?")]
    public bool setExtraForce = false;

    [Tooltip("active lorsqu'on est hors de l'écran ?")]
    public bool desactiveOutOfScreen = false;


    [Tooltip("Le taux de random dans les variables de temps")]
    [Range(1, 10f)] public int randomize;

    private float timeToGo;                         //Variable d'optimisation
    private DrawSolidArc drawSolidArc;                                      //ref du script DrawSolidArc attaché au gameObject
    private IsOnScreen IOS;

    // Use this for initialization
    private void Awake()
    {
                                                        //remplace tout les spawner des oeufs par des vrais oeufs
    }


    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        drawSolidArc = gameObject.GetComponent<DrawSolidArc>();
        IOS = gameObject.GetComponent<IsOnScreen>();
        StartCoroutine(SpawnWaves());
    }

    /// <summary>
    /// création de l'objet en question
    /// </summary>
    void createObject()
    {
        if (!IOS.isOnScreen && desactiveOutOfScreen)
            return;
        //get prefabs from list at random
        GameObject prefabsFromList = hazard[Random.Range(0, hazard.Length)];

        //change position
        Vector3 pos = spawnValues.position;
        pos += gameObject.transform.up * Random.Range(-xDeviation, xDeviation);
        pos += gameObject.transform.right * Random.Range(-yDeviation, yDeviation);

        //change rotation
        Quaternion spawnRotation = prefabsFromList.transform.rotation;                                             //défini la rotation de l'objet
        if (setRotation)                                                                           //si drawSolidArc est présent
            spawnRotation = gameObject.transform.rotation;                                          //change la rotation de l'objet créé à la rotation courante du spawner

        GameObject tmpObject = Instantiate(prefabsFromList, pos, spawnRotation, gameObject.transform) as GameObject;   //créé l'objet à la bonne position et rotation
        if (drawSolidArc && setExtraForce)                                                                           //si drawsolidarc est présent
        {
            Rigidbody tmpRb = tmpObject.GetComponent<Rigidbody>();                                  //si le Rigidbody de l'objet existe...
            if (!tmpRb)
                return;
            tmpRb.AddForce(tmpObject.transform.up * drawSolidArc.strengthOfAttraction * 30);        //ajoute une force à l'objet

        }
        //défini la vitesse initiale de l'objet
    }

    /// <summary>
    /// gestion des vagues d'objets à lancer
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait - (startWait / Random.Range(1, randomize)) + (startWait / Random.Range(1, randomize)));
        while (true)
        {
            int hazardCountRandomize = hazardCount - (hazardCount / Random.Range(1, randomize)) + (hazardCount / Random.Range(1, randomize));
            for (int i = 0; i < hazardCountRandomize; i++)
            {
                if (hazard.Length > 0)
                    createObject();
                yield return new WaitForSeconds(spawnWait - (spawnWait / Random.Range(1, randomize)) + (spawnWait / Random.Range(1, randomize)));
            }
            yield return new WaitForSeconds(waveWait - (waveWait / Random.Range(1, randomize)) + (waveWait / Random.Range(1, randomize)));
        }
    }


    // Update is called once per frame
    void Update ()
    {
        if (Time.fixedTime >= timeToGo)
        {

            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
