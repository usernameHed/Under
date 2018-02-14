using System.Collections;
using UnityEngine;

public class FirePortals : MonoBehaviour 
{
    /// <summary>
    /// public
    /// </summary>

    [Space(10)]
    [Header("new variable")]
    public GameObject groupOnPlayer;
    public GameObject FPinit;
    public GameObject FPattract;
    public GameObject FPrepulse;
    public GameObject FPprojectile;
    public bool fired = false;
    public bool repulsor = false;
    public bool connexion = false;
    public float rangePortalInit = 20.0f;
    [HideInInspector] public GameObject GroupPortals;                                 //object contenant les object du monde !
    /// <summary>
    /// private
    /// </summary>
    private TimeWithNoEffect TWNE;                                  //timer du FP
    private GameObject playerParent;                                //link du parent (le joueur)
    [HideInInspector] public PlayerController PC;                   //link du PC du joueur
    

    /// <summary>
    /// private serealized
    /// </summary>
    [Space(10)]
    [Header("private serialized field")]
    [SerializeField] private float speed = 1500;                    //speed du projectile
    [SerializeField] private Transform spawnPosition;
    
    private float timeToGo;
    [Range(0, 0.1f)]        public float timeOpti = 1f;                       //optimisation des fps

    /// <summary>
    /// initialize
    /// </summary>
    private void Awake()
    {
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        playerParent = transform.parent.gameObject;
        if (playerParent)
            PC = playerParent.GetComponent<PlayerController>();
        GroupPortals = GameObject.FindGameObjectWithTag("GroupPortals");    //group où mettre les portals
        if (!spawnPosition)
            Debug.LogError("le spawn position est off ?");
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        
    }

    private void OnEnable()
    {
        initPortal();
    }

    /// <summary>
    /// initialise les portals selon le type de players à l'init du player
    /// </summary>
    /// <param name="portalInit">l'objet où placer les objects</param>
    /// <param name="isMulti">le jeu est-il multijoueur ?</param>
    /// <param name="indexPlayer">en mode solo: 0, en coop: 0 ou 1 selon le joueur !</param>
    public void initPortal()
    {
        Debug.Log("ici n'initialisation des portals (à l'init du joueur)");

        //initialise toute les variable
        fired = false;
        repulsor = false;
        connexion = false;

        //if (FPprojectile.activeSelf)                                                            //s'il existe un projectile, le détruire !!
        //  Destroy(projectiles);

        if (!GroupPortals)
            Debug.LogError("no groupPortalInit !!! mauvais enchainement !");
        //set l'objet où placer les portals dans le monde
        FPinit.transform.SetParent(GroupPortals.transform);                     //set le portal init
        FPattract.transform.SetParent(GroupPortals.transform);                  //set le portal attract
        FPrepulse.transform.SetParent(GroupPortals.transform);                  //set le portal repulse
        FPprojectile.transform.SetParent(GroupPortals.transform);               //set le portal projectile

        FPinit.SetActive(false);                                                    //desactive le portal init (non active au début !)
        FPattract.SetActive(false);
        FPrepulse.SetActive(false);
        FPprojectile.SetActive(false);

        /*
        FPinit.GetComponent<PortalLinks>().refPlayer = this;
        FPattract.GetComponent<PortalLinks>().refPlayer = this;
        FPrepulse.GetComponent<PortalLinks>().refPlayer = this;
        FPprojectile.GetComponent<ProjectileScript>().refPlayer = this;
        FPprojectile.GetComponent<PortalLinks>().refPlayer = this;
        */
    }

    /// <summary>
    /// initialise qu'une seule fois en début de partie les attributs des portals de la fourmis
    /// </summary>
    /// <param name="isMulti"></param>
    /// <param name="indexPlayer"></param>
    /// <param name="idTeam"></param>
    public void initColorPortal(bool isMulti, int indexPlayer, int idTeam)
    {
        //0: yellow, 1: orange, 2: blue, 3: red
        int typeColor = 0;  //yellow
        if (isMulti)
            Debug.Log("ici en multi: " + idTeam);

        if (isMulti)                                                                //si on est en multi
        {
            if (idTeam == 1)
                typeColor = 2;   //blue                                              //la team 1: en bleu
            else
                typeColor = 3;     //red                                             //la team 2: en rouge
        }
        else
        {
            idTeam = 1;     //pas nous nis notre amis en coop...
            if (indexPlayer == 0)                                                   //en solo: normal, en jaune
                typeColor = 0;  //yellow
            else
                typeColor = 1;  //orange                                             //si on est en coop et le deuxième: en orange
        }

        FPinit.SetActive(false);                                                    //desactive le portal init (non active au début !)
        FPattract.SetActive(false);
        FPrepulse.SetActive(false);
        FPprojectile.SetActive(false);
        FPinit.GetComponent<PortalLinks>().initPlayerParticle(this, idTeam, typeColor);
        FPattract.GetComponent<PortalLinks>().initPlayerParticle(this, idTeam, typeColor);
        FPrepulse.GetComponent<PortalLinks>().initPlayerParticle(this, idTeam, typeColor);
        FPprojectile.GetComponent<PortalLinks>().initPlayerParticle(this, idTeam, typeColor);

        //défini le liens unique entre le portal repulse et le portal aspire !
        FPattract.GetComponent<PortalLinks>().PC.targetLocation = FPrepulse.GetComponent<PortalLinks>().PC.spawnLocation;

        FPprojectile.GetComponent<ProjectileScript>().refPlayer = this;
        FPprojectile.GetComponent<ProjectileScript>().typeColor = typeColor;
    }

