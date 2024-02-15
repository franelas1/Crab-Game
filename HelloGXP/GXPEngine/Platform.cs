using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Platform : Sprite
    {
        int margin = 50; // how many pixels away the walls are from the sides of the screen
        public Platform() : base("square.png")
        {
            SetOrigin(width / 2, 0);
            scaleX = Utils.Random(2f, 3f);
            scaleY = 0.5f;
            x = Utils.Random(margin + width, game.width - margin - width);
            y = 200;
        }

        public void updatePlatform()
        {
            fall();
            destroyPlatform();
        }

        void fall()
        {
            y++;
        }

        void destroyPlatform()
        {
            if (y > game.height)
            {
                this.Remove();
                this.Destroy();
            }
        }
    }
}
