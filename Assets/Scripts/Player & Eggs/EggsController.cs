using System.Collections;
using UnityEngine;

/*
edit:

see:

*/

[RequireComponent(typeof(Rigidbody))]                                       //requiert un rigidBody
[RequireComponent(typeof(Renderer))]                                        //requiert un Renderer pour pouvoir changer le material de l'oeuf !
[RequireComponent(typeof(TimeWithEffectOnCamera))]                          //cet objet peut être dans la liste de la caméra !
[RequireComponent(typeof(TimeWithNoEffect))]                                //cet objet requiert un TWNE pour limiter certaine action !
[RequireComponent(typeof(ObjectiveIndicator))]                              //requiert un OP pour l'affichage
public class EggsController : MonoBehaviour
{
    #region public variable
    [Header("Main")]
    [Range(0, 3)]
    public float speedChange = 0.5f;                    //le speed necessaire au changement de material (quand 2 Metal colide)
    [Range(0, 3)]
    public float speedFreez = 0.1f;                     //le temps en seconde de changement d'état, de NORMAL à FREEZ;
    [Range(0, 0.1f)]
    public float timeOpti = 0.1f;                       //optimisation
    [Range(0, 100.0f)]
    public float maxDragForChangingState = 20.0f;       //quand un oeuf est controllé par les répulsor,  sa drag augmente pourqu'il s'arrete progressivement, ce max sert à savoir quand arreter.
    public bool isCloseToWalls = false;                                     //vrai ou faux si les oeuf sont proche du mur

    [Space(10)]
    [Header("Debug")]
    public Material[] materials;               //liste des materials pour les oeufs
    public enum stateOfEggs                     //state courant de l'oeuf, d'abord à Normal. (état pour le materials: blanc, bleu, rouge, bleu kinetic
    {
        NONE = -1,
        Normal = 0,
        Repulsor = 1,
        Attractor = 2,
        BlackHole = 3,
        Flaire = 4,
        Kinematic = 5,
    }
    public stateOfEggs SOE = stateOfEggs.Normal;
    public enum stateOfParticleEggs                                             //state courant de la particule de l'oeuf... d'abord à Normal (rien)
    {
        NONE = -1,
        Normal = 0,
        Repulsor = 1,
        Attractor = 2,
        BlackHole = 3,
        Flaire = 4,
        RepulsorStatic = 5,
        Kinematic = 6,
        KinematicExplose = 7,
    }
    public stateOfParticleEggs SOPE = stateOfParticleEggs.Normal;

    public bool isKinematicSave = false;                                        //sauvegarde si on a été kinetic une fois
    public bool isControlled = false;                                           //Pour savoir si l'oeuf est controllé par un joueur
    public bool isFirstInfected = false;                                        //variable qui défini si l'oeuf est infecté (le début de l'effection)    
    public bool isGreenControlled = false;                                      //l'oeuf est-il controllé par un flaire
    public bool isPushedByOther = false;                                        //si l'oeuf est controllé mais est poussé par autre chose

    public GameObject GroupEggs;                            //position du parent.
    public GameObject GroupPoolEggs;                        //position du pool

    [Space(10)]
    [Header("particle")]
    //int currentParticle = -1;   //particule courante du l'oeuf
    public GameObject[] particles;                                              //tableau des différentes particules de l'oeuf
    public GameObject deadParent;                                               //objet contenant les particule Dead de l'oeuf
    public GameObject[] dead;                                                   //liste des particule dead de l'oeuf
    public bool isInKeep = false;                                               //pour savoir si l'oeuf est dans la coroutine KeepAddingForce ou pas
    public bool addNotTwice = false;                                            //pour ne pas effectuer 2 fois l'action trigger dans la reine...
    public float timeBeforeKillWhenKinematic = 10.0f;                           //détruit l'oeuf après 10 seconde de konematic
    public bool stopControlGreen = false;                                      //l'oeuf est "controllé" par les répulseur de la fourmis, sans pour autan têtre controllé par le trail !

