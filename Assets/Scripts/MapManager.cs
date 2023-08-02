using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject piranhas;
    public GameObject bird;
    public GameObject seaMonster;
    public GameObject sharkBoss;

    [Header("")]
    public GameObject campSite;


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
