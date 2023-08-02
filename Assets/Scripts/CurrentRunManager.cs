using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentRunManager : MonoBehaviour
{
    public static CurrentRunManager instance;

    public List<string> PlayerDeck;

    [Header("Current stats")]
    public CharacterStats Stats;
    public int Gold;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI HpText;
    [SerializeField] private TextMeshProUGUI ManaText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image manaBar;

    [SerializeField] private GameObject gameOverScreen;

    [Header("")]
    [SerializeField] private GameObject map;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateHealthAndMana();
        Stats.OnHpZero += HpHitZero;
        CharacterStats.HpChanged += (int x, bool ignore, bool ignore2, float ignore3) => { UpdateHealthAndMana(); };
    }

    private void HpHitZero()
    {
        FindObjectOfType<CombatManager>().PlayerHpReachedZero();
        gameOverScreen.SetActive(true);
    }

    public void UpdateHealthAndMana()
    {
        HpText.text = Stats.CurrentHealth + "/" + Stats.MaxHealth + "HP";
        ManaText.text = Stats.Mana + "/" + Stats.MaxMana + "MP";

        hpBar.fillAmount = (float)Stats.CurrentHealth / (float)Stats.MaxHealth;
        manaBar.fillAmount = (float)Stats.Mana / (float)Stats.MaxMana;
    }

    public void ReturnToMap()
    {
        FindObjectOfType<CombatManager>().CloseCombatScreen();
        map.SetActive(true);
    }
}