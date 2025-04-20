using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
  [SerializeField]
  private Button turnEndButton;

  [SerializeField]
  private TextMeshProUGUI turnText;

  public void Initialize(Battle battle)
  {
    turnEndButton.interactable = false;
      turnText.text = "***";
    turnEndButton.onClick.AddListener(() => battle.SetTurn(Battle.Turns.Enemy));
  }

  public void Set(Battle battle)
  {
    if (battle.Turn == Battle.Turns.Player)
    {
      turnEndButton.interactable = true;
      turnText.text = "プレイヤーターン終了";
    }
    else
    {
      turnEndButton.interactable = false;
      turnText.text = "敵のターン";
    }
  }
}
