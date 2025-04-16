using System.Collections.Generic;

public class JsonExpression
{
  public string Value;

  public Dictionary<string, JsonExpression> Elements;

  public JsonExpression()
  {
    Value = null;
    Elements = new Dictionary<string, JsonExpression>();
  }

  public JsonExpression(string value)
  {
    Value = value;
  }

  public JsonExpression(Dictionary<string, JsonExpression> elements)
  {
    Value = null;
    Elements = elements;
  }
}
