using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class Pushable : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Range(0, 100)] public int factorMultiply = 3;                     //coef de multiplication sur les objets Pushable
    public float forceToBreakEggs = 1000.0f;                               //force à laquelle les oeufs se brise au contact du rocher
    public float forceToBreakPlayer = 500.0f;                               //force à laquelle les oeufs se brise au contact du rocher
    [Range(0, 10f)]             public float timeOpti = 0.1f;
    public bool isControlled = false;                   //défini si le caillous est poussé/attiré ou pas
    public float timeBeforeDie = 1.0f;                  //temps avant de mourir
    public float speedAnim = 5f;                        //vitesse de scale down de l'objet
    [Range(1.0f, 100.0f)]           public float maxVelocity = 10.0f;   //velocity max de l'objet

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;
    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private float timeToGo;
    private bool isInCoroutine = false;
    private bool active = true;
    private Vector3 targetScale;
    private Rigidbody Rb;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [SerializeField] public List<GameObject> particleList;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        Rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        timeToGo = Time.fixedTime + timeOpti;
        targetScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    #endregion

    #region core script

    public void setControl(bool active, int type)
    {
        particleList[type].SetActive(active);
        isControlled = active;
        if ((type == 2 || type == 3) && active && !isInCoroutine)
        {
            StopCoroutine("testGreenControl");
            StartCoroutine(testGreenControl(1f, type));
        }
    }

    public IEnumerator testGreenControl(float timer, int type)
    {
        isInCoroutine = true;
        yield return new WaitForSeconds(timer);
        isInCoroutine = false;
        setControl(false, type);
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// détecte la colision avec un oeuf ou un joueur, si elle est assez puissante, détruite l'oeuf/joueur !
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (!active)
            return;
        if (collision.gameObject.CompareTag("Eggs") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Destructible")/* && collision.relativeVelocity.magnitude * gameObject.GetComponent<Rigidbody>().mass > forceToBreakEggs*/)
        {
            float velocityBoth = collision.relativeVelocity.magnitude;                                                                                    //vélocité de rocher + de l'objet cible
            float forcesOeuf = 0.5f * gameObject.GetComponent<Rigidbody>().mass * Mathf.Pow(velocityBoth, 2);                                             //pour les oeufs: masse du rocher * (vitesse rocher + vitesse oeuf)
            float forceAnts = 0.5f * gameObject.GetComponent<Rigidbody>().mass * Mathf.Pow(gameObject.GetComponent<Rigidbody>().velocity.magnitude, 2);   //pour le joueur: masse du rocher * vitesse du rocher (on ne prend pas en compte la vitesse du joueur, pour eviter que le joueur se "cogne" en fonçant dessus)

            //si l'objet cible est un oeuf et que la force d'impact est assez haute, détruite l'oeuf
            if (collision.gameObject.CompareTag("Eggs") && forcesOeuf > forceToBreakEggs)
            {
                EggsController EC = collision.gameObject.GetComponent<EggsController>();
                if (EC)
                {
                    Debug.Log("pushable  ?");
                    EC.destroyThis();
                }

            }
            else if (collision.gameObject.CompareTag("Destructible"))
            {
                DestructibleWalls DW = collision.gameObject.GetComponent<DestructibleWalls>();
                if (DW)
                {
                    if ((!DW.autoDestroy && forcesOeuf > forceToBreakEggs) || DW.autoDestroy)
                        DW.destroyThis();
                }
                    
            }
            //si l'objet cible est un player et que la force d'impact est assez haute...
            else if (collision.gameObject.CompareTag("Player") && forceAnts > forceToBreakPlayer)
            {
                Debug.Log("ici supper collision player");

                //vérifier entre la direction du vecteur directeur Player - rocher et le vercteur directeur vitesse du rocher (selon les axes -X, X, -Y, Y)
                Vector3 dirRockToAnts = collision.gameObject.transform.position - gameObject.transform.position;            //vecteur directeur Rocher - fourmis
                Vector3 dirVelocity = gameObject.GetComponent<Rigidbody>().velocity;                                        //vecteur directeur vitesse du rocher

                if ((dirRockToAnts.x < 0 && dirVelocity.x < 0)
                    || (dirRockToAnts.x > 0 && dirVelocity.x > 0)
                    || (dirRockToAnts.x == 0 && dirVelocity.y > 0 && dirRockToAnts.y > 0)
                    || (dirRockToAnts.x == 0 && dirVelocity.y < 0 && dirRockToAnts.y < 0) )
                {
                    PlayerController PC = collision.gameObject.GetComponent<PlayerController>();
                    if (PC)
                    {
                        PC.destroyThis();
                    }
                }
            }
        }
    }

    private IEnumerator waitBeforeDie(float time)
    {
        //
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    /// <summary>
    /// détruit l'objet courrant
    /// </summary>
    public void destroyThis()
    {
        if (!active)
            return;
        active = false;
        //Anim.enabled = true;
        //Anim.Play("delete");
        if (particleList.Count >= 5)
            particleList[4].SetActive(true);
        else
        {
            //les stalactite n'on pas de particule de défaite, il en créé une !
            gameObject.GetComponent<Stalactite>().destroyThis();
        }
        StartCoroutine(waitBeforeDie(timeBeforeDie));
    }

    private void testVelocityMax()
    {
        Rb.velocity = Vector3.ClampMagnitude(Rb.velocity, maxVelocity);
    }

    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()                                                   //update
    {
        if (!active)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speedAnim * Time.deltaTime);
        if (Time.fixedTime >= timeToGo)
        {
            testVelocityMax();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    #endregion
}
