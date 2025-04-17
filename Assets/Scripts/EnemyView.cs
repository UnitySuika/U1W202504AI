using TMPro;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI nameText;
  
  [SerializeField]
  private HpView hpView;

  public void Initialize(Enemy enemy)
  {
    nameText.text = enemy.Source.Id;
    hpView.SetHp(enemy.Hp, enemy.MaxHp);
  }
}
