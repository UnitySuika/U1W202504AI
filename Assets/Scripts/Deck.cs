using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
  public List<Card> cards;

  public Deck()
  {
    cards = new List<Card>();
  }
}
