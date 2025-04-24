using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusEffectView : MonoBehaviour
{
  [Serializable]
  public class EffectTypeImageSource
  {
    public Character.StatusEffect.EffectTypes StatusEffectType;
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

  public Character.StatusEffect TargetStatusEffect { get; private set; }

  public async UniTask Set(Character.StatusEffect statusEffect, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    TargetStatusEffect = statusEffect;

    statusEffectTypeImage.sprite = Array.Find(effectTypeImageSource, item => item.StatusEffectType == statusEffect.Type).ImageSource;
    statusEffectValueText.text = statusEffect.Value.ToString();
    statusEffectTurnText.text = statusEffect.RemainingTurn.ToString();
    GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
    await GetComponent<RectTransform>().DOSizeDelta(new Vector2(50f, 50f), 0.5f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask UpdateTurn(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    statusEffectTypeImage.sprite = Array.Find(effectTypeImageSource, item => item.StatusEffectType == TargetStatusEffect.Type).ImageSource;
    statusEffectValueText.text = TargetStatusEffect.Value.ToString();
    statusEffectTurnText.text = TargetStatusEffect.RemainingTurn.ToString();
    RectTransform turnTextRectTransform = statusEffectTurnText.GetComponent<RectTransform>();
    Vector2 originalSizeDelta = turnTextRectTransform.sizeDelta;

    await turnTextRectTransform.DOSizeDelta(new Vector2(0f, 0f), 0.1f)
      .SetEase(Ease.OutSine)
      .ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
    await turnTextRectTransform.DOSizeDelta(originalSizeDelta, 0.1f)
      .SetEase(Ease.OutBounce)
      .ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask Delete(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    TargetStatusEffect = null;

    GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
    statusEffectValueText.gameObject.SetActive(false);
    statusEffectTurnText.gameObject.SetActive(false);

    await GetComponent<RectTransform>().DOSizeDelta(new Vector2(0f, 0f), 0.5f).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }
}

