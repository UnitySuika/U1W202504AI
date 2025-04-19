using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

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

  [SerializeField]
  private PlayArea playArea;
  
  [SerializeField]
  private CharacterView characterView;

  [SerializeField]
  private EnergyView energyView;

  public PlayArea BattlePlayArea => playArea;

  public List<EnemyView> EnemyViews { get; private set; }

  private Battle battle;

  private void BattleInitialize()
  {
    Deck deck = new Deck();
    foreach (CardSource cardSource in initialCardSources)
    {
      deck.Add(new Card(cardSource));
    }

    Character character = new Character("冒険者", 50);
    
    battle = new Battle(character, enemySources, minEnemyNumber, maxEnemyNumber, deck);

    characterView.Set(character);
    
    EnemyViews = new List<EnemyView>();
    int en = battle.Enemies.Count;
    for (int i = 0; i < en; ++i)
    {
      EnemyView enemyView = Instantiate(enemyViewPrefab, enemyViewsParent);
      enemyView.Set(battle.Enemies[i]);
      enemyView.GetComponent<RectTransform>().anchoredPosition = new Vector2
      (
        -1 * enemyViewsInterval * (en - 1) / 2f + enemyViewsInterval * i,
        enemyViewsPosY
      );
      EnemyViews.Add(enemyView);
    }
  }

  private async UniTask PlayerTurnInitialize(CancellationToken token)
  {
    battle.SetEnergy(2);
    energyView.Set(battle.Energy);
    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    handView.Initialize(this);
    for (int deal_i = 0; deal_i < 4; ++deal_i)
    {
      Card card = battle.DealCard();
      if (card == null) break;
      await handView.AddCard(card, token);
      token.ThrowIfCancellationRequested();
    }

    battle.SetNextEnemyActions();

    foreach (EnemyView enemyView in EnemyViews)
    {
      enemyView.SetActions();
    }

    // 操作可能にする
    foreach (CardView cardView in handView.CardViews)
    {
      cardView.Validate();
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

  public void OnPlayCard(Card card)
  {
    card.PlayEffect();
    handView.RemoveCard(card, this.GetCancellationTokenOnDestroy()).Forget();
  }
}
