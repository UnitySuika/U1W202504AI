using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

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
  
  [SerializeField]
  private RectTransform cardViewParentMoving;

  public List<CardView> CardViews { get; set; }

  private BattleSceneManager battleSceneManager;

  public void Initialize(BattleSceneManager battleSceneManager)
  {
    CardViews = new List<CardView>();
    this.battleSceneManager = battleSceneManager;
  }

  public async UniTask AddCard(Card card, CancellationToken token)
  {
    token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    
    List<UniTask> tasks = new List<UniTask>();

    CardView added = Instantiate(cardViewPrefab, cardViewParent);
    added.Initialize(card, battleSceneManager, cardViewParentMoving);
    CardViews.Add(added);
    added.Invalidate();

    for (int cv_i = 0; cv_i < CardViews.Count; ++cv_i)
    {
      Vector2 pos = new Vector2(
        -1 * handInterval * (CardViews.Count - 1) / 2f + handInterval * cv_i,
        handPosY
      );
      tasks.Add(CardViews[cv_i].SetPosition(pos, token));
    }
    
    tasks.Add(added.MoveAlpha(0f, 1f, 0.25f, token));

    await UniTask.WhenAll(tasks);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask RemoveCard(Card card, bool isPlayed, CancellationToken token)
  {
    token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;

    List<UniTask> tasks = new List<UniTask>();

    int removeAt = -1;

    for (int cv_i = 0; cv_i < CardViews.Count; ++cv_i)
    {
      if (CardViews[cv_i].ViewCard == card)
      {
        removeAt = cv_i;
        tasks.Add(CardViews[removeAt].Erase(isPlayed, token));
      }
    }

    if (removeAt == -1) return;

    CardViews.RemoveAt(removeAt);

    for (int cv_i = 0; cv_i < CardViews.Count; ++cv_i)
    {
      Vector2 pos = new Vector2(
        -1 * handInterval * (CardViews.Count - 1) / 2f + handInterval * cv_i,
        handPosY
      );
      tasks.Add(CardViews[cv_i].SetPosition(pos, token));
    }

    await UniTask.WhenAll(tasks);
    token.ThrowIfCancellationRequested();
  }
}
