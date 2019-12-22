using System.Collections;
using System.Collections.Generic;
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
        Dictionary<int, int> temp = Counter(player.cards);
        Thief.text = temp[(int)CardType.Thief].ToString();
        VP.text = temp[(int)CardType.VP].ToString();
        TotalCards.text = player.cards.Count.ToString();
    }


    private Dictionary<int, int> Counter(Dictionary<string, Card> cards){
        Dictionary<int, int> temp = new Dictionary<int, int>();

        foreach(KeyValuePair<string, Card> card in cards)
        {
            if(!card.Value.used) continue;
            
            if(!temp.ContainsKey((int)card.Value.cardType)){
                temp.Add((int)card.Value.cardType, 1);
            }else{
                temp[(int)card.Value.cardType] += 1;
            }

        }
        return temp;
    }
}
