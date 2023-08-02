using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPoint : MonoBehaviour
{
    public Image[] dottedLines;
    public Image[] itemsToGrey;
    public Button[] buttonsToDisable;

    public void Pressed()
    {
        FindObjectOfType<MapManager>().MapPointPressed(this);
    }

    public void MakeLinesGrey()
    {
        foreach(var item in itemsToGrey)
        {
            item.color = Color.grey;
        }
        foreach (var item in buttonsToDisable)
        {
            item.enabled = false;
        }
    }

    public void MakeLinesGreen()
    {
        foreach (var item in dottedLines)
        {
            item.color = Color.green;
        }

    }
}
