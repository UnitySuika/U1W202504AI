using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

  [SerializeField]
  private CanvasGroup canvasGroup;

  public Card ViewCard { get; private set; }

  private BattleSceneManager battleSceneManager;

  private bool isPointerOn = false;

  private RectTransform rectTransform;

  private RectTransform parentRectTransform;

  private Camera mainCamera;

  private Vector2 settedPos;

  private CancellationToken myToken;

  public CancellationToken MyToken
  {
    get
    {
      if (myToken == null)
      {
        myToken = this.GetCancellationTokenOnDestroy();
      }
      return myToken;
    }
  }

  private bool isValid = false;

  private RectTransform parentRectTransformMoving;

  public void Initialize(Card card, BattleSceneManager battleSceneManager, RectTransform parentMoving)
  {
    rectTransform = GetComponent<RectTransform>();
    parentRectTransform = (RectTransform)rectTransform.parent;
    parentRectTransformMoving = parentMoving;
    mainCamera = Camera.main;

    ViewCard = card;
    this.battleSceneManager = battleSceneManager;
    Set(card);
  }

  public void Validate()
  {
    isValid = true;
  }
  public void Invalidate()
  {
    isValid = false;
  }

  public void Set(Card card)
  {
    nameText.text = card.Source.Id;
    effectText.text = card.Source.EffectDescription;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!isValid) return;
    isPointerOn = true;
    frameImage.color = Color.blue;
    barImage.color = Color.blue;
  }
  public void OnPointerExit(PointerEventData eventData)
  {
    if (!isValid) return;
    isPointerOn = false;
    frameImage.color = Color.white;
    barImage.color = Color.white;
  }

  public async UniTask MoveAlpha(float originAlpha, float targetAlpha, float seconds, CancellationToken token)
  {
    canvasGroup.alpha = originAlpha;

    if (token != myToken)
    {
      token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    }

    await canvasGroup.DOFade(targetAlpha, seconds).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask SetPosition(Vector2 pos, CancellationToken token)
  {
    settedPos = pos;

    token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;

    await rectTransform.DOAnchorPos(pos, 0.25f)
      .ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  private void Update()
  {
    if (!isValid) return;

    if (isPointerOn && ViewCard.Energy <= battleSceneManager.CurrentBattle.Energy)
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (ViewCard.CardType == Card.CardTargetTypes.ForEnemy)
        {
          foreach (EnemyView enemyView in battleSceneManager.EnemyViews)
          {
            enemyView.Validate();
          }
        }
        else if (ViewCard.CardType == Card.CardTargetTypes.Normal)
        {
          battleSceneManager.BattlePlayArea.Validate();
        }
        rectTransform.SetParent(parentRectTransformMoving);
      }
      
      if (Input.GetMouseButton(0))
      {
        // 掴み状態
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, mainCamera, out Vector2 mousePos);

        rectTransform.anchoredPosition = mousePos;
      }
      
      if (Input.GetMouseButtonUp(0))
      {
        // 離す
        if (battleSceneManager.BattlePlayArea.IsPointerOn)
        {
          battleSceneManager.OnPlayCard(ViewCard, new Card.PlayedData(Card.PlayedData.PlayedTypes.Normal, null));
        }
        else
        {
          bool isEnemySelected = false;
          foreach (EnemyView enemyView in battleSceneManager.EnemyViews)
          {
            if (enemyView.IsPointerOn)
            {
              isEnemySelected = true;
              battleSceneManager.OnPlayCard(ViewCard, new Card.PlayedData(Card.PlayedData.PlayedTypes.Enemy, enemyView.TargetEnemy));
            }
          }

          if (!isEnemySelected)
          {
            rectTransform.DOAnchorPos(settedPos, 0.25f);
          }
        }
        
        if (ViewCard.CardType == Card.CardTargetTypes.ForEnemy)
        {
          foreach (EnemyView enemyView in battleSceneManager.EnemyViews)
          {
            enemyView.Invalidate();
          }
        }
        else if (ViewCard.CardType == Card.CardTargetTypes.Normal)
        {
          battleSceneManager.BattlePlayArea.Invalidate();
        }

        rectTransform.SetParent(parentRectTransform);
      }
    }
  }

  public async UniTask Erase(CancellationToken token)
  {
    isValid = false;
    if (token != myToken)
    {
      token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    }
    await MoveAlpha(1f, 0f, 0.5f, token);
    token.ThrowIfCancellationRequested();
    Destroy(gameObject);
  }
}
