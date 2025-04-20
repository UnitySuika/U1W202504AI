using System;
using System.Collections;
using System.Collections.Generic;
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

  public void Set(Enemy.StatusEffect statusEffect)
  {
    statusEffectTypeImage.sprite = Array.Find(effectTypeImageSource, item => item.StatusEffectType == statusEffect.Type).ImageSource;
    statusEffectValueText.text = statusEffect.Value.ToString();
    statusEffectTurnText.text = statusEffect.RemainingTurn.ToString();
  }
}

