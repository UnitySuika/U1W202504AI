using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
  [System.Serializable]
  public class Parameter
  {
    public string Id;
    public int Value;
  }

  public CardSource Source;

  public int Energy;

  public Parameter[] Parameters;

  public JsonExpression Effect;

  public Card(CardSource source)
  {
    Source = source;
    Energy = source.Energy;
    Parameters = source.Parameters;
    Effect = CardUtility.CardLangToObject(source.Effect);
    // Debug.Log(CardUtility.JsonExpressionToString(Effect));
  }
}
