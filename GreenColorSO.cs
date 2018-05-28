using UnityEngine;


namespace YellowGreenMod
{
  internal class GreenColorSO : SimpleColorSO
  {
    public override Color color
    {
      get
      {
        return new Color(0f, 0f, 0f);
      }
    }
  }
}
