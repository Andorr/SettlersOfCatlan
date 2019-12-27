using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using State;

public class CardItemController : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject cardView;
    public GameObject cardObject;
    public GameObject cardContainer;
    public Player player;
    public Dictionary<string, GameObject> cardListGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Card> tempCards = new Dictionary<string, Card>();

    public void UpdateCards(Player player){
        ClearCards();

        List<Card> cacheCardList = CardUpdateCards(player.cards);


        foreach(Card card in cacheCardList)
        {
            GameObject cardEntryObject = Instantiate(cardObject);

            cardEntryObject.transform.SetParent(cardContainer.transform);
            cardEntryObject.transform.localScale = Vector3.one;
            cardEntryObject.transform.Find("Title").GetComponent<Text>().text = card.getTitle();
            cardEntryObject.GetComponent<Clickable>().OnClick += () => {

            };
            cardListGameObjects.Add(card.id, cardEntryObject);
        }
    }

    private void ClearCards(){
        foreach(GameObject obj in cardListGameObjects.Values)
        {
            Destroy(obj);
        }
        cardListGameObjects.Clear();
    }

    private List<Card> CardUpdateCards(Dictionary<string, Card> newCards){
        List<Card> temp = new List<Card>();

        foreach(Card card in newCards.Values){
            if(!card.used){
                temp.Add(card);
            }
        }
        return temp;

    }
}
