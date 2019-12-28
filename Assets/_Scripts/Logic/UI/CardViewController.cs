using System.Collections;
using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour
{
    [Header("CardTexts")]
    public Text Thief;
    public Text VP;
    public Text TotalCards;


    public void UpdateCardCount(Player player) {
        var usedCards = player.cards.Values.Where(c => c.used);
        Thief.text = usedCards.Where(c => c.cardType == CardType.Thief).Count().ToString();
        VP.text = usedCards.Where(c => c.cardType == CardType.VP).Count().ToString();
        TotalCards.text = player.cards.Values.Where(c => !c.used).ToString();
    }
}
