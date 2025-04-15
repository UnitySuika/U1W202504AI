using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage
{
  public enum StageTypes
  {
    Battle,
    Rest,
    Fork,
  }

  public StageTypes Type;

  public Stage NextStageMain;

  public Stage NextStageSub;

  public Stage(StageTypes type)
  {
    Type = type;
  }
}

public class Map
{
  public Stage CurrentStage;

  public Map()
  {
    CurrentStage = new Stage(Stage.StageTypes.Battle);
    CurrentStage.NextStageMain = new Stage(Stage.StageTypes.Fork);
  }

#if UNITY_EDITOR
  
  public void DebugMapInfo()
  {
    string s = "";

    var queue = new Queue<(int parentId, int depth, Stage stage)>();
    queue.Enqueue((-1, 0, CurrentStage));

    int id = 0;

    while (queue.Count > 0)
    {
      var stageInfo = queue.Dequeue();
      s += $"id: {id}\nparent_id: {stageInfo.parentId}\ndepth: {stageInfo.depth}\ntype: {stageInfo.stage.Type}\n\n";

      if (stageInfo.stage.NextStageMain != null)
      {
        queue.Enqueue((id, stageInfo.depth + 1, stageInfo.stage.NextStageMain));

        if (stageInfo.stage.NextStageSub != null)
        {
          queue.Enqueue((id, stageInfo.depth + 1, stageInfo.stage.NextStageSub));
        }
      }

      ++id;
    }

    Debug.Log(s);
  }

#endif

  public void CreateStagesFromFork(Stage originalFork)
  {
    Stage[,] stages = new Stage[2, 4];
    for (int dir_i = 0; dir_i < 2; ++dir_i)
    {
      for (int stg_i = 0; stg_i < 4; ++stg_i)
      {
        Stage.StageTypes stageType;
        int typeNumber = stg_i == 3 ? 3 : Random.Range(0, 3);
        switch (typeNumber)
        {
          case 0:
          case 1:
            stageType = Stage.StageTypes.Battle;
            break;
          case 2:
            stageType = Stage.StageTypes.Rest;
            break;
          default:
            stageType = Stage.StageTypes.Fork;
            break;
        }
        Stage stage = new Stage(stageType);
        if (stg_i > 0)
        {
          stages[dir_i, stg_i - 1].NextStageMain = stage;
        }
        else
        {
          if (dir_i == 0)
          {
            originalFork.NextStageMain = stage;
          }
          else
          {
            originalFork.NextStageSub = stage;
          }
        }
        stages[dir_i, stg_i] = stage;
      }
    }
  }

  public void Advance(bool isMainDirection = true)
  {
    if (CurrentStage.NextStageMain == null) return;

    Stage nextStage = isMainDirection ? CurrentStage.NextStageMain : CurrentStage.NextStageSub;

    if (nextStage.Type == Stage.StageTypes.Fork)
    {
      CreateStagesFromFork(nextStage);
    }

    CurrentStage = nextStage;
  }
}