    public void resetAll()
    {
        fired = false;                  //le joueur peut retirer
        repulsor = false;
        connexion = false;
        CancelInvoke("stopProjectile"); //annule le temps d'attente pour kill le projectile...
        desactivePorjectile();          //d'abord désactive projectile (pour la caméra)
        desactivePortal();              //ensuite désactive tous les portals
    }

    /// <summary>
    /// met dans la pool le portalInit et le désactive
    /// </summary>
    public void destroyOldInit()
    {
        /*if (!lastPortalInit)
            return;
        lastPortalInit.transform.parent = groupPortalsPoolInit.transform;
        lastPortalInit.SetActive(false);*/
    }

    /// <summary>
    /// met dans la pool le portalAttract et le désactive
    /// </summary>
    public void destroyOldAttract()
    {
        /*if (!lastPortalAttract)
            return;
        lastPortalAttract.transform.parent = groupPortalsPoolAttract.transform;
        lastPortalAttract.SetActive(false);*/
    }

    /// <summary>
    /// met dans la pool le portalRepulse et le désactive
    /// </summary>
    public void destroyOldRepulse()
    {
        /*if (!lastPortalRepulse)
            return;
        lastPortalRepulse.transform.parent = groupPortalsPoolRepulse.transform;
        lastPortalRepulse.SetActive(false);*/
    }

    /// <summary>
    /// est-ce que le player lui-même est dans la range ?
    /// </summary>
    /// <param name="posRange"></param>
    /// <returns></returns>
    public bool isInRangeOkCreate(Vector3 posRange)
    {
        Vector3 offset = posRange - transform.position;
        float sqrLen = offset.sqrMagnitude;
        //soit on est dans la range, et on créé les portals,
        if (sqrLen < rangePortalInit * rangePortalInit)
            return (true);
        return (false);
    }

    /// <summary>
    /// anulation du black hole ! (input FireB)
    /// </summary>
    public void inputFireB()
    {
        if (fired || !TWNE.isOk)             //si une particule est tiré, il faut attendre qu'elle arrive ! On restreint aussi l'action
        {
            SoundManager.GetSingleton.playSound("CancelBlackHole Denied" + PC.nb_player);
            return;
        }
            
        TWNE.isOk = false;              //limite de répétition
        SoundManager.GetSingleton.playSound("CancelBlackHole" + PC.nb_player);
        Debug.Log("Ici on appuis sur B");
        resetAll();                     //reset les portals
    }

    /// <summary>
    /// anulation du black hole ! (input FireB)
    /// </summary>
    public void inputFireY()
    {
        if (fired || !TWNE.isOk || !connexion)             //si une particule est tiré, il faut attendre qu'elle arrive ! On restreint aussi l'action
            return;
        TWNE.isOk = false;              //limite de répétition
        SoundManager.GetSingleton.playSound("Switch" + PC.nb_player);
        switchPortals();
    }

    /// <summary>
    /// désactive le joueur (ou switch), remet ses portals à l'intérieur de lui
    /// </summary>
    public void desactivePortal()
    {
        FPinit.GetComponent<TimeWithEffectOnCamera>().stopNow();
        FPattract.GetComponent<TimeWithEffectOnCamera>().stopNow();
        FPrepulse.GetComponent<TimeWithEffectOnCamera>().stopNow();

        FPinit.SetActive(false);                                                    //desactive le portal init (non active au début !)
        FPattract.SetActive(false);
        FPrepulse.SetActive(false);
        FPprojectile.SetActive(false);

        /*Debug.Log("ICI replace les portals dans le joueurs...");
        //replace les portals dans le joueur
        FPinit.transform.SetParent(groupOnPlayer.transform);
        FPattract.transform.SetParent(groupOnPlayer.transform);
        FPrepulse.transform.SetParent(groupOnPlayer.transform);
        FPprojectile.transform.SetParent(groupOnPlayer.transform);*/
    }

