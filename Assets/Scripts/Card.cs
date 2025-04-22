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

  public string PlayEffectRecursive(JsonExpression je, Battle battle, PlayedData playedData, BattleEventQueue beq)
  {
    if (je.Value != null)
    {
      return je.Value;
    }
    else if (je.Elements.ContainsKey("root"))
    {
      return PlayEffectRecursive(je.Elements["root"], battle, playedData, beq);
    }

    string type = PlayEffectRecursive(je.Elements["type"], battle, playedData, beq);

    switch (type)
    {
      case "conditional":
        bool condition;
        if (PlayEffectRecursive(je.Elements["condition"], battle, playedData, beq) == "TRUE")
        {
          condition = true;
        }
        else
        {
          condition = false;
        }

        if (condition)
        {
          return PlayEffectRecursive(je.Elements["then"], battle, playedData, beq);
        }

        break;
      case "comparison_greater":
        int l1 = int.Parse(PlayEffectRecursive(je.Elements["left"], battle, playedData, beq));
        int r1 = int.Parse(PlayEffectRecursive(je.Elements["right"], battle, playedData, beq));
        return (l1 > r1) ? "TRUE" : "FALSE";
      case "multiply":
        int l2 = int.Parse(PlayEffectRecursive(je.Elements["left"], battle, playedData, beq));
        int r2 = int.Parse(PlayEffectRecursive(je.Elements["right"], battle, playedData, beq));
        return (l2 * r2).ToString();
      // ここから関数群
      case "attack":
        int attackValue = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData, beq));

        bool isDead = playedData.SelectedEnemy.ReceiveDamage(attackValue, beq);

        return isDead ? "TRUE" : "FALSE";
      case "heal":
        int healValue = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData, beq));
        
        battle.MainCharacter.Heal(healValue, beq);

        break;
      case "card_exist_number":
        string included = PlayEffectRecursive(je.Elements["value"], battle, playedData, beq);
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
        string value = PlayEffectRecursive(je.Elements["value"], battle, playedData, beq);
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

  public void PlayEffect(Battle battle, PlayedData playedData, BattleEventQueue beq)
  {
    battle.SetEnergy(battle.Energy - Energy);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.DecreaseEnergy);
      be.Value = battle.Energy;
      beq.Enqueue(be);
    }

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CardPlayEffect);
      beq.Enqueue(be);
    }

    PlayEffectRecursive(EffectObject, battle, playedData, beq);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EndCardPlayEffect);
      beq.Enqueue(be);
    }
  }
}
