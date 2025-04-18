using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
  [SerializeField]
  private float handInterval;
  
  [SerializeField]
  private float handPosY;

  [SerializeField]
  private CardView cardViewPrefab;

  [SerializeField]
  private Transform cardViewParent;

  private List<CardView> cardViews;

  private BattleSceneManager battleSceneManager;

  public void Initialize(BattleSceneManager battleSceneManager)
  {
    cardViews = new List<CardView>();
    this.battleSceneManager = battleSceneManager;
  }

  public void AddCard(Card card)
  {
    foreach (CardView cardView in cardViews)
    {
      cardView.GetComponent<RectTransform>().anchoredPosition += new Vector2(-handInterval / 2f, 0);
    }
    CardView added = Instantiate(cardViewPrefab, cardViewParent);
    added.Initialize(card, battleSceneManager);
    cardViews.Add(added);
    added.GetComponent<RectTransform>().anchoredPosition = new Vector2
    (
      -1 * handInterval * (cardViews.Count - 1) / 2f + handInterval * (cardViews.Count - 1),
      handPosY
    );
  }

  public void RemoveCard(Card card)
  {
    bool isLaterCard = false;
    for (int i = 0; i < cardViews.Count; ++i)
    {
      if (cardViews[i].ViewCard == card)
      {
        isLaterCard = true;
      }
      else if (isLaterCard)
      {
        cardViews[i].GetComponent<RectTransform>().anchoredPosition -= new Vector2(handInterval / 2, 0);
      }
      else
      {
        cardViews[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(handInterval / 2, 0);
      }
    }
  }
}
