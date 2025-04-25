using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Parameter(string id, int value)
    {
      Id = id; 
      Value = value;
    }
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

  public int LoveCost;

  public int LoveNumber { get; private set; }

  public string EffectDescription { get; private set; }

  public void SetEffectDescription()
  {
    EffectDescription = "";
    bool isReading = false;
    string reading = "";
    for (int i = 0; i < Source.EffectDescription.Length; ++i)
    {
      char c = Source.EffectDescription[i];
      if (c == '{')
      {
        isReading = true;
        reading = "";
      }
      else if (c == '}')
      {
        isReading = false;
        foreach (Parameter parameter in Parameters)
        {
          if (reading == parameter.Id)
          {
            EffectDescription += $" {parameter.Value} ";
          }
        }
      }
      else if (isReading)
      {
        reading += c;
      }
      else
      {
        EffectDescription += c;
      }
    }
  }

  public Card(CardSource source)
  {
    Source = source;
    Energy = source.Energy;
    CardType = source.CardType;

    Parameters = new Parameter[source.Parameters.Length];
    for (int i = 0; i < source.Parameters.Length; ++i)
    {
      Parameter param = source.Parameters[i];
      Parameters[i] = new Parameter(param.Id, param.Value);
    }
    
    EffectObject = CardUtility.CardLangToObject(source.EffectCLANG);
    RandomNumber = UnityEngine.Random.Range(0, 10);
    LoveCost = source.LoveCost;

    SetEffectDescription();

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
        else
        {
          if (je.Elements.ContainsKey("else"))
          {
            return PlayEffectRecursive(je.Elements["else"], battle, playedData, beq);
          }
          else
          {
            return "NULL";
          }
        }
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
      case "all_attack":
        int attackValue2 = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData, beq));

        foreach (Enemy enemy in battle.Enemies)
        {
          enemy.ReceiveDamage(attackValue2, beq);
        }

        return "NULL";
      case "heal":
        int healValue = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData, beq));
        
        battle.MainCharacter.Heal(healValue, beq);

        break;
      case "defence_up":
        int defence_up_value = int.Parse(PlayEffectRecursive(je.Elements["value"], battle, playedData, beq));

        int defence_up_turn = int.Parse(PlayEffectRecursive(je.Elements["turn"], battle, playedData, beq));

        Character.StatusEffect statusEffect = new Character.StatusEffect(Character.StatusEffect.EffectTypes.DefenceUp, defence_up_value, defence_up_turn);
        battle.MainCharacter.StatusEffects.Add(statusEffect);

        {
          BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CharacterGetStatusEffect);
          be.TargetCharacterStatusEffects = new List<Character.StatusEffect>()
          { statusEffect };
          beq.Enqueue(be);
        }
        
        break;
      case "card_exist_number":
        string included = PlayEffectRecursive(je.Elements["value"], battle, playedData, beq);
        int existCount = 0;
        foreach (Card card in battle.Hand)
        {
          if (card.Source.EffectDescription.Contains(included))
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
          case "PLAYER_MAXHP_HALF":
            return (battle.MainCharacter.MaxHp / 2).ToString();
          default:
            foreach (Parameter parameter in Parameters)
            {
              if (parameter.Id == value)
              {
                return parameter.Value.ToString();
              }
            }
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

  public void AddLove()
  {
    ++LoveNumber;
  }

  public void Enhance(int ratio)
  {
    foreach (Parameter parameter in Parameters)
    {
      parameter.Value *= ratio;
    }

    SetEffectDescription();
  }
}
