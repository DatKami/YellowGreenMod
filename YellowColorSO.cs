﻿using UnityEngine;


namespace YellowGreenMod
{
  internal class YellowColorSO : SimpleColorSO
  {
    public override Color color
    {
      get
      {
        return new Color(1f, .8f, 0f);
      }
    }
  }
}
