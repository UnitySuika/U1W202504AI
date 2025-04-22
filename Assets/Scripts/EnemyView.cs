using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

  public Queue<EnemyStatusEffectView> StatusEffectViews = new Queue<EnemyStatusEffectView>();

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

  /*
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
  */

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

        // SetStatusEffects();

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

  
  public async UniTask StartAction(Enemy.EnemyActionData action, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    AudioManager.Instance.PlaySe("monster", false);

    EnemyActionView actionView = enemyActionViews.Dequeue();

    List<UniTask> tasks = new List<UniTask>
    {
      actionView.Play(token),
      rectTransform.DOAnchorPosY(-100f, 0.5f)
        .SetRelative()
        .SetEase(Ease.OutSine)
        .ToUniTask(cancellationToken: token)
    };

    await UniTask.WhenAll(tasks);
    token.ThrowIfCancellationRequested();

    Destroy(actionView.gameObject);
  }

  public async UniTask EndAction(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    await rectTransform.DOAnchorPosY(100f, 0.25f).SetRelative().SetEase(Ease.InSine).ToUniTask(cancellationToken: token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask<bool> ReceiveDamage(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    Set(TargetEnemy);
    
    AudioManager.Instance.PlaySe("damaged", false);

    await GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 10)
      .SetEase(Ease.OutSine)
      .ToUniTask(cancellationToken: token);
      
    token.ThrowIfCancellationRequested();

    if (TargetEnemy.Hp == 0)
    {
      AudioManager.Instance.PlaySe("die", false);
      return true;
    }
    
    return false;
  }

  public async UniTask Heal(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
    
    AudioManager.Instance.PlaySe("heal", false);

    await transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f)
      .SetEase(Ease.OutSine)
      .SetRelative()
      .ToUniTask(cancellationToken: token);

    token.ThrowIfCancellationRequested();

    transform.localEulerAngles = new Vector3(0, 0, 0);
    
    Set(TargetEnemy);
  }

  public async UniTask GetStatusEffect(Enemy.StatusEffect statusEffect, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    AudioManager.Instance.PlaySe("add_status_effect", false);

    EnemyStatusEffectView view = Instantiate(enemyStatusEffectViewPrefab, enemyStatusEffectViewParent);
    StatusEffectViews.Enqueue(view);
    await view.Set(statusEffect, token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask UpdateStatusEffects(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
    foreach (EnemyStatusEffectView statusView in StatusEffectViews)
    {
      if (statusView.TargetStatusEffect.RemainingTurn <= 0)
      {
        await statusView.Delete(token);
        token.ThrowIfCancellationRequested();
      }
      else
      {
        await statusView.UpdateTurn(token);
        token.ThrowIfCancellationRequested();
      }
    }

    ReCreateStatusEffectViewQueue();
  }

  public void ReCreateStatusEffectViewQueue()
  {
    Queue<EnemyStatusEffectView> next = new Queue<EnemyStatusEffectView>();
    foreach (EnemyStatusEffectView statusView in StatusEffectViews)
    {
      if (statusView.TargetStatusEffect == null)
      {
        Destroy(statusView.gameObject);
      }
      else
      {
        next.Enqueue(statusView);
      }
    }
    StatusEffectViews = next;
  }
}
