using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTest : MonoBehaviour
{
  [SerializeField] private EnemySource[] enemySources;
  [SerializeField] private CardSource cardSource;

  private void Start()
  {
    Deck deck = new Deck();
    deck.Add(new Card(cardSource));
    Battle battle = new Battle(enemySources, 1, 3, deck);
    Debug.Log("敵\n");
    foreach (Enemy e in battle.Enemies)
    {
      Debug.Log(e.Source.Id);
    }
  }
}
