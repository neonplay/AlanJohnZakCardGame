using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayingManager : MonoBehaviour
{
    InGameCard currentCard = null;

    public Transform playerHandParent;
    public Transform heldCardParent;

    public int Mana = 20;

    private void Start()
    {
        HandSorter.UpdateHandPositionsAndRotations(playerHandParent);
    }

    public void CardSelectedFromHand(InGameCard card)
    {
        card.transform.SetParent(heldCardParent);
    }

    public bool TryPlayCard(InGameCard card, float yPos)
    {
        if(card.ManaCost <= Mana && yPos >= 500)
        {
            //take mana away
            //do card ability
            //destroy card
            Destroy(card.gameObject);

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
