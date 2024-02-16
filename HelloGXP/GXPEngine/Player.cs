namespace GXPEngine
{
    class Player : Sprite
    {
        //Variables
        bool ableToJump;

        int margin = 140; //Player movement additional restriction

        float speedX = 4;
        float speedY;
        float oldY;         //Last frame player Y position

        //Player constructor (coordinate X, coordinate Y) : (sprite)     will need to change to animated sprite 
        public Player(float tempX, float tempY) : base("circle.png")
        {
            //Setting player origin at the middle of bottom side
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
        }

        //Updating player movement. Takes in 3 inputs (for now) for each right, left and up buttons.
        public void updatePlayer(bool right, bool left, bool up)
        {
            movementLR(right, left);
            movementUD(up);

        }

        //Player movement Left Right
        void movementLR(bool goRight, bool goLeft)
        {
            //If right button pressed 
            if (goRight)
            {
                x += speedX;
                if (x + width / 2 > game.width - margin)
                {
                    x -= speedX;
                }
            }

            //If left button pressed
            if (goLeft)
            {
                x -= speedX;
                if (x - width / 2 < margin)
                {
                    x += speedX;
                }
            }
        }

        //Player movement Up Down
        void movementUD(bool jump)
        {
            //Saves last frame's Y coordinate of the player
            if (oldY != y)
            {
                oldY = y;
            }

            //Always tries to move down
            speedY += 0.2f;
            y += speedY;

            //If below floor go back, reset falling speed and enable jump again 
            if (y > game.height)
            {
                y -= speedY;
                speedY = 0;
                ableToJump = true;
            }
            //Else (if not on the floor) disable jumping
            else
            {
                ableToJump = false;
            }

            //If able to jump and jump button is pressed, jump
            if (jump && ableToJump)
            {
                speedY = -12;
            }
        }

        public void onPlatform(Platform plat)
        {
            if ((oldY < plat.y && y > plat.y) && speedY > 0 && (x > plat.x + plat.width/2 && x < plat.x - plat.width/2))
            {
                y -= speedY;
                speedY = 0;
            }
        }
    }
}
