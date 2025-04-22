using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActionView : MonoBehaviour
{
  [Serializable]
  public class ActionKindImageSource
  {
    public Enemy.EnemyActionTypes EnemyAction;
    public Sprite ImageSource;
  }

  [SerializeField]
  private Image actionKindImage;

  [SerializeField]
  private TextMeshProUGUI actionValueText;

  [SerializeField]
  private ActionKindImageSource[] actionKindImageSources;

  [SerializeField]
  private Image playImage;

  public Enemy.EnemyActionData ActionData;

  public void Set(Enemy.EnemyActionData actionData)
  {
    ActionData = actionData;
    actionKindImage.sprite = Array.Find(actionKindImageSources, item => item.EnemyAction == actionData.ActionType).ImageSource;
    actionValueText.text = actionData.Value.ToString();
  }

  public async UniTask Play(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    playImage.gameObject.SetActive(true);
    await playImage.DOFade(1f, 0.3f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
    await playImage.DOFade(0f, 0.2f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }
}
