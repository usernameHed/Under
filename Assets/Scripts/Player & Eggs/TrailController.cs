using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(TimeWithNoEffect))]        //ce script a besoin d'un TWNE :
public class TrailController : MonoBehaviour
{
    #region variables public
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    public float pointSpacing = 0.1f;                                           //espace entre 2 création de points
    public int minPoint = 5;                                                    //minimum de points de la trail
    public float startTime = 2.0f;                                              //temps avant que la trail ne commence à disparaitre
    public float repeateTime = 0.05f;                                            //interval de temps pour qu ela trail disparaisse
    public float widthTrail = 0.2f;                                             //size de la trail et du collider trails
    public float widthTrailCollider = 0.5f;                                             //size de la trail et du collider trails
    public float margeOfErrorDistance = 100.0f;                                 //si le joueur se "téléporte" d'une distance de X, alors ne pas créé de trail entre les 2 points !
    [Range(0, 999)] public int amountToSub = 5;                                               //montant à enlever quand on fait la trail
    [Range(0, 999)] public int amountToAdd = 10;                                              //montant à ajouter quand la trail reload
    [Range(0, 999)] public int amountBeforeRecreate = 10;                                       //montant d'attante de la recharge avant de recréé une trails
    public float forceTrail = 5.0f;                                             //force de poussé du trail
    public int numberPointInFront = 5;                                          //nombre de point à aller chercher devant ou derrière
    [Range(1, 100)] public float forceToDivideWhenPushed = 10;                  //division de la force du trail
    public GameObject trailPlayer;                                              //reference du group de trail du joueur
    public GameObject progressBarGreen;                                         //objet qui contient le progressBar
    public int minBeforeAttract = 10;                                            //nombre de point min iavant d'attirer !
    public GameObject prefabsCollider;                                          //prefabs du colliders particule
    public int moduloForLine = 4;                                               //l'espace entre X point où créé un nouveau collider

    public struct TrailStruct                                                   //structure d'une trails
    {
        public List<Vector2> points;                                            //la liste de points de la trail
        public GameObject trailObject;                                          //l'objet qui contient le line et le col
        public LineRenderer line;                                               //lineRenderer de l'objet
        public bool isFront;                                                    //est-ce une trail rpédéfini ?
    }

    public List<TrailStruct> listTrails = new List<TrailStruct>();              //cree une liste des infos des trails


    //public List<GameObject> listFrontTrail = new List<GameObject>();            //list des 2 trails front !
    //public List<TrailStruct> listTrailsFront = new List<TrailStruct>();         //cree une liste des infos des trails front (left/right)
    [SerializeField] private Transform GroupTrailParticle;                      //transform du parent contenan ttout les trails particle
    [SerializeField] private GameObject prefabsTrailsParticle;                 //le rpefabs du trails particle
    [SerializeField] private GameObject frontTrail;                             //groupe des attracteurs
    //public GameObject explodeDebug;

    [HideInInspector] public ColoredProgressBar CPB;
    #endregion

    #region variables privé serealized
    [SerializeField] private GameObject prefabsTrail;                           //prefabs du trails
    [SerializeField] private Transform playerAnts;                              //link du player
    [SerializeField] private GameObject prefabsMinAttract;                      //prefabs de l'attracteur mini en début de trail
    #endregion

    #region variables privé
    //variables
    private bool released = true;                                               //reload ou pas la nitro
    private float timeToGo;
    //private GameObject TrailsParticleCreating;                                  //particleTrail en cours de création...
    //private List<GameObject> trailParticleList = new List<GameObject>();        //liste des anciennes trails                        

    //objects
    [HideInInspector] public PlayerController PC;                                            //ref du script du player
    //private GameObject Trail;                                               //l'objet qui contient le linerender et le collider
    //private LineRenderer line;
    #endregion

    private void Awake()
    {
        PC = playerAnts.gameObject.GetComponent<PlayerController>();
        //Trail = GameObject.FindGameObjectWithTag("Trail");                  //objet contenant les line et col
        //line = Trail.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        if (progressBarGreen)
            progressBarGreen.SetActive(true);
        //InvokeRepeating("setPosOfLine", 0, 1.0f);
        //points.Clear();
        //setPoints();                                                        //commence et set le premier points
    }

