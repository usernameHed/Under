using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(BoxCollider))]
public class DestructibleWalls : MonoBehaviour
{

    [SerializeField] private GameObject pullOfBlocks;
    public float sizeSphereToKillPortal = 1.4f;
    public bool autoDestroy = false;

    private BoxCollider Bc;

    private void Awake()
    {
        Bc = gameObject.GetComponent<BoxCollider>();
    }

    /// <summary>
    /// set actif tout les rochers à l'intérieur !
    /// </summary>
    void SetActiveRocks()
    {
        //pullOfBlocks.SetActive(true);
        foreach (Transform child in pullOfBlocks.transform)
        {
            //child is your child transform
            child.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    /// <summary>
    /// détruit le mur
    /// </summary>
    [Button("destroyThis")]
	public void destroyThis()
    {
        pullOfBlocks.transform.parent = gameObject.transform.parent;
        Bc.enabled = false;
        SetActiveRocks();
        

        //capture tout objet dans le layer layerMask, dans la range
        Destroy(gameObject);
    }
}
