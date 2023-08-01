using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRunManager : MonoBehaviour
{
    public static CurrentRunManager instance;

    public List<string> PlayerDeck;
    private AllCardsData allCardsData;

    [Header("Current stats")]
    public CharacterStats Stats;

    public int Mana = 20;
    public int Gold;

    [Header("Rewards")]
    public GameObject cardRewardsPanel;
    public Transform rewardCardsParent;
    public Transform cardRewardAcceptedParent;
    public Transform deckPosition;
    [SerializeField] private HorizontalLayoutGroup rewardsLayoutGroup;
    private bool cardAccepted = false;

    private void Awake()
    {
        instance = this;
        allCardsData = FindObjectOfType<AllCardsData>();
    }

    public void Heal(int amount)
    {
    }

    #region Card Rewards

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) OfferRewards();
    }

    public void OfferRewards()
    {
        cardRewardsPanel.SetActive(true);
        rewardsLayoutGroup.enabled = true;

        rewardCardsParent.gameObject.SetActive(false);
        var rewards = new List<InGameCard>();
        cardAccepted = false;

        var listOfPotentialRewards = new List<InGameCard>();
        foreach (var card in allCardsData.CardsList)
        {
            if (card.isRewardable)
            {
                int roll = Random.Range(0, 100);
                listOfPotentialRewards.Add(roll < 70 ? card.Base : card.Upgrade);
            }
        }

        for(int i = 0; i < 3; i++)
        {
            int x = Random.Range(0, listOfPotentialRewards.Count);
            var card = Instantiate(listOfPotentialRewards[x], rewardCardsParent);
            card.transform.localScale = Vector3.zero;
            card.isReward = true;
            rewards.Add(card);
            listOfPotentialRewards.RemoveAt(x);
        }

        rewardCardsParent.gameObject.SetActive(true);

        IEnumerator ScaleCardsUp()
        {
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 5;
                yield return new WaitForEndOfFrame();

                foreach (var card in rewards)
                {
                    card.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                }
            }
        }
        StartCoroutine(ScaleCardsUp());
    }

    public void RewardCardSelected(InGameCard card)
    {
        if (cardAccepted) return;
        IEnumerator LerpCardToDeck()
        {
            card.transform.SetParent(cardRewardAcceptedParent);

            var cardsToDestroy = new List<GameObject>();
            foreach (Transform card in rewardCardsParent)
            {
                cardsToDestroy.Add(card.gameObject);
            }

            Vector2 startPos = card.transform.position;
            Vector2 endPos = deckPosition.position;

            float progress = 0;
            
            while (progress < 1)
            {
                progress += Time.deltaTime * 3;
                yield return new WaitForEndOfFrame();

                card.transform.position = Vector3.Lerp(startPos, endPos, progress);
                card.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);

                foreach (var cardToDestroy in cardsToDestroy)
                {
                    cardToDestroy.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress * 2);
                }
            }

            foreach (var card in cardsToDestroy)
            {
                Destroy(card.gameObject);
            }
            Destroy(card.gameObject);
            cardRewardsPanel.SetActive(false);
        }
        cardAccepted = true;
        rewardsLayoutGroup.enabled = false;
        PlayerDeck.Add(card.CardName);
        StartCoroutine(LerpCardToDeck());
    }

    #endregion
}

public class CharacterStats
{
    public int MaxHealth;
    public int CurrentHealth;
    public int Armour;

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        int remainingDamage = amount;
        if(Armour > 0)
        {
            remainingDamage -= Armour;
            Armour -= amount;
        }
        if(remainingDamage > 0)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        }
    }
}
