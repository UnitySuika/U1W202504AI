using TMPro;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI nameText;
  
  [SerializeField]
  private HpView hpView;

  public void Set(Character character)
  {
    nameText.text = character.Name;
    hpView.SetHp(character.Hp, character.MaxHp);
  }
}
