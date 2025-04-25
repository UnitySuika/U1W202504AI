using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class TransitionMotionManager : MonoBehaviour
{
  public enum TransitionMotionTypes
  {
    FadeNormal,
    Horizontal,
  }

  [Serializable]
  public class TransitionMotionObjectItem
  {
    public TransitionMotionTypes Type;
    public TransitionMotionObject TObjectPrefab;
  }

  [SerializeField]
  private TransitionMotionObjectItem[] tObjectItems;

  public static TransitionMotionManager Instance;

  public bool IsTransitioning { get; private set; } = false;

  private void Awake()
  {
    if (Instance)
    {
      Destroy(gameObject);
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  public async UniTask PlayTransitionMotion(string nextSceneName, TransitionMotionTypes type)
  {
    if (IsTransitioning) return;
    IsTransitioning = true;
    CancellationToken token = this.GetCancellationTokenOnDestroy();

    TransitionMotionObject prefab = Array.Find(tObjectItems, item => item.Type == type).TObjectPrefab;
    TransitionMotionObject tObject = Instantiate(prefab, transform);

    await tObject.PlayTransitionMotion(nextSceneName, token);
    token.ThrowIfCancellationRequested();

    Destroy(tObject.gameObject);

    IsTransitioning = false;
  }
}
