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

    bool combatOver;

    private void Awake()
    {
        cardPlayingManager = FindObjectOfType<CardPlayingManager>();
        endTurnButton.gameObject.SetActive(false);

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
        }
        else
        {
            cardPlayingManager.DrawCards();
            StartCoroutine(EnableEndturnButton());
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
        yield return new WaitForSeconds(2);

        currentEnemy.DoAbility();

        yield return new WaitForSeconds(2);

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

    public void CloseCombatScreen()
    {
        combatPanel.SetActive(false);
        Destroy(currentEnemy.gameObject);
        currentEnemy = null;
        endTurnButton.gameObject.SetActive(false);
    }
}