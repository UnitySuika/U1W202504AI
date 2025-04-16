using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
  public List<Card> cards;

  public Deck()
  {
    cards = new List<Card>();
  }

  public void Add(Card card)
  {
    cards.Add(card);
  }

  public void Remove(Card card)
  {
    cards.Remove(card);
  }

  public List<Card> GetClone()
  {
    return new List<Card>(cards);
  }
}
