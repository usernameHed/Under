using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class Toto : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    //[EnableIf("coop")]

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
    private static Toto SS;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")]
    [ValidateInput("debugTimeOpti", "optimisation supérieur à 0", InfoMessageType.Warning)]
    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;

    [FoldoutGroup("Debug")]
    [Tooltip("Activation de la singularité du script")]
    [OnValueChanged("testSingularity")]
    [SerializeField]
    private bool enableSingularity = false;

    [FoldoutGroup("Debug")]
    [Tooltip("l'objet est-il global inter-scène ?")]
    [SerializeField]
    private bool dontDestroyOnLoad = false;

    #endregion

    #region fonction debug variables
    /// <summary>
    /// retourne une erreur si le timeOpti est inférieur ou égal à 0.
    /// </summary>
    /// <returns></returns>
    private bool debugTimeOpti(float timeOpti)
    {
        if (timeOpti <= 0)
            return (false);
        return (true);
    }

    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    private void testSingularity()
    {
        if (!enableSingularity)
            return;

        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// récupère la singularité (si ok par le script)
    /// </summary>
    /// <returns></returns>
    static public Toto getSingularity()
    {
        if (!SS || !SS.enableSingularity)
        {
            Debug.LogError("impossible de récupéré la singularité");
            return (null);
        }
        return (SS);
    }

    /// <summary>
    /// set l'objet en dontDestroyOnLoad();
    /// </summary>
    private void setDontDestroyOnLoad()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        testSingularity();                                                  //set le script en unique ?
        setDontDestroyOnLoad();                                             //set le script Inter-scène ?
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps
    }
    #endregion

    #region core script
    /// <summary>
    /// functionTest
    /// </summary>
    [ContextMenu("functionTest")]
    private void functionTest()                                             //test
    {

    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// effectué à chaque frame
    /// </summary>
    private void Update()
    {
        //effectué à chaque opti frame
        if (Time.fixedTime >= timeToGo)
        {
            //ici action optimisé

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        Destroy(gameObject);
    }
    #endregion
}
