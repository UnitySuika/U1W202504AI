using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI valueText;

  public void Set(int value)
  {
    valueText.text = value.ToString();
  }
}
