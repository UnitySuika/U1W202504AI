using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpView : MonoBehaviour
{
  [SerializeField]
  private Image hpBar;

  [SerializeField]
  private TextMeshProUGUI hpText;

  public void SetHp(int currentHp, int maxHp)
  {
    hpBar.fillAmount = (float)currentHp / maxHp;
    hpText.text = $"{currentHp} / {maxHp}";
  }
}
