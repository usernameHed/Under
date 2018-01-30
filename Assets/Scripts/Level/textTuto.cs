using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textTuto : MonoBehaviour
{
    public bool controller = false;
    public List<GameObject> controlList;

    public int TypeTuto
    {
        get
        {
            //Some other code
            return typeTuto;
        }
        set
        {
            //Some other code
            typeTuto = value;
            for (int i = 0; i < controlList.Count; i++)
            {
                if (i == typeTuto)
                    controlList[i].SetActive(true);
                else
                    controlList[i].SetActive(false);
            }
        }
    }
    public int typeTuto = 0;


    /// <summary>
    /// setup la liste des objets
    /// </summary>
    private void Start()
    {
        int numberChild = gameObject.transform.childCount;

        if (numberChild <= 0)
            return;
        controlList.Add(gameObject.transform.GetChild(0).gameObject);
        if (controller && numberChild >= 3)
        {
            controlList.Add(gameObject.transform.GetChild(1).gameObject);
            controlList.Add(gameObject.transform.GetChild(2).gameObject);
        }
    }

}
