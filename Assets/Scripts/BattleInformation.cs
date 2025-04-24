using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleInformation
{
  public static int FloorNumber = 1;
  public static int TurnSum = 0;
  
  public static Deck Deck;

  public static List<CardSource> CardSources;

  public static void Initialize()
  {
    FloorNumber = 1;
    TurnSum = 0;
  }
}
