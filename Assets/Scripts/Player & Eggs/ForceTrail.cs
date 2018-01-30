using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTrail : MonoBehaviour
{
    [Range(0, 10f)]
    public float timeOpti = 0.05f;       //optimisation
    public bool endQueue = false;
    public float defaultForceTrail = 40f;
    [HideInInspector] public TrailController TC;

    private float timeToGo;                     //optimisation
    [SerializeField] private Vector2 orientation;

    // Use this for initialization
    void Start()
    {
        timeToGo = Time.fixedTime;
    }

    #region core script
    /// <summary>
    /// change l'orientation du collider
    /// </summary>
    /// <param name="dir"></param>
    public void SetOrientation(Vector3 dir)
    {
        orientation.x = dir.x;
        orientation.y = dir.y;
        Debug.DrawRay(gameObject.transform.position, orientation * 5, Color.red, 0.5f);
    }

    /// <summary>
    /// renvoi l'orientation du collider
    /// </summary>
    /// <returns></returns>
    public Vector3 GetOrientation()
    {
        return (orientation);
    }

    /// <summary>
    /// applique une force sur un oeuf
    /// </summary>
    /// <param name="collide"></param>
    /// <param name="eggs"></param>
    /// <param name="TCother"></param>
    void applyForceOnEggs(bool collide, GameObject eggs)
    {
        EggsController EC = eggs.GetComponent<EggsController>();
        if (!EC || EC.isKinematicSave)
            return;
        Rigidbody ECRb = eggs.GetComponent<Rigidbody>();

        //si COLLISION
        if (collide)
        {
            //s'il n'est pas encore controllé, c'est la première fois qu'il est controllé !
            if (!EC.isGreenControlled)
            {
                //GameObject particle = Instantiate(prefabsParticle, contact.point, Quaternion.identity);             //cree un effet de particule
                //particle.SetActive(true);

                if (!endQueue)
                {
                    EC.isGreenControlled = true;                                                                        //change l'oeuf en state "controllé par le green"
                    ECRb.velocity = Vector3.zero;                                          //set la vélocité à zero
                    ECRb.angularVelocity = Vector3.zero;
                    ECRb.useGravity = false;                                               //l'oeuf n'est plus sujet à la gravité
                }
                //if (TC.notApplyIfStartTrail(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point))
                //{
                //Vector3 moveDir = TC.findVectorForce(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point);    //get le vecteur directeur du points d'impact et du points suivant du trails (la direction du trail)
                Vector3 moveDir = GetOrientation();
                Debug.DrawRay(gameObject.transform.position, moveDir, Color.green, 5f);
                if (TC)
                    ECRb.AddForce(((EC.isPushedByOther) ? TC.forceTrail / TC.forceToDivideWhenPushed : TC.forceTrail) * moveDir);
                else
                    ECRb.AddForce(defaultForceTrail * moveDir);
                //}

                //EC.stopGreenWithCoroutine();
                EC.greenControlled(true);
            }
            else
            {
                if (TC && TC.PC.isHoldingY)
                {
                    EC.greenStopped(true);
                    return;
                }
                else
                {
                    EC.greenStopped(false);
                }

                //if (TC.notApplyIfStartTrail(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point))
                //{
                //Vector3 moveDir = TC.findVectorForce(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point);    //get le vecteur directeur du points d'impact et du points suivant du trails (la direction du trail)
                Vector3 moveDir = GetOrientation();
                Debug.DrawRay(gameObject.transform.position, moveDir, Color.green, 5f);

                if (TC)
                    ECRb.AddForce(((EC.isPushedByOther) ? TC.forceTrail / TC.forceToDivideWhenPushed : TC.forceTrail) * moveDir);
                else
                    ECRb.AddForce(defaultForceTrail * moveDir);

                EC.greenControlled(true);
                //}
            }
        }
        else //si on sort de collisions...
        {
            Vector3 moveDir = GetOrientation();
            Debug.DrawRay(gameObject.transform.position, moveDir, Color.green, 5f);
            ECRb.AddForce(((EC.isPushedByOther) ? TC.forceTrail / TC.forceToDivideWhenPushed : TC.forceTrail) * moveDir);
        }
    }

    /// <summary>
    /// applique une force sur un player
    /// </summary>
    /// <param name="collide"></param>
    /// <param name="player"></param>
    /// <param name="TCother"></param>
    void applyForceOnPlayer(bool collide, GameObject player)
    {
        PlayerController PC = player.GetComponent<PlayerController>();
        Rigidbody RBPC = player.GetComponent<Rigidbody>();

        if (!PC || !RBPC)
            return;

        if (collide && PC.getTypePowerPlayer() != 4)
        {
            Vector3 moveDir = GetOrientation();
            //Vector3 moveDir = TC.findVectorForce(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point);    //get le vecteur directeur du points d'impact et du points suivant du trails (la direction du trail)
            //refObject.GetComponent<Rigidbody>().AddForce(TC.forceTrail * moveDir);
            if (TC)
                PC.pushAway(moveDir, TC.forceTrail * RBPC.mass * 2);
            else
                PC.pushAway(moveDir, defaultForceTrail * RBPC.mass * 2);
        }
    }

    /// <summary>
    /// applique une force sur un rocher
    /// </summary>
    /// <param name="collide"></param>
    /// <param name="rock"></param>
    /// <param name="TCother"></param>
    void applyForceOnPushable(bool collide, GameObject rock)
    {
        Pushable PS = rock.GetComponent<Pushable>();
        Rigidbody RBR = rock.GetComponent<Rigidbody>();
        if (!PS || !RBR)
            return;

        if (collide)
        {
            //TC = hit.gameObject.GetComponent<TrailDeletePoints>().TC;                                           //get la référence du TrailController
            Vector3 moveDir = GetOrientation();
            //Vector3 moveDir = TC.findVectorForce(hit.gameObject.GetComponent<TrailDeletePoints>().index, contact.point);    //get le vecteur directeur du points d'impact et du points suivant du trails (la direction du trail)
            //refObject.GetComponent<Rigidbody>().AddForce(TC.forceTrail * moveDir);

            RBR.AddForce(TC.forceTrail * moveDir * PS.factorMultiply * 2);
            PS.setControl(true, 3);
                //hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection * hit.transform.gameObject.GetComponent<Pushable>().factorMultiply);

        }
        else
        {
            PS.setControl(false, 3);
            //hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * forceDirection * hit.transform.gameObject.GetComponent<Pushable>().factorMultiply);
        }
    }

    /// <summary>
    /// selectionne l'objet dans le trail
    /// </summary>
    /// <param name="collide"></param>
    /// <param name="otherC"></param>
    public void TriggerType(bool collide, GameObject other)
    {
        if (!other)
            return;

        if (other.CompareTag("Eggs"))
        {
            applyForceOnEggs(collide, other);
        }
        else if (other.CompareTag("Player"))
        {
            applyForceOnPlayer(collide, other);
            
        }
        else if (other.CompareTag("Pushable"))
        {
            applyForceOnPushable(collide, other);
        }
    }

    #endregion

    #region unity fonction and ending

    public void BothTriggerStay(GameObject other)
    {
        if (Time.fixedTime >= timeToGo)
        {
            Debug.DrawRay(gameObject.transform.position, orientation * 5, Color.blue, timeOpti);
            TriggerType(true, other);
            timeToGo = Time.fixedTime + timeOpti;
        }
    }


    /// <summary>
    /// Exécuté chaque frame
    /// </summary>
    private void Update()
    {
        //Debug.DrawRay(gameObject.transform.position, orientation * 5, Color.blue, timeOpti);
    }
    #endregion

    // Update is called once per frame

}