    public listEmitter listEmitt; //la list des emmiteur du joueur
    #endregion


    #region private variable
    private stateOfEggs oldPlayer = stateOfEggs.NONE;                            //old temporaire pour déterminer quand le state change
    private stateOfParticleEggs oldParticlePlayer = stateOfParticleEggs.NONE;    //old temporaire pour déterminer quand le state change
    [HideInInspector]    public TimeWithEffectOnCamera TWEOC;
    [HideInInspector]    public TimeWithNoEffect TWNE;
    [HideInInspector]    public GameObject playerFocus;
    [SerializeField] private GameObject prefabsCanvasAddOne;                    //canvas "+1" quand un oeufs entre dans la reine
    private GameObject gameController;
    private GameObject variableGlobal;
    private PlayerConnected PC;
    //private float speed = 0;                                                    //current speed
    //Vector3 lastPosition = Vector3.zero;                                        //tmp last position for have speed of gameObject
    private Rigidbody Rb;
    private Renderer rend;
    private float timeToGo;
    private bool breakAll = false;                                              //break keepAddingForce when Dead !!
    private ObjectiveIndicator OP;                                              //l'objective Indicator de l'oeuf !

    private bool stopControlYellow = false;

    private float timeControlYellow = 0;
    private float timeControlGreen = 0;

    
    #endregion

