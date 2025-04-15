using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CardSource")]
public class CardSource : ScriptableObject
{
  public string Id;

  public int Energy;

  public int[] Parameters;

  public string Effect;
}
