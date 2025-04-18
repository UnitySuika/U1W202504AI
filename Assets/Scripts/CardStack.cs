using System.Collections.Generic;
using UnityEngine;

public class CardStack
{
  private LinkedList<Card> cards;

  public CardStack(Deck deck)
  {
    cards = new LinkedList<Card>(deck.GetClone());
  }

  public Card ShowTop()
  {
    if (cards.Count == 0) return null;
    return cards.Last.Value;
  }

  public Card PickUpTop()
  {
    if (cards.Count == 0) return null;
    Card card = cards.Last.Value;
    cards.RemoveLast();
    return card;
  }
  
  public void InsertBottom(Card card)
  {
    cards.AddFirst(card);
  }

  public void Shuffle()
  {
    LinkedList<Card> nextCards = new LinkedList<Card>();
    for (int notErased = cards.Count; notErased > 0; --notErased)
    {
      int eraseIndex = Random.Range(0, notErased);
      LinkedListNode<Card> eraseTarget = cards.First;
      for (int i = 0; i < eraseIndex; ++i)
      {
        eraseTarget = eraseTarget.Next;
      }
      nextCards.AddLast(eraseTarget.Value);
      cards.Remove(eraseTarget);
    }
    cards = nextCards;
  }
}
