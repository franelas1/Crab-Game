using System;

namespace GXPEngine
{
    public class Player : AnimationSprite
    {

        const int DISTTHRESHOLDPLAYERDISTCOMPARE = 99999;

        int playerID;                   //determine if player is player 1 or player 2
        bool standsOnPlatform = false;  //check if player stands on the platform (collision logic)
        bool shouldBeFalling = false;   //checks if player falls off the platform (collision logic)

        Platform oldFlatform;

        bool ableToJump; //player can jump if true

        float speedX = 4;
        float speedY;
        float oldY;         //Last frame player Y position


        int margin;
        public bool hasSomeInput;
        public bool hasPlayerCollision;

        //tempX: X position of player at spawn
        //tempY: Y position of player at spawn

        GameObject[] collisions;
        public Player(int playerID, float tempX, float tempY, int margin, string filename) : base(filename, 1, 1)
        {
            //Setting player origin at the middle of bottom side
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
            this.playerID = playerID;
            this.margin = margin;
        }

        //Updating player movement. Takes in 3 inputs (for now) for each right, left and up buttons.
        public void updatePlayer()
        {
            hasSomeInput = false;
            hasPlayerCollision = false;
            collisions = GetCollisions();
            CheckCollisionWithPlayer();

            if (playerID == 1)
            {
                if (Input.GetKey(Key.D) || Input.GetKey(Key.A))
                {
                    hasSomeInput = true;
                }
                movementLR(Input.GetKey(Key.D), Input.GetKey(Key.A));
                movementUD(Input.GetKey(Key.W));
            }

            if (playerID == 2)
            {
                if (Input.GetKey(Key.RIGHT) || Input.GetKey(Key.LEFT))
                {
                    hasSomeInput = true;
                }
                movementLR(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT));
                movementUD(Input.GetKey(Key.UP));
            }

            //platform collision logic
            y += 15; //need to move player y temporaly for the collision logic to work
            CheckCollisionWithPlatform();
            y -= 15;
        }

        //Player movement Left Right
        void movementLR(bool goRight, bool goLeft)
        {
            if (goRight && goLeft)
            {
                return;
            }

            //If right button pressed 
            if (goRight)
            {
                if (playerID == 1)
                {
                    //hasPlayerCollision == false
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x += speedX;

                    if (x + width / 2 > game.width - margin)
                    {
                        x -= speedX;
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x < Gamedata.player2.x 
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    {
                        Console.WriteLine("player 1 goes right cancel");
                        x -= speedX;
                    }

                    else if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x > Gamedata.player2.x
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    { 
                        Gamedata.player2.x += speedX; 
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x += speedX;


                    if (x + width / 2 > game.width - margin)
                    {

                        x -= speedX;

                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.A)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x -= speedX;
                    }
                }
            }

            //If left button pressed
            if (goLeft)
            {
                if (playerID == 1)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedX;



                    if (x - width / 2 < margin)
                    {

                        x += speedX;
                    }



                    if (hasPlayerCollision == true && Input.GetKey(Key.RIGHT) &&
                        Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 1 goes left cancel");
                        x += speedX;
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedX;

                    if (x - width / 2 < margin)
                    {

                        x += speedX;
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.D)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x += speedX;
                    }
                }
            }

            if (hasSomeInput == false)
            {
                if (playerID == 1)
                {
                    if (hasPlayerCollision)
                    {
                        if ((Input.GetKey(Key.RIGHT) && (Input.GetKey(Key.LEFT))))
                        {
                            return;
                        }

                        if (Input.GetKey(Key.RIGHT) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x += speedX;
                            if (x + width / 2 > game.width - margin)
                            {
                                x -= speedX;
                                x -= speedX;
                            }
                        }

                        if (Input.GetKey(Key.LEFT)
                            && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x -= speedX;
                            if (x - width / 2 < margin)
                            {
                                x += speedX;
                                x += speedX;
                            }
                        }
                    }
                }

                if (playerID == 2)
                {
                    if (hasPlayerCollision)
                    {
                        if ((Input.GetKey(Key.D) && (Input.GetKey(Key.A))))
                        {
                            return;
                        }

                        if (Input.GetKey(Key.D) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x += speedX;
                            if (x + width / 2 > game.width - margin)
                            {
                                x -= speedX;
                                x -= speedX;
                            }
                        }

                        if (Input.GetKey(Key.A) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x -= speedX;
                            if (x - width / 2 < margin)
                            {
                                x += speedX;
                                x += speedX;
                            }
                        }
                    }
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
            speedY += 0.6f;
            y += speedY;

            if (y > game.height + height / 2)
            {
                Gamedata.restartStage = 2;

                if (playerID == 1)
                {
                    Gamedata.playerWin = 2;
                }

                if (playerID == 2)
                {
                    Gamedata.playerWin = 1;
                }
                return;
            }

            //If below floor go back, reset falling speed and enable jump again 
            if (standsOnPlatform && !shouldBeFalling)
            {

                y -= speedY;
                speedY = 0;
                ableToJump = true;

                if (Gamedata.playerMoved)
                {
                    if (playerID == 1)
                    {
                        if (Gamedata.currentPlayer1Platform != null)
                        {
                            y += Gamedata.currentPlayer1Platform.theSpeed;
                        }
                    }

                    if (playerID == 2)
                    {
                        if (Gamedata.currentPlayer2Platform != null)
                        {
                            y += Gamedata.currentPlayer2Platform.theSpeed;
                        }
                    }
                }
            }

            //Else (if not on the floor) disable jumping
            else
            {
                ableToJump = false;
            }

            //If able to jump and jump button is pressed, jump
            if (jump && ableToJump)
            {
                hasSomeInput = true;
                Gamedata.playerMoved = true;
                speedY = -12;
                standsOnPlatform = false;
                Gamedata.currentPlayer1Platform = null;

            }
        }

        void CheckCollisionWithPlayer()
        {
            foreach (GameObject theCollision in collisions) //determine the collision
            {
                if (theCollision is Player)
                {
                    hasPlayerCollision = true;
                }
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
                            y -= height / 2;
                            Gamedata.CheckPlat(1);
                            y += height / 2;

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
                        if (hasSomeInput)
                        {
                            standsOnPlatform = false;
                            shouldBeFalling = false;
                        }
                    }
                }

                if (playerID == 2)
                {
                    if (checkStandOnPlatform == false || oldFlatform != Gamedata.currentPlayer2Platform) //if this condition is true, this means player falls off the platform
                    {
                        if (hasSomeInput)
                        {
                            standsOnPlatform = false;
                            shouldBeFalling = false;
                        }
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
                                    int heightt = 25;
                                    if (speedY > 0) //jump falling
                                    {
                                        Gamedata.detectPlatformPlayer1 = false;
                                        y += heightt;
                                        Gamedata.CheckPlat(1);
                                        y -= heightt;
                                    }

                                    //(this part needed?)
                                    else
                                    {
                                        Gamedata.detectPlatformPlayer1 = false;
                                        y -= heightt;
                                        Gamedata.CheckPlat(1);
                                        y += heightt;
                                    }
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
                                    y -= height / 2;
                                    Gamedata.CheckPlat(2);
                                    y += height / 2;
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
