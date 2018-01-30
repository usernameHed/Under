using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    public GameObject[] impactParticlePrefabs;
    public GameObject[] projectileParticleprefabs;
    public GameObject[] muzzleParticlePrefabs;
    public int typeColor = 0;                       //0: yellow, 1: orange, 2: blue, 3: red

    private GameObject impactParticle;
    private GameObject projectileParticle;
    private GameObject muzzleParticle;
    [SerializeField] private TimeWithNoEffect TWNEchild;
    [SerializeField] private FmodEventEmitter emitterScript;          //l'emmiter pour tween

    public GameObject[] trailParticles;
    

    [HideInInspector]    public Vector3 impactNormal; //Used to rotate impactparticle.
    [HideInInspector]    public FirePortals refPlayer;

    [HideInInspector]   public TimeWithEffectOnCamera TWNEOC;

    /// <summary>
    /// private 
    /// </summary>
    public bool hasCollided = false;

    private void Awake()
    {
        TWNEOC = gameObject.GetComponent<TimeWithEffectOnCamera>();
        if (!emitterScript)
            Debug.LogError("nop");
    }

    /// <summary>
    /// start
    /// </summary>
    void Start()
    {
        //initParticle();
    }

    /// <summary>
    /// initialise à l'activation les particule anexe.
    /// </summary>
    void initParticle()
    {
        //CancelInvoke("DestroyMe");      //ne supprime plus les projectiles d'avant !
        hasCollided = false;            //on reset, ducoup on n'a pas encore collider !
        TWNEOC.isOk = false;
        TWNEOC.alwaysOnCamera = true;

        projectileParticle = Instantiate(projectileParticleprefabs[typeColor], transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticlePrefabs[typeColor], transform.position, transform.rotation) as GameObject;
            muzzleParticle.transform.rotation = transform.rotation * Quaternion.Euler(180, 0, 0);
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
        }
    }

    /// <summary>
    /// lorsque 
    /// </summary>
    private void OnEnable()
    {
        initParticle();
    }

    /// <summary>
    /// détruit le projectil sans création de portals !
    /// </summary>
    public void destroyThis()
    {
        impactParticle = Instantiate(impactParticlePrefabs[typeColor], transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;

        SoundManager.SS.playSound(emitterScript);

        foreach (GameObject trail in trailParticles)
        {
            if (projectileParticle)
            {
                Transform tmpFind = transform.Find(projectileParticle.name + "/" + trail.name);
                if (tmpFind)
                {
                    GameObject curTrail = tmpFind.gameObject;
                    curTrail.transform.parent = null;
                    Destroy(curTrail, 3f);
                }
            }
        }
        //StartCoroutine(DestroyMe(projectileParticle, 3.0f));
        //StartCoroutine(DestroyMe(impactParticle, 5.0f));

        refPlayer.desactivePorjectile();
        refPlayer.CancelInvoke("stopProjectile");
    }

    IEnumerator DestroyMe(GameObject toDestroy, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(toDestroy);
    }

    /// <summary>
    /// gère les collisions 
    /// </summary>
    /// <param name="hit"></param>
    void OnCollisionEnter(Collision hit)
    {
        if (hasCollided)
            return;

        if (hit.collider.CompareTag("ParticleCollider") && !TWNEchild.isOk)
            return;

        //gère les collisions collider - collider avec un trail !
        if (hit.collider.CompareTag("Trail") || hit.collider.CompareTag("EvilParticle")
            || hit.collider.CompareTag("ParticleCollider") || hit.collider.CompareTag("Destructible"))
        {
            hasCollided = true;         //on entre en collision !
            destroyThis();
        }
        else if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Portal"))
        {
            hasCollided = true;         //on entre en collision !
                                        //transform.DetachChildren();
                                        //ContactPoint contact = hit.contacts[0];
            refPlayer.projectileHit(gameObject.transform.position, hit.gameObject, hit.contacts[0].normal);

            //Debug.Log("create portal !!!!");


            destroyThis();
        }
    }
}