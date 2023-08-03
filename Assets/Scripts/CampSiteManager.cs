using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSiteManager : MonoBehaviour
{
    [SerializeField] GameObject campsiteScreen;

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    public void UpgradePressed()
    {
        FindObjectOfType<UpgradesManager>().OpenUpgradesPanel();
    }

    public void RestPressed()
    {
        CurrentRunManager.instance.Stats.Heal(5000);
        CurrentRunManager.instance.Stats.ChangeMana(5000);
        CurrentRunManager.instance.UpdateHealthAndMana();
        campsiteScreen.gameObject.SetActive(false);
        //CurrentRunManager.instance.ReturnToMap();
    }
}
