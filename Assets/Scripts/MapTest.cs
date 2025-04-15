using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
  private void Start()
  {
    #if UNITY_EDITOR

    Map map = new Map();
    map.DebugMapInfo();
    map.Advance();
    map.DebugMapInfo();
    for (int i = 0; i < 10; ++i)
    {
      map.Advance();
    }
    map.DebugMapInfo();

    #endif
  }
}
