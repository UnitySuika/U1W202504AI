using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTest : MonoBehaviour
{
  [SerializeField] private CardSource cardSource;

  private Deck deck;

  private void Start()
  {
    deck = new Deck();
    Card card = new Card(cardSource);

    deck.Add(card);
    
    foreach (Card c in deck.GetClone())
    {
      Debug.Log(c.Source.Id);
    }

    // 試しに発動
  }
}
