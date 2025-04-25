using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using unityroom.Api;

public class LoseSceneView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI floorNumberText;

  [SerializeField]
  private TextMeshProUGUI turnSumText;

  private void Start()
  {
    floorNumberText.text = $"{BattleInformation.FloorNumber} 階まで到達した！";
    turnSumText.text = $"経過ターン数： {BattleInformation.TurnSum}";
    UnityroomApiClient.Instance.SendScore(1, BattleInformation.FloorNumber - 1, ScoreboardWriteMode.HighScoreDesc);
  }

  public void NextGameButton()
  {
    if (TransitionMotionManager.Instance.IsTransitioning) return;
    BattleInformation.Initialize();
    TransitionMotionManager.Instance.PlayTransitionMotion("Battle", TransitionMotionManager.TransitionMotionTypes.Horizontal).Forget();
  }
}
