using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class TransitionMotionObject : MonoBehaviour
{
  public abstract UniTask PlayTransitionMotion(string nextSceneName, CancellationToken token);
}
