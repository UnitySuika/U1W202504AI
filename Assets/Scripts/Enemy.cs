using System;

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

  public Action<Battle> AI { get; private set; }

  public int MaxHp { get; private set; }

  public int Hp { get; private set; }

  public Enemy(EnemySource source, Battle battle)
  {
    Source = source;
    Parameters = source.Parameters;
    AI = source.GenerateAI(battle);
    MaxHp = Array.Find(Parameters, param => param.Id == "max_hp").Value;
    Hp = MaxHp;
  }
}
