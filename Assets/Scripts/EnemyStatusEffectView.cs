using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusEffectView : MonoBehaviour
{
  [Serializable]
  public class EffectTypeImageSource
  {
    public Enemy.StatusEffect.EffectTypes StatusEffectType;
    public Sprite ImageSource;
  }

  [SerializeField]
  private Image statusEffectTypeImage;

  [SerializeField]
  private TextMeshProUGUI statusEffectValueText;

  [SerializeField]
  private TextMeshProUGUI statusEffectTurnText;

  [SerializeField]
  private EffectTypeImageSource[] effectTypeImageSource;

  public async UniTask Set(Enemy.StatusEffect statusEffect, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    statusEffectTypeImage.sprite = Array.Find(effectTypeImageSource, item => item.StatusEffectType == statusEffect.Type).ImageSource;
    statusEffectValueText.text = statusEffect.Value.ToString();
    statusEffectTurnText.text = statusEffect.RemainingTurn.ToString();
    GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
    await GetComponent<RectTransform>().DOSizeDelta(new Vector2(50f, 50f), 0.5f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask Delete(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
    statusEffectValueText.gameObject.SetActive(false);
    statusEffectTurnText.gameObject.SetActive(false);

    await GetComponent<RectTransform>().DOSizeDelta(new Vector2(0f, 0f), 0.5f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }
}

