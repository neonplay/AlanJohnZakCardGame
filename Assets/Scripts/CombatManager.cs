using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CombatManager : MonoBehaviour
{
    public int Turn { get; set; } = 0;

    public bool PlayerTurn { get; set; } = false;

    [SerializeField] private Transform enemyHolder;
    [SerializeField] private GameObject combatPanel;

    Enemy currentEnemy;
    CardPlayingManager cardPlayingManager;

    [SerializeField] private Button endTurnButton;
    [SerializeField] private GameObject victoryScreen;

    [Header("damage numbers")]
    [SerializeField] private GameObject damageNumber;
    [SerializeField] private Transform damageHolder;
    [SerializeField] private Transform playerDamagePosition;

    [Header("Enemy ui")]
    [SerializeField] private TextMeshProUGUI enemyHp;
    [SerializeField] private Image enemyHpBar;
    [SerializeField] private Image enemyHpBarSlower;
    [SerializeField] private TextMeshProUGUI enemyArmour;
    [SerializeField] private GameObject enemyBurn;
    [SerializeField] private GameObject enemyParalysis;
    [SerializeField] private GameObject enemyPower;
    [SerializeField] private GameObject enemyDodge;
    private RectTransform enemyRect;
    private int enemyPreviousHP;

    bool combatOver;

    private void Awake()
    {
        cardPlayingManager = FindObjectOfType<CardPlayingManager>();
        endTurnButton.gameObject.SetActive(false);

        CharacterStats.HpChanged += ShowHpChangedNumber;
    }

    private void ShowHpChangedNumber(int hpChange, bool player, bool damage, float delay)
    {
        StartCoroutine(ShowNumber());
        IEnumerator ShowNumber()
        {
            yield return new WaitForSeconds(delay);
            var dmg = Instantiate(damageNumber, damageHolder);

            if (damage)
            {
                dmg.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (-hpChange).ToString();
                dmg.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                dmg.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = hpChange.ToString();
                dmg.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.green;
            }

            if (!player)
            {
                dmg.transform.position = enemyHolder.position;
                dmg.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
            }
            else
            {
                dmg.transform.position = playerDamagePosition.position;
                dmg.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
            }

            UpdateEnemyHpAndStatuses();

            yield return new WaitForSeconds(2.5f);
            Destroy(dmg);
        }
    }

    public void UpdateEnemyHpAndStatuses()
    {
        enemyHp.text = currentEnemy.Stats.CurrentHealth + "/" + currentEnemy.Stats.MaxHealth;
        enemyHpBar.fillAmount = (float)currentEnemy.Stats.CurrentHealth / (float)currentEnemy.Stats.MaxHealth;

        enemyArmour.text = currentEnemy.Stats.Armour.ToString();
        enemyArmour.transform.parent.gameObject.SetActive(currentEnemy.Stats.Armour > 0);
        enemyHp.text = currentEnemy.Stats.CurrentHealth + "/" + currentEnemy.Stats.MaxHealth;
        enemyHpBar.fillAmount = (float)currentEnemy.Stats.CurrentHealth / (float)currentEnemy.Stats.MaxHealth;
        enemyHpBarSlower.DOFillAmount(((float)currentEnemy.Stats.CurrentHealth / (float)currentEnemy.Stats.MaxHealth), 0.2f);

        enemyArmour.text = currentEnemy.Stats.Armour.ToString();
        enemyArmour.transform.parent.gameObject.SetActive(currentEnemy.Stats.Armour > 0);
        if (currentEnemy.Stats.CurrentHealth < enemyPreviousHP)
        {
            enemyRect.DOComplete();
            enemyRect.DOPunchScale(Vector3.one / 2f, 0.2f);
            enemyRect.DOShakePosition(0.3f, 30);
        }
        enemyPreviousHP = currentEnemy.Stats.CurrentHealth;

        enemyBurn.SetActive(currentEnemy.Stats.BuffsAndDebuffs.Dot > 0);
        enemyPower.SetActive(currentEnemy.Stats.BuffsAndDebuffs.Strength > 0);
        enemyParalysis.SetActive(currentEnemy.Stats.BuffsAndDebuffs.LessDamage > 0);
        enemyDodge.SetActive(false);
    }

    public void StartCombat(GameObject enemy)
    {
        CurrentRunManager.instance.UpdateHealthAndMana();
        CurrentRunManager.instance.CloseMap();
        endTurnButton.gameObject.SetActive(true);
        Turn = 0;
        PlayerTurn = true;
        combatOver = false;
        currentEnemy =  Instantiate(enemy, enemyHolder).GetComponent<Enemy>();
        currentEnemy.PickAbilityForNextTurn();
        enemyRect = currentEnemy.GetComponent<RectTransform>();
        combatPanel.SetActive(true);
        cardPlayingManager.StartBattle();
        StartCoroutine(EnableEndturnButton());
        UpdateEnemyHpAndStatuses();
    }

    public void EndTurnPressed()
    {
        endTurnButton.interactable = false;
        if (PlayerTurn)
        {
            cardPlayingManager.EndTurn();
        }

        PlayerTurn = !PlayerTurn;

        if(!PlayerTurn)
        {
            StartCoroutine(DoEnemyTurn());
            CurrentRunManager.instance.Stats.BuffsAndDebuffs.ReduceDebuffsAfterTurnEnd();
            currentEnemy.Stats.BuffsAndDebuffs.ReduceDebuffsOnTurnStart();
            currentEnemy.Stats.Armour = 0;
        }
        else
        {
            cardPlayingManager.DrawCards();
            StartCoroutine(EnableEndturnButton());
            currentEnemy.Stats.BuffsAndDebuffs.ReduceDebuffsAfterTurnEnd();
            CurrentRunManager.instance.Stats.BuffsAndDebuffs.ReduceDebuffsOnTurnStart();
            CurrentRunManager.instance.Stats.Armour = 0;
        }

        CurrentRunManager.instance.UpdateHealthAndMana();
        UpdateEnemyHpAndStatuses();

        Turn++;
    }

    IEnumerator EnableEndturnButton()
    {
        yield return new WaitForSeconds(2);
        endTurnButton.interactable = true;
    }

    private IEnumerator DoEnemyTurn()
    {
        yield return new WaitForSeconds(0.5f);

        currentEnemy.Stats.ApplyDotDamage();

        if (currentEnemy.Stats.CurrentHealth <= 0) yield break;

        yield return new WaitForSeconds(1);

        currentEnemy.DoAbility();
        currentEnemy.transform.DOPunchScale(Vector3.one * 1.2f, 0.2f);

        yield return new WaitForSeconds(0.1f);

        UpdateEnemyHpAndStatuses();
        CurrentRunManager.instance.UpdateHealthAndMana();

        yield return new WaitForSeconds(1.5f);

        EndTurnPressed();
        currentEnemy.PickAbilityForNextTurn();

    }

    public void PlayerHpReachedZero()
    {
        combatOver = true;
    }

    public void EnemyHpHitZero()
    {
        combatOver = true;
        victoryScreen.SetActive(true);
    }

    public void VictoryNextPressed()
    {
        victoryScreen.SetActive(false);
        FindObjectOfType<RewardsManager>().OfferRewards();
    }

    public void CloseCombatScreen()
    {
        CurrentRunManager.instance.Stats.BuffsAndDebuffs.ResetBuffsAndDebuffs();
        CurrentRunManager.instance.UpdateHealthAndMana();

        combatPanel.SetActive(false);
        if(currentEnemy != null)
            Destroy(currentEnemy.gameObject);
        endTurnButton.gameObject.SetActive(false);
    }
}