    /// <summary>
    /// une fois le projectile ayant touché quelque chose... le désactiver !
    /// </summary>
    public void desactivePorjectile()
    {
        if (FPprojectile.activeSelf)
        {
            FPprojectile.GetComponent<ProjectileScript>().TWNEOC.stopNow();
            FPprojectile.SetActive(false);
            fired = false;
        }
    }

    /// <summary>
    /// à l'appui de la touche Y,
    /// switch les 2 portals existant...
    /// </summary>
    void switchPortals()
    {
        Vector3 tmpPosRepulse = FPrepulse.transform.position;
        Quaternion tmpRotationRepulse = FPrepulse.transform.rotation;

        FPrepulse.transform.SetPositionAndRotation(FPattract.transform.position, FPattract.transform.rotation);
        FPattract.transform.SetPositionAndRotation(tmpPosRepulse, tmpRotationRepulse);

        FPattract.SetActive(false);
        FPattract.SetActive(true);

        FPrepulse.SetActive(false);
        FPrepulse.SetActive(true);
    }

    /// <summary>
    /// le projectile a touché quelque chose...
    /// </summary>
    /// <param name="hit"></param>
    public void projectileHit(Vector3 pos, GameObject other, Vector3 dir)
    {
        CancelInvoke("stopProjectile"); //annule le temps d'attente pour kill le projectile...
        Debug.Log("ici creation de quelque chose ??");

        if (other.CompareTag("Portal"))          //on a touché un portal existant...
        {
           //si on a touché un portal ennemi
            if (other.GetComponent<PortalLinks>().refPlayer.PC.nb_player != PC.nb_player)
            {
                SoundManager.GetSingleton.playSound("PortalEnemyTouch" + PC.nb_player);
                other.GetComponent<PortalLinks>().refPlayer.PC.FP.deletePortal();
            }
        }
        else if (other.CompareTag("ParticleCollider") || other.CompareTag("EvilParticle") || other.CompareTag("Destructible"))
        {
            
        }
        else
        {
            
            HandlePortalCreation(pos, dir);
        }
    }

    /// <summary>
    /// nos portals on été détruit !
    /// </summary>
    public void deletePortal()
    {
        Debug.Log("ici delete portals");
        stopAttract();
        stopRepulse();
    }

    /// <summary>
    /// gère la création des portals à l'endroit du projectils
    /// </summary>
    /// <param name="hit"></param>
    void HandlePortalCreation(Vector3 pos, Vector3 dir)
    {
        
        if (connexion)                                  //s'il y a déja le portal repulse + attract qui n'a pas fini...
        {
            FPattract.SetActive(false);                                              //activer le projectile
            createAttract();
            createRepulse(pos, dir);
        }
        else if (repulsor)                                 //s'il y a déjà un portal repulsor de créé...
        {
            createAttract();
            createRepulse(pos, dir);
            connexion = true;
        }
        else                                            //sinon, il  n'y a rien du tout... créé un init (repulsor) !
        {
            createRepulse(pos, dir);
            repulsor = true;
        }
    }

    /// <summary>
    /// le portal attract n'emmet plus rien, le remetre à son stade initiale !
    /// </summary>
    public void stopAttract()
    {
        FPattract.SetActive(false);                                              //activer le projectile
        FPattract.transform.localScale = new Vector3(1, 1, 1);
        connexion = false;
    }

    /// <summary>
    /// annule le portal repulsif
    /// </summary>
    public void stopRepulse()
    {
        FPrepulse.SetActive(false);                                              //activer le projectile
        repulsor = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contact"></param>
    void createAttract()
    {
        //SoundManager.getSingularity().playSound("Create portal Repulse", -1);

        FPattract.GetComponent<TimeWithEffectOnCamera>().restart = true;        //restart le timer du portals

        FPattract.transform.position = FPrepulse.transform.position;
        FPattract.transform.rotation = FPrepulse.transform.rotation;

        //FPattract.transform.Rotate(0, 0, 180);

        FPattract.SetActive(true);                                              //activer le projectile
    }

    /// <summary>
    /// cree le portal repulse a la position pos, orienté à hit
    /// </summary>
    /// <param name="contact"></param>
    void createRepulse(Vector3 pos, Vector3 dir)
    {
        //SoundManager.getSingularity().playSound("Create portal Repulse", -1);
        
        FPrepulse.GetComponent<TimeWithEffectOnCamera>().restart = true;        //restart le timer du portals

        //ContactPoint contact = hit.contacts[0];

        //FPrepulse.transform.position = new Vector3(transform.position.x, transform.position.y, -0.52f);
        FPrepulse.transform.position = new Vector3(pos.x, pos.y, -0.52f); //position du portal repule au point d'impact
        FPrepulse.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir/*contact.normal*/);                   //rotation du portal repulse

        FPrepulse.transform.Rotate(0, 0, 180);
        FPrepulse.transform.eulerAngles = new Vector3(0, 0, FPrepulse.transform.eulerAngles.z);

        FPrepulse.transform.position += FPrepulse.transform.up * 0.5f;

        FPrepulse.SetActive(true);                                              //activer le projectile
    }

