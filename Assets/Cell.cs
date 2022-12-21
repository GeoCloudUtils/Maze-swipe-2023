using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public RectTransform[] Borders;

    public Transform GetTopBorder()
    {
        foreach (RectTransform border in Borders)
        {
            Image img = border.GetComponent<Image>();
            img.color = Color.black;
        }
        RectTransform topBorder = Borders.OrderBy(x => x.anchoredPosition.y).Last();
        Image borderImage = topBorder.GetComponent<Image>();
        borderImage.color = Color.red;
        return topBorder.transform;
    }
}
