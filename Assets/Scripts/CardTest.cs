using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTest : MonoBehaviour
{
  [SerializeField] private CardSource cardSource;
  private void Start()
  {
    Card card = new Card(cardSource);

    // 試しに発動
  }
}
