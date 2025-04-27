using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewsReference : MonoBehaviour
{
  [SerializeField]
  private bool isFlip;

  [SerializeField]
  private float scrollSpeed;
  private void Update()
  {
    float mouseScroll = isFlip ? Input.mouseScrollDelta.y : -Input.mouseScrollDelta.y;
    if (mouseScroll != 0f)
    {
      Vector2 currentPos = GetComponent<RectTransform>().anchoredPosition;
      GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Clamp(currentPos.x + mouseScroll * scrollSpeed, -1920 * 2, 0), currentPos.y);
    }
  }
}
