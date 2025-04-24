using System.Collections.Generic;
using UnityEngine;

public class Battle
{
  public List<Enemy> Enemies { get; private set; }

  public CardStack CStack { get; private set; }

  public int Energy { get; private set; }

  public enum States
  {
    Main,
    Win,
    Lose,
  }

  public enum Turns
  {
    Player,
    Enemy,
  }

  public States State { get; private set; }

  public Turns Turn { get; private set; }

  public List<Card> Hand { get; private set; }

  public Character MainCharacter { get; private set; }

  public Card SpecialCard { get; private set; }

  public Battle(Character mainCharacter, EnemySource[] enemySources, int minEnemyNumber, int maxEnemyNumber, Deck deck)
  {
    State = States.Main;
    MainCharacter = mainCharacter;
    Hand = new List<Card>();
    Enemies = new List<Enemy>();
    int enemyNumber = Random.Range(minEnemyNumber, maxEnemyNumber + 1);
    for (int i = 0; i < enemyNumber; ++i)
    {
      EnemySource enemySource = enemySources[Random.Range(0, enemySources.Length)];
      Enemy enemy = new Enemy(enemySource);
      Enemies.Add(enemy);
    }

    CStack = new CardStack(deck);
    CStack.Shuffle();

    Energy = 0;

    SetTurn(Turns.Player);

    AudioManager.Instance.PlayBgm("battle");
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

  public void InsertCard(Card card)
  {
    Hand.Remove(card);
    CStack.InsertBottom(card);
  }

  public void SetNextEnemyActions()
  {
    foreach (Enemy enemy in Enemies)
    {
      enemy.SetNextActionData(this);
    }
  }

  public void UpdateStatus()
  {
    List<Enemy> nextEnemies = new List<Enemy>();
    foreach (Enemy enemy in Enemies)
    {
      if (enemy.Hp > 0)
      {
        nextEnemies.Add(enemy);
      }
    }
    Enemies = nextEnemies;
  }

  public void EvaluateState()
  {
    if (MainCharacter.Hp == 0)
    {
      State = States.Lose;
    }
    else if (Enemies.Count == 0)
    {
      State = States.Win;
    }
  }

  public void SetTurn(Turns turn)
  {
    Turn = turn;
  }

  public void EnemyTurnStart()
  {
    foreach (Enemy enemy in Enemies)
    {
      enemy.AdvanceTurn();
    }
  }

  public void ReturnStackToDeck()
  {
    CStack.ReturnToDeck();
  }

  public void CardToSpecial(Card card)
  {
    Hand.Remove(card);
    SpecialCard = card;
  }

  public void DestroySpecialCard()
  {
    SpecialCard = null;
  }
}
