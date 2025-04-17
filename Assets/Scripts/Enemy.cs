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

  public Enemy(EnemySource source, Battle battle)
  {
    Source = source;
    Parameters = source.Parameters;
    AI = source.GenerateAI(battle);
  }
}
