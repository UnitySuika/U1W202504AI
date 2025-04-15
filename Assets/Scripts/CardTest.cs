using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTest : MonoBehaviour
{

  [SerializeField] private CardSource[] cardSources;

  private JsonExpression[] cardEffects;

  private List<List<string>> DivideToPhrase(List<string> words)
  {

    List<List<string>> phraseList = new List<List<string>>();
    List<string> currentPhrase = new List<string>();
    List<int> wordDepthList = new List<int>();
    
    int depth = 0;
    bool isAddDepth = false;
    for (int i = 0; i < words.Count; ++i)
    {
      if (isAddDepth)
      {
        isAddDepth = false;
        ++depth;
      }

      if (words[i] == "(")
      {
        isAddDepth = true;
      }
      else if (words[i] == ")")
      {
        --depth;
      }

      wordDepthList.Add(depth);
    }

    for (int i = 0; i < words.Count; ++i)
    {
      if (i > 0 && wordDepthList[i] == 0 && wordDepthList[i - 1] == 0) // 次のフレーズを開始
      {
        phraseList.Add(currentPhrase);
        currentPhrase = new List<string>();
      }

      if (wordDepthList[i] == 0 && (words[i] == "(" || words[i] == ")"))
      {
        continue;
      }

      currentPhrase.Add(words[i]);
    }

    phraseList.Add(currentPhrase);

    return phraseList;
  }

  private string JsonExpressionToString(JsonExpression jsonExpression, int tabNumber)
  {
    string tab = new string(' ', tabNumber * 4);
    string s = "";
    if (jsonExpression.Value != null)
    {
      s += tab + "    " + "\"" + jsonExpression.Value + "\"" + "\n";
    }
    else
    {
      s += tab + "{\n";
      foreach (var item in jsonExpression.Elements)
      {
        s += tab + "    " + item.Key + " : \n";
        s += JsonExpressionToString(item.Value, tabNumber + 1);
      }
      s += tab + "}\n";
    }
    return s;
  }

  private JsonExpression CardSourceToObject(CardSource source)
  {
    // Source.Effectの構文解析によりオブジェクトを生成
    string effect = source.Effect;
    JsonExpression root = new JsonExpression();
    
    var next = new Queue<(JsonExpression parent, string title, List<string> words)>();
    next.Enqueue((root, "root", effect.Split(' ').ToList()));

    while (next.Count > 0)
    {
      var current = next.Dequeue();
      JsonExpression je = new JsonExpression();
      current.parent.Elements.Add(current.title, je);

      List<List<string>> phraseList = DivideToPhrase(current.words);
      
      // 型を測定
      if (phraseList.Count > 1)
      {
        if (phraseList[0].Count == 1 && phraseList[0][0] == "if")
        {
          je.Elements.Add("type", new JsonExpression("conditional"));
          next.Enqueue((je, "condition", phraseList[1]));
          next.Enqueue((je, "then", phraseList[3]));
        }
        else if (phraseList[0].Count == 1 && phraseList[0][0] == "attack")
        {
          je.Elements.Add("type", new JsonExpression("attack"));
          next.Enqueue((je, "value", phraseList[1]));
        }
        else if (phraseList[0].Count == 1 && phraseList[0][0] == "card_exist_number")
        {
          je.Elements.Add("type", new JsonExpression("card_exist_number"));
          next.Enqueue((je, "value", phraseList[1]));
        }
        else if (phraseList[1].Count == 1 && phraseList[1][0] == ">")
        {
          je.Elements.Add("type", new JsonExpression("comparison"));
          next.Enqueue((je, "left", phraseList[0]));
          next.Enqueue((je, "right", phraseList[2]));
        }
        else if (phraseList[1].Count == 1 && phraseList[1][0] == "*")
        {
          je.Elements.Add("type", new JsonExpression("multiply"));
          next.Enqueue((je, "left", phraseList[0]));
          next.Enqueue((je, "right", phraseList[2]));
        }
        else
        {
          je.Elements.Add("type", new JsonExpression("variable"));
          je.Elements.Add("value", new JsonExpression(phraseList[0][0]));
        }
      }
      else
      {
        je.Elements.Add("type", new JsonExpression("variable"));
        je.Elements.Add("value", new JsonExpression(phraseList[0][0]));
      }
    }

    Debug.Log(JsonExpressionToString(root, 0));

    return root;
  }

  private void PlayEffect(JsonExpression je)
  {
    
  }

  private void Start()
  {
    cardEffects = new JsonExpression[cardSources.Length];
    for (int i = 0; i < cardSources.Length; ++i)
    {
      cardEffects[i] = CardSourceToObject(cardSources[i]);
    }

    // 試しに発動

    PlayEffect(cardEffects[0]);
  }
}
