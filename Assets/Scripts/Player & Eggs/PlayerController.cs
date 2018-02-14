using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipU1NkSzJiU0U1am8%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipU1NkSzJiU0U1am8
see:
https://drive.google.com/file/d/0Byzet-SVq6ipZ2ZHaV9CODF4OW8/view
*/


/// <summary>
/// Chaque joueur à un identifiant:
/// nb_player.. Sa porté / angle de tir / force sont calibré par rapport au script DrawSolidArc,
///     script se trouvant aussi sur le joueur, le but étant de pouvoir visualiser la range et l'angle dans l'éditeur de Unity, tout en ayant un script séparé qui gère ça.
/// Le joueur une fois initialisé "s'ajoute" dans les listes de la caméra et du gameController(pour que ceux-ci puisse influer dessus).
/// Il récupère aussi les variables des états des mannettes de XinputGamePad.
/// 
///Le joueur peut se déplacer.
///Si il active sa force, la variable releaseEggsChild change.
///     Selon cette valeur, une force est appliqué ou non sur la liste des Eggs dans la range.
///Il peut décider aussi de Colorier les Eggs.
/// </summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [Header("Main")]
    public int nb_player = -1;                                                      //ID du joueur (0 à 5) -> controller
    public int nb_team = 1;                                                         //ID de la team (1 ou 2)
    public int indexSolo = 0;                                                       //index du joueur en mode solo (0 ou 1);
    public enum TypeOfPlayer                                                        //type de pouvoir
    {
        White = 0,
        Repulsor = 1,
        Attractor = 2,
        BlackHole = 3,
        Flaire = 4,
    }
    public TypeOfPlayer TP = TypeOfPlayer.Attractor;
    [Range(0, 100000)]
    public float speed = 6.0f;                              //vitesse de la fourmis
    [Range(300, 1500)]
    public float turnRate = 600f;                               //vitesse de rotation
    public bool useRaycast = true;                                                  //utilise ou non un raycast (pouvoir passe à travers les murs)
    public bool isCloseToWalls = false;                                             //le joueur est proche d'un mur
    [Range(0, 200)]
    public float repulsionWhenCollide = 20.0f;              //répulsion des oeufs
    public bool isWalkingOnSnow = false;
    public bool isHoldingY = false;                                                 //holding Y
    public bool isHoldingX = false;

    [Space(10)]
    [FoldoutGroup("Only for blue & red")]
    [Range(0.01f, 10)]
    public float additionForcingPlayer = 1;                     //force multiplié aux autres joueurs
    [FoldoutGroup("Only for blue & red")]
    [Range(0, 10.0f)]
    public float slowWhenFiring = 1f;                           //valeur à soustraire à la vitesse de la fourmis quand il contrôle plein d'oeuf

    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(0, 200)]
    public int itterationWhenPressed = 10;
    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(0, 2)]
    public float timeBetweenItterationWhenPressed = 0.1f;
    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(-300, 300)]
    public float strengthOfAttractionWhenPressed = 20.0f;
    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(0, 0.5f)]
    public float weakeningOverTime = 0.05f;
    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(-1, 300f)]
    public float rangeDistance = 30.0f;
    [FoldoutGroup("Control over distance (for blue & red)")]
    public bool projectAtTheEnd = false;
    [FoldoutGroup("Control over distance (for blue & red)")]
    [Range(-300, 300)]
    public float coefWhenStop = 0f;                   //coef appelé dans letalController > testIfBlueControlled

    [Space(10)]
    [FoldoutGroup("Only for yellow")]
    [Range(0, 2)]
    public int projectileState = 0;

    [Space(10)]
    [FoldoutGroup("Only for green")]
    public float speedFlaire = 10.0f;


    [FoldoutGroup("Freez variable")]
    [Range(0, 100000)]
    public float slowWhenHitAmount = 2.0f;
    [FoldoutGroup("Freez variable")]
    [Range(0, 10)]
    public float slowWhenHitTiming = 2.0f;
    [FoldoutGroup("Freez variable")]
    public bool isSlow = false;
    [FoldoutGroup("Freez variable")]
    public bool asAlreadyMove = false;
    [FoldoutGroup("Freez variable")]
    private Vector3 lastMove = new Vector3(0, 0, 0);
    [FoldoutGroup("Freez variable")]
    public float amountSpeedOfFreez = 2f;
    [FoldoutGroup("Freez variable")]
    public float amountSpeedWhenInSnow = 2f;
    [FoldoutGroup("Freez variable")]
    public float timeRotationSpeed = 2f;
    [FoldoutGroup("Freez variable")]
    private Vector3 velocityFreez = Vector3.zero;
    [FoldoutGroup("Freez variable")]
    public float gravityScale = -300f;


    [FoldoutGroup("Gaz variable")]
    [Range(0, 100000)]
    public float slowWhenHitGazAmount = 10.0f;
    [FoldoutGroup("Gaz variable")]
    [Range(0, 10)]
    public float slowWhenHitGazTiming = 2.0f;               //l'effet de aprticule de gaz
    [FoldoutGroup("Gaz variable")]
    public int numberOfOtorizedGaz = 7;                     //nombre de fois où on peut toucher le gaz avant de mourir
    [FoldoutGroup("Gaz variable")]
    public bool isAlreadySlowedByGaz = false;
    [FoldoutGroup("Gaz variable")]
    public int numberOfTimzGazed = 0;                       //nombre de fois où il a été gazé



    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1.0f)]
    public float vibrateLeftFiring = 0.5f;
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1000)]
    public int LeftFiringTiming = 30;
    [Space(10)]
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1.0f)]
    public float vibrateRightPushed = 0.5f;
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1000)]
    public int RightFiringTiming = 3;
    [Space(10)]
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1.0f)]
    public float vibrateBothDying = 1.0f;
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1000)]
    public int BothDyingTiming = 6;
    [Space(10)]
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1.0f)]
    public float vibrateRightWhenSlow = 1.0f;
    [FoldoutGroup("GamePad Vibration")]
    [Range(0, 1000)]
    public int BothSlowTiming = 4;




    [Space(10)]
    [Header("Debug")]
    public Transform pointCam;
    public Transform pointCamSmooth;
    public List<GameObject> eggsArray = new List<GameObject>();                 //list d'oeufs controllé
    public List<GameObject> pushableArray = new List<GameObject>();             //list de pushable controllé
    public List<ZoneEffector> effectorArray = new List<ZoneEffector>();             //list de pushable controllé

    [Range(0, 0.1f)]
    public float timeOpti = 0.1f;                       //optimisation des fps
    public GameObject particle;
    public GameObject Boom;
    public GameObject SpriteRend;
    public GameObject SlowParticle;
    public GameObject SlowParticleGaz;
    public GameObject SlowParticleGazStay;
    public List<GameObject> ColliderWalls = new List<GameObject>();         //liste de coloderWalls de chaque joueurs
    public Light lightPlayer;                                               //lumière sur le joueur
    public GameObject OPicon;                                               //l'op de l'icon du pouvoir courant du joueur
    public FirePortals FP;                       //script de tire de portals
    public listEmitter listEmitt; //la list des emmiteur du joueur

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    [HideInInspector]
    public float oldSpeed;                                                 //sauvegarde la old speed;
    [HideInInspector]
    public GameObject avatarPlayer;                     //avatar du player en GUI
    [HideInInspector]
    public GameObject avatarPlayerRespawn;                     //avatar du player en GUI
    [HideInInspector]
    public bool releaseMetalicaChild = true;            //permet de savoir si le joueur manitient son pouvoir ou pas

    [HideInInspector]
    public bool active = true;                          //activation des fourmis
    [HideInInspector]
    public bool activeControl = true;                   //activation des controls
    [HideInInspector]
    public GameObject groupPointCam;                  //groupe des points de caméra des joueurs
    [HideInInspector]
    public bool winnedIntern = false;                 //as-ton-gagné ?

    
    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private float fovRange;
    private float fovAngle;
    private float strengthOfAttraction;
    private GameObject gameController;                                      //référence du gameController
    [HideInInspector]
    public GameManager gameManager;                     //ref du script gameManager
    private SwitchAnts SA;                                                  //red du script pour switch de ants
    private GameObject variableGlobal;                                      //référence sur les data global
    private GameObject mainCamera;                                          //reference de la mainCamera
    private CameraController cameraController;                              //ref du script cameraController
    private GameObject groupPlayer;                                         //rérérence sur le parent groupPlayer
    private Rigidbody Rb;                                                   //Utilisé dans EggsCOntroller pour récupérer la mass
    private DrawSolidArc drawSolidArc;                                      //ref du script DrawSolidArc attaché au gameObject
    private Animator anim;                                                  //animator du joueur
    private ObjectiveIndicator OP;                                          //OP du joueur
    //private FirePortals FP;                                                 //firePortal du joueur (s'il y en a)
    private float timeToGo;                                                 //optimisation
    private int layerType = 8;
    private int layerMask = 1 << 8;                                         //select layer 8 (metallica, players and colider)
    private bool deadAnts = false;
    private PlayerConnected PC;                                             //ref des manettes connecté
    private bool isSlowedByGaz = false;                                     //détermine si le joueur est ralenti par le gaz

    private Quaternion _lookRotation;
    private Vector3 _direction;
    private bool firstWalking = false;                                           //défini si le joueur est en train de marcher ou non
    private float previousHeading = 0;                                      //save la prefious heading

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [Space(10)]
    [Header("private serialized field")]
    [SerializeField]    private TrailController trailController;               //script de création de trails
    
    #endregion

    #region  initialisation
    /// <summary>
    /// initialise
    /// </summary>
    private void Awake()
    {
        if (Camera.main)                                                        //s'il n'y a pas de main camera dans la scène, on a un gros problème !
        {
            mainCamera = Camera.main.gameObject;
            cameraController = mainCamera.GetComponent<CameraController>();
        }
        groupPlayer = GameObject.FindGameObjectWithTag("GroupPlayer");          //save l'objet GroupPlayer du niveau
        gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController)                                                     //s'il n'y a pas de gameController, gros probleme !
        {
            gameManager = gameController.GetComponent<GameManager>();
            SA = gameController.GetComponent<SwitchAnts>();
        }

        variableGlobal = GameObject.FindGameObjectWithTag("Global");
        if (variableGlobal)
            PC = variableGlobal.GetComponent<PlayerConnected>();
        drawSolidArc = gameObject.GetComponent<DrawSolidArc>();
        Rb = GetComponent<Rigidbody>();
        anim = SpriteRend.GetComponent<Animator>();
        OP = gameObject.GetComponent<ObjectiveIndicator>();

        effectorArray.Clear();
        //if (TP == TypeOfPlayer.BlackHole)
        //  FP.initPortal();
        //FP = gameObject.transform.GetChild(0).gameObject.GetComponent<FirePortals>();
        //cc = GetComponent<CharacterController>();                                               //get the character controller
        //setRefToOther();                                                        //envoi les références aux tableau
        if (!listEmitt)
            Debug.LogError("nop");
        else
        {
            listEmitt.setupAdditiveName(nb_player.ToString());
        }
    }

    /// <summary>
    /// start
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        oldSpeed = speed;
        previousHeading = 0;
        getPublicVariable();                                    //get les variables du sccript drawSolidArc
        ResetPosPointCam(true);
    }
    #endregion

    #region core script
    /// <summary>
    /// Get le rigidBody du joueur
    /// </summary>
    public Rigidbody getRb()
    {
        return (Rb);
    }

    public AnimationClip GetAnimationClipFromAnimatorByName(Animator animator, string name)
    {
        //can't get data if no animator
        if (animator == null)
            return null;

        //favor for above foreach due to performance issues
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            Debug.Log(animator.runtimeAnimatorController.animationClips[i].name);
            if (animator.runtimeAnimatorController.animationClips[i].name == name)
                return animator.runtimeAnimatorController.animationClips[i];
        }

        Debug.LogError("Animation clip: " + name + " not found");
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="win"></param>
    public void winned(bool win)
    {
        winnedIntern = win;
        if (winnedIntern)
        {
            anim.SetBool("isWalking", false);                //lance l'animation de marche
            //GetAnimationClipFromAnimatorByName(anim, "Idle").wrapMode = WrapMode.Loop;
            anim.Play("Tweening");
        }
        else
            anim.Play("Nothing");
        restrictVelocity();
        Rb.isKinematic = true;
        this.enabled = false;
    }

    /// <summary>
    /// récupère le type de pouvoir (1 à 4) du joueur
    /// </summary>
    /// <returns></returns>
    public int getTypePowerPlayer()
    {
        return ((int)TP);
    }

    void findColliderWallsInObject(GameObject player)
    {
        for (int k = 0; k < player.transform.childCount; k++)
        {
            if (player.transform.GetChild(k).gameObject.tag == "ColliderWalls")
            {
                ColliderWalls.Add(player.transform.GetChild(k).gameObject);
                break;
            }
        }
    }








    ///////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Ajouter la zone à la list (si elle n'est pas déjà dedans)
    /// </summary>
    /// <param name="zone"></param>
    public void addZone(ZoneEffector zone)
    {
        if (effectorArray.IndexOf(zone) < 0)
            effectorArray.Add(zone);
    }

    /// <summary>
    /// ici supprime la zone de la liste (trouver l'élément, et le supprimer)
    /// </summary>
    /// <param name="zone"></param>
    public void deleteZone(ZoneEffector zone)
    {
        for (int i = 0; i < effectorArray.Count; i++)
        {
            if (zone == effectorArray[i])
            {
                effectorArray.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// retourne la liste courante du player
    /// </summary>
    /// <returns></returns>
    public List<ZoneEffector> getListZoneEffector()
    {
        return (effectorArray);
    }

    /// <summary>
    /// ici clear la liste
    /// </summary>
    public void zoneCamClear()
    {
        effectorArray.Clear();
    }

    /// <summary>
    /// ici set la list effector
    /// </summary>
    /// <param name="listZone"></param>
    public void setListZoneEffector(List<ZoneEffector> listZone)
    {
        effectorArray = listZone;
    }

    /// <summary>
    /// ici on est mort, JUSTE avant de désactiver le player (le spawner est déjà en court...)
    /// </summary>
    private void resetZoneEffector()
    {
        for (int i = 0; i < effectorArray.Count; i++)
        {
            if (!gameManager.otherPlayerInZone(gameObject, effectorArray[i]))
            {
                //ici aucun autre joueur n'est dans la zone, on peut désactiver la zone !
                // (comme si le player est sortie de la zone avant de mourrir)
                effectorArray[i].activeZoneByScript(false);
            }
        }
        zoneCamClear();
    }

    /// <summary>
    /// retourne vrai si la zone se trouve dans la liste
    /// </summary>
    /// <returns></returns>
    public bool isEffectorOnMyList(ZoneEffector zone)
    {
        if (effectorArray.IndexOf(zone) < 0)
            return (false);
        return (true);
    }












    /// <summary>
    ///cree une référence des colliderWalls de tout les joueurs
    ///(pour pouvoir les activer et désactiver lors de raycast...)
    /// </summary>
    public void setColliderWallsOther()
    {
        ColliderWalls.Clear();
        int countPlayer = 0;
        for (int i = 0; i < groupPlayer.transform.childCount; i++)
        {
            if (groupPlayer.transform.GetChild(i).gameObject.activeSelf)
            {
                if (gameManager.multi)                                          //si on est en multi, chercher juste le ColliderWalls 1/2/3/4
                {
                    findColliderWallsInObject(groupPlayer.transform.GetChild(i).gameObject);
                    //ColliderWalls.Add(GameObject.Find("ColliderWalls" + (countPlayer + 1)));
                }
                else
                {
                    //si on est en solo, chercher le tag colliderWalls à l'intérieur de l'objet SoloPlayer1/2 (celui qui est actif)
                    for (int j = 0; j < groupPlayer.transform.GetChild(i).transform.childCount; j++)    //parcourt les enfants des SoloPlayerX
                    {
                        if (groupPlayer.transform.GetChild(i).transform.GetChild(j).gameObject.activeSelf)  //si le joueur est actif.. chercher le colliderWalls à l'intérieur !
                        {
                            findColliderWallsInObject(groupPlayer.transform.GetChild(i).transform.GetChild(j).gameObject);
                        }
                    }
                }
                countPlayer++;
            }

        }
    }

    /// <summary>
    /// à l'initialisation, set la référence du player dans les autres scripts
    /// </summary>
    public void setRefToOther()
    {
        cameraController.AddPlayer(gameObject);
        cameraController.addToCam(gameObject);
        cameraController.addToCam(pointCamSmooth.gameObject);
        gameManager.AddPlayer(gameObject);
    }

    /// <summary>
    /// get les variable public du script drawSolidArc sur le player courrant
    /// </summary>
    private void getPublicVariable()
    {
        if (drawSolidArc)
        {
            fovRange = drawSolidArc.fovRange;
            fovAngle = drawSolidArc.fovAngle;
            strengthOfAttraction = drawSolidArc.strengthOfAttraction;
        }
    }

    /// <summary>
    /// affiche lla range dan sl'éditeur de unity
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <param name="scale"></param>
    private void DrawHelperAtCenter(Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(transform.position, destination);
    }

    /// <summary>
    /// gère l'accélération progressif de la fourmis
    /// </summary>
    void HandleThrustForMove()
    {
        if (firstWalking)                                           //si le joueur est en train de marcher... le faire bouger !
            MoveSelected();
        if (isWalkingOnSnow)
        {
            Rb.AddForce(-9.81f * gravityScale * Vector3.down);
            Debug.DrawRay(gameObject.transform.position, -9.81f * gravityScale * Vector3.down, Color.yellow, 1f);
        }
    }

    /// <summary>
    /// déplace la fourmis
    /// </summary>
    void MoveSelected()
    {
        float horizMove = PC.getPlayer(nb_player).GetAxis("Move Horizontal");
        float vertiMove = PC.getPlayer(nb_player).GetAxis("Move Vertical");
        float tmpSpeed = (!isSlow) ? speed : Mathf.Abs(speed - slowWhenHitAmount);

        float heading = Mathf.Atan2(horizMove * (Mathf.Max(1, tmpSpeed - slowWhenFiring * eggsArray.Count)) * Time.deltaTime, vertiMove * (Mathf.Max(1, tmpSpeed - slowWhenFiring * eggsArray.Count)) * Time.deltaTime);

        if (horizMove != 0.0f || vertiMove != 0.0f)
        {
            //if (!isWalkingOnSnow)
            asAlreadyMove = true;
            restrictVelocity(isWalkingOnSnow);

            //float heading = Mathf.Atan2(moveVector.x * 1000 * Time.deltaTime, moveVector.y * 1000 * Time.deltaTime);

            Quaternion _targetRotation = Quaternion.identity;


            _targetRotation = Quaternion.Euler(0f, 0f, -heading * Mathf.Rad2Deg);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnRate * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, ((!isWalkingOnSnow) ? turnRate : turnRate / amountSpeedWhenInSnow) * Time.deltaTime);

            //rb.MovePosition(transform.position + transform.forward * Time.deltaTime);
            //float lerpFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
            //pushAway(transform.up * (Mathf.Max(1, tmpSpeed - slowWhenFiring * eggsArray.Count)) * Time.fixedDeltaTime * lerpFactor);


            Vector3 dir = transform.up * (Mathf.Max(1, tmpSpeed - slowWhenFiring * eggsArray.Count));

            float master = Mathf.Min(Mathf.Abs(horizMove) + Mathf.Abs(vertiMove), 1.0f);

            if (!isHoldingX)
            {
                pushAway((!isWalkingOnSnow) ? dir : dir / amountSpeedWhenInSnow, master);               //avance la fourmis
                //pushAway(dir, master);               //avance la fourmis
            }
                





            ///
            ///lorsque le joueur tourne, rapprocher rapidement la caméra de lui même pour éviter que la caméra ne reste derière
            ///
            if (Mathf.Abs(previousHeading - heading) > 0.1f)
            {
                //Debug.Log("ici on a tourné, diminuer le truck !");
                //ici réduit si on a tourné !
                Vector3 lerpPosCamRotate = Vector3.Lerp(pointCam.transform.position, gameObject.transform.position, Time.deltaTime * 20);
                pointCam.transform.position = new Vector3(lerpPosCamRotate.x, lerpPosCamRotate.y, 0);
                previousHeading = heading;
            }


            ///
            /// augmenter la position du pointCam lorsque le joueur avance
            /// Ajout avec un lerp, en limitant (clamp)
            ///

            //ici gère la rapidité de la caméra selon la vitesse de base de la fourmis
            float clampedSpeed = 1 + (tmpSpeed * 1.0f / 18000f);

            Vector3 lerpPosCam = Vector3.Lerp(pointCam.transform.position, pointCam.transform.position + pointCam.transform.up, Time.deltaTime * master * 6 * clampedSpeed);
            lerpPosCam.x = Mathf.Clamp(lerpPosCam.x, gameObject.transform.position.x - 8f, gameObject.transform.position.x + 8f);
            lerpPosCam.y = Mathf.Clamp(lerpPosCam.y, gameObject.transform.position.y - 8f, gameObject.transform.position.y + 8f);
            pointCam.transform.position = lerpPosCam;   // re-assigning the transform's position will clamp it


            if (!isWalkingOnSnow)
                lastMove = dir;
            else
            {
                lastMove = Vector3.SmoothDamp(lastMove, dir, ref velocityFreez, timeRotationSpeed);
            }
        }
        if (!winnedIntern)
            anim.SetBool("isWalking", true);                //lance l'animation de marche

    }

    /// <summary>
    /// set la fourmis lente, avec un effet de aprticule de glace (on est en glace)
    /// </summary>
    /// <returns></returns>
    public IEnumerator setSlow()
    {
        Debug.Log("set slow !");
        isSlow = true;
        if (SlowParticle && !SlowParticle.activeSelf)
            SlowParticle.SetActive(true);
        yield return new WaitForSeconds(slowWhenHitTiming);
        if (SlowParticle && SlowParticle.activeSelf)
            SlowParticle.SetActive(false);
        isSlow = false;
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// attend X seconde avant d'enlever la particule de gaz principale
    /// </summary>
    /// <returns></returns>
    public IEnumerator setNotSlowByGaz()
    {
        yield return new WaitForSeconds(slowWhenHitGazTiming);
        stopSlowGaz();
    }

    /// <summary>
    /// lors du respawn, il faut reset le gaz !
    /// </summary>
    public void stopGazRespawn()
    {
        speed = oldSpeed;
        SlowParticleGazStay.SetActive(false);
        resetSlowGaz();
    }

    /// <summary>
    /// resetles variable du gaz, appelé lors de l'activation de la fourmis
    /// </summary>
    void resetSlowGaz()
    {
        numberOfTimzGazed = 0;
        numberOfOtorizedGaz = 7;
        isAlreadySlowedByGaz = false;
        stopSlowGaz();
    }

    /// <summary>
    /// appelé lors du switch
    /// </summary>
    public void setGazWhenSwitch(int tmpNumberOfOtorizedGaz, bool TmpIsAlreadySlowedByGaz, int tmpNumberOfTimzGazed)
    {
        numberOfOtorizedGaz = tmpNumberOfOtorizedGaz;
        isAlreadySlowedByGaz = TmpIsAlreadySlowedByGaz;
        numberOfTimzGazed = tmpNumberOfTimzGazed;
        if (isAlreadySlowedByGaz)
            SlowParticleGazStay.SetActive(true);
        if (getTypePowerPlayer() == 4)
            SlowParticleGazStay.SetActive(false);
        else
            speed -= numberOfTimzGazed * slowWhenHitGazAmount;
    }

    /// <summary>
    /// quand le joueur est gazéifié, on lui rajoute un effet de particule
    /// </summary>
    public void setSlowGaz()
    {
        if (isSlowedByGaz)          //s'il est déjà gazifié, retour
            return;
        isAlreadySlowedByGaz = true;
        isSlowedByGaz = true;
        Debug.Log("ici gaz");
        numberOfTimzGazed++;
        SlowParticleGazStay.SetActive(true);
        SlowParticleGaz.SetActive(true);
        speed = speed - slowWhenHitGazAmount;
        numberOfOtorizedGaz--;
        if (numberOfOtorizedGaz <= 0)
        {
            destroyThis();
            return;
        }


        StopCoroutine("setNotSlowByGaz");                                   //stop la coroutine de glace
        StartCoroutine(setNotSlowByGaz());
    }

    /// <summary>
    /// stop le slow du gaz
    /// </summary>
    void stopSlowGaz()
    {
        isSlowedByGaz = false;
        SlowParticleGaz.SetActive(false);
    }

    /// <summary>
    /// stop le slow
    /// </summary>
    void StopSlow()
    {
        StopCoroutine("setSlow");                                   //stop la coroutine de glace
        isSlow = false;
        SlowParticle.SetActive(false);
    }

    /// <summary>
    /// effectue un switch de fourmis
    /// </summary>
    void switchPower()
    {
        //mettre un tout petit timer ou pas ?
        //if (!SA.panelDisplayed[indexSolo])
        //{
        int power = 0;
        if (PC.getPlayer(nb_player).GetAxisRaw("Switch Vertical") > 0.2f)
            power = 1;
        else if (PC.getPlayer(nb_player).GetAxisRaw("Switch Horizontal") > 0.2f)
            power = 2;
        else if (PC.getPlayer(nb_player).GetAxisRaw("Switch Vertical") < -0.2f)
            power = 3;
        else if (PC.getPlayer(nb_player).GetAxisRaw("Switch Horizontal") < -0.2f)
            power = 4;
        if (power != 0)
            SA.SwitchPanel(indexSolo, power);
        //}
    }

    /// <summary>
    /// défini si la fourmis est en train de marcher ou non
    /// </summary>
    /// <param name="walk"></param>
    private void walking(int terrain)
    {
        if (terrain > 0) //on marche
        {
            if (firstWalking == false) //au départ il est égal a -1
            {
                firstWalking = true;
                anim.Play("Running");
                SoundManager.GetSingleton.playSound("Walk" + nb_player);
            }

        }
        else //on est arreté
        {
            if (firstWalking == true)  //on est en train de marcher
            {


                firstWalking = false;
                anim.SetBool("isWalking", false);               //joueur à l'arret
                anim.Play("Nothing");
                SoundManager.GetSingleton.playSound("Walk" + nb_player, true);
            }

            ///
            /// lorsqu'on est arreté, est-ce qu'on diminue le pointcam ?
            ///
            //Vector3 lerpPosCam = Vector3.Lerp(pointCam.transform.position, gameObject.transform.position, Time.deltaTime * 1);
            //pointCam.transform.position = new Vector3(lerpPosCam.x, lerpPosCam.y, 0);


            restrictVelocity(isWalkingOnSnow);
        }
    }

    /// <summary>
    /// tout les inputs du joueurs, ses déplacements et ses touches d'actions
    /// </summary>
    void playerInput()
    {
        if (PC.getPlayer(nb_player).GetAxisRaw("Move Horizontal") != 0 || PC.getPlayer(nb_player).GetAxisRaw("Move Vertical") != 0)
        {
            //TODO: tester sur quel surface la fourmis est en train de marcher ??
            walking(1);
            //
        }
        else
            walking(0);                                     //stop walking

        if (TP == TypeOfPlayer.White)                       //si le joueur est blanc, il n'a pas de pouvoir !
            return;

        if (TP == TypeOfPlayer.BlackHole)
            powerBlackHole();                               //input du pouvoir BlackHole
        else
            powerActionPlayer();                            //input des pouvoirs Repulse, Attract ou Flaire

        if (!gameManager.multi)                             //si nous ne somme pas en mode multi, autorise le switch de pouvoir (s'il y a a plusieurs...)
            switchPower();                                  //cree un switch de player;
    }

    /// <summary>
    /// Action du joueur quand il tire selon son pouvoirs (pour bleu, rouge et verte)
    /// </summary>
    void powerActionPlayer()
    {
        if (PC.getPlayer(nb_player).GetButton("FireA") && releaseMetalicaChild)
        {
            releaseMetalicaChild = false;
            if (TP == TypeOfPlayer.Attractor)
                SoundManager.GetSingleton.playSound("Attract" + nb_player);
            else if (TP == TypeOfPlayer.Repulsor)
                SoundManager.GetSingleton.playSound("Repulse" + nb_player);

            powerFlaire(0);                      //cree des trails
            if (particle)
                particle.SetActive(true);
        }
        if (!PC.getPlayer(nb_player).GetButton("FireA") && !releaseMetalicaChild)
        {
            releaseMetalicaChild = true;
            if (TP == TypeOfPlayer.Attractor)
                SoundManager.GetSingleton.playSound("Attract" + nb_player, true);
            else if (TP == TypeOfPlayer.Repulsor)
                SoundManager.GetSingleton.playSound("Repulse" + nb_player, true);

            powerFlaire(2);                     //relache les trails
            if (particle)
                particle.SetActive(false);
        }
        if (PC.getPlayer(nb_player).GetButton("FireB") && TP == TypeOfPlayer.Flaire)
        {
            releaseMetalicaChild = true;
            powerFlaire(3);     //kill les trails
        }
        if (PC.getPlayer(nb_player).GetButton("FireY") && !isHoldingY)
        {
            isHoldingY = true;
            if (TP == TypeOfPlayer.Flaire && trailController.listTrails.Count > 0)
                SoundManager.GetSingleton.playSound("GreenInverse" + nb_player);
        }
        if (!PC.getPlayer(nb_player).GetButton("FireY") && isHoldingY)
        {
            isHoldingY = false;
            if (TP == TypeOfPlayer.Flaire && trailController.listTrails.Count > 0)
                SoundManager.GetSingleton.playSound("GreenInverse" + nb_player, true);
        }
        if (PC.getPlayer(nb_player).GetButton("FireX") && !isHoldingX)
        {
            isHoldingX = true;
        }
        if (!PC.getPlayer(nb_player).GetButton("FireX") && isHoldingX)
        {
            isHoldingX = false;
        }
    }

    /// <summary>
    /// pouvoir du trou noirs
    /// </summary>
    void powerBlackHole()
    {
        if (PC.getPlayer(nb_player).GetButton("FireA"))
        {
            //Debug.Log("ici ?");
            if (FP)
                FP.FirePortal();

        }
        if (PC.getPlayer(nb_player).GetButton("FireB"))
        {
            if (FP)
                FP.inputFireB();
        }
        if (PC.getPlayer(nb_player).GetButton("FireY"))
        {
            if (FP)
                FP.inputFireY();
        }
        if (PC.getPlayer(nb_player).GetButton("FireX") && !isHoldingX)
        {
            isHoldingX = true;
        }
        if (!PC.getPlayer(nb_player).GetButton("FireX") && isHoldingX)
        {
            isHoldingX = false;
        }
    }

    /// <summary>
    /// pouvoir du flaire
    /// create vrai: on reste appuyé
    /// create faux: on a relaché
    /// </summary>
    void powerFlaire(int state)
    {
        if (!trailController)
            return;
        trailController.powerFlare(state);
    }

    /// <summary>
    /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <param name="n">reference</param>
    /// <returns></returns>
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
        float sign = 1;// Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
        return (angle360);
    }

    /// <summary>
    /// test is l'objet est dans la range
    /// </summary>
    /// <param name="other">l'objet en question</param>
    /// <param name="checkDistance">est-ce qu'on fait un test de distance ?</param>
    /// <param name="withRaycast">est-ce qu'on fait un test de raycast ?</param>
    /// <returns>retourne si oui ou non l'objet est dans la range</returns>
    bool isInRange(Transform other, bool checkDistance = false, bool withRaycast = false)
    {
        Vector3 B = other.transform.position - gameObject.transform.position;
        Vector3 C = Quaternion.AngleAxis(90 + fovAngle / 2, -transform.forward) * -transform.right;
        if (SignedAngleBetween(transform.up, B, transform.up) <= SignedAngleBetween(transform.up, C, transform.up) || fovAngle == 360)
        {
            if (checkDistance || !useRaycast)
            {
                Vector3 offset = other.position - transform.position;
                float sqrLen = offset.sqrMagnitude;
                if (sqrLen < fovRange * fovRange)
                    return (true);
                return (false);
            }
            if (!withRaycast)
                return (true);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, B, out hit, layerMask))
            {
                Debug.DrawRay(transform.position, B, Color.red, 0.5f);
                //if (hit.transform.gameObject.tag == "Eggs")
                return (true);
            }
        }
        return (false);
    }

    /// <summary>
    /// active ou désactive tout les colliderWalls des joueurs
    /// (pour permettre des lancers de rayons)
    /// </summary>
    /// <param name="active">si on active ou désactive</param>
    void setActiveColliders(bool active)
    {
        if (!useRaycast)
            return;
        for (int i = 0; i < ColliderWalls.Count; i++)
        {
            if (ColliderWalls[i])
                ColliderWalls[i].SetActive(active);
        }
    }

    /// <summary>
    /// applique une force (Repulse ou Attract)
    /// </summary>
    void Force()
    {
        gameObject.layer = 0;
        setActiveColliders(false);

        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, fovRange, layerMask);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Eggs" || hitColliders[i].tag == "Player" || hitColliders[i].tag == "Pushable")
            {
                Vector3 B = hitColliders[i].transform.position - gameObject.transform.position;
                Vector3 C = Quaternion.AngleAxis(90 + fovAngle / 2, -transform.forward) * -transform.right;
                if (SignedAngleBetween(transform.up, B, transform.up) <= SignedAngleBetween(transform.up, C, transform.up) || fovAngle == 360)
                {
                    if (useRaycast)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, B, out hit, layerMask))
                        {
                            Debug.DrawRay(transform.position, B, Color.red, 0.5f);
                            ActionAntsForce(hit.collider);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, B, Color.red, 0.5f);
                        ActionAntsForce(hitColliders[i]);
                    }
                }
            }
        }
        setActiveColliders(true);
        gameObject.layer = layerType;
    }

    /// <summary>
    /// supprime un oeuf précis de la liste d'objet
    /// </summary>
    /// <param name="metal"></param>
    public void destroyMetalByGameObject(GameObject metal)
    {
        for (int i = 0; i < eggsArray.Count; i++)
        {
            if (eggsArray[i].GetInstanceID() == metal.GetInstanceID())
                eggsArray.RemoveAt(i);
        }
    }

    /// <summary>
    /// relache tout les oeuf du tableau
    /// </summary>
    public void releaseAllEggs()
    {
        //Debug.Log("player :" + gameObject.name);
        for (int i = 0; i < eggsArray.Count; i++)
        {
            if (eggsArray[i])
            {
                eggsArray[i].transform.gameObject.GetComponent<EggsController>().StopBeingControlled();
                eggsArray.RemoveAt(i);
            }
        }
        for (int i = 0; i < pushableArray.Count; i++)
        {
            if (pushableArray[i])
            {
                if (pushableArray[i].transform.gameObject.GetComponent<Pushable>())
                    pushableArray[i].transform.gameObject.GetComponent<Pushable>().setControl(false, getTypePowerPlayer() - 1);
                pushableArray.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// applique une force sur l'oeuf touché
    /// </summary>
    /// <param name="hit"></param>
    void ForceOnEgg(Collider hit)
    {
        if (hit.GetComponent<Rigidbody>().isKinematic)
            return;

        if (eggsArray.IndexOf(hit.transform.gameObject) < 0)
        {
            if (hit.transform.gameObject.GetComponent<EggsController>().TWEOC.isOk)
            {
                hit.transform.gameObject.GetComponent<EggsController>().TWEOC.isOk = false;
            }
            eggsArray.Add(hit.transform.gameObject);
        }
    }

    /// <summary>
    /// pousse le joueur dans la direction voulu
    /// </summary>
    /// <param name="push"></param>
    /// <param name="master"></param>
    public void pushAway(Vector3 push, float master = 1)
    {
        //float horizMove = PC.getPlayer(nb_player).GetAxis("Move Horizontal");
        //float vertiMove = PC.getPlayer(nb_player).GetAxis("Move Vertical");

        //Rb.AddForce(push * coefThrustFactor);
        Rb.AddForce(push * master/** (Mathf.Abs(horizMove) + Mathf.Abs(vertiMove))*/);
    }

    /// <summary>
    /// applique une force sur le player ennemi
    /// </summary>
    /// <param name="hit"></param>
    void ForceOnPlayer(Collider hit)
    {
        Vector3 forceDirection = transform.position - hit.transform.position;
        if (hit.transform.gameObject.GetComponent<PlayerController>())
        {
            hit.transform.gameObject.GetComponent<PlayerController>().pushAway((strengthOfAttraction * forceDirection * 1 * additionForcingPlayer) * Time.deltaTime * 100);
        }
    }

    /// <summary>
    /// pousse les objets
    /// </summary>
    /// <param name="hit"></param>
    void ForceOnPushable(Collider hit)
    {
        Vector3 forceDirection = transform.position - hit.transform.position;
        if (pushableArray.IndexOf(hit.transform.gameObject) < 0)
            pushableArray.Add(hit.transform.gameObject);
        if (hit.transform.gameObject.GetComponent<Rigidbody>() && hit.transform.gameObject.GetComponent<Pushable>())
        {
            hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection * hit.transform.gameObject.GetComponent<Pushable>().factorMultiply);
            hit.transform.gameObject.GetComponent<Pushable>().setControl(true, getTypePowerPlayer() - 1);
            //hit.transform.gameObject.GetComponent<Pushable>().setControl(getTypePowerPlayer() - 1);
        }
    }

    /// <summary>
    /// action de force selon le type d'objet touché
    /// </summary>
    /// <param name="hit"></param>
    void ActionAntsForce(Collider hit)
    {
        switch (hit.tag)
        {
            case ("Eggs"):
                ForceOnEgg(hit);
                break;

            case ("Player"):
                ForceOnPlayer(hit);
                break;

            case ("Pushable"):
                ForceOnPushable(hit);
                break;
        }
    }

    /// <summary>
    /// parcourt la liste de tout les oeufs sous contrôles du pouvoirs
    /// </summary>
    void checkForMettalicaChild()
    {
        //parcourt la liste des oeufs
        for (int i = 0; i < eggsArray.Count; i++)
        {
            //si le metal n'existe pas, ou qu'il est freezed (gelé), le supprimer de la lsite et continuer
            if (!eggsArray[i] || eggsArray[i].transform.gameObject.GetComponent<EggsController>() && eggsArray[i].transform.gameObject.GetComponent<EggsController>().getCurrentParticle() == 3)
            {
                eggsArray.RemoveAt(i);
                continue;
            }

            //test si l'oeuf courrant est dans la range
            bool isInRangeTmp = isInRange(eggsArray[i].transform, true, false);

            //si le pouvoir est désactivé... stop d'être controllé, et le supprime de la liste
            if (releaseMetalicaChild)
            {
                //Debug.Log("relache tout les oeufs");
                eggsArray[i].transform.gameObject.GetComponent<EggsController>().StopBeingControlled();
                eggsArray.RemoveAt(i);
            }
            //sinon, le pouvoir est ACTIVé, mais l'oeuf est trop loin (commencer le pouvoirs éloignée)
            else if (!isInRangeTmp)
            {
                if (!eggsArray[i].transform.gameObject.GetComponent<EggsController>().isInKeep)
                {
                    StartCoroutine(eggsArray[i].transform.gameObject.GetComponent<EggsController>().keepAddingForce(gameObject));
                }
            }
            //sinon, le pouvoir est ACTIVé, et l'oeuf est DANS la range, appliquer une force...
            else if (isInRangeTmp)
            {
                EggsController EC = eggsArray[i].transform.gameObject.GetComponent<EggsController>();
                if (EC)
                {
                    changeParticleEggs(EC);             //changer la particule de l'oeuf par rapport au joueur
                    EC.playerFocus = gameObject;        //change le playerFocus de l'oeuf au joueur qui applique la force
                    EC.isControlled = true;             //change le state de l'oeuf en controllé
                }
                //enfin, effectue une force sur l'oeuf
                Vector3 forceDirection = transform.position - eggsArray[i].transform.position;
                eggsArray[i].transform.gameObject.GetComponent<Rigidbody>().drag = 0;
                eggsArray[i].transform.gameObject.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection * 1);
                EC.checkIfIsPushWhenInTrail();
            }
        }
        //parcourt la liste des pushable
        for (int i = 0; i < pushableArray.Count; i++)
        {
            //hit.transform.gameObject.GetComponent<Pushable>().setControl(getTypePowerPlayer() - 1);
            //si le pushable n'existe pas, le supprimer de la lsite et continuer
            if (!pushableArray[i])
            {
                pushableArray.RemoveAt(i);
                continue;
            }

            //test si l'oeuf courrant est dans la range
            bool isInRangeTmp = isInRange(pushableArray[i].transform, true, false);

            if (releaseMetalicaChild)   //si on a relaché la touche, le supprimer !
            {
                //Debug.Log("relache tout les oeufs");
                if (pushableArray[i].transform.gameObject.GetComponent<Pushable>())
                    pushableArray[i].transform.gameObject.GetComponent<Pushable>().setControl(false, getTypePowerPlayer() - 1);
                pushableArray.RemoveAt(i);
            }
            //sinon, le pouvoir est ACTIVé, et le pushable est DANS la range, appliquer une force...
            else if (isInRangeTmp)
            {
                if (pushableArray[i].transform.gameObject.GetComponent<Pushable>())
                    pushableArray[i].transform.gameObject.GetComponent<Pushable>().setControl(true, getTypePowerPlayer() - 1);
            }
            //sinon, le pouvoir est ACTIVé, mais le pushable est trop loins...
            else
            {
                if (pushableArray[i].transform.gameObject.GetComponent<Pushable>())
                    pushableArray[i].transform.gameObject.GetComponent<Pushable>().setControl(false, getTypePowerPlayer() - 1);
                pushableArray.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// change le state de la particule de l'
    /// TP = 1 à 4
    /// </summary>
    /// <param name="EC"></param>
    void changeParticleEggs(EggsController EC)
    {
        EC.changeCurrentParticle((int)TP);
    }

    /// <summary>
    /// change la référence de l'avatar du joueur, créé en début de scène
    /// </summary>
    /// <param name="avatar"></param>
    public void setAvatar(GameObject avatar, GameObject avatarRespawn)
    {
        avatarPlayer = avatar;
        avatarPlayerRespawn = avatarRespawn;
    }

    /// <summary>
    /// affiche l'avatar de mort du player dans la GUI
    /// </summary>
    void changeGUIAvatar(bool active)
    {
        if (!active)    //ici on est mort
        {
            avatarPlayer.transform.GetChild(0).gameObject.SetActive(false);
            avatarPlayer.transform.GetChild(1).gameObject.SetActive(true);

            avatarPlayerRespawn.transform.GetChild(0).gameObject.SetActive(true);   //active Dead
            avatarPlayerRespawn.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true); //active aussi le timer !

            //avatarPlayerRespawn.SetActive(true);
        }
        else //ici en vie
        {
            avatarPlayer.transform.GetChild(0).gameObject.SetActive(true);   //le sprite de l'avatar est activé
            avatarPlayer.transform.GetChild(1).gameObject.SetActive(false);   //le sprite de l'avatar mort est désactivé

            avatarPlayerRespawn.transform.GetChild(0).gameObject.SetActive(false);
            //avatarPlayerRespawn.SetActive(false);
        }
    }

    /// <summary>
    /// est appelé lors de l'initialisation (OU après un switch)
    /// pour changer le sprite de l'avatar du joueur
    /// dans le canvas et le respawn...
    /// </summary>
    public void changeSpriteAvatar(Sprite alive, Sprite Dead)
    {
        avatarPlayer.transform.GetChild(0).GetComponent<Image>().sprite = alive;         //change le sprite représentant la fourmis vivante
        avatarPlayer.transform.GetChild(1).GetComponent<Image>().sprite = Dead;         //change le sprite représentant la fourmis vivante
        avatarPlayerRespawn.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>().sprite = Dead;    //change le sprite de mort
    }

    /// <summary>
    /// active ou non la marche dans la snow
    /// </summary>
    public void WalkOnSnow(bool onSnow)
    {
        isWalkingOnSnow = onSnow;
        Rb.useGravity = onSnow;
    }

    /// <summary>
    /// restrict la vélocité des fourmis
    /// </summary>
    void restrictVelocity(bool inSnow = false)
    {
        if (Rb.velocity != Vector3.zero)
        {
            Rb.velocity = Vector3.zero;
        }
        if (Rb.angularVelocity != Vector3.zero)
        {
            Rb.angularVelocity = Vector3.zero;
        }


        if (inSnow)
        {
            if (asAlreadyMove)
            {
                pushAway(lastMove / amountSpeedOfFreez);
                //Rb.AddForce(-9.81f * gravityScaleFreez * Vector3.down);
            }
            return;
        }

    }

    /// <summary>
    /// cache l'op du joueur
    /// </summary>
    void HideOp(bool active)
    {
        if (!OP)
            return;
        if (active)                         //restart playing... afficher !
        {
            if (gameManager.multi)
                OP.hideOnScreen = false;                                        //affiche l'OP du joueur en multi
            else
            {
                OP.hideOnScreen = true;                                         //cache l'OP du joueur en solo
                if (gameManager.soloAnd2Player)
                    OP.hideOnScreen = false;                                         //affiche l'OP du joueur en solo
            }


            OP.hideOutofScreen = false;                                         //affiche l'OP lorsqu ele joueur sort de l'écran
            OPicon.GetComponent<ObjectiveIndicator>().hideOnScreen = true;     //cache l'icon du pouvoir du joueur
        }
        else                                //cacher...
        {
            OP.hideOnScreen = true;                                     //cache l'OP du joueur
            OP.hideOutofScreen = true;                                  //idem pour le out
            OPicon.GetComponent<ObjectiveIndicator>().hideOnScreen = true;  //de même pour l'icon du pouvoir du joueur
            OPicon.GetComponent<ObjectiveIndicator>().SetInvisible();

            if (gameManager.soloAnd2Player)
            {
                OP.hideOnScreen = true;                                         //affiche l'OP du joueur en solo
                OP.SetInvisible();
            }

        }
    }

    /// <summary>
    /// reset les positions des pointsCams
    /// </summary>
    void ResetPosPointCam(bool active)
    {
        ///stop everythings
        if (!active)
        {
            pointCam.gameObject.SetActive(false);
            pointCamSmooth.gameObject.SetActive(false);
            pointCam.position = gameObject.transform.position;
            pointCamSmooth.position = pointCam.position;
        }
        else        //active
        {
            pointCamSmooth.SetParent(groupPointCam.transform);      //set le parent aux groupe lors de l'activation...

            
            pointCam.position = gameObject.transform.position;
            pointCamSmooth.position = gameObject.transform.position;

            pointCam.gameObject.SetActive(true);
            pointCamSmooth.gameObject.SetActive(true);
        }
    }

    #endregion

    #region unity fonction and ending
    /// <summary>
    /// attend X seconde avant de détruire l'objet
    /// cela permet à la caméra de rester focus un temps avant que le
    /// joueur ne meurt totalement
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitBeforeDie()
    {
        //wait X seconde
        yield return new WaitForSeconds(gameManager.timeBeforGoToQueen);
        deadAnts = true;
        resetZoneEffector();
        gameObject.SetActive(false);
        //Destroy(gameObject);
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// fonction appelé lorsque l'on veut détruire le player
    /// </summary>
    [ContextMenu("destroyThis")]
    public void destroyThis()
    {
        if (TP == TypeOfPlayer.Flaire)                              //si c'est un flaire, supprimer son GUI follow
            trailController.progressBarGreen.SetActive(false);
        //  Destroy(trailController.progressBarGreen);

        stopEverything();                                           //stop tout les contrôles et autres
        changeGUIAvatar(false);                                          //affiche l'avatar de mort dans la GUI
        gameManager.playerDeath(gameObject);                        //appelle playerDeath dans le gameManager pour gerer les conditions de fins
        SpriteRend.SetActive(false);                                //dache le spirte du joueur
        //Boom.transform.SetParent(gameController.transform);         //Change la particule d'explosion dans le world space
        Boom.SetActive(true);                                       //cree une explosion

        PC.VibrationGamePad(nb_player, vibrateBothDying, vibrateBothDying, BothDyingTiming);    //active les vibration des manettes
        Rb.useGravity = true;
        StartCoroutine(waitBeforeDie());                            //attend X seconde avant de détruire totalement le joueur
    }

    /// <summary>
    /// permet de stoper le joueur sans le détruire
    /// </summary>
    public void stopEverything()
    {
        if (!active)
            return;

        listEmitt.stopAllEmitter();             //stop tout les sons !
        cameraController.deleteToCam(pointCamSmooth.gameObject);
        ResetPosPointCam(false);
        HideOp(false);
        WalkOnSnow(false);  //reset walkOnSnow, & useGravity ! (when die, remake gravity true again in destroyTHis)
        Rb.useGravity = false;

        asAlreadyMove = false;
        if (FP)
            FP.resetAll();

        if (TP == TypeOfPlayer.Flaire)                              //si c'est un flaire, supprimer ses Trails
        {
            if (trailController && trailController.progressBarGreen)
                trailController.progressBarGreen.SetActive(false);
            powerFlaire(3);
        }
        StopSlow();                                                 //stop le slow
        StopCoroutine("setNotSlowByGaz");                                   //stop la coroutine de glace
        stopSlowGaz();

        //stop le joueur pour pas qu'il dérive dans l'espace...
        restrictVelocity();
        Rb.velocity = Vector3.zero;                                 //set 0 à la vélocité
        Rb.angularVelocity = Vector3.zero;

        releaseMetalicaChild = true;                                //permet de "lacher" les oeufs
        releaseAllEggs();                                          //relache tout les métaux précédement sous contrôle

        gameObject.layer = 0;                                       //change le layer à 0 pour éviter les raycast
        if (particle)                                               //test si elle existe
            particle.SetActive(false);                                  //désactive la particule de pouvoirs

        active = false;                                             //variable active du joueur à faux
    }

    /// <summary>
    /// permet de redémarer le joueur
    /// </summary>
    public void restartEverything()
    {
        if (TP == TypeOfPlayer.Flaire)                              //si c'est un flaire, supprimer ses Trails
            trailController.progressBarGreen.SetActive(true);
        //else if (TP == TypeOfPlayer.BlackHole)
        //FP.initPortal();
        Debug.Log("ici restart player");

        ResetPosPointCam(true);
        //pointCamSmooth.GetComponent<moveToPlayer>().target = gameObject.transform;

        WalkOnSnow(false);  //reset walkOnSnow, & useGravity !
        Rb.useGravity = false;

        asAlreadyMove = false;
        restrictVelocity();
        releaseAllEggs();                                           //relache tous les oeufs
        cameraController.AddPlayer(gameObject);                     //ajoute le joueur à la caméra
        cameraController.addToCam(gameObject);
        cameraController.addToCam(pointCamSmooth.gameObject);
        SpriteRend.SetActive(true);                                //affiche le spirte du joueur
        Boom.SetActive(false);                                       //cache l'explosion
        firstWalking = false;
        previousHeading = 0;
        //SA.resetPanel(indexSolo);                                   //reset le panel à alpha = 0;

        resetSlowGaz();

        HideOp(true);
        changeGUIAvatar(true);
        gameObject.layer = 8;                                       //set le layer du joueur à 8
        //Rb.isKinematic = false;
        OPicon.GetComponent<ObjectiveIndicator>().SetVisible();
        active = true;
    }

    /// <summary>
    /// lorsque 
    /// </summary>
    private void OnEnable()
    {
        if (deadAnts)
        {
            restartEverything();
            deadAnts = false;
        }
        else
        {
            //première fois qu'on est dans onEnable et qu'on est pas mort avant...

        }
    }

    /// <summary>
    /// applique une mini force sur les oeufs en colisions avec nous !
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        Vector3 forceDirection = transform.position - other.transform.position;
        if (other.CompareTag("Eggs") /*&& getTypePowerPlayer() != 4*/)
        {
            other.transform.gameObject.GetComponent<Rigidbody>().AddForce(-repulsionWhenCollide * forceDirection * 1);
        }
    }

    /// <summary>
    /// à chaque frame
    /// </summary>
    private void Update()
    {
        if (!active || !activeControl)                                                //ne fait rien si le joueur n'est pas activé (s'il est en train de mourir)
            return;

        if (Time.fixedTime >= timeToGo)
        {
            //getPublicVariable();                                    //get les variables du sccript drawSolidArc
            timeToGo = Time.fixedTime + timeOpti;
        }

        if (gameManager && !gameManager.LMG.gamePaused)
            playerInput();                                              //get les inputs dans l'update, et les actions dans la physique !

    }

    /// <summary>
    /// a chaque frame physique
    /// </summary>
    void FixedUpdate()
    {
        if (!active || !activeControl)                                                //ne fait rien si désactivé
            return;
        //restrictVelocity();
        //Rb.velocity = Vector3.zero;                                 //inhibe la vélocité du joueur
        //Rb.angularVelocity = Vector3.zero;
        if (!releaseMetalicaChild)                                  //si le joueur effectue une force...
        {
            if (TP != TypeOfPlayer.Flaire)                          //si le pouvoirs est Repulse ou Attract...
                Force();                                            //effectue une force
            if (TP == TypeOfPlayer.Flaire)                          //si c'est une flaire
                powerFlaire(1);                                     //continuer la force de flaire !
        }
        HandleThrustForMove();                                      //move player
        //HandleTwinning();

        checkForMettalicaChild();                                   //effectue une force sur tout les oeufs dans le tableau (tableau remplie grace au pouvoir Force();
    }
    #endregion
}