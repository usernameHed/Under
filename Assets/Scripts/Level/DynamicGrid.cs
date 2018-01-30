using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour {

    public int col = 2, row = 1;

    private void Start()
    {
        col = gameObject.transform.childCount;
    }

    private void Update()
    {
        RectTransform parent = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup grid = gameObject.GetComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(parent.rect.width / col, parent.rect.height / row);
    }
}
