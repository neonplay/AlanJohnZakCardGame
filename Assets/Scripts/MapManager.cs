using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public MapPoint[] stage1;
    public MapPoint[] stage2;

    [Header("Enemies")]
    public GameObject piranhas;
    public GameObject bird;
    public GameObject seaMonster;
    public GameObject sharkBoss;

    [Header("")]
    public GameObject campSite;


    public void MapPointPressed(MapPoint point)
    {

    }

    public void MapPointPressed(string type)
    {
        if(type == "camp")
        {
            campSite.SetActive(true);
        }

        if(type == "skip")
        {

        }

        switch (type)
        {
            case "piranhas":
                FindObjectOfType<CombatManager>().StartCombat(piranhas);
                break;
            case "bird":
                FindObjectOfType<CombatManager>().StartCombat(bird);
                break;
            case "seaMonster":
                FindObjectOfType<CombatManager>().StartCombat(seaMonster);
                break;
            case "sharkBoss":
                FindObjectOfType<CombatManager>().StartCombat(sharkBoss);
                break;
        }
    }
}
