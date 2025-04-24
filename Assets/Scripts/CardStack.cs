using System.Collections.Generic;
using UnityEngine;

public class CardStack
{
  public LinkedList<Card> Cards { get; private set; }

  public CardStack(Deck deck)
  {
    Cards = new LinkedList<Card>(deck.GetClone());
  }

  public Card ShowTop()
  {
    if (Cards.Count == 0) return null;
    return Cards.Last.Value;
  }

  public Card PickUpTop()
  {
    if (Cards.Count == 0) return null;
    Card card = Cards.Last.Value;
    Cards.RemoveLast();
    return card;
  }
  
  public void InsertBottom(Card card)
  {
    Cards.AddFirst(card);
  }

  public void Shuffle()
  {
    LinkedList<Card> nextCards = new LinkedList<Card>();
    for (int notErased = Cards.Count; notErased > 0; --notErased)
    {
      int eraseIndex = Random.Range(0, notErased);
      LinkedListNode<Card> eraseTarget = Cards.First;
      for (int i = 0; i < eraseIndex; ++i)
      {
        eraseTarget = eraseTarget.Next;
      }
      nextCards.AddLast(eraseTarget.Value);
      Cards.Remove(eraseTarget);
    }
    Cards = nextCards;
  }

  public void ReturnToDeck()
  {
    BattleInformation.Deck = new Deck();

    int c = 1;
    LinkedListNode<Card> node = Cards.First;

    while (node != Cards.Last)
    {
      node = node.Next;
      ++c;
    }

    while (Cards.Count > 0)
    {
      LinkedListNode<Card> cardNode = Cards.First;
      BattleInformation.Deck.Add(cardNode.Value);
      Cards.RemoveFirst();
    }
  }
}
