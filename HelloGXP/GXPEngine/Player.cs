using GXPEngine.Core;
using System;
using System.IO;

namespace GXPEngine
{
    public class Player : Sprite
    {
        int playerID; //determine if player is player 1 or player 2
        bool standsOnPlatform = false; //check if player stands on the platform (collision logic)
        bool shouldBeFalling = false; //checks if player falls off the platform (collision logic)

        bool ableToJump; //player can jump if true

        float speedX = 4;
        float speedY;
        float oldY;         //Last frame player Y position

        //tempX: X position of player at spawn
        //tempY: Y position of player at spawn
        //TODO: change to animated sprite 
        public Player(int playerID, float tempX, float tempY) : base("circle.png")
        {
            //Setting player origin at the middle of bottom side
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
            this.playerID = playerID;
        }

        //Updating player movement. Takes in 3 inputs (for now) for each right, left and up buttons.
        public void updatePlayer(bool right, bool left, bool up)
        {
            movementLR(right, left);
            movementUD(up);
            
            //platform collision logic
            y += 20; //need to move player y temporaly for the collision logic to work
            CheckCollisionWithPlatform();
            y -= 20;
        }

        //Player movement Left Right
        void movementLR(bool goRight, bool goLeft)
        {
            //If right button pressed 
            if (goRight)
            {
                x += speedX;
                if (x + width / 2 > game.width)
                {
                    x -= speedX;
                }
            }

            //If left button pressed
            if (goLeft)
            {
                x -= speedX;
                if (x - width / 2 < 0)
                {
                    x += speedX;
                }
            }
        }

        //Player movement Up Down
        void movementUD(bool jump)
        {
            //Saves last frame's Y coordinate of the player
            oldY = y;

            //Always tries to move down
            speedY += 0.1f;
            y += speedY;

            //If below floor go back, reset falling speed and enable jump again 
            if ((y > game.height || standsOnPlatform) && !shouldBeFalling)
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
                speedY = -8;
                standsOnPlatform = false;
                Gamedata.currentPlayer1Platform = null;

            }
        }

        //platform collision logic
        void CheckCollisionWithPlatform()
        {

            GameObject[] collisions = GetCollisions();  //get all thing the player is colliding with

            if (standsOnPlatform) //if player is standing on a platform
            {
                
                bool checkStandOnPlatform = false; //for helping determine if player falls off the platform

                foreach (GameObject theCollision in collisions) //determine the collision
                {
                
                    if (theCollision is Platform) //if collide with platform
                    {
                        checkStandOnPlatform = true; //this means player is still standing on the platform
                    }
                }

                if (checkStandOnPlatform == false) //if this condition is true, this means player falls off the platform
                {
                    standsOnPlatform = false;
                    shouldBeFalling = false;
                }
            }

            if (!standsOnPlatform) //if player isn't standing on a platform or is floating/jumping
            {
                foreach (GameObject theCollision in collisions)
                {
                    if (theCollision is Platform) 
                    {
                        if (playerID == 1) //if player is player 1
                        {
                            Gamedata.detectPlatformPlayer1 = false;
                            y += 40;
                            Gamedata.CheckPlat(1);
                            y -= 40;
                            
                            if (Gamedata.currentPlayer1Platform != null)
                            {

                                //the condition below check the condition that determine player successfully stands on a platform
                                //first determine if platform found, if player is falling, and player at specific height relative
                                //to the determined platform
                                if (!Gamedata.detectPlatformPlayer1 && speedY > 0
                                    && Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)) < 3)
                                {
                                    y += 2; //ajust y for more accurate landing
                                    standsOnPlatform = true;
                                    shouldBeFalling = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