    #region initialisation
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        variableGlobal = GameObject.FindGameObjectWithTag("Global");
        if (variableGlobal)
            PC = variableGlobal.GetComponent<PlayerConnected>();
        GroupEggs = GameObject.FindGameObjectWithTag("GroupEggs");
        GroupPoolEggs = GameObject.FindGameObjectWithTag("GroupPoolEggs");
        Rb = gameObject.GetComponent<Rigidbody>();
        rend = gameObject.GetComponent<Renderer>();
        TWEOC = gameObject.GetComponent<TimeWithEffectOnCamera>();
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
        OP = gameObject.GetComponent<ObjectiveIndicator>();
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
    }
    #endregion

    #region core script

    /// <summary>
    /// change le matérial de l'oeuf
    /// </summary>
    /// <param name="newMat"></param>
    private void SetMaterial(Material newMat)
    {
        if (rend && newMat)
        {
            rend.material = newMat;
        }
    }

    /// <summary>
    /// lorsque les joueurs ou pièges applique une force à l'oeuf,
    /// test si l'oeuf est dans une trail d'un joueur, si oui, set la variable
    /// isPushedByOther à vrai pour dire au trail d'avoir moin de force pour
    /// cette oeuf la.
    /// </summary>
    public void checkIfIsPushWhenInTrail()
    {
        if (isGreenControlled)
            isPushedByOther = true;
    }

    /// <summary>
    /// change le stage de l'oeuf (le material !)
    /// Attention, il peut y avoir 4 joueurs Bleu, cela ne définit pas le score !!
    /// </summary>
    /// <param name="state">type de state, de 1 à 4</param>
    public void changeCurrentPlayer(int stateMaterial)
    {
        SOE = (stateOfEggs)stateMaterial;
    }

    public int getCurrentPlayer()
    {
        return (int)SOE;
    }

    /// <summary>
    /// la verte est-elle controllé ?
    /// par les 
    /// </summary>
    public void greenControlled(bool controlled)
    {
        if (particles[4].activeSelf != controlled)
            particles[4].SetActive(controlled);
        if (controlled)
        {
            stopControlGreen = true;
            timeControlGreen = Time.fixedTime + 0.3f;
        }
        else
        {
            stopControlGreen = false;
            //
        }
    }

    /// <summary>
    /// la verte est-elle controllé ?
    /// par les 
    /// </summary>
    public void greenControlledTestStop()
    {
        if (stopControlGreen && Time.fixedTime >= timeControlGreen/* && !isGreenControlled*/)
        {
            greenControlled(false);
            stopControlOfGreen();
        }
    }

    /// <summary>
    /// la verte est-elle controllé ?
    /// par les 
    /// </summary>
    public void yellowControlled(bool controlled)
    {
        particles[3].SetActive(controlled);
        if (controlled)
        {
            stopControlYellow = true;
            timeControlYellow = Time.fixedTime + 0.3f;
        }
        else
        {
            stopControlYellow = false;
        }
    }

    /// <summary>
    /// la verte est-elle controllé ?
    /// par les 
    /// </summary>
    public void yellowControlledTestStop()
    {
        if (stopControlYellow && Time.fixedTime >= timeControlYellow)
        {
            //timeControlGreen = Time.fixedDeltaTime + 1;
            yellowControlled(false);
        }
    }

    /// <summary>
    /// la verte est-elle controllé ?
    /// </summary>
    public void greenStopped(bool stopped)
    {
        if (stopped)
            Rb.velocity = Rb.velocity / 2;
        if (particles[8].activeSelf != stopped)
            particles[8].SetActive(stopped);
    }



    /// <summary>
    /// change la particule courante de l'oeuf, particle = 1 à 4, SOPE = 1 à 4 pour les principale particule d'interaction
    /// </summary>
    /// <param name="particle"></param>
    public void changeCurrentParticle(int particle)
    {
        SOPE = (stateOfParticleEggs)particle;
    }

    /// <summary>
    /// Si l'oeuf à un playerFocus, cela veut dire qu'on est en mode multi, et que l'oeuf est d'une couleur
    /// d'un joueur... Soustraire à ce joueur 1 points
    /// </summary>
    public void stopControlOfGreen()
    {
        //Debug.Log("from eggs 2");
        isGreenControlled = false;
        isPushedByOther = false;
        Rb.useGravity = true;
        greenControlled(false);
        greenStopped(false);
        //Debug.Log("sub score ? DO NOTHING FOR NOW");
        //if (playerFocus)
        //  gameController.GetComponent<GameManager>().subToScore(playerFocus.GetComponent<PlayerController>().getNbPlayer(), 1);
    }

    /// <summary>
    /// retourne la statue de SOPE,
    /// de 1 à 4 c'est les joueurs
    /// </summary>
    /// <returns></returns>
    public int getCurrentParticle()
    {
        return ((int)SOPE);
    }

    /// <summary>
    /// Change la PARTICULE (et restart le TWNE).
    /// </summary>
    private void ChoseParticle()
    {
        if (oldParticlePlayer != SOPE)                                  //si la particule à changé d'avant, l'activer !
        {
            for (int i = 0; i < particles.Length; i++)                  //parcours la liste des particules
            {
                if (SOPE == stateOfParticleEggs.NONE || SOPE == stateOfParticleEggs.Normal)      //si SOPE est à NONE (-1) ou normal (0) -> par exemple le début, ou lorsque l'oeuf devient NON-controllé), désactiver toute particule courante
                {
                    if (particles[i] && particles[i].activeSelf)        //vérifie si elle existe et est activé
                        particles[i].SetActive(false);                  //désactive
                }
                else                                                    //sinon il y a une particule à mettre !
                {
                    //Si dans le parcourt de particles[index], l'index est égale au type: activer seulement la particule current, et désactiver les autres
                    if (i == (int)SOPE)                                 //si on est arrivé à l'index de la bonne particule (de 1 à 7)
                    {
                        if (particles[i] && !particles[i].activeSelf)
                            particles[i].SetActive(true);               //activer la particule relative à l'index SOPE

                        //cas particulier pour le kinematic (: activer aussi celle qui explose !
                        if (i == 6 && particles[6] && !particles[6].activeSelf) //Kinematic = 6
                            particles[7].SetActive(true);               //activer la particule kiinetic explose (KinematicExplose = 7)

                        TWEOC.restart = true;                           //restart le compteur TWEOC, comme ça l'oeuf reste dans la liste Targets de la caméra, et dès qu'elle ne l'est plus: le compteur commence pendant X seconde et la camera reste encore un peu sur elle     
                    }
                    else                                                //sinon, désactiver la aprticule
                    {

                        if (i != (int)stateOfParticleEggs.KinematicExplose  //si c'est la particule d'explosion, la laisser active, elle va se désactiver toute seule...
                            && particles[i] && particles[i].activeSelf)     //pour les autres, si elle est activé...
                            particles[i].SetActive(false);                  //désactiver la particule
                    }

                }
            }
            oldParticlePlayer = SOPE;                               //à la fin, mettre le tmp à la particule courante pour ne pas effectuer l'action 2 fois.
        }

        //le Metal est kinetic et la particule qui devrai boucler ne boucle pas...
        if (SOE == stateOfEggs.Kinematic && !particles[6].activeSelf)//(Kinematic = 6)
        {
            particles[6].SetActive(true);                           //activer la particule bleu-kinetic qui loop (Kinematic = 6)
            particles[7].SetActive(true);                           //activer la particule kinetic qui explose (KinematicExplose = 7)
        }
        //si la particule est kinetic mais que la particle qui explose n'est pas activé, forcer l'activation !
        //elle s'auto-détruit de toute façon
        if (SOE == stateOfEggs.Kinematic && particles[7] && !particles[7].activeSelf)         //(KinematicExplose = 7)
            particles[7].SetActive(true);
    }

    /// <summary>
    /// Change le MATERIAL, si il est kinetic, forcer à 5 et mettre le layer à 0 pour éviter les raycasts
    /// </summary>
    private void ChoseMaterial()
    {
        if (oldPlayer == SOE)
            return;
        // /!\ si c'est kinématic, l'objet est obligatoirement immobile (glacé) donc = 5;
        if (isKinematicSave)
            SOE = stateOfEggs.Kinematic;

        switch (SOE)                                                            //test l'état du matérial
        {
            case stateOfEggs.Normal:                                            //état initial
                if (rend.material != materials[0])                              //si current = 0 changer le material en blanc
                    SetMaterial(materials[0]);
                break;

            case stateOfEggs.Repulsor: //état type 1 (blue)
                if (rend.material != materials[1])                              //si current = 1 changer le material en bleu
                    SetMaterial(materials[1]);
                break;

            case stateOfEggs.Attractor: //état type 2 (red)
                if (rend.material != materials[2])                              //si current = 2 changer le material en rouge
                    SetMaterial(materials[2]);
                break;

            case stateOfEggs.BlackHole: //état type 3 (blackHole)
                if (rend.material != materials[3])                              //si current = 2 changer le material en blackHole
                    SetMaterial(materials[3]);
                break;

            case stateOfEggs.Flaire: //état type 4 (flaire)
                if (rend.material != materials[4])                              //si current = 4 changer le material en Flaire
                    SetMaterial(materials[4]);
                break;

            case stateOfEggs.Kinematic: //état freezed
                if (rend.material != materials[5])                              //si current = 5, changer le material a Freezed, et procéder aux actions lorsque l'oeuf freez la premiere fois
                {
                    SetMaterial(materials[5]);
                    ActionWhenFreezed(true);
                }
                break;
        }
        oldPlayer = SOE;
    }

    /// <summary>
    /// Fonction qui s'exécute une fois quand l'eggs est freezed par un piège.
    /// Pour l'instant, juste désactive son layer pour qu'il ne soit plus raycasté par les pièges / joueurs
    /// </summary>
    void ActionWhenFreezed(bool active)
    {
        if (active)
        {
            Rb.isKinematic = true;
            gameObject.layer = 0;           //set le layer à 0 pour éviter les raycast dessus
        }
        else
        {
            Rb.isKinematic = false;
            gameObject.layer = 8;           //set le layer à 0 pour éviter les raycast dessus
        }
    }

    public void StopBeingControlled()
    {
        isControlled = false;
        playerFocus = null;
        isInKeep = false;
        //breakAll = true;                                                                //force l'arrêt de la coroutine keepAddingForce
        StopCoroutine("keepAddingForce");                                               //TODO: si ça marche, enlever breakAll ?
        Rb.drag = 0;
        SOPE = stateOfParticleEggs.NONE;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Eggs")
        {
            //si METAL contre METAL et un BLANC touche une COULEUR, SET le material a la couleur de l'autre
            if (SOE == stateOfEggs.Normal && other.GetComponent<EggsController>().SOE != stateOfEggs.Normal)
            {
                SOE = other.GetComponent<EggsController>().SOE;
            }
            else
            {
                //Si la vitesse de l'un est Assez grand par rapport à l'autre...
                if (Mathf.Abs(speed - other.GetComponent<EggsController>().speed) > speedChange)
                {
                    //set le current à celui qui va le plus vite !
                    if (speed < other.GetComponent<EggsController>().speed)
                        SOE = other.GetComponent<EggsController>().SOE;
                    else
                        other.GetComponent<EggsController>().SOE = SOE;
                }
            }
        }
    }*/

    public void setKinetic(bool isKine)
    {
        //Debug.Log(gameObject.name);
        if ((isKinematicSave && isKine) || (!isKinematicSave && !isKine))
            return;
        Rb.isKinematic = isKine;
        isKinematicSave = isKine;
        isControlled = false;
        oldPlayer = stateOfEggs.NONE;
        SOPE = stateOfParticleEggs.Kinematic;
        ChoseParticle();
        StartCoroutine(waitBeforKill());
    }

    IEnumerator waitBeforKill()
    {
        yield return new WaitForSeconds(timeBeforeKillWhenKinematic);
        Debug.Log("waitToBeKill  ?");
        destroyThis();
    }

    public IEnumerator waitToSetKinematic()
    {
        yield return new WaitForSeconds(speedFreez);    //Wait one frame
        setKinetic(true);
    }

    //get and set le speed courant de l'object
    /*private void changeSpeedOfObject()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }*/

    private bool isOutOfRange()
    {
        if (!playerFocus)
            return (true);
        //test rangeDistance of playerFocus, si trop long: isControlled = false, currentParticle = -1, return true;
        Vector3 offset = playerFocus.transform.position - transform.position;
        float sqrLen = offset.sqrMagnitude;                                                             //récupère la distance² du joueur/métal
        float rangeDist = playerFocus.GetComponent<PlayerController>().rangeDistance;                   //récupère rangeDist du joueur
        if (rangeDist == -1)                                                                            //si rangeDist de player = -1, ne prend pas en compte la distance
            return (false);
        if (sqrLen > rangeDist * rangeDist)                                                             //si la distance est supérieur au joueur, désactiver le contrôles
        {
            isControlled = false;                                                                       //définir l'oeuf comme non controlé
            SOPE = stateOfParticleEggs.NONE;                                                            //change la particule en indéfini
            return (true);
        }
        return (false);
    }

    private void testIfStillControlled(float coef = 1)
    {
        if (!isControlled || !playerFocus || !gameObject || isOutOfRange())
            return;
        //Debug.Log("controlled by: " + playerFocus.GetComponent<PlayerController>().nb_player);
        PlayerController PCC = playerFocus.GetComponent<PlayerController>();
        if (PCC)
        {
            Vector3 forceDir = Vector3.zero;
            if (PCC.getTypePowerPlayer() == 1)
                //forceDir = -playerFocus.GetComponent<PlayerController>().GetVectorForce();
                forceDir = -playerFocus.transform.up * PCC.strengthOfAttractionWhenPressed * 1;// PCC.getRb().mass;
            else if (PCC.getTypePowerPlayer() == 2)
            {
                //forceDir = playerFocus.GetComponent<PlayerController>().GetVectorForce();
                forceDir = -(playerFocus.transform.position - transform.position) * PCC.strengthOfAttractionWhenPressed * 1;// PCC.getRb().mass;
            }

            Debug.DrawRay(transform.position, forceDir, Color.green);
            Rb.AddForce(forceDir * coef);
        }

        /*
        Vector3 forceDir = -playerFocus.GetComponent<PlayerController>().GetVectorForce();
        Debug.DrawRay(transform.position, forceDir, Color.green);
        Rb.AddForce(forceDir * coef);
        */
    }

    /// <summary>
    /// fonction qui est appelé lorsque l'oeuf est HORS RANGE mais encore controllé par le bleu ou le rouge
    /// </summary>
    /// <param name="playerToFocus"></param>
    /// <returns></returns>
    public IEnumerator keepAddingForce(GameObject playerToFocus)
    {
        playerFocus = playerToFocus;
        if (!playerFocus)
            yield break;
        isInKeep = true;                                                                    //défini qu'on est à l'intérieur de l'IEnumerator
        PlayerController PCC = playerFocus.GetComponent<PlayerController>();
        if (PCC)
        {
            for (int i = 0; i < PCC.itterationWhenPressed; i++)
            {
                if (breakAll)                                                               //si l'oeuf est en train d'être détruit, sort d'ici !
                    yield break;

                //si le bouton du joueur lache, ou que sa particule courante est devenu -1, ou si il devient kinetic (5) ou kinetic explose(6)
                if (PCC.releaseMetalicaChild || SOPE == stateOfParticleEggs.NONE || SOPE == stateOfParticleEggs.Kinematic || SOPE == stateOfParticleEggs.KinematicExplose/* || SOPE == stateOfParticleEggs.RepulsorStatic*/)
                    break;
                if (PC && PCC)
                    PC.VibrationGamePad(PCC.nb_player, PCC.vibrateLeftFiring, 0, PCC.LeftFiringTiming);
                testIfStillControlled();

                if (!Rb)
                    break;
                Rb.drag = Rb.drag + playerFocus.GetComponent<PlayerController>().weakeningOverTime * i;

                TWEOC.restart = true;                                                           //restart la caméra tant que l'oeuf est controllé

                if (Rb.drag > maxDragForChangingState && SOPE == stateOfParticleEggs.Repulsor)  //si la Drag de l'oeuf est suppérieur au max && la particule courante est Repulsor (le premier)
                {
                    SOPE = stateOfParticleEggs.RepulsorStatic;                                  //défini la particule en blue static (RepulsorStatic = 5)
                }
                if (Rb.drag > maxDragForChangingState + maxDragForChangingState / 2 && SOPE == stateOfParticleEggs.RepulsorStatic)
                    break;

                yield return new WaitForSeconds(playerFocus.GetComponent<PlayerController>().timeBetweenItterationWhenPressed);
            }
        }
        //Debug.Log("ici particle:" + currentParticle);
        //////////////////////////////////// le contrôle est définitivement fini
        SOPE = stateOfParticleEggs.NONE;                                                        //enlever toute sorte de partcule sur l'oeuf
        if (Rb && Rb.drag > 0)                                                                  //si l'oeuf contient un rigidbody...
        {
            Rb.drag = 0;                                                                        //remettre la drag à 0 (au cas ou elle a été augmenté)
            //Rb.angularVelocity = Vector3.zero;
        }
        if (PCC && PCC.projectAtTheEnd && playerFocus)                                          //si la variable projectAtTheEnd est true...
            testIfStillControlled(playerFocus.GetComponent<PlayerController>().coefWhenStop);   //ajouter une dernière ultime force

        isControlled = false;                                                                   //désactiver le controle de l'oeuf par un quelquonque player

        if (PCC)
            PCC.destroyMetalByGameObject(gameObject);                                            //se supprime de l'array du player

        isInKeep = false;                                                                       //on sort définitivement de l'Ienumerator
    }
    #endregion

    #region unity function and ending

    /// <summary>
    /// lors de la collision avec quelque chose...
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.SS.playSound(listEmitt.getInList(0));  //lance un son de collision
    }

    /// <summary>
    /// update/// à chaque frame
    /// </summary>
    void Update()
    {
        if (Time.fixedTime >= timeToGo)
        {
            ChoseMaterial();            //change le material par rapport à la variable currentPlayer + weasted
            testIfStillControlled();     //si blue est encore controllé, set la direction du playerFocus
            ChoseParticle();            //choisi la bonne particule

            yellowControlledTestStop();
            greenControlledTestStop();

            timeToGo = Time.fixedTime + timeOpti;
        }
        //changeSpeedOfObject();    //get le speed de l'oeuf
    }

    /// <summary>
    /// enlève le kinematic
    /// </summary>
    void unSetKinematic()
    {
        StopCoroutine(waitBeforKill());
        isKinematicSave = false;
        Rb.isKinematic = false;
        particles[7].SetActive(false);
        particles[6].SetActive(false);
        ActionWhenFreezed(false);
    }

    /// <summary>
    /// à l'activation de l'oeuf
    /// (au début du jeu, ou au spawn de l'oeuf)
    /// </summary>
    private void OnEnable()
    {
        //Debug.Log("ici enable eggs");
        gameObject.transform.SetParent(GroupEggs.transform);
        OP.hideOutofScreen = false;          //réafficher l'objective pointer
        breakAll = false;                                                               //réactiver la variable parmettant de "casser" la coroutine
        dead[(int)SOE].transform.SetParent(deadParent.transform);                       //remet le parent de la dernière particule su rl'oeuf
        dead[(int)SOE].transform.transform.position = gameObject.transform.position;    //remet sa position sur l'oeuf !
        SOE = stateOfEggs.Normal;                                                       //change l'état à normal
        oldPlayer = stateOfEggs.NONE;                                                   //change l'état à NNONE
        Rb.velocity = Vector3.zero;                                                     //inhibe la velocité
        Rb.angularVelocity = Vector3.zero;
        Rb.drag = 0f;                                                                   //inhibe le drag
        unSetKinematic();
    }

    /// <summary>
    /// lors de la désactivation de l'oeuf
    /// </summary>
    private void OnDisable()
    {
        OP.hideOutofScreen = true;           //cacher l'objective pointer (le supprime juste après !)
    }

    /// <summary>
    /// Attend avant de crever !
    /// </summary>
    /// <param name="fromQueen"></param>
    /// <returns></returns>
    IEnumerator waitBeforeDeath(bool fromQueen)
    {
        OP.hideAfterDeath();    //cacher l'objective pointer (le supprime juste après !)
        yield return new WaitForSeconds(0.3f);
        unSetKinematic();


        dead[(int)SOE].transform.SetParent(gameController.transform);                   //change le parent de la particule par rapport au monde (le gameController)
        if (!fromQueen)
            dead[(int)SOE].SetActive(true);                                                 //active la particule de destruction (par rapport à son type de metal)
        else
        {
            Vector3 pos = gameObject.transform.position;
            //pos.y += 35f;

            pos.x += Random.Range(-3, 3);
            pos.y += Random.Range(-3, 3);

            Instantiate(prefabsCanvasAddOne, pos, Quaternion.identity);
        }
        breakAll = true;                                                                //force l'arrêt de la coroutine keepAddingForce
        StopCoroutine("keepAddingForce");                                               //"casse" la coroutine
        TWNE.isOk = false;                // what ???
        isControlled = false;                                                           //l'oeuf n'est plus controllé
        if (playerFocus)                                                                //s'il existe un playerFocus, appelle sa fonction pour se supprimer de son array
            playerFocus.GetComponent<PlayerController>().destroyMetalByGameObject(gameObject);
        playerFocus = null;                                                             //enlève le playerFous s'il y en a
        TWEOC.stopNow();                                                                //s'enlève de la caméra tout de suite
        changeCurrentParticle(-1);                                                      //change la particule courante de l'oeuf à: rien
        gameObject.transform.SetParent(GroupPoolEggs.transform);                        //changer le parent de l'oeuf dans la pool !
        gameObject.SetActive(false);
    }

    /// <summary>
    /// s'occupe de détruire l'oeuf
    /// </summary>
    public void destroyThis(bool fromQueen = false)
    {
        StartCoroutine(waitBeforeDeath(fromQueen));
    }
    #endregion
}