    public void powerFlare(int state)
    {
        switch (state)
        {
            case 0:                         //press pour la premiere fois
                PC.speed = PC.speedFlaire;
                SoundManager.SS.playSound(PC.listEmitt.getInList(1));
                //SoundManager.SS.playSound("Green Power Start", PC.nb_player, CPB.getProgress() * 1.0f / 1000.0f);  //commence le son avec le start courrant de vitesse
                createTrail();
                break;
            case 1:                         //continue de presser...
                continiueTrail();
                break;
            case 2:                         //relache !
                PC.speed = PC.oldSpeed;
                SoundManager.SS.playSound(PC.listEmitt.getInList(1), true);
                //SoundManager.SS.playSound("Green Power End", PC.nb_player, CPB.getProgress() * 1.0f / 1000.0f);
                releaseTrail();
                break;
            case 3:
                PC.speed = PC.oldSpeed;
                SoundManager.SS.playSound(PC.listEmitt.getInList(2));
                //SoundManager.SS.playSound("Green Power Stop", PC.nb_player, CPB.getProgress() * 1.0f / 1000.0f);
                clearAll();                 //kill all !
                break;
        }
    }

    /// <summary>
    /// initialisation des trails
    /// </summary>
	public void createTrail()
    {
        released = false;
        frontTrail.SetActive(true);

        List<Vector2> points = new List<Vector2>();                                 //cree une nouvelle liste de points

        GameObject newTrail = Instantiate(prefabsTrail) as GameObject;              //créé un trail pour le player !
        newTrail.transform.parent = trailPlayer.transform;                          //set le trail dans le groupe du joueur

        TrailStruct trailStruct;                                                    //cree une nouvelle structure qui va contenir les infos du trails
        trailStruct.trailObject = newTrail;                                         //link l'objet de reference
        trailStruct.points = points;                                                //link la liste de points
        trailStruct.line = newTrail.GetComponent<LineRenderer>();                   //link la line
        newTrail.GetComponent<LineRenderer>().startWidth = widthTrail;              //set la size de la trail
        //newTrail.GetComponent<EdgeCollider2D>().edgeRadius = widthTrailCollider;    //set la size du collider trail
        //trailStruct.col = newTrail.GetComponent<EdgeCollider2D>();                  //link le edge collider
        trailStruct.isFront = false;                                                //ce n'est pas un trial de front !
        listTrails.Add(trailStruct);                                                //ajoute la structure à la liste de structure (liste de trail !)

        newTrail.GetComponent<TrailDeletePoints>().index = listTrails.Count - 1;    //set l'index
        newTrail.GetComponent<TrailDeletePoints>().TC = this;                       //set la référence du controller

        GameObject miniAttract = Instantiate(prefabsMinAttract) as GameObject;      //cree le mini attracteur
        miniAttract.transform.parent = newTrail.transform;                          //set le parent du trail cree
        miniAttract.transform.position = gameObject.transform.position;             //set la position courrante du joueur
        miniAttract.GetComponent<TrapsController>().idTeam = PC.nb_team;            //set la team de l'attracteur (pour que la trail attire uniquement les enemi)

        //créé une particule trail pour le player !
        //TrailsParticleCreating = Instantiate(prefabsTrailsParticle, gameObject.transform.position, gameObject.transform.rotation, GroupTrailParticle) as GameObject;
        //trailParticleList.Add(TrailsParticleCreating);

        setPoints(listTrails.Count - 1);                                            //init le points
        //setCollider(listTrails.Count - 1);
    }

    /// <summary>
    /// continuaton des trails
    /// </summary>
    public void continiueTrail()
    {
        if (released)
        {
            if (CPB.getProgress() > amountBeforeRecreate)
            {
                
                powerFlare(0);
            }  
            return;
        }
            
        released = false;
        if (listTrails.Count == 0 || !listTrails[listTrails.Count - 1].line)
        {
            clearAll();
            powerFlare(0);
            Debug.Log("line too short ! recreate one !!");
            return;
        }

        CPB.setProgress(CPB.getProgress() - amountToSub);                     //diminue 1

        if (CPB.getProgress() == 0)
        {
            //supprime la trail courrante ?
            releaseTrail();
            PC.speed = PC.oldSpeed;
            //powerFlare(0);
            return;
        }

        Vector2 offset = listTrails[listTrails.Count - 1].points.Last() - new Vector2(playerAnts.position.x, playerAnts.position.y);
        float sqrLen = offset.sqrMagnitude;
        if (sqrLen > pointSpacing * pointSpacing)
        {
            //ici si la distance est trop grande, fermer la ligne et en recreer une !
            if (sqrLen > (pointSpacing * pointSpacing * margeOfErrorDistance))
            {
                Debug.Log("error too long !");
                powerFlare(2);  //relacher la trail courante
                //powerFlare(0);  //cree une nouvelle trail ! :)
            }
            else
            {
                setPoints(listTrails.Count - 1);                    //création d'un nouveau points
                if (listTrails[listTrails.Count - 1].points.Count % moduloForLine == 0)
                    setCollider(listTrails.Count - 1);                    //création d'un nouveau points
            }



        }
        else
            addTimeToTrail();                                   //ajoute du temps à tous les trails

        /*TrailStruct tmp = listTrails[listTrails.Count - 1];
        tmp.fiveStep++;
        listTrails[listTrails.Count - 1] = tmp;

        //trailStruct.fiveStep = 0;*/
    }
    
