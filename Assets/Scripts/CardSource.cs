using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CardSource")]
public class CardSource : ScriptableObject
{
  public string Id;

  public int Energy;

  public Card.Parameter[] Parameters;

  public string Effect;
}
