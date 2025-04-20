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

  public void ReceiveDamage(int damageValue)
  {
    Hp = Mathf.Max(0, Hp - damageValue);
  }

  public void Heal(int healValue)
  {
    Hp = Mathf.Min(MaxHp, Hp + healValue);
  }
}
