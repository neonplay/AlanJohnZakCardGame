using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPoint : MonoBehaviour
{
    public Image[] dottedLines;
    public Button[] buttonsToDisable;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Pressed);
    }

    public void Pressed()
    {
        FindObjectOfType<MapManager>().MapPointPressed(this);
        MakeLinesGreen();
        MakeLinesGrey();
    }

    public void MakeLinesGrey()
    {
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
