using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using unityroom.Api;

public class ClearSceneView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI turnSumText;

  private void Start()
  {
    turnSumText.text = $"経過ターン数： {BattleInformation.TurnSum}";
    AudioManager.Instance.PlaySe("clear", false);
    UnityroomApiClient.Instance.SendScore(2, BattleInformation.TurnSum, ScoreboardWriteMode.HighScoreAsc);
  }

  public void ToTitleButton()
  {
    if (TransitionMotionManager.Instance.IsTransitioning) return;
    BattleInformation.Initialize();
    TransitionMotionManager.Instance.PlayTransitionMotion("Title", TransitionMotionManager.TransitionMotionTypes.Horizontal).Forget();
  }
}