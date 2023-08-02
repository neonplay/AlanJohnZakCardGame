using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayingManager : MonoBehaviour
{
    [SerializeField] private AllCardsData allCardsData;
    private CurrentRunManager runManager;

    [Header("Transforms")]
    public Transform playerHandParent;
    public Transform heldCardParent;
    public Transform drawStartPosition;
    public Transform discardPosition;

    public List<string> Deck { get; set; }
    public List<string> DiscardPile { get; set; }
    public List<string> ExhuastedCards { get; set; }

    
    private bool replenishingDeck;

    private void Awake()
    {
        runManager = FindObjectOfType<CurrentRunManager>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            DrawCards();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EndTurn();
        }
    }

    public void StartBattle()
    {
        DiscardPile = new List<string>();
        Deck = new List<string>();

        foreach(var card in runManager.PlayerDeck)
        {
            Deck.Add(card);
        }

        ExhuastedCards = new List<string>();

        ShuffleDeck();
        DrawCards();
    }

    public void CardSelectedFromHand(InGameCard card)
    {
        card.transform.SetParent(heldCardParent);
    }

    public void DrawCards()
    {
        StartCoroutine(DrawCards(5));
    }

    private IEnumerator DrawCards(int cardsToDraw)
    {
        for(int i = 0; i < cardsToDraw; i++)
        {

            if(Deck.Count <= 0)
            {
                ReplenishDeck();
            }

            while(replenishingDeck)
            {
                yield return null;
            }

            InGameCard card = Instantiate(allCardsData.CardsDictionary[Deck[0]].gameObject, drawStartPosition).GetComponent<InGameCard>();
            card.transform.localPosition = Vector3.zero;
            card.transform.SetParent(playerHandParent);
            HandSorter.UpdateHandPositionsAndRotations(playerHandParent);
            Deck.RemoveAt(0);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DiscardRemainingCards()
    {
        IEnumerator DiscardCards(List<InGameCard> cards)
        {
            foreach(var card in cards)
            {
                DiscardPile.Add(card.CardName);
                card.SendToDiscard(discardPosition);
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        var list = new List<InGameCard>();

        foreach (Transform card in playerHandParent)
        {
            list.Add(card.GetComponent<InGameCard>());
        }
        StartCoroutine(DiscardCards(list));
    }

    public void ReplenishDeck()
    {
        replenishingDeck = true;
        foreach(var card in DiscardPile)
        {
            Deck.Add(card);
        }

        ShuffleDeck();
        DiscardPile = new List<string>();
        replenishingDeck = false;
    }

    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();

        int n = Deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = value;
        }
    }

    public void EndTurn()
    {
        DiscardRemainingCards();
    }

    public bool TryPlayCard(InGameCard card, float yPos)
    {
        if(card.CurrentStats.ManaCost <= CurrentRunManager.instance.Mana && yPos >= 500)
        {
            CurrentRunManager.instance.Mana -= card.CurrentStats.ManaCost;

            card.CurrentStats.DoAbility(CurrentRunManager.instance.Stats, FindObjectOfType<Enemy>().Stats);
            card.gameObject.SetActive(true);
            Destroy(card.gameObject, 1);
            DiscardPile.Add(card.CardName);

            HandSorter.UpdateHandPositionsAndRotations(playerHandParent);
            return true;
        }
        else
        {
            card.ReturnToHand(playerHandParent);
            HandSorter.UpdateHandPositionsAndRotations(playerHandParent);
            return false;
        }
    }
}