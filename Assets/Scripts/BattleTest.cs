using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTest : MonoBehaviour
{
  [SerializeField] private EnemySource[] enemySources;

  private void Start()
  {
    Battle battle = new Battle(enemySources, 1, 3, new Deck());
    Debug.Log("敵\n");
    foreach (Enemy e in battle.Enemies)
    {
      Debug.Log(e.Source.Id);
    }
  }
}
