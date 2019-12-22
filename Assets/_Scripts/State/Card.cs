namespace State
{
using System;
    public enum CardType
    {
        Thief,
        VP
    }

    [System.Serializable]
    public class Card
    {
        public string id;
        public CardType cardType;
        public bool used = false;
        public Card(string id, CardType cardType){
            this.id = id;
            this.cardType = cardType;
        }

        public void UseCard(){
            used = true;
        }

    }

}