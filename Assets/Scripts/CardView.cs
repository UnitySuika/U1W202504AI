using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private TextMeshProUGUI nameText;

  [SerializeField]
  private TextMeshProUGUI effectText;

  [SerializeField]
  private Image frameImage;

  [SerializeField]
  private Image barImage;

  [SerializeField]
  private Button cardButton;

  public Card ViewCard { get; private set; }

  private BattleSceneManager battleSceneManager;

  public void Initialize(Card card, BattleSceneManager battleSceneManager)
  {
    ViewCard = card;
    this.battleSceneManager = battleSceneManager;
    cardButton.onClick.AddListener(() => 
    {
      battleSceneManager.OnClickCard();
    });
    Set(card);
  }

  public void Set(Card card)
  {
    nameText.text = card.Source.Id;
    effectText.text = card.Source.EffectDescription;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    frameImage.color = Color.blue;
    barImage.color = Color.blue;
  }
  public void OnPointerExit(PointerEventData eventData)
  {
    frameImage.color = Color.white;
    barImage.color = Color.white;
  }
}
