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

    await GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 10)
      .SetEase(Ease.OutSine)
      .ToUniTask(cancellationToken: token);

    token.ThrowIfCancellationRequested();
  }
}
