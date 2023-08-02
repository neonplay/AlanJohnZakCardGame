using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public int Turn { get; set; } = 0;

    public bool PlayerTurn { get; set; } = false;

    [SerializeField] private Transform enemyHolder;
    [SerializeField] private GameObject combatPanel;
    public GameObject dummyEnemy;

    Enemy currentEnemy;
    CardPlayingManager cardPlayingManager;

    [SerializeField] private Button endTurnButton;
    [SerializeField] private GameObject victoryScreen;

    [Header("damage numbers")]
    [SerializeField] private GameObject damageNumber;
    [SerializeField] private Transform damageHolder;
    [SerializeField] private Transform playerDamagePosition;

    bool combatOver;

    private void Awake()
    {
        cardPlayingManager = FindObjectOfType<CardPlayingManager>();
        endTurnButton.gameObject.SetActive(false);

        CharacterStats.DamageTaken += ShowDamageNumber;
    }

    private void ShowDamageNumber(int damage, bool player)
    {
        var dmg = Instantiate(damageNumber, damageHolder);
        dmg.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = (-damage).ToString();

        if(!player)
        {
            dmg.transform.position = enemyHolder.position;
            dmg.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        }
        else
        {
            dmg.transform.position = playerDamagePosition.position;
            dmg.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-25, 25), Random.Range(-25, 25));
        }

        StartCoroutine(DeleteDmgNumber());

        IEnumerator DeleteDmgNumber()
        {
            yield return new WaitForSeconds(2.5f);
            Destroy(dmg);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) StartCombat(dummyEnemy);
    }

    public void StartCombat(GameObject enemy)
    {
        endTurnButton.gameObject.SetActive(true);
        Turn = 0;
        PlayerTurn = true;
        combatOver = false;
        currentEnemy =  Instantiate(enemy, enemyHolder).GetComponent<Enemy>();
        currentEnemy.PickAbilityForNextTurn();
        combatPanel.SetActive(true);
        cardPlayingManager.StartBattle();
        StartCoroutine(EnableEndturnButton());
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
        }
        else
        {
            cardPlayingManager.DrawCards();
            StartCoroutine(EnableEndturnButton());
            currentEnemy.Stats.BuffsAndDebuffs.ReduceDebuffsAfterTurnEnd();
            CurrentRunManager.instance.Stats.BuffsAndDebuffs.ReduceDebuffsOnTurnStart();
        }

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

        yield return new WaitForSeconds(1);

        currentEnemy.DoAbility();

        yield return new WaitForSeconds(2);

        EndTurnPressed();
        currentEnemy.PickAbilityForNextTurn();

    }

    public void PlayerHpReachedZero()
    {
        combatOver = true;
        StopAllCoroutines();
    }

    public void EnemyHpHitZero()
    {
        combatOver = true;
        victoryScreen.SetActive(true);
        StopAllCoroutines();
    }

    public void VictoryNextPressed()
    {
        victoryScreen.SetActive(false);
        CurrentRunManager.instance.OfferRewards();
    }

    public void CloseCombatScreen()
    {
        combatPanel.SetActive(false);
        Destroy(currentEnemy.gameObject);
        currentEnemy = null;
        endTurnButton.gameObject.SetActive(false);
    }
}
