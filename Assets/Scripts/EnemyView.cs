using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private TextMeshProUGUI nameText;
  
  [SerializeField]
  private HpView hpView;
  
  [SerializeField]
  private EnemyActionView enemyActionViewPrefab;

  [SerializeField]
  private RectTransform enemyActionViewParent;
  
  [SerializeField]
  private EnemyStatusEffectView enemyStatusEffectViewPrefab;

  [SerializeField]
  private RectTransform enemyStatusEffectViewParent;

  [SerializeField]
  private Image image;

  [SerializeField]
  private RectTransform rectTransform;

  [SerializeField]
  private Color notPointerOnColor;

  [SerializeField]
  private Color pointerOnColor;
  
  public Enemy TargetEnemy { get; private set; }

  public bool IsPointerOn { get; private set; } = false;

  public bool IsValid { get; private set; }

  private RectTransform parentRectTransform;

  private Camera mainCamera;

  public Queue<EnemyActionView> enemyActionViews = new Queue<EnemyActionView>();

  public void Set(Enemy enemy)
  {
    TargetEnemy = enemy;
    nameText.text = enemy.Source.Id;
    hpView.SetHp(enemy.Hp, enemy.MaxHp);
  }

  public void SetActions()
  {
    foreach (EnemyActionView actionView in enemyActionViews)
    {
      Destroy(actionView.gameObject);
    }
    enemyActionViews = new Queue<EnemyActionView>();
    
    foreach (Enemy.EnemyActionData enemyAction in TargetEnemy.NextActions)
    {
      EnemyActionView view = Instantiate(enemyActionViewPrefab, enemyActionViewParent);
      view.Set(enemyAction);
      enemyActionViews.Enqueue(view);
    }
  }

  public void SetStatusEffects()
  {
    foreach (Transform t in enemyStatusEffectViewParent)
    {
      Destroy(t.gameObject);
    }
    
    foreach (Enemy.StatusEffect statusEffect in TargetEnemy.StatusEffects)
    {
      EnemyStatusEffectView view = Instantiate(enemyStatusEffectViewPrefab, enemyStatusEffectViewParent);
      view.Set(statusEffect);
    }
  }

  public void Validate()
  {
    IsValid = true;
    image.DOFade(0.25f, 0.25f);
  }
  public void Invalidate()
  {
    IsValid = false;
    IsPointerOn = false;
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

  public async UniTask PlayAction(CharacterView characterView, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    EnemyActionView actionView = enemyActionViews.Dequeue();

    await actionView.Play(token);
    Destroy(actionView.gameObject);
  
    switch (actionView.ActionData.ActionType)
    {
      case Enemy.EnemyActionTypes.Attack:
        await rectTransform.DOAnchorPosY(-100f, 0.5f).SetRelative().SetEase(Ease.OutSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();
        
        await characterView.ReceiveDamage(token);
        token.ThrowIfCancellationRequested();

        await rectTransform.DOAnchorPosY(100f, 0.25f).SetRelative().SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();

        break;
      case Enemy.EnemyActionTypes.Defend:
        await rectTransform.DOAnchorPosY(50f, 0.5f).SetRelative().SetEase(Ease.OutSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();

        SetStatusEffects();

        await rectTransform.DOAnchorPosY(-50f, 0.25f).SetRelative().SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();

        break;
      case Enemy.EnemyActionTypes.Heal:
        await rectTransform.DOAnchorPosY(50f, 0.5f).SetRelative().SetEase(Ease.OutSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();

        await rectTransform.DOAnchorPosY(-50f, 0.25f).SetRelative().SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
        token.ThrowIfCancellationRequested();

        break;
    }
  }
}
