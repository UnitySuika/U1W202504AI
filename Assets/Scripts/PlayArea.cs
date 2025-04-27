using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private Image image;

  [SerializeField]
  private TextMeshProUGUI descriptionText;

  [SerializeField]
  private RectTransform rectTransform;

  [SerializeField]
  private Color notPointerOnColor;

  [SerializeField]
  private Color pointerOnColor;
  

  public bool IsPointerOn { get; private set; } = false;

  public bool IsValid { get; private set; }

  private RectTransform parentRectTransform;

  private Camera mainCamera;

  public void Validate()
  {
    IsValid = true;
    Debug.Log(descriptionText);
    Debug.Log(descriptionText.gameObject);
    descriptionText.gameObject.SetActive(true);
    image.DOFade(0.25f, 0.25f);
  }
  public void Invalidate()
  {
    IsValid = false;
    IsPointerOn = false;
    descriptionText.gameObject.SetActive(false);
    image.DOFade(0f, 0.1f);
  }

  private void Start()
  {
    parentRectTransform = (RectTransform)transform.parent;
    mainCamera = Camera.main;
  }

  private void Update()
  {
    if (IsValid)
    {
      RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, mainCamera, out Vector2 mousePos);

      Vector2 ap = rectTransform.anchoredPosition;
      if (mousePos.x > ap.x - rectTransform.sizeDelta.x / 2f && 
          mousePos.x < ap.x + rectTransform.sizeDelta.x / 2f &&
          mousePos.y > ap.y - rectTransform.sizeDelta.y / 2f && 
          mousePos.y < ap.y + rectTransform.sizeDelta.y / 2f)
      {
        IsPointerOn = true;
        Color c = pointerOnColor;
        c.a = image.color.a;
        image.color = c;
      }
      else
      {
        IsPointerOn = false;
        Color c = notPointerOnColor;
        c.a = image.color.a;
        image.color = c;
      }
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (IsValid)
    {
      IsPointerOn = true;
    }
  }
  public void OnPointerExit(PointerEventData eventData)
  {
    if (IsValid)
    {
      IsPointerOn = false;
    }
  }
}
