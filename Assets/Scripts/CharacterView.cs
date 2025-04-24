using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI nameText;
  
  [SerializeField]
  private HpView hpView;
  
  [SerializeField]
  private CharacterStatusEffectView characterStatusEffectViewPrefab;
  
  [SerializeField]
  private RectTransform characterStatusEffectViewParent;

  public Queue<CharacterStatusEffectView> StatusEffectViews = new Queue<CharacterStatusEffectView>();

  private Character chara;

  public void Set(Character character)
  {
    chara = character;
    nameText.text = character.Name;
    hpView.SetHp(character.Hp, character.MaxHp);
  }

  public async UniTask ReceiveDamage(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    Set(chara);
    
    AudioManager.Instance.PlaySe("damaged", false);

    await GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 10)
      .SetEase(Ease.OutSine)
      .ToUniTask(cancellationToken: token);

    token.ThrowIfCancellationRequested();
  }

  public async UniTask Heal(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
    
    AudioManager.Instance.PlaySe("heal", false);

    await transform.DOLocalRotate(new Vector3(0, 0, 360f), 0.5f)
      .SetEase(Ease.OutSine)
      .SetRelative()
      .ToUniTask(cancellationToken: token);

    token.ThrowIfCancellationRequested();

    transform.localEulerAngles = new Vector3(0, 0, 360f);
    
    Set(chara);
  }

  public async UniTask GetStatusEffect(Character.StatusEffect statusEffect, CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    AudioManager.Instance.PlaySe("add_status_effect", false);

    CharacterStatusEffectView view = Instantiate(characterStatusEffectViewPrefab, characterStatusEffectViewParent);
    StatusEffectViews.Enqueue(view);
    await view.Set(statusEffect, token);
    token.ThrowIfCancellationRequested();
  }

  public async UniTask UpdateStatusEffects(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
    foreach (CharacterStatusEffectView statusView in StatusEffectViews)
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
    Queue<CharacterStatusEffectView> next = new Queue<CharacterStatusEffectView>();
    foreach (CharacterStatusEffectView statusView in StatusEffectViews)
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
