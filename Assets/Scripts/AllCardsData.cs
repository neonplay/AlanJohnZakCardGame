using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCardsData : MonoBehaviour
{
    public List<CardAndUpgrade> CardsList;
    public Dictionary<string, InGameCard> CardsDictionary;
    public Dictionary<string, InGameCard> UpgradeForBaseCard;

    private void Awake()
    {
        CardsDictionary = new Dictionary<string, InGameCard>();
        UpgradeForBaseCard = new Dictionary<string, InGameCard>();
        foreach(var card in CardsList)
        {
            CardsDictionary.Add(card.Base.CardName, card.Base);
            CardsDictionary.Add(card.Upgrade.CardName, card.Upgrade);
            UpgradeForBaseCard.Add(card.Base.CardName, card.Upgrade);
        }
    }
}

[System.Serializable]
public class CardAndUpgrade
{
    public InGameCard Base;
    public InGameCard Upgrade;
    public bool isRewardable;
}
