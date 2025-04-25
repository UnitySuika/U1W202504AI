using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSceneManager : MonoBehaviour
{
  [SerializeField]
  private StageSource[] stageSources;

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
  private List<CardSource> selectiveCardSources;
  
  [SerializeField]
  private List<CardSource> initialCardSources;

  [SerializeField]
  private PlayArea playArea;
  
  [SerializeField]
  private PlayArea destroyArea;
  
  [SerializeField]
  private CharacterView characterView;

  [SerializeField]
  private EnergyView energyView;

  [SerializeField]
  private GameObject winView;
  
  [SerializeField]
  private GameObject loseView;
  
  [SerializeField]
  private GameObject cardLostView;

  [SerializeField]
  private TurnView turnView;

  [SerializeField]
  private Image floorView;

  [SerializeField]
  private TextMeshProUGUI floorText;

  [SerializeField]
  private TextMeshProUGUI turnText;

  public PlayArea BattlePlayArea => playArea;
  public PlayArea DestroyArea => destroyArea;

  public List<EnemyView> EnemyViews { get; private set; }

  public Battle CurrentBattle { get; private set; }

  private Card playedCard = null;
  private Card.PlayedData playedData = null;

  [SerializeField]
  private CardView cardViewPrefab;
  
  [SerializeField]
  private RectTransform specialCardViewParent;
  
  [SerializeField]
  private RectTransform cardViewParentMoving;

  [SerializeField]
  private int ClearFloor;

  public CardView SpecialCardView { get; private set; }

  private void BattleInitialize()
  {
    Deck deck;
    if (BattleInformation.FloorNumber == 1)
    {
      deck = new Deck();

      for (int i = 0; i < initialCardSources.Count; ++i)
      {
        deck.Add(new Card(initialCardSources[i]));
      }

      BattleInformation.CardSources = selectiveCardSources;
      BattleInformation.Deck = deck;
    }
    else
    {
      selectiveCardSources = BattleInformation.CardSources;
      deck = BattleInformation.Deck;

      if (selectiveCardSources.Count > 0)
      {
        int random = UnityEngine.Random.Range(0, selectiveCardSources.Count);
        CardSource source = selectiveCardSources[random];
        selectiveCardSources.RemoveAt(random);
        deck.Add(new Card(source));
      }
    }

    int hp;
    if (BattleInformation.FloorNumber == 1)
    {
      hp = 15;
    }
    else 
    {
      hp = BattleInformation.CharacterHp;
    }
    Character character = new Character("護衛対象", 15);
    character.SetHp(hp);
    
    int f = BattleInformation.FloorNumber - 1;
    CurrentBattle = new Battle(character, stageSources[f].SelectiveEnemies, stageSources[f].EnemyNumberMin, stageSources[f].EnemyNumberMax, deck);

    characterView.Set(character);
    
    EnemyViews = new List<EnemyView>();
    int en = CurrentBattle.Enemies.Count;
    for (int i = 0; i < en; ++i)
    {
      EnemyView enemyView = Instantiate(enemyViewPrefab, enemyViewsParent);
      enemyView.Set(CurrentBattle.Enemies[i]);
      enemyView.GetComponent<RectTransform>().anchoredPosition = new Vector2
      (
        -1 * enemyViewsInterval * (en - 1) / 2f + enemyViewsInterval * i,
        enemyViewsPosY
      );
      EnemyViews.Add(enemyView);
    }

    turnView.Initialize(CurrentBattle);
  }

  private async UniTask PlayerTurnInitialize(CancellationToken token)
  {
    CurrentBattle.SetEnergy(3);
    energyView.Set(CurrentBattle.Energy);
    CurrentBattle.MainCharacter.AdvanceTurn();
    await characterView.UpdateStatusEffects(token);
    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    handView.Initialize(this);
    for (int deal_i = 0; deal_i < 4; ++deal_i)
    {
      Card card = CurrentBattle.DealCard();
      if (card == null) break;
      AudioManager.Instance.PlaySe("card_deal", false);
      await handView.AddCard(card, token);
      token.ThrowIfCancellationRequested();
    }

    foreach (Card target in CurrentBattle.Hand)
    {
      target.AddLove();
    }

    foreach (CardView cardView in handView.CardViews)
    {
      cardView.SetLove(cardView.ViewCard.LoveNumber);
    }

    CurrentBattle.SetNextEnemyActions();

    foreach (EnemyView enemyView in EnemyViews)
    {
      enemyView.SetActions();
    }

    // 操作可能にする
    foreach (CardView cardView in handView.CardViews)
    {
      cardView.Validate();
    }
    turnView.Set(CurrentBattle);
  }

  public async UniTask PlayBattleEventQueue(BattleEventQueue beq, CancellationToken token)
  {
    while (beq.Events.Count > 0)
    {
      BattleEvent be = beq.Dequeue();
      switch (be.Type)
      {
        case BattleEvent.EventTypes.CardPlayEffect:
          break;
        case BattleEvent.EventTypes.EnemyPlayAction:
          EnemyView enemyView1 = null;
          foreach (EnemyView view in EnemyViews)
          {
            if (view.TargetEnemy == be.TargetEnemies[0]) enemyView1 = view;
          }
          await enemyView1.StartAction(be.TargetAction, token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.CharacterDamage:
          await characterView.ReceiveDamage(token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.CharacterHeal:
          await characterView.Heal(token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.EnemyDamage:
          EnemyView enemyView3 = null;
          foreach (EnemyView view in EnemyViews)
          {
            if (view.TargetEnemy == be.TargetEnemies[0]) enemyView3 = view;
          }
          bool isDie = await enemyView3.ReceiveDamage(token);
          if (isDie) 
          {
            EnemyViews.Remove(enemyView3);
            Destroy(enemyView3.gameObject);
          }
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.EnemyHeal:
          EnemyView enemyView4 = null;
          foreach (EnemyView view in EnemyViews)
          {
            if (view.TargetEnemy == be.TargetEnemies[0]) enemyView4 = view;
          }
          await enemyView4.Heal(token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.CharacterGetStatusEffect:
          await characterView.GetStatusEffect(be.TargetCharacterStatusEffects[0], token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.EnemyGetStatusEffect:
          EnemyView enemyView5 = null;
          foreach (EnemyView view in EnemyViews)
          {
            if (view.TargetEnemy == be.TargetEnemies[0]) enemyView5 = view;
          }
          await enemyView5.GetStatusEffect(be.TargetEnemyStatusEffects[0], token);
          token.ThrowIfCancellationRequested();
          break;
        case BattleEvent.EventTypes.DecreaseEnergy:
          energyView.Set(CurrentBattle.Energy);
          break;
        case BattleEvent.EventTypes.IncreaseEnergy:
          energyView.Set(CurrentBattle.Energy);
          break;
        case BattleEvent.EventTypes.EndCardPlayEffect:
          break;
        case BattleEvent.EventTypes.EndEnemyPlayAction:
          EnemyView enemyView2 = null;
          foreach (EnemyView view in EnemyViews)
          {
            if (view.TargetEnemy == be.TargetEnemies[0]) enemyView2 = view;
          }
          await enemyView2.EndAction(token);
          token.ThrowIfCancellationRequested();
          break;
      }
    }
  }

  private async UniTask PlayFloorView(CancellationToken token)
  {
    AudioManager.Instance.PlaySe("battle_start", false);
    floorView.gameObject.SetActive(true);
    floorText.text = $"ステージ {BattleInformation.FloorNumber}";
    turnText.text = $"{BattleInformation.TurnSum} ターン経過";
    await UniTask.Delay(3000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    floorView.GetComponent<CanvasGroup>().DOFade(0f, 1f)
      .OnComplete(() => floorView.gameObject.SetActive(false))
      .ToUniTask(cancellationToken: token).Forget();
  }

  private async UniTask BattleMain(CancellationToken token)
  {
    await PlayFloorView(this.GetCancellationTokenOnDestroy());
    BattleInitialize();
    while (true)
    {
      await PlayerTurnInitialize(token);
      token.ThrowIfCancellationRequested();

      playedCard = null;

      while (true)
      {
        if (playedCard == null)
        {
          if (isAllDestroy)
          {
            foreach (Enemy enemy in CurrentBattle.Enemies)
            {
              enemy.ReceiveDamage(1000, new BattleEventQueue());
            }
            CurrentBattle.UpdateStatus();
            CheckGameState();
            return;
          }
          if (CurrentBattle.Turn == Battle.Turns.Enemy) break;
          await UniTask.Yield(PlayerLoopTiming.Update, token);
          token.ThrowIfCancellationRequested();
          continue;
        }
        
        // 操作不能にする
        foreach (CardView cardView in handView.CardViews)
        {
          cardView.Invalidate();
        }
        
        turnView.Invalidate();

        if (playedData.PlayedType == Card.PlayedData.PlayedTypes.Special)
        {
          CurrentBattle.CardToSpecial(playedCard);
          await handView.RemoveCard(playedCard, true, this.GetCancellationTokenOnDestroy());
          token.ThrowIfCancellationRequested();
          
          playedCard.Enhance(3);

          CardView cardView = Instantiate(cardViewPrefab, specialCardViewParent);
          cardView.Initialize(playedCard, this, cardViewParentMoving, true);
          cardView.Validate();
          cardView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
          playedCard = null;
          SpecialCardView = cardView;
          continue;
        }
        
        BattleEventQueue beq = new BattleEventQueue();
        playedCard.PlayEffect(CurrentBattle, playedData, beq);

        if (SpecialCardView != null)
        {
          CurrentBattle.DestroySpecialCard();
          await SpecialCardView.Erase(true, token);
          token.ThrowIfCancellationRequested();

          await PlayBattleEventQueue(beq, token);
          token.ThrowIfCancellationRequested();

          SpecialCardView = null;
          
          turnView.Validate();

          CurrentBattle.UpdateStatus();
        }
        else 
        {
          await handView.RemoveCard(playedCard, true, this.GetCancellationTokenOnDestroy());
          token.ThrowIfCancellationRequested();

          await PlayBattleEventQueue(beq, token);
          token.ThrowIfCancellationRequested();

          turnView.Validate();

          CurrentBattle.UpdateStatus();

          CurrentBattle.Hand.Remove(playedCard);
          CurrentBattle.CStack.InsertBottom(playedCard);
        }

        energyView.Set(CurrentBattle.Energy);

        bool isAllRemoved = CurrentBattle.Hand.Count + CurrentBattle.CStack.Cards.Count == 0;
        if ((!isAllRemoved || BattleInformation.FloorNumber == ClearFloor - 1) && CheckGameState()) return;

        if (isAllRemoved)
        {
          cardLostView.SetActive(true);
          turnView.Invalidate();
          CardLost(token).Forget();
          return;
        }

        playedCard = null;
        
        // 操作可能にする
        foreach (CardView cardView in handView.CardViews)
        {
          cardView.Validate();
        }

        await UniTask.Yield(PlayerLoopTiming.Update, token);
        token.ThrowIfCancellationRequested();
      }

      turnView.Set(CurrentBattle);

      int handNumber = CurrentBattle.Hand.Count;

      for (int i = 0; i < handNumber; ++i)
      {
        CurrentBattle.InsertCard(CurrentBattle.Hand[0]);
      }

      foreach (CardView cView in handView.CardViews)
      {
        cView.Invalidate();
      }

      for (int i = 0; i < handNumber; ++i)
      {
        await handView.RemoveCard(handView.CardViews[0].ViewCard, false, token);
        token.ThrowIfCancellationRequested();
      }

      await UniTask.Delay(1000, cancellationToken: token);
      token.ThrowIfCancellationRequested();

      CurrentBattle.EnemyTurnStart();

      foreach (EnemyView enemyView in EnemyViews)
      {
        await enemyView.UpdateStatusEffects(token);
      }
      await UniTask.Delay(250, cancellationToken: token);
      token.ThrowIfCancellationRequested();

      foreach (Enemy enemy in CurrentBattle.Enemies)
      {
        while (enemy.CanPlayAction())
        {
          BattleEventQueue beq = new BattleEventQueue();
          Enemy.EnemyActionData action = enemy.PlayAction(CurrentBattle, beq);
          await PlayBattleEventQueue(beq, token);
          if (CheckGameState()) return;
          token.ThrowIfCancellationRequested();
          await UniTask.Delay(500, cancellationToken: token);
          token.ThrowIfCancellationRequested();
        }
      }

      ++BattleInformation.TurnSum;

      CurrentBattle.SetTurn(Battle.Turns.Player);
    }
  }

  private void Start()
  {
    BattleMain(this.GetCancellationTokenOnDestroy()).Forget();
  }

  /*
  private void UpdateViews()
  {
    CurrentBattle.UpdateStatus();
    List<EnemyView> nextEnemyViews = new List<EnemyView>();
    foreach (EnemyView enemyView in EnemyViews)
    {
      if (enemyView.TargetEnemy.Hp == 0)
      {
        Destroy(enemyView.gameObject);
      }
      else
      {
        enemyView.Set(enemyView.TargetEnemy);
        nextEnemyViews.Add(enemyView);
        enemyView.SetActions();
        enemyView.SetStatusEffects();
      }
    }
    EnemyViews = nextEnemyViews;
    characterView.Set(CurrentBattle.MainCharacter);
  }
  */

  private async UniTask WinTask(CancellationToken token)
  {
    AudioManager.Instance.StopBgm();

    int handNumber = CurrentBattle.Hand.Count;

    for (int i = 0; i < handNumber; ++i)
    {
      CurrentBattle.InsertCard(CurrentBattle.Hand[0]);
    }

    for (int i = 0; i < handNumber; ++i)
    {
      await handView.RemoveCard(handView.CardViews[0].ViewCard, false, token);
      token.ThrowIfCancellationRequested();
    }
    
    CurrentBattle.ReturnStackToDeck();

    ++BattleInformation.FloorNumber;

    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    string nextScene = BattleInformation.FloorNumber == ClearFloor ? "Clear" : "Battle";
    TransitionMotionManager.Instance.PlayTransitionMotion(nextScene, TransitionMotionManager.TransitionMotionTypes.FadeNormal).Forget();
  }

  private async UniTask LoseTask(CancellationToken token)
  {
    AudioManager.Instance.StopBgm();
    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    TransitionMotionManager.Instance.PlayTransitionMotion("Lose", TransitionMotionManager.TransitionMotionTypes.FadeNormal).Forget();
  }

  private async UniTask CardLost(CancellationToken token)
  {
    BattleInformation.CharacterHp = CurrentBattle.MainCharacter.Hp;
    AudioManager.Instance.StopBgm();
    await UniTask.Delay(1000, cancellationToken: token);
    token.ThrowIfCancellationRequested();
    TransitionMotionManager.Instance.PlayTransitionMotion("Lose", TransitionMotionManager.TransitionMotionTypes.FadeNormal).Forget();
  }

  private bool CheckGameState()
  {
    CurrentBattle.EvaluateState();
    if (CurrentBattle.State == Battle.States.Win)
    {
      BattleInformation.CharacterHp = CurrentBattle.MainCharacter.Hp;
      winView.SetActive(true);
      foreach (CardView cardView in handView.CardViews)
      {
        cardView.Invalidate();
      }
      turnView.Set(CurrentBattle);
      WinTask(this.GetCancellationTokenOnDestroy()).Forget();
      return true;
    }
    else if (CurrentBattle.State == Battle.States.Lose)
    {
      BattleInformation.CharacterHp = CurrentBattle.MainCharacter.Hp;
      loseView.SetActive(true);
      foreach (CardView cardView in handView.CardViews)
      {
        cardView.Invalidate();
      }
      turnView.Set(CurrentBattle);
      LoseTask(this.GetCancellationTokenOnDestroy()).Forget();
      return true;
    }
    return false;
  }

  public void OnPlayCard(Card card, Card.PlayedData playedData)
  {
    playedCard = card;
    this.playedData = playedData;
  }

  private bool isAllDestroy = false;

  public void AllDestroy()
  {
    isAllDestroy = true;
  }
}
