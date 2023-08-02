using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSiteManager : MonoBehaviour
{
    public void UpgradePressed()
    {
        FindObjectOfType<UpgradesManager>().OpenUpgradesPanel();
    }

    public void RestPressed()
    {
        CurrentRunManager.instance.Stats.Heal(5000);
        CurrentRunManager.instance.Stats.ChangeMana(5000);
        CurrentRunManager.instance.UpdateHealthAndMana();
        CurrentRunManager.instance.ReturnToMap();
    }
}
