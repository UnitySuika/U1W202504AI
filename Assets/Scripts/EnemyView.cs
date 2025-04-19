using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI nameText;
  
  [SerializeField]
  private HpView hpView;
  
  [SerializeField]
  private EnemyActionView enemyActionViewPrefab;

  [SerializeField]
  private RectTransform enemyActionViewParent;

  public Enemy TargetEnemy { get; private set; }

  public void Set(Enemy enemy)
  {
    TargetEnemy = enemy;
    nameText.text = enemy.Source.Id;
    hpView.SetHp(enemy.Hp, enemy.MaxHp);
  }

  public void SetActions()
  {
    foreach (Transform t in enemyActionViewParent)
    {
      Destroy(t.gameObject);
    }
    
    for (int i = 0; i < TargetEnemy.NextActions.Count; ++i)
    {
      EnemyActionView view = Instantiate(enemyActionViewPrefab, enemyActionViewParent);
      view.Set(TargetEnemy.NextActions[i]);
    }
  }
}
