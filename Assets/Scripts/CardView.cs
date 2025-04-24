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
  private Image baseImage;

  [SerializeField]
  private Button cardButton;

  [SerializeField]
  private CanvasGroup canvasGroup;
  
  [SerializeField]
  private TextMeshProUGUI energyCostText;

  /*
  [SerializeField]
  private TextMeshProUGUI loveCostText;
  */
  
  [SerializeField]
  private TextMeshProUGUI loveText;

  [SerializeField]
  private Color specialColor;

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

  private bool isTukami = false;

  public void Initialize(Card card, BattleSceneManager battleSceneManager, RectTransform parentMoving, bool isSpecial = false)
  {
    rectTransform = GetComponent<RectTransform>();
    parentRectTransform = (RectTransform)rectTransform.parent;
    parentRectTransformMoving = parentMoving;
    mainCamera = Camera.main;

    if (isSpecial)
    {
      baseImage.color = specialColor;
      loveText.transform.parent.gameObject.SetActive(false);
    }

    ViewCard = card;
    this.battleSceneManager = battleSceneManager;
    Set(card);
  }

  public void Validate()
  {
    isValid = true;
    frameImage.color = new Color(1f, 1f, 1f);
    barImage.color = new Color(1f, 1f, 1f);
  }
  public void Invalidate()
  {
    isValid = false;
    frameImage.color = new Color(0.5f, 0.5f, 0.5f);
    barImage.color = new Color(0.5f, 0.5f, 0.5f);
  }

  public void Set(Card card)
  {
    nameText.text = card.Source.Id;
    effectText.text = card.EffectDescription;
    energyCostText.text = card.Energy.ToString();
    //loveCostText.text = card.LoveCost.ToString();
  }

  public void SetLove(int loveNumber)
  {
    loveText.text = loveNumber.ToString();
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

    if (isTukami)
    {
      if (Input.GetMouseButton(0))
      {
        // 掴み状態
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransformMoving, Input.mousePosition, mainCamera, out Vector2 mousePos);

        rectTransform.anchoredPosition = mousePos;
      }
      
      if (Input.GetMouseButtonUp(0))
      {
        // 離す
        if (battleSceneManager.BattlePlayArea.IsPointerOn)
        {
          battleSceneManager.OnPlayCard(ViewCard, new Card.PlayedData(Card.PlayedData.PlayedTypes.Normal, null));
        }
        else if (battleSceneManager.DestroyArea.IsPointerOn)
        {
          battleSceneManager.OnPlayCard(ViewCard, new Card.PlayedData(Card.PlayedData.PlayedTypes.Special, null));
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
        
        battleSceneManager.DestroyArea.Invalidate();

        rectTransform.SetParent(parentRectTransform);

        isTukami = false;
      }
    }

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

        if (battleSceneManager.SpecialCardView == null && ViewCard.LoveNumber >= 3)
        {
          battleSceneManager.DestroyArea.Validate();
        }

        rectTransform.SetParent(parentRectTransformMoving);
        AudioManager.Instance.PlaySe("card_get", false);

        isTukami = true;
      }
    }
  }

  public async UniTask Erase(bool isPlayed, CancellationToken token)
  {
    isValid = false;
    if (token != myToken)
    {
      token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    }
    List<UniTask> eraseTasks = new List<UniTask>();
    float fadeTime = isPlayed ? 1f : 0.25f;
    eraseTasks.Add(MoveAlpha(1f, 0f, fadeTime, token));
    if (isPlayed)
    {
      AudioManager.Instance.PlaySe("card_play", false);
      eraseTasks.Add(transform.DOScale(2f, fadeTime).SetRelative().ToUniTask(cancellationToken: token));
    }
    await UniTask.WhenAll(eraseTasks);
    token.ThrowIfCancellationRequested();
    Destroy(gameObject);
  }
}
