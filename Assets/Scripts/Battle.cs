using System.Collections.Generic;
using UnityEngine;

public class Battle
{
  public List<Enemy> Enemies { get; private set; }

  public CardStack CStack { get; private set; }

  public int Energy { get; private set; }

  public Battle(EnemySource[] enemySources, int minEnemyNumber, int maxEnemyNumber, Deck deck)
  {
    Enemies = new List<Enemy>();
    int enemyNumber = Random.Range(minEnemyNumber, maxEnemyNumber + 1);
    for (int i = 0; i < enemyNumber; ++i)
    {
      EnemySource enemySource = enemySources[Random.Range(0, enemySources.Length)];
      Enemy enemy = new Enemy(enemySource, this);
      Enemies.Add(enemy);
    }

    CStack = new CardStack(deck);
    CStack.Shuffle();

    Energy = 0;
  }
}