    /// <summary>
    /// tire un portals !
    /// </summary>
    public void FirePortal()
    {
        //si on a déja tiré y'a pas longtemp, ou on a déja tiré, ou on est en train de désactiver nos portals... ou ... "groupPortalsInit.transform.childCount" ?
        if (!TWNE.isOk || fired)
            return;
        CancelInvoke("stopProjectile"); //annule le temps d'attente pour kill le projectile...

        fired = true;                                                                   //on a tiré
        TWNE.isOk = false;                                                              //timer entre 2 coups
        SoundManager.GetSingleton.playSound("FirePortal" + PC.nb_player);

        //GameObject projectile = Instantiate(projectiles, spawnPosition.position/* + spawnPosition.up * 1*/, Quaternion.identity) as GameObject;

        FPprojectile.transform.position = spawnPosition.position;   //changer la position du projectile
        FPprojectile.SetActive(true);                               //activer le projectile

        //projectile.transform.parent = GroupPortals.transform;                       //set le projectil dans le group
        FPprojectile.transform.LookAt(spawnPosition.transform.position + spawnPosition.up * 2f);    //rotation du projectile ??

        Debug.DrawRay(spawnPosition.position, spawnPosition.up, Color.blue, 2.0f);

        Rigidbody RBP = FPprojectile.GetComponent<Rigidbody>();
        RBP.angularVelocity = Vector3.zero;
        RBP.velocity = Vector3.zero;

        RBP.AddForce(FPprojectile.transform.forward * speed);    //ajoute une force au projectile

        FPprojectile.GetComponent<ProjectileScript>().impactNormal = spawnPosition.up;

        Invoke("stopProjectile", 9.9f);

        //projectile.transform.GetChild(1).gameObject.GetComponent<TrapsController>().idTeam = PC.nb_team;
    }

    /// <summary>
    /// détruit le projectile s'il va trop loin !
    /// </summary>
    void stopProjectile()
    {
        if (fired)
        {
            desactivePorjectile();
            SoundManager.GetSingleton.playSound("ProjectilDesactived" + PC.nb_player);
        }
    }

    /// <summary>
    /// check si le player est out des ranges de ses portals...
    /// </summary>
    void checkIfPlayerOutOf()
    {
        /*if (NumberPortalInit == 1 && !lastPortalInit)
            resetAll();
        //s'il y a un portal init et qu'on est pas dans sa range...
        else if (NumberPortalInit == 1 && !isInRangeOkCreate(lastPortalInit.transform.position))
        {
            SoundManager.getSingularity().playSound("Player Out Of Range", -1);
            resetProjectilScript();                 //détruire tout les ProjectilsScript si y'en a !
            StateFire = 0;                          //reset les tirs à 0
            resetAll();
        }
        //s'il y a 2 portal actif et qu'on est pas dans la range du milieu des 2...
        else if (isPortalsActive)
        {
            float x1 = lastPortalAttract.transform.position.x;
            float y1 = lastPortalAttract.transform.position.y;
            float x2 = lastPortalRepulse.transform.position.x;
            float y2 = lastPortalRepulse.transform.position.y;
            Vector3 other = new Vector3(x1 + (x2 - x1) / 2, y1 + (y2 - y1) / 2, 0);
            if (!isInRangeOkCreate(other))
            {
                SoundManager.getSingularity().playSound("Player Out Of Range", -1);
                resetProjectilScript();                 //détruire tout les ProjectilsScript si y'en a !
                StateFire = 0;                          //reset les tirs à 0
                resetAll();
            }
        }*/
    }

    /// <summary>
    /// à chaque frame
    /// </summary>
    private void Update()
    {
        if (!PC.active)
            return;

        if (Time.fixedTime >= timeToGo)
        {
            checkIfPlayerOutOf();
            timeToGo = Time.fixedTime + timeOpti;
        }
    }
}
