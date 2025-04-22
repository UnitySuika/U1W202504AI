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
    turnEndButton.onClick.AddListener(() => 
    {
      battle.SetTurn(Battle.Turns.Enemy);
      AudioManager.Instance.PlaySe("button_click", false);
    });
  }

  public void Set(Battle battle)
  {
    if (battle.State != Battle.States.Main)
    {
      turnEndButton.interactable = false;
      turnText.text = "***";
    }
    else if (battle.Turn == Battle.Turns.Player)
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

  public void Invalidate()
  {
    turnEndButton.interactable = false;
  }
  public void Validate()
  {
    turnEndButton.interactable = true;
  }
}
