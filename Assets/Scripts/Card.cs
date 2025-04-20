using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
  public class PlayedData
  {
    public enum PlayedTypes
    {
      Normal,
      Enemy,
      Special,
    }

    public PlayedTypes PlayedType;

    public Enemy SelectedEnemy;

    public PlayedData(PlayedTypes playedType, Enemy selectedEnemy)
    {
      PlayedType = playedType;
      SelectedEnemy = selectedEnemy;
    }
  }

  [System.Serializable]
  public class Parameter
  {
    public string Id;
    public int Value;
  }

  public enum CardTargetTypes
  {
    Normal,
    ForEnemy,
  }

  public CardTargetTypes CardType;

  public CardSource Source;

  public int Energy;

  public Parameter[] Parameters;

  public JsonExpression EffectObject;

  public int RandomNumber;

  public Card(CardSource source)
  {
    Source = source;
    Energy = source.Energy;
    CardType = source.CardType;
    Parameters = source.Parameters;
    EffectObject = CardUtility.CardLangToObject(source.EffectCLANG);
    RandomNumber = UnityEngine.Random.Range(0, 10);
    // Debug.Log(CardUtility.JsonExpressionToString(Effect));
  }

  public string PlayEffectRecursive(JsonExpression je, Battle battle, PlayedData playedData)
  {
    if (je.Value != null)
    {
      return je.Value;
    }
    else if (je.Elements.ContainsKey("root"))
    {
      return PlayEffectRecursive(je.Elements["root"], battle, playedData);
    }

    string type = PlayEffectRecursive(je.Elements["type"], battle, playedData);

    switch (type)
    {
      case "conditional":
        bool condition;
        if (PlayEffectRecursive(je.Elements["condition"], battle, playedData) == "TRUE")
        {
          condition = true;
        }
        else
        {
          condition = false;
        }

        if (condition)
        {
          return PlayEffectRecursive(je.Elements["then"], battle, playedData);
        }

        break;
      case "comparison_greater":
        int l1 = int.Parse(PlayEffectRecursive(je.Elements["left"], battle, playedData));
        int r1 = int.Parse(PlayEffectRecursive(je.Elements["right"], battle, playedData));
        return (l1 > r1) ? "TRUE" : "FALSE";
      case "multiply":
        int l2 = int.Parse(PlayEffectRecursive(je.Elements["left"], battle, playedData));
        int r2 = int.Parse(PlayEffectRecursive(je.Elements["right"], battle, playedData));
        return (l2 * r2).ToString();
      // ここから関数群
      case "attack":
        int attackValue = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData));
        return playedData.SelectedEnemy.ReceiveDamage(attackValue) ? "TRUE" : "FALSE";
      case "heal":
        int healValue = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData));
        battle.MainCharacter.Heal(healValue);
        break;
      case "card_exist_number":
        string included = PlayEffectRecursive(je.Elements["value"], battle, playedData);
        int existCount = 0;
        foreach (Card card in battle.Hand)
        {
          if (card.Source.Id.Contains(included))
          {
            ++existCount;
          }
        }
        return existCount.ToString();
      case "value":
        string value = PlayEffectRecursive(je.Elements["value"], battle, playedData);
        switch (value)
        {
          case "PLAYER_HP":
            return battle.MainCharacter.Hp.ToString();
          default:
            return value;
        }
      default:
        break;
    }

    // とりあえず返す
    return "NULL";
  }

  public void PlayEffect(Battle battle, PlayedData playedData)
  {
    battle.SetEnergy(battle.Energy - Energy); 
    PlayEffectRecursive(EffectObject, battle, playedData);
  }
}
