using System;
using System.Collections.Generic;

public class Enemy
{
  [Serializable]
  public class Parameter
  {
    public string Id;
    public int Value;
  }

  public EnemySource Source { get; private set; }

  public Parameter[] Parameters { get; private set; }

  public Func<Battle, List<EnemyActionData>> AI { get; private set; }

  public int MaxHp { get; private set; }

  public int Hp { get; private set; }

  public enum EnemyActions
  {
    Attack,
    Defend,
    Heal,
  }

  public class EnemyActionData
  {
    public EnemyActions EnemyAction;
    public int Value;

    public EnemyActionData(EnemyActions enemyAction, int value)
    {
      EnemyAction = enemyAction;
      Value = value;
    }
  }

  public List<EnemyActionData> NextActions { get; private set; }

  public Enemy(EnemySource source, Battle battle)
  {
    Source = source;
    Parameters = source.Parameters;
    AI = source.GenerateAI();
    MaxHp = Array.Find(Parameters, param => param.Id == "max_hp").Value;
    Hp = MaxHp;
  }

  public void SetNextActionData(Battle battle)
  {
    NextActions = AI(battle);
  }
}
