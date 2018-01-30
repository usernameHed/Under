using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// détruite l'objet courant
/// </summary>
public class toDelete : MonoBehaviour
{

    private void Awake()
    {
        Destroy(gameObject);
    }
}