    /// <summary>
    /// ajoute du temps à tous les trails du joueur
    /// </summary>
    void addTimeToTrail()
    {
        if (Time.fixedTime >= timeToGo)
        {
            for (int i = 0; i < listTrails.Count; i++)
            {
                if (listTrails[i].trailObject)
                    listTrails[i].trailObject.GetComponent<TrailDeletePoints>().timeToWait += listTrails[i].trailObject.GetComponent<TrailDeletePoints>().timeToAdd;
            }

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    /// <summary>
    /// relacher la trails
    /// supprime la dernière trails is elle est trop courte !
    /// </summary>
    public void releaseTrail()
    {
        released = true;
        frontTrail.SetActive(false);

        /*if (TrailsParticleCreating)
        {
            TrailsParticleCreating.transform.SetParent(null);
            Destroy(TrailsParticleCreating, 5.0f);
        }*/
            

        //si la dernière trais est trop courte, la supprimer !
        if (listTrails.Count == 0)
            return;
        if (listTrails[listTrails.Count - 1].points.Count < minPoint)
        {
            if (trailPlayer.transform.childCount > 0)
                Destroy(trailPlayer.transform.GetChild(trailPlayer.transform.childCount - 1).gameObject);
            listTrails.RemoveAt(listTrails.Count - 1);
        }
    }

    /// <summary>
    /// kill all trail
    /// </summary>
    public void clearAll()
    {
        foreach (Transform child in trailPlayer.transform)
            Destroy(child.gameObject);

        /*foreach (GameObject child in trailParticleList)
            Destroy(child);
        trailParticleList.Clear();*/

        listTrails.Clear();
        frontTrail.SetActive(false);
        released = true;
    }

    /// <summary>
    /// est appelé par chaque trail pour supprimer un point de leur trails
    /// </summary>
    /// <param name="indexTrail">index de la trail</param>
    /// <param name="indexPoint">index du point dans la trail</param>
    public bool deletePoint(int indexTrail, int indexPoint)
    {
        //Debug.Log("delete points 0 de: " + indexTrail);
        //update les listes sauvegardé dans les données
        listTrails[indexTrail].points.RemoveAt(indexPoint);                                         //change les points

        //s'il n'y a pas assez de points dans la liste, delete le trails !
        if (listTrails[indexTrail].points.Count < minPoint)
        {
            releaseTrail();
            return (false);
        }
        //listTrails[indexTrail].col.points = listTrails[indexTrail].points.ToArray<Vector2>();       //change les collider

        if (listTrails[indexTrail].points.Count % moduloForLine == 0)
        {
            //si le trail à un enfant, c'est l'attract ! le supprimer quand le trail commence à disparaitre !
            if (listTrails[indexTrail].trailObject.transform.childCount > 0/* && listTrails[indexTrail].trailObject.GetComponent<TrapsController>()*/)
                Destroy(listTrails[indexTrail].trailObject.transform.GetChild(0).gameObject);
        }
        return (true);
    }

    /// <summary>
    /// création d'un nouveau points
    /// index: l'index de la liste de structure trails
    /// </summary>
    void setPoints(int index)
    {
        //CPB.setProgress(CPB.getProgress() - 1);                     //diminue 1

        listTrails[index].points.Add(playerAnts.position);                                              //ajoute un points à la liste

        if (!listTrails[index].line)
        {
            clearAll();
            powerFlare(0);
            Debug.Log("plus de line ?");

        }
        else
        {
            listTrails[index].line.positionCount = listTrails[index].points.Count;                          //set le nombre max de point au nombre d'élément dans notre liste
            listTrails[index].line.SetPosition(listTrails[index].points.Count - 1, playerAnts.position);    //ajoute un point à la ligne à l'index voulu

            //change l'array du collider à la liste de notre lineRenderer (transform list en array)
            //if (listTrails[index].points.Count > 1)
                //listTrails[index].col.points = listTrails[index].points.ToArray<Vector2>();

            
        }
    }

    /// <summary>
    /// Set le collider
    /// </summary>
    /// <param name="index"></param>
    void setCollider(int index)
    {
        if (listTrails[index].line)
        {
            Vector3 pos = new Vector3(listTrails[index].line.GetPosition(listTrails[index].points.Count - 1).x, listTrails[index].line.GetPosition(listTrails[index].points.Count - 1).y, 0);
            GameObject ParticleCollider = Instantiate(prefabsCollider, pos, gameObject.transform.rotation, listTrails[index].trailObject.transform) as GameObject;
            ForceTrail FT = ParticleCollider.GetComponent<ForceTrail>();

            ParticleCollider.GetComponent<SphereCollider>().radius = widthTrailCollider;
            //FT.orientation = findVectorForce(index, pos);
            FT.TC = this;
            FT.SetOrientation(playerAnts.up);
            FT.endQueue = false;             //défini qu'on est a la fin

            int countChild = listTrails[index].trailObject.transform.childCount;
            int pointCount = listTrails[index].points.Count;
            int indexChildCollider = 1;

            //exécute l'action 10 fois
            for (int i = 0; i < numberPointInFront; i++)
            {
                //si il y a moins de 10 points, arrêté quand on arrive au bout !
                if (pointCount <= i || countChild - indexChildCollider < 2)
                    break;

                //si on est sur un modulo, ok on exécute un collider !
                if (i % moduloForLine == 0)
                {
                    //prendre le child à partir de la fin !
                    GameObject ForceObjectTmp = listTrails[index].trailObject.transform.GetChild(countChild - indexChildCollider).gameObject;
                    ForceTrail FTtmp = ForceObjectTmp.GetComponent<ForceTrail>();

                    if (Mathf.Max(-1, 4 - moduloForLine - i) >= 0)
                    {
                        FTtmp.endQueue = true;
                        //4 - 1 - 0 = 3
                        //4 - 1 - 1 = 2
                        //4 - 1 - 2 = 1
                        //4 - 1 - 3 = 0;

                        //4 - 2 - 0 = 2
                        //4 - 2 - 1 = 1
                        //4 - 2 - 2 = 0;0;0

                        //4 - 3 - 0 = 1
                        //4 - 3 - 1 = 0;
                    }
                    else
                        FTtmp.endQueue = false;

                    FTtmp.SetOrientation(playerAnts.transform.position - ForceObjectTmp.transform.position);

                    indexChildCollider++;
                }

            }

            //aller voir les X collider derrière ?
            //pour chaque: findeVector(index, pos de l'objet)
        }
    }

    /// <summary>
    /// reload le trail si il est relaché
    /// </summary>
    void reloadTrail()
    {
        if (released && CPB)
            CPB.setProgress(CPB.getProgress() + amountToAdd);
    }

    /// <summary>
    /// trouve le points dans la liste des points du trial la plus proche
    /// </summary>
    /// <param name="index"></param>
    /// <param name="impactPoint"></param>
    /// <returns></returns>
    int findClosestIndex(int index, Vector3 impactPoint)
    {
        Vector2 impact2D = new Vector2(impactPoint.x, impactPoint.y);               //convertir le point d'impact en 2D
        float minDist = 1000.0f;                                                    //fixez la distance Min à 1000
        int tmpIndex = -1;                                                          //set l'index tmp

        for (int i = 0; i < listTrails[index].points.Count; i++)
        {
            Vector2 offset = listTrails[index].points[i] - impact2D;                //check la distance * distance
            float sqrLen = offset.sqrMagnitude;
            //si la distance entre le points de trail du le points d'impact est plus petit que ce qu'on a trouvé avant
            //on le sauvegarde et on set la nouvelle minDist
            if (sqrLen < minDist)
            {
                minDist = sqrLen;
                tmpIndex = i;
            }
        }
 
        return (tmpIndex);
    }


    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    Vector3 realPosInWorld(Vector2 wrongPos)
    {
        Vector3 pos = new Vector3(wrongPos.x, wrongPos.y, 0);
        return (realPosInWorld(pos));
    }
    Vector3 realPosInWorld(Vector3 wrongPos)
    {
        Vector3 pos = new Vector3(transform.position.x + wrongPos.x, transform.position.y + wrongPos.y, 0);
        //GameObject popWhite = Instantiate(explodeDebug, pos, Quaternion.identity) as GameObject;
        pos = RotatePointAroundPivot(pos, new Vector3(transform.position.x, transform.position.y, 0), playerAnts.transform.eulerAngles);
        //popWhite.transform.position = new Vector3(popWhite.transform.position.x + 1, popWhite.transform.position.y + 1, 0);
        //popWhite.transform.position += popWhite.transform.up * 5;
        pos += playerAnts.transform.up * 1;
        return (pos);
    }

    public Vector3 returnClosestPosition(int index, Vector3 impactPoint)
    {
        Vector2 impact2d = new Vector2();

        int closestIndex = findClosestIndex(index, impactPoint);
        if (listTrails.Count == 0 || index >= listTrails.Count || closestIndex >= listTrails[index].points.Count
                || !listTrails[index].trailObject)
            return (new Vector3(0, 0, 0));
        impact2d = listTrails[index].points[closestIndex];
 
        return (new Vector3(impact2d.x, impact2d.y, 0));
    }

    /// <summary>
    /// n'applique pas la force sur l'oeuf si c'est le début du trail...
    /// </summary>
    /// <returns></returns>
    public bool notApplyIfStartTrail(int index, Vector3 impactPoint)
    {
        //Debug.Log("index du trail: " + index);
        //Debug.Log("list trail count: " + listTrailsFront.Count);

        if (index >= listTrails.Count || listTrails.Count == 0 || listTrails[index].points.Count == 0 || listTrails[index].points.Count == 1)
            return (false);

        //Debug.Log("ici pas de bug de trail");
        int tmpIndex = -1;                                                          //set l'index tmp
        tmpIndex = findClosestIndex(index, impactPoint);                            //trouve l'index du point le plus proche

        if (tmpIndex == -1)                                                         //si l'index est = -1 y'a eu une erreur
            return (false);

        //Debug.Log("ok on test ");
        //Debug.Log("count trail: " + listTrails[index].points.Count);
        //Debug.Log("index où l'oeuf touche: " + tmpIndex);
        if (listTrails[index].points.Count - tmpIndex < minBeforeAttract)
        {
            return (false);
        }
        //listTrailsFront[index].points.Count

        return (true);  //appliquer la force
    }

    /// <summary>
    /// /// <summary>
    /// fonction appelé lorsqu'un oeuf est en collision avec un trail: on a
    /// la position de l'impacte et l'index de la trail.
    /// La fonction renvoi la force à appliquer à l'oeuf
    /// </summary>
    /// <param name="index">index de la trail</param>
    /// <param name="impactPoint">le points d'impacte</param>
    /// <returns>le vecteur force à appliquer à l'oeuf</returns>
    public Vector3 findVectorForce(int index, Vector3 impactPoint)
    {
        Vector2 impact2D = new Vector2(impactPoint.x, impactPoint.y);               //convertir le point d'impact en 2D
        Vector2 impact1;
        Vector2 impact2;

        if (index >= listTrails.Count || listTrails.Count == 0 || listTrails[index].points.Count == 0 || listTrails[index].points.Count == 1)
            return (new Vector3(0, 0, 0));

        int tmpIndex = -1;                                                          //set l'index tmp
        tmpIndex = findClosestIndex(index, impactPoint);                            //trouve l'index du point le plus proche

        if (tmpIndex == -1)                                                         //si l'index est = -1 y'a eu une erreur
            return (new Vector3(0, 0, 0));

        //si l'index est le dernier ellement de la liste... prendre le dernier et l'avant dernier pour le vecteur dir !
        if (tmpIndex == listTrails[index].points.Count - 1)
        {
            impact1 = listTrails[index].points[tmpIndex - 1];               //set le premier au points se trouvant avant à l'élément le plus proche trouvé
            impact2 = listTrails[index].points[tmpIndex];                   //set le deuxième à l'élément le plus proche trouvé
            impact2D = impact2 - impact1;                                   //get le vecteur directeur en les soustractant.
            return (new Vector3(impact2D.x, impact2D.y, 0.0f));
        }
        else
        {
            impact1 = listTrails[index].points[tmpIndex];                       //set le premier à l'élément le plus proche trouvé
            int indexForward = tmpIndex + numberPointInFront;
            while (tmpIndex < listTrails[index].points.Count - 1 && tmpIndex < indexForward)
            {
                tmpIndex++;
            }
            impact2 = listTrails[index].points[tmpIndex];               //set le deuxième par rapport au points suivant de la liste
            impact2D = impact2 - impact1;                                   //get le vecteur directeur en les soustractant.
            return (new Vector3(impact2D.x, impact2D.y, 0.0f));
        }
    }
    
    /// <summary>
    /// update
    /// </summary>
    private void Update()
    {
        /*if (Time.fixedTime >= timeToGo)
        {
            //reloadTrail();
            timeToGo = Time.fixedTime + timeOpti;
        }*/

    }

    private void FixedUpdate()
    {
        reloadTrail();
    }
}
