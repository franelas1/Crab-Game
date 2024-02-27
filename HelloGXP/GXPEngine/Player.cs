using GXPEngine.Core;
using System;
using System.IO;

namespace GXPEngine
{
    public class Player : Sprite
    {
        int playerID;                   //determine if player is player 1 or player 2
        bool standsOnPlatform = false;  //check if player stands on the platform (collision logic)
        bool shouldBeFalling = false;   //checks if player falls off the platform (collision logic)

        Platform oldFlatform;

        bool ableToJump; //player can jump if true

        float speedX = 4;
        float speedY;
        float oldY;         //Last frame player Y position
        int oldPower = 0;       //just a temp variable


        int margin;
        int divider = 10;  //Input valus from the arduino range from -100 to 100 so we want to divie that to the amount of pixels 
                            // we want the player to move the fastest (used in movementLR for arduino)

        //tempX: X position of player at spawn
        //tempY: Y position of player at spawn
        //TODO: change to animated sprite 
        public Player(int playerID, float tempX, float tempY, int margin) : base("circle.png")
        {
            //Setting player origin at the middle of bottom side
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
            this.playerID = playerID;
            this.margin = margin;
        }

        //Updating player movement. Takes in 3 inputs (for now) for each right, left and up buttons.
        //public void updatePlayer(bool right, bool left, bool up)
        public void updatePlayer(int LR, int up, int power)
        {
            //movementLR(right, left);
            movementLR(LR);
            movementUD(up);
            
            if(oldPower != power)
            {
                if (power == 1) SetColor(Utils.Random(0f, 1f), Utils.Random(0f, 1f), Utils.Random(0f, 1f));
                oldPower = power;
            }
            
            //platform collision logic
            y += 20; //need to move player y temporaly for the collision logic to work
            CheckCollisionWithPlatform();
            y -= 20;
        }

        //Player movement Left Right
        /*void movementLR(bool goRight, bool goLeft)
        {
            //If right button pressed 
            if (goRight)
            {
                Gamedata.playerMoved = true;
                x += speedX;
                if (x + width / 2 > game.width - margin)
                {
                    x -= speedX;
                }
            }

            //If left button pressed
            if (goLeft)
            {
                Gamedata.playerMoved = true;
                x -= speedX;
                if (x - width / 2 < margin)
                {
                    x += speedX;
                }
            }
        }*/

        void movementLR(int moveAmount)
        {
            x += (moveAmount) / divider;
        }

        //Player movement Up Down
        void movementUD(int jump)
        {
            //Saves last frame's Y coordinate of the player
            if (oldY != y)
            {
                oldY = y;
            }

            //Always tries to move down
            speedY += 0.4f;
            y += speedY;

            if (y > game.height + height / 2)
            {
                Destroy();
                return;
            }

            //If below floor go back, reset falling speed and enable jump again 
            if (standsOnPlatform && !shouldBeFalling)
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
            if (jump == 1 && ableToJump)
            {
                Gamedata.playerMoved = true;
                speedY = -12;
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
                        if (playerID == 1)
                        {
                            Gamedata.detectPlatformPlayer1 = false;
                            //update the current platform player collides with
                            Gamedata.CheckPlat(1);

                            if (Gamedata.detectPlatformPlayer1)
                            {
                                checkStandOnPlatform = true; //this means player is still standing on the platform
                            }
                        }

                        else
                        {
                            Gamedata.detectPlatformPlayer2 = false;
                            y -= height / 2;
                            Gamedata.CheckPlat(2);
                            y += height / 2;

                            if (Gamedata.detectPlatformPlayer2)
                            {
                                checkStandOnPlatform = true; //this means player is still standing on the platform
                            }
                        }

                    }
                }

                if (playerID == 1)
                {
                    if (checkStandOnPlatform == false || oldFlatform != Gamedata.currentPlayer1Platform) //if this condition is true, this means player falls off the platform
                    {
                        standsOnPlatform = false;
                        shouldBeFalling = false;
                    }
                }

                if (playerID == 2)
                {
                    if (checkStandOnPlatform == false || oldFlatform != Gamedata.currentPlayer2Platform) //if this condition is true, this means player falls off the platform
                    {
                        standsOnPlatform = false;
                        shouldBeFalling = false;
                    }
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
                            if (speedY > 0) //jump falling
                            {
                                Gamedata.detectPlatformPlayer1 = false;
                                y += height / 2;
                                Gamedata.CheckPlat(1);
                                y -= height / 2;
                            }

                            //(this part needed?)
                            else
                            {
                                Gamedata.detectPlatformPlayer1 = false;
                                y -= height / 2;
                                Gamedata.CheckPlat(1);
                                y += height / 2;


                            }

                            if (Gamedata.currentPlayer1Platform != null)
                            {
                                
                                //debug messages
                                /*
                                Console.WriteLine("---------------");
                                Console.WriteLine(speedY > 0);
                                Console.WriteLine(Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)));
                                */

                                //if detection fails, try again (this might not be needed)
                                if (Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)) < 10 == false)
                                {
                                    y -= 20;
                                    Gamedata.CheckPlat(1);
                                    y += 20;
                                }


                                //the conditions below determine if player successfully stands on a platform
                                //first determine if player is falling, and player at specific height relative
                                //to the detected platform
                                if (speedY > 0
                                    && Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)) < 10)
                                {
                                    y += Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                        - (y - height / 2)); //ajust y for more accurate landing

                                    standsOnPlatform = true;
                                    shouldBeFalling = false;
                                    oldFlatform = Gamedata.currentPlayer1Platform;
                                }
                            }
                        }

                        if (playerID == 2) //if player is player 1
                        {
                            if (speedY > 0) //jump falling
                            {
                                Gamedata.detectPlatformPlayer2 = false;
                                y += height / 2;
                                Gamedata.CheckPlat(2);
                                y -= height / 2;
                            }

                            //(this part needed?)
                            else
                            {
                                Gamedata.detectPlatformPlayer2 = false;
                                y -= height / 2;
                                Gamedata.CheckPlat(2);
                                y += height / 2;


                            }

                            if (Gamedata.currentPlayer2Platform != null)
                            {
                                //if detection fails, try again (this might not be needed)
                                if (Math.Abs(Gamedata.currentPlayer2Platform.y - (Gamedata.currentPlayer2Platform.height / 2)
                                    - (y - height / 2)) < 10 == false)
                                {
                                    y -= 20;
                                    Gamedata.CheckPlat(2);
                                    y += 20;
                                }


                                //the conditions below determine if player successfully stands on a platform
                                //first determine if player is falling, and player at specific height relative
                                //to the detected platform
                                if (speedY > 0
                                    && Math.Abs(Gamedata.currentPlayer2Platform.y - (Gamedata.currentPlayer2Platform.height / 2)
                                    - (y - height / 2)) < 10)
                                {
                                    y += Math.Abs(Gamedata.currentPlayer2Platform.y - (Gamedata.currentPlayer2Platform.height / 2)
                                        - (y - height / 2)); //ajust y for more accurate landing

                                    standsOnPlatform = true;
                                    shouldBeFalling = false;
                                    oldFlatform = Gamedata.currentPlayer2Platform;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
