using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LaserSource : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Header("Object particules")]
    public GameObject beamStart;
    public GameObject beamEnd;
    public GameObject beam;

    [Header("Adjustable Variables")]
    public float laserTime = 3.0f;
    public float timeBetweenLaser = 1.0f;
    public bool continiue = false;
    public float maxDistance = 1000f;
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    [Space(10)]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
    public float textureLengthScale = 3; //Length of the beam texture
    private float timeToGo;

    //private int layerMaskMetallica = (1 << 8);                                         //select layer 8 (metallica, players and colider)
    //private int layerMaskPortals = (1 << 11);                                         //select layer 11 (portalColliders)


    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;
    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private LineRenderer line;
    private TimeWithNoEffect TWNE;                                          //temps entre 2 tir
    private bool fire = false;

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
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        timeToGo = Time.fixedTime + timeOpti;
    }
    #endregion

    #region core script

    /// <summary>
    /// initialise les lasers
    /// </summary>
    void initLaser()
    {
        beamStart.SetActive(true);
        beamEnd.SetActive(true);
        beam.SetActive(true);
        fire = true;
        line = beam.GetComponent<LineRenderer>();
        if (!continiue)
            StartCoroutine(stopLaserWhenFinish());                        //démare la boucle fireLaser;
    }

    /// <summary>
    /// attend laserTime seconde, puis désacrtive le laser !
    /// </summary>
    /// <returns></returns>
    IEnumerator stopLaserWhenFinish()
    {
        yield return new WaitForSeconds(laserTime);
        beamStart.SetActive(false);
        beamEnd.SetActive(false);
        beam.SetActive(false);
        fire = false;
    }

    /// <summary>
    /// tir continuellement un laser avec un raycast !
    /// </summary>
    void FireLaser()
    {
        //Vector2 tdir2d = Vector2.zero;
        Vector2 endPoint1 = Vector2.zero;
        //Vector3 endPoint2 = Vector3.zero;

        var layerMask = ~((1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Lock")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) );

        /*RaycastHit2D hit2d = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, maxDistance, layerMask);
        if (hit2d)
        {
            if (hit2d.collider.gameObject.tag == "Trail")
            {
                tdir2d = hit2d.point - new Vector2(transform.position.x, transform.position.y);
                endPoint1 = new Vector3(hit2d.point.x, hit2d.point.y, 0);
                //Debug.DrawLine(gameObject.transform.position, endPoint1, Color.red, 1.0f);
            }
        }*/
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.up, out hit, maxDistance, layerMask))
            endPoint1 = hit.point;

        //si aucun raycast ne fonctionne pas...
        if (endPoint1 == Vector2.zero)
            return;

        /*if (hit.collider.gameObject.CompareTag("Player"))
        {

        }*/

        ShootBeamInDir(gameObject.transform.position, endPoint1);
    }

    /// <summary>
    /// tir un projectile
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    void ShootBeamInDir(Vector2 start, Vector2 end)
    {
        //line.SetVertexCount(2);
        line.positionCount = 2;

        line.SetPosition(0, start);
        beamStart.transform.position = start;

        beamEnd.transform.position = end;
        line.SetPosition(1, end);

        beamStart.transform.LookAt(beamEnd.transform.position);
        beamEnd.transform.LookAt(beamStart.transform.position);

        float distance = Vector2.Distance(start, end);
        line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
        line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
    }

    #endregion

    #region unity fonction and ending
    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            /////////////////////////tir un laser quand le temsp est fini ou si c'est au début !
            if (TWNE.isOk)
            {
                TWNE.timeWithNoEffect = laserTime + timeBetweenLaser;
                initLaser();                        //initialise les lasers
                TWNE.isOk = false;
            }
            if (fire)
                FireLaser();            //tir un laser
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
    #endregion
}
