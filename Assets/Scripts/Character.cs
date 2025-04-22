using UnityEngine;

public class Character
{
  public string Name { get; private set; }
  public int MaxHp { get; private set; }
  public int Hp { get; private set; }

  public Character(string name, int maxHp)
  {
    Name = name;
    MaxHp = maxHp;
    Hp = MaxHp;
  }

  public void ReceiveDamage(int damageValue, BattleEventQueue beq)
  {
    Hp = Mathf.Max(0, Hp - damageValue);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CharacterDamage);
      be.Value = Hp;
      beq.Enqueue(be);
    }
  }

  public void Heal(int healValue, BattleEventQueue beq)
  {
    Hp = Mathf.Min(MaxHp, Hp + healValue);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CharacterHeal);
      be.Value = Hp;
      beq.Enqueue(be);
    }
  }
}
