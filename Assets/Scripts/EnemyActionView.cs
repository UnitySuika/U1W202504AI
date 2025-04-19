using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActionView : MonoBehaviour
{
  [Serializable]
  public class ActionKindImageSource
  {
    public Enemy.EnemyActions EnemyAction;
    public Sprite ImageSource;
  }

  [SerializeField]
  private Image actionKindImage;

  [SerializeField]
  private TextMeshProUGUI actionValueText;

  [SerializeField]
  private ActionKindImageSource[] actionKindImageSources;

  public void Set(Enemy.EnemyActionData actionData)
  {
    actionKindImage.sprite = Array.Find(actionKindImageSources, item => item.EnemyAction == actionData.EnemyAction).ImageSource;
    actionValueText.text = actionData.Value.ToString();
  }
}
