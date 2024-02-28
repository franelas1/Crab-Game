using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Button : Sprite
    {
        public Button(float posX, float posY, string filename) : base(filename)
        {
            x = posX;
            y = posY;
        }

        public bool checkActivate()
        {
            return CustomUtil.CheckPointWithRect(new Vector2(x, y), width, height, new Vector2(Input.mouseX, Input.mouseY));
        }

    }
}
