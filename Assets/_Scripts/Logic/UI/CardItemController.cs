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
    public Sprite[] cardImages;
    public Dictionary<string, GameObject> cardListGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Card> tempCards = new Dictionary<string, Card>();

    public void UpdateCards(PlayerController player){
        ClearCards();

        List<Card> cacheCardList = CardUpdateCards(player.player.cards);


        foreach(Card card in cacheCardList)
        {
            GameObject cardEntryObject = Instantiate(cardObject);

            cardEntryObject.transform.SetParent(cardContainer.transform);
            cardEntryObject.transform.localScale = Vector3.one;
            cardEntryObject.GetComponent<Image>().sprite = getCardSprite(card);
      
            cardEntryObject.GetComponent<Clickable>().ResetEvent();
            cardEntryObject.GetComponent<Clickable>().OnClick += () => {
                player.UseCard(card.id);
                Disable();
            };

            cardListGameObjects.Add(card.id, cardEntryObject);
        }
    }

    public void showCardItems(){
        this.cardView.SetActive(true);
    }

    private Sprite getCardSprite(Card card){
        switch(card.cardType){
            case CardType.Thief:
                return cardImages[0];
            case CardType.VP:
                return cardImages[1];
            default:
                return cardImages[2];
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
    private void Disable(){
        this.cardView.SetActive(false);
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Disable();
        }
    }
}
