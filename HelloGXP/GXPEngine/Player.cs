using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Player : Sprite
    {
        bool ableToJump;

        float speedX = 4;
        float speedY;
        public Player(float tempX, float tempY) : base("circle.png")
        {
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
        }

        public void updatePlayer()
        {
            movementLR(Input.GetKey(Key.D), Input.GetKey(Key.A));
            movementUD(Input.GetKeyDown(Key.W));
            
        }

        void movementLR(bool goRight, bool goLeft)
        {
            if(goRight)
            {
                x += speedX;
                if(x + width / 2 > game.width)
                {
                    x -= speedX;
                }
            }
            if(goLeft)
            {
                x -= speedX;
                if(x - width / 2 < 0)
                {
                    x += speedX;
                }
            }
        }

        void movementUD(bool jump)
        {
            speedY += 0.15f;
            y += speedY;
            
            if(y > game.height)
            {
                y -= speedY;
                ableToJump = true;
            }

            if(jump && ableToJump)
            {
                speedY = -10;
                ableToJump = false;
            }
            
        }

        
    }
}
