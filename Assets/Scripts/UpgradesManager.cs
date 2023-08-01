using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public GameObject UpgradesPanel;
    public Transform UpgradeableCardsParent;
    public GameObject CardListPanel;

    [Header("Upgrade confirmation")]
    public GameObject upgradeConfirmationScreen;
    public Transform baseCardHolder;
    public Transform upgradedCardHolder;

    [Header("Upgrade complete")]
    public GameObject UpgradeCompleteScreen;
    public Transform UpgradedCardHolder;

    AllCardsData allCardsData;

    private void Awake()
    {
        allCardsData = FindObjectOfType<AllCardsData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) OpenUpgradesPanel();
    }

    public void OpenUpgradesPanel()
    {
        upgradeConfirmationScreen.SetActive(false);
        UpgradeCompleteScreen.SetActive(false);
        UpgradesPanel.SetActive(true);
        CardListPanel.SetActive(true);

        var cardsToShow = new List<InGameCard>();
        int index = 0;
        foreach (var cardName in CurrentRunManager.instance.PlayerDeck)
        {
            var card = allCardsData.CardsDictionary[cardName];
            if (!card.isUpgraded)
            {
                var item = Instantiate(card, UpgradeableCardsParent);
                item.upgrading = true;
                item.deckIndex = index;
            }

            index++;
        }
    }

    public void ShowUpgradeConfirm(InGameCard card)
    {
        var baseCard = Instantiate(allCardsData.CardsDictionary[card.CardName], baseCardHolder);
        var upgradedCard = Instantiate(allCardsData.UpgradeForBaseCard[card.CardName], upgradedCardHolder);

        upgradeConfirmationScreen.SetActive(true);
        cardToUpgrade = card.CardName;
    }

    string cardToUpgrade;

    public void ConfirmedUpgrade()
    {
        DestroyAllCards();
        ShowUpgradedCard();
    }

    private void DestroyAllCards()
    {
        var destroyList = new List<GameObject>();
        foreach (Transform item in UpgradeableCardsParent)
        {
            destroyList.Add(item.gameObject);
        }

        foreach (var item in destroyList)
        {
            Destroy(item);
        }
    }

    public void ShowUpgradedCard()
    {
        upgradeConfirmationScreen.SetActive(false);
        CardListPanel.SetActive(false);
        UpgradeCompleteScreen.SetActive(true);
        var upgradedCard = Instantiate(allCardsData.UpgradeForBaseCard[cardToUpgrade], UpgradedCardHolder);
        upgradedCard.transform.localScale = new Vector3(2, 2, 2);
        upgradedCard.enabled = false;

        StartCoroutine(CloseScreen());

        IEnumerator CloseScreen()
        {
            yield return new WaitForSeconds(2);
            CloseUpgradeScreen();
        }
    }

    public void CancelUpgradeConfirmation()
    {
        Destroy(baseCardHolder.GetChild(0));
        Destroy(upgradedCardHolder.GetChild(0));
        upgradeConfirmationScreen.SetActive(false);
        cardToUpgrade = "";
    }

    public void CloseUpgradeScreen()
    {
        DestroyAllCards();
        if(UpgradedCardHolder.childCount > 0)
        {
            Destroy(upgradedCardHolder.GetChild(0).gameObject);
        }
        UpgradeCompleteScreen.SetActive(false);
        UpgradesPanel.SetActive(false);

    }
}
