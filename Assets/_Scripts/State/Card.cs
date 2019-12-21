namespace State
{
using System;
    public enum CardType
    {
        Thief
    }

    [System.Serializable]
    public class Card
    {
        public Guid id;
        public CardType cardType;
        public bool used;
        public Card(Guid id, CardType cardType){
            this.id = id;
            this.cardType = cardType;
        }

        public void UseCard(){
            used = true;
        }

    }

}