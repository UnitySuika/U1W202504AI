using System.Collections.Generic;
using UnityEngine;

public class Battle
{
  public List<Enemy> Enemies { get; private set; }

  public CardStack CStack { get; private set; }

  public int Energy { get; private set; }

  public enum Turns
  {
    Player,
    Enemy,
  }

  public Turns Turn { get; private set; }

  public List<Card> Hand { get; private set; }

  public Character MainCharacter { get; private set; }

  public Battle(Character mainCharacter, EnemySource[] enemySources, int minEnemyNumber, int maxEnemyNumber, Deck deck)
  {
    MainCharacter = mainCharacter;
    Hand = new List<Card>();
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

    Turn = Turns.Player;
  }

  public void SetEnergy(int value)
  {
    Energy = value;
  }

  public Card DealCard()
  {
    if (CStack.ShowTop() == null) return null;
    Card dealed = CStack.PickUpTop();
    Hand.Add(dealed);
    return dealed;
  }

  public void SetNextEnemyActions()
  {
    foreach (Enemy enemy in Enemies)
    {
      enemy.SetNextActionData(this);
    }
  }
}
