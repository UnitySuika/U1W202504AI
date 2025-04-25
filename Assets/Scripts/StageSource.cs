using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Stage")]
public class StageSource : ScriptableObject
{
  public int EnemyNumberMin;

  public int EnemyNumberMax;

  public EnemySource[] SelectiveEnemies;
}
