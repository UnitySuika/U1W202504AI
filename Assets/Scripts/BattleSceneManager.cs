using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
  [SerializeField]
  private EnemySource[] enemySources;

  [SerializeField]
  private int minEnemyNumber;

  [SerializeField]
  private int maxEnemyNumber;

  [SerializeField]
  private EnemyView enemyViewPrefab;
  
  [SerializeField]
  private float enemyViewsInterval;
  
  [SerializeField]
  private float enemyViewsPosY;

  [SerializeField]
  private Transform enemyViewsParent;

  [SerializeField]
  private HandView handView;
  
  [SerializeField]
  private CardSource[] initialCardSources;

  private Battle battle;

  private void BattleInitialize()
  {
    Deck deck = new Deck();
    foreach (CardSource cardSource in initialCardSources)
    {
      deck.Add(new Card(cardSource));
    }
    battle = new Battle(enemySources, minEnemyNumber, maxEnemyNumber, deck);
    int en = battle.Enemies.Count;
    for (int i = 0; i < en; ++i)
    {
      EnemyView enemyView = Instantiate(enemyViewPrefab, enemyViewsParent);
      enemyView.Initialize(battle.Enemies[i]);
      enemyView.GetComponent<RectTransform>().anchoredPosition = new Vector2
      (
        -1 * enemyViewsInterval * (en - 1) / 2f + enemyViewsInterval * i,
        enemyViewsPosY
      );
    }
  }

  private async UniTask PlayerTurnInitialize(CancellationToken token)
  {
    battle.SetEnergy(2);
    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    handView.Initialize(this);
    for (int deal_i = 0; deal_i < 4; ++deal_i)
    {
      Card card = battle.DealCard();
      if (card == null) break;
      handView.AddCard(card);
      await UniTask.Delay(1000, cancellationToken: token);
      token.ThrowIfCancellationRequested();
    }
  }

  private async UniTask BattleMain(CancellationToken token)
  {
    BattleInitialize();
    await PlayerTurnInitialize(token);
  }

  private void Start()
  {
    BattleMain(this.GetCancellationTokenOnDestroy()).Forget();
  }

  public void OnClickCard()
  {
    Debug.Log("クリックされた");
  }
}
