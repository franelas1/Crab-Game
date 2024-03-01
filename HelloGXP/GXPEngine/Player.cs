using System;
using System.Collections.Generic;

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

        public float speedX = 10;
        public float speedXTemp = 10;
        float speedY;
        float oldY;         //Last frame player Y position


        int margin;
        public bool hasSomeInput;
        public bool hasPlayerCollision;

        //Arduino input variables 
        public int moveXAmount;
        public int jumpButton;
        public int powerButton;

        //tempX: X position of player at spawn
        //tempY: Y position of player at spawn

        GameObject[] collisions;

        public bool isInAbility;
        public List<Ability> theAbilities = new List<Ability>();

        int jumpHeight = 18;

        //1 == idle, 2 = jump, 3 = walk
        int animationStage = 1;

        public int animationDelay = 30;

        //60
        int idleFrame = 1;
        int idleFrames = 1;
        int idleFrameDelay = 60;

        //5
        int jumpFrame = 0;
        int jumpFrames = 7;
        int jumpFrameDelay = 5;

        //10
        int walkFrame = 9;
        int walkFrames = 2;
        int walkFramesDelay = 10;

        //   bool abilityActivated = false;

        bool abilitySound = false;

        public Player(int playerID, float tempX, float tempY, float scaleX, float scaleY, int margin, string filename, int columns, int rols,
            int animationDelay, int idleFrame, int idleFrames, int idleFrameDelay,
            int jumpFrame, int jumpFrames, int jumpFrameDelay, int walkFrame, int walkFrames, int walkFramesDelay) : base(filename, columns, rols)
        {
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            //Setting player origin at the middle of bottom side
            SetOrigin(width / 2, height);
            x = tempX;
            y = tempY;
            this.playerID = playerID;
            this.margin = margin;
            this.animationDelay = animationDelay;
            this.idleFrame = idleFrame;
            this.idleFrames = idleFrames;
            this.jumpFrame = jumpFrame;
            this.jumpFrames = jumpFrames;
            this.walkFrame = walkFrame;
            this.walkFrames = walkFrames;
            this.idleFrameDelay = idleFrameDelay;
            this.jumpFrameDelay = jumpFrameDelay;
            this.walkFramesDelay = walkFramesDelay;

            SetCycle(idleFrame, idleFrames, (byte)idleFrameDelay);
            animationStage = 1;

        }
        //Updating player movement. Takes in 3 inputs (for now) for each right, left and up buttons.
        public void updatePlayer()
        {            
            if (animationStage == 2)
            {
                if (playerID == 1)
                {
                    if (currentFrame == 6)
                    {
                        SetCycle(idleFrame, idleFrames, (byte)idleFrameDelay);
                        animationStage = 1;
                    }
                }

                if (playerID == 2)
                {
                    if (currentFrame == 4)
                    {
                        SetCycle(idleFrame, idleFrames, (byte)idleFrameDelay);
                        animationStage = 1;
                    }
                }
            }


            Animate();


            foreach (Ability theAbility in theAbilities)
            {
                if (theAbility.markedForDeathPrep != 0)
                {
                    theAbilities.Remove(theAbility);
                    break;
                }
            }
            
            foreach (Ability theAbility in theAbilities)
            {
                if (theAbility.hasStart == false)
                {
                    if (((playerID == 1 && (Input.GetKey('F') || Gamedata.player1.powerButton == 1))) || 
                        (playerID == 2 && (Input.GetKey('G') || Gamedata.player2.powerButton == 1)))
                    {
                        theAbility.hasStart = true;
                        theAbility.UpdateAbility();
                        if (playerID == 1)
                        {
                            SoundChannel theSound = new Sound("SFX_Power-up_001_Crab.wav", false, false).Play();
                        }
                        else
                        {
                            SoundChannel theSound = new Sound("SFX_Power-up_001_Lobster.wav", false, false).Play();
                        }
                    }
                }

                if (theAbility.isOver == false && theAbility.hasStart == true)
                {


                    //Console.WriteLine(theAbility.theAbility);
                    switch (theAbility.theAbility) 
                    {
                        case "ability_gralicPiece":
                            if (playerID == 1)
                            {
                                Gamedata.player2.speedXTemp = Gamedata.player2.speedX  - (float)(Gamedata.player2.speedX * 0.25);
                            }

                            if (playerID == 2)
                            {
                                Gamedata.player1.speedXTemp = Gamedata.player1.speedX  - (float)(Gamedata.player1.speedX * 0.25);
                            }
                            Console.WriteLine("gralic piece");
                            break;
                        case "ability_chiliPepperPiece":
                            Console.WriteLine("chili piece");
                            break;
                        case "ability_lavenderFlower":
                            Console.WriteLine("lavender flower");
                            break;
                        case "ability_basilLeaf":
                            Console.WriteLine("basil leaf");
                            Gamedata.inBasilLEffect = true;
                            break;
                    }
                }

                else if (theAbility.hasStart == true)
                {
                    switch (theAbility.theAbility)
                    {
                        case "ability_gralicPiece":
                            if (playerID == 1)
                            {
                                Gamedata.player2.speedXTemp = Gamedata.player2.speedX;
                            }

                            if (playerID == 2)
                            {
                                Gamedata.player1.speedXTemp = Gamedata.player1.speedX;
                            }
                            break;
                        case "ability_chiliPepperPiece":
                            break;
                        case "ability_lavenderFlower":
                            break;
                        case "ability_basilLeaf":
                            if (playerID == 1)
                            {
                                if (Gamedata.player2.hasAbility("ability_basilLeaf") == false)
                                {
                                    Gamedata.inBasilLEffect = false;
                                }
                            }

                            if (playerID == 2)
                            {
                                if (Gamedata.player1.hasAbility("ability_basilLeaf") == false)
                                {
                                    Gamedata.inBasilLEffect = false;
                                }
                            }

                            break;
                    }

                    if (playerID == 1)
                    {
                            theAbility.markedForDeathPrep = 1;
                    }

                    if (playerID == 2)
                    {
                        theAbility.markedForDeathPrep = 2;
                    }
                }
            }


            hasSomeInput = false;
            hasPlayerCollision = false;
            collisions = GetCollisions();
            CheckCollisionWithPlayer();

            if (playerID == 1)
            {
                if (Input.GetKey(Key.D) || Input.GetKey(Key.A) || moveXAmount != 0)
                {
                    hasSomeInput = true;
                }
                movementLR(Input.GetKey(Key.D), Input.GetKey(Key.A));
              // movementLR(Gamedata.player1.moveXAmount);
                movementUD(Input.GetKey(Key.W), Gamedata.player1.jumpButton);

            }

            if (playerID == 2)
            {
                if (Input.GetKey(Key.RIGHT) || Input.GetKey(Key.LEFT) || moveXAmount != 0)
                {
                    hasSomeInput = true;
                }
               movementLR(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT));
                //movementLR(Gamedata.player2.moveXAmount);
                movementUD(Input.GetKey(Key.UP), Gamedata.player2.jumpButton);
            }

            if (animationStage == 3 && hasSomeInput == false)
            {
                SetCycle(idleFrame, idleFrames, (byte) idleFrameDelay);
                animationStage = 1;
            }

            //platform collision logic
            y += 15; //need to move player y temporaly for the collision logic to work
            CheckCollisionWithPlatform();
            y -= 15;

        }

        public bool hasAbility(string abilityName)
        {
            bool hasAbility = false;
            foreach (Ability thAbility in theAbilities)
            {
                if (thAbility.theAbility == abilityName)
                {
                    hasAbility = true;
                }
            }
            return hasAbility;
        }

        //Player movement Left Right
        void movementLR(bool goRight, bool goLeft)
        {
            if (goRight && goLeft)
            {
                return;
            }

            if ((goRight || goLeft) && standsOnPlatform)
            {
                SetCycle(walkFrame, walkFrames, (byte)walkFramesDelay);
                animationStage = 3;
            }

            //If right button pressed 
            if (goRight)
            {
                if (playerID == 1)
                {
                    //hasPlayerCollision == false
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    //speedXTemp = moveX;
                    x += speedXTemp;

                    if (x + width / 2 > game.width - (margin + 100))
                    {
                        x -= speedXTemp;
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x < Gamedata.player2.x 
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    {
                        Console.WriteLine("player 1 goes right cancel");
                        x -= speedXTemp;
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x -= (float)(speedXTemp * 2) * 0.20f;
                        }
                    }

                    else if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x > Gamedata.player2.x
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    { 
                        Gamedata.player2.x += speedXTemp;
                        if (hasAbility("ability_lavenderFlower"))
                        {
                            Gamedata.player2.x += (float)(speedXTemp * 2) * 0.20f;
                        }
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x += speedXTemp;


                    if (x + width / 2 > game.width - (margin + 100))
                    {

                        x -= speedXTemp;

                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.A)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x -= speedXTemp;
                        if (Gamedata.player1.hasAbility("ability_lavenderFlower"))
                        {
                            x -= (float) (speedXTemp * 2) * 0.20f;
                        }
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
                    x -= speedXTemp;


                    if (x - width / 2 < (margin * 2 - 100))
                    {

                        x += speedXTemp;
                    }



                    if (hasPlayerCollision == true && Input.GetKey(Key.RIGHT) &&
                        Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 1 goes left cancel");
                        x += speedXTemp;
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x += (float)(speedXTemp * 2) * 0.20f;
                        }
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedXTemp;

                    if (x - width / 2 < (margin * 2 - 100))
                    {

                        x += speedXTemp;
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.D)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x += speedXTemp;
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x += (float)(speedXTemp * 2) * 0.20f;
                        }
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

                        if ((Input.GetKey(Key.RIGHT)) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x += speedXTemp;
                            if (x + width / 2 > game.width - margin)
                            {
                                x -= speedXTemp;
                                x -= speedXTemp;
                            }
                        }

                        if ((Input.GetKey(Key.LEFT))
                            && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x -= speedXTemp;
                            if (x - width / 2 < margin)
                            {
                                x += speedXTemp;
                                x += speedXTemp;
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

                        if ((Input.GetKey(Key.D)) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x += speedXTemp;
                            if (x + width / 2 > game.width - margin)
                            {
                                x -= speedXTemp;
                                x -= speedXTemp;
                            }
                        }

                        if ((Input.GetKey(Key.A)) &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x -= speedXTemp;
                            if (x - width / 2 < margin)
                            {
                                x += speedXTemp;
                                x += speedXTemp;
                            }
                        }
                    }
                }
            }

        }

        void movementLR(int moveX)
        {
            if (moveX != 0 && standsOnPlatform)
            {
                SetCycle(walkFrame, walkFrames, (byte)walkFramesDelay);
                animationStage = 3;
            }

            //If right button pressed 
            if (moveX > 0)
            {
                if (playerID == 1)
                {
                    //hasPlayerCollision == false
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    //speedXTemp = moveX;
                    x -= speedXTemp * (moveX / 100f);

                    if (x + width / 2 > game.width - (margin + 100))
                    {
                        x += speedXTemp * (moveX / 100f);
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x < Gamedata.player2.x
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    {
                        SoundChannel theSound = new Sound("SFX_Push_001_Crab.wav", false, false).Play();
                        Console.WriteLine("player 1 goes right cancel");
                        x += speedXTemp * (moveX / 100f);
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x += speedXTemp * 0.4f * (moveX / 100f);
                        }
                    }

                    else if (hasPlayerCollision == true && Input.GetKey(Key.LEFT) && Gamedata.player1.x > Gamedata.player2.x
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) < 2)
                    {
                        Gamedata.player2.x += speedXTemp * (moveX / 100f);
                        if (hasAbility("ability_lavenderFlower"))
                        {
                            Gamedata.player2.x -= speedXTemp * 0.4f * (moveX / 100f);
                        }
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedXTemp * (moveX / 100f);


                    if (x + width / 2 > game.width - (margin + 100))
                    {

                        x += speedXTemp * (moveX / 100f);

                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.A)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x += speedXTemp * (moveX / 100f);
                        SoundChannel theSound = new Sound("SFX_Push_001_Lobster.wav", false, false).Play();
                        if (Gamedata.player1.hasAbility("ability_lavenderFlower"))
                        {
                            x += speedXTemp * 0.4f * (moveX / 100f);
                        }
                    }
                }
            }

            //If left button pressed
            if (moveX < 0)
            {
                if (playerID == 1)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedXTemp * (moveX / 100f);

                    if (x - width / 2 < (margin * 2 - 100))
                    {

                        x += speedXTemp * (moveX / 100f);
                    }



                    if (hasPlayerCollision == true && Input.GetKey(Key.RIGHT) &&
                        Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 1 goes left cancel");
                        x += speedXTemp * (moveX / 100f);
                        SoundChannel theSound1 = new Sound("SFX_Push_001_Crab.wav", false, false).Play();
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x += speedXTemp * 0.4f * (moveX / 100f);
                        }
                    }
                }

                if (playerID == 2)
                {
                    hasSomeInput = true;
                    Gamedata.playerMoved = true;
                    x -= speedXTemp * (moveX / 100f);

                    if (x - width / 2 < (margin * 2 - 100))
                    {

                        x += speedXTemp * (moveX / 100f);
                    }

                    if (hasPlayerCollision == true && Input.GetKey(Key.D)
                        && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                    {
                        Console.WriteLine("player 2 goes right cancel");
                        x += speedXTemp * (moveX / 100f);
                        SoundChannel theSound = new Sound("SFX_Push_001_Lobster.wav", false, false).Play();
                        if (Gamedata.player2.hasAbility("ability_lavenderFlower"))
                        {
                            x += speedXTemp * 0.4f * (moveX / 100f);
                        }
                    }
                }
            }

            if (hasSomeInput == false)
            {
                if (playerID == 1)
                {
                    if (hasPlayerCollision)
                    {
                        if (Gamedata.player1.moveXAmount == 0)
                        {
                            return;
                        }

                        if (Gamedata.player1.moveXAmount > 0 &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            SoundChannel theSound = new Sound("SFX_Push_001_Crab.wav", false, false).Play();
                            x -= speedXTemp * (moveX / 100f);

                            if (x + width / 2 > game.width - margin)
                            {


                                x += speedXTemp * (moveX / 100f);
                                x += speedXTemp * (moveX / 100f);
                            }
                        }

                        if (Gamedata.player1.moveXAmount < 0
                            && Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            SoundChannel theSound = new Sound("SFX_Push_001_Crab.wav", false, false).Play();
                            x -= speedXTemp * (moveX / 100f);
                            if (x - width / 2 < margin)
                            {
                                x += speedXTemp * (moveX / 100f);
                                x += speedXTemp * (moveX / 100f);
                            }
                        }
                    }
                }

                if (playerID == 2)
                {
                    if (hasPlayerCollision)
                    {
                        if (Gamedata.player2.moveXAmount == 0)
                        {
                            return;
                        }

                        if (Gamedata.player2.moveXAmount > 0 &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            x -= speedXTemp * (moveX / 100f);
                            SoundChannel theSound = new Sound("SFX_Push_001_Lobster.wav", false, false).Play();
                            if (x + width / 2 > game.width - margin)
                            {
                                x += speedXTemp * (moveX / 100f);
                                x += speedXTemp * (moveX / 100f);
                            }
                        }

                        if (Gamedata.player2.moveXAmount < 0 &&
                            Math.Abs(Gamedata.player1.width - CustomUtil.GetDistance(Gamedata.player1, Gamedata.player2)) <= DISTTHRESHOLDPLAYERDISTCOMPARE)
                        {
                            hasSomeInput = true;
                            Gamedata.playerMoved = true;
                            SoundChannel theSound = new Sound("SFX_Push_001_Lobster.wav", false, false).Play();
                            x -= speedXTemp * (moveX / 100f);


                            if (x - width / 2 < margin)
                            {
                                x += speedXTemp * (moveX / 100f);
                                x += speedXTemp * (moveX / 100f);
                            }
                        }
                    }
                }
            }

        }

        //Player movement Up Down
        void movementUD(bool jump, int jumpA)
        {

            //Saves last frame's Y coordinate of the player
            if (oldY != y)
            {
                oldY = y;
            }

            //Always tries to move down
            speedY += 0.6f;
            y += speedY;

            if (y < 0)
            {
                speedY = 0;
            }
            
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
                    
                    if (playerID == 1 && Gamedata.countdownOver)
                    {
                        if (Gamedata.currentPlayer1Platform != null)
                        {
                            
                            if (Gamedata.inBasilLEffect)
                            {
                              y += Gamedata.platformSpeed - (float)(Gamedata.platformSpeed * 0.25);
                            }

                            else
                            {
                               y += Gamedata.platformSpeed;
                            }
                            
                            
                        }
                    }
                    
                    
                    if (playerID == 2 && Gamedata.countdownOver)
                    {
                        if (Gamedata.currentPlayer2Platform != null)
                        {
                            
                            if (Gamedata.inBasilLEffect)
                            {
                                y += Gamedata.platformSpeed - (float)(Gamedata.platformSpeed * 0.25);
                            }

                            else
                            {
                               y += Gamedata.platformSpeed;
                            }
                            
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
            if ((jump || jumpA == 1) && ableToJump)
            {

                if (playerID == 1)
                {
                    SoundChannel theSound = new Sound("SFX_Jump_001_Crab.wav", false, false).Play();
                }

                else
                {
                    SoundChannel theSound = new Sound("SFX_Jump_001_Lobster.wav", false, false).Play();
                }

                animationStage = 2;
                SetCycle(jumpFrame, jumpFrames, (byte)jumpFrameDelay);
                hasSomeInput = true;
                Gamedata.playerMoved = true;
                if (hasAbility("ability_chiliPepperPiece"))
                {
                    Console.WriteLine("jump increase");
                    speedY = -1 * (jumpHeight + (float) (jumpHeight*0.25));
                }

                else
                {
                    speedY = -1 * jumpHeight;
                }
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
                bool detectPlat = false;

                foreach (GameObject theCollision in collisions) //determine the collision
                {

                    if (theCollision is Platform) //if collide with platform
                    {
                        detectPlat = true;
                        if (playerID == 1)
                        {
                            float OGXValue = x;
                            Gamedata.detectPlatformPlayer1 = false;

                            y += height / 2;
                            Gamedata.CheckPlat(1);
                            y -= height / 2;

                            if (Gamedata.currentPlayer1Platform != null)
                            {
                                //update the current platform player collides with
                                if ((x + width / 2) < Gamedata.currentPlayer1Platform.x)
                                {
                                    x += width / 2;
                                }

                                if ((x + width / 2) > Gamedata.currentPlayer1Platform.x)
                                {
                                    x += width / 2;
                                }

                            }

                            Gamedata.detectPlatformPlayer1 = false;

                            y += height / 2;
                            Gamedata.CheckPlat(1);
                            y -= height / 2;
                           

                            x = OGXValue;

                            if (Gamedata.detectPlatformPlayer1)
                            {
                                checkStandOnPlatform = true; //this means player is still standing on the platform
                            }
                        }

                        else
                        {
                            float OGXValue = x;

                            Gamedata.detectPlatformPlayer2 = false;
                            y += height / 2;
                            Gamedata.CheckPlat(2);
                            y -= height / 2;
                            

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

                if (detectPlat == false)
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
                            if (speedY > 0) //jump falling
                            {
                                Gamedata.detectPlatformPlayer1 = false;
                                x += width / 2;
                                y += height / 2;
                                Gamedata.CheckPlat(1);
                                y -= height / 2;
                                x -= width / 2;
                            }

                            if (Gamedata.currentPlayer1Platform != null)
                            {

                                //debug messages


                                /*
                                Console.WriteLine("---------------");
                                Console.WriteLine(speedY > 0);
                                Console.WriteLine(Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)));
                                Console.WriteLine(Math.Abs(y - Gamedata.currentPlayer1Platform.y));
                                */



                                //if detection fails, try again (this might not be needed)
                                if (Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                    - (y - height / 2)) < Gamedata.currentPlayer1Platform.detectionValue == false)
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
                                        /*
                                        Gamedata.detectPlatformPlayer1 = false;
                                        y -= heightt;
                                        Gamedata.CheckPlat(1);
                                        y += heightt;
                                        */
                                    }
                                }

                                //      Console.WriteLine("s: " + Math.Abs(y + (height / 2) - Gamedata.currentPlayer1Platform.y));
                                //the conditions below determine if player successfully stands on a platform
                                //first determine if player is falling, and player at specific height relative
                                //to the detected platform

                                //Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
                                //     -(y - height / 2)) < Gamedata.currentPlayer1Platform.detectionValue
                                if (speedY > 0 
                                    && Math.Abs(y - (height / 2) - (Gamedata.currentPlayer1Platform.y)) < Gamedata.currentPlayer1Platform.detectionValue
                                    )
                                {
                                    //      y += (y - height / 2) - Gamedata.currentPlayer1Platform.y; //ajust y for more accurate landing
                                    float ogY = y;
                                    y = Gamedata.currentPlayer1Platform.y - Gamedata.currentPlayer1Platform.height / 2;

                                    if (y < 0)
                                    {
                                        y = ogY;
                                    }

                                    if (y > 0)
                                    {
                                        SoundChannel theSound = new Sound("SFX_Land_001_Crab.wav", false, false).Play();
                                        standsOnPlatform = true;
                                        shouldBeFalling = false;
                                        oldFlatform = Gamedata.currentPlayer1Platform;
                                    }


                                }
                            }
                        }

                        if (playerID == 2) //if player is player 1
                        {
                            if (speedY > 0) //jump falling
                            {
                                Gamedata.detectPlatformPlayer2 = false;
                                x += width / 2;
                                y += height / 2;
                                Gamedata.CheckPlat(2);
                                y -= height / 2;
                                x -= width / 2;
                            }

                            //(this part needed?)
                            else
                            {
                                /*
                                Gamedata.detectPlatformPlayer2 = false;
                                y -= height / 2;
                                Gamedata.CheckPlat(2);
                                y += height / 2;
*/
                            }
                            if (Math.Abs(Gamedata.currentPlayer2Platform.y - (Gamedata.currentPlayer2Platform.height / 2)
                                - (y - height / 2)) < Gamedata.currentPlayer2Platform.detectionValue == false)
                            {
                                int heightt = 25;
                                if (speedY > 0) //jump falling
                                {
                                    Gamedata.detectPlatformPlayer2 = false;
                                    y += heightt;
                                    Gamedata.CheckPlat(2);
                                    y -= heightt;
                                }


                                //the conditions below determine if player successfully stands on a platform
                                //first determine if player is falling, and player at specific height relative
                                //to the detected platform
                                if (speedY > 0
                                    && Math.Abs(y - (height / 2) - (Gamedata.currentPlayer2Platform.y)) < Gamedata.currentPlayer2Platform.detectionValue)
                                {
                                    //      y += (y - height / 2) - Gamedata.currentPlayer1Platform.y; //ajust y for more accurate landing
                                    float ogY = y;
                                    y = Gamedata.currentPlayer2Platform.y - Gamedata.currentPlayer2Platform.height / 2;

                                    if (y < 0)
                                    {
                                        y = ogY;
                                    }

                                    if (y > 0)
                                    {
                                        SoundChannel theSound = new Sound("SFX_Land_001_Lobster.wav", false, false).Play();
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
}
