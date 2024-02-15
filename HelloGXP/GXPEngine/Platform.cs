using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Platform : Sprite
    {
        int margin = 50;
        public Platform() : base("square.png")
        {
            SetOrigin(width / 2, 0);
            scaleY = 0.5f;
            x = Utils.Random(margin + width, game.width - margin - width);
            y = game.height - 200;
        }
    }
}
