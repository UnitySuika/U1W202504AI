using System.Collections.Generic;

public class CardStack
{
  private LinkedList<Card> cards;

  public CardStack(Deck deck)
  {
    cards = new LinkedList<Card>(deck.GetClone());
  }

  public Card ShowTop()
  {
    return cards.Last.Value;
  }

  public Card PickUpTop()
  {
    Card card = cards.Last.Value;
    cards.RemoveLast();
    return card;
  }
  
  public void InsertBottom(Card card)
  {
    cards.AddFirst(card);
  }
}
