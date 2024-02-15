using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.Managers;
using TiledMapParser;

/*
 * All characters (the player and enemies) are inherited from this class. Provide walking and jumping movements, and gravity. 
 */
public class Character : AnimationSpriteCustom
{
    protected float theSpeed;
    float currentSpeedX;
    protected float currentSpeedY; 
    protected bool canAllowTopJump = false; //Can jump to ceiling and keep jumping into the ceiling if holding jump (not working anymore)
    public bool isJumping = false;
    float jumpHeightAndSpeed;
    protected bool isMovingHoz = false;
    TiledObject obj = null; //Map parser data
    int temp = 0;
    bool lastState;
    float lastStateX;
    protected float acceleration = 1;
    protected float moveAmount;

    protected bool trueJumpFalling = false;

    public Character(string theImageName, int columns, int rows, TiledObject obj=null) :
    base(theImageName, columns, rows, obj)
    {
        theSpeed = obj.GetFloatProperty("f_theSpeed", 4); //The moving left and right speed
        jumpHeightAndSpeed = obj.GetFloatProperty("f_jumpHeightAndSpeed", 1); //The jump height
        currentSpeedX = theSpeed;
        currentSpeedY = jumpHeightAndSpeed;
        this.obj = obj;
    }

    public Character(string filenName, float scaleX, float scaleY, int singleFrameID, int columns, int rows,
    int numberOfFrames, int startFrame, int nextFrameDelay, bool textureKeepInCache, bool hasCollision) :
    base(filenName, scaleX, scaleY, singleFrameID, columns, rows,
     numberOfFrames, startFrame, nextFrameDelay, textureKeepInCache, hasCollision)
    {
    }

    //Setting the moving left and right speed
    public void setSpeed(float theSpeed)
    {
        this.theSpeed = theSpeed;
        currentSpeedX = this.theSpeed;
    }

    //Setting the jump height
    public void setJumpHeightAndSpeed(float jumpHeightAndSpeed)
    {
        this.jumpHeightAndSpeed = jumpHeightAndSpeed;
        currentSpeedY = this.jumpHeightAndSpeed;
    }
    protected override void Update()
    {
        base.Update();


        if (y > GameData.deathYPlayer)
        {
            if (this is Player)
            {

                SoundChannel theSound = new Sound("playerDead.wav", false, false).Play();

                if (id == "Player1")
                {
                    Console.WriteLine("Player 2 win");
                }

                else
                {
                    Console.WriteLine("Player 1 win");
                }
                GameData.playerDead = true;

                
            }

            else
            {
                LateDestroy();
            }
        }
    }
    public virtual void VerticalMovement(bool hasJumpIntent, int thePlayernumber)
    {
        float oldY = y;

        //When player is on the ground and wants to jump, so start jmup
        if (hasJumpIntent && !isJumping)
        {
            isJumping = true;

            if (this is Player)
            {
                SoundChannel newSound = new Sound("hop.wav", false, false).Play();
            }

            currentSpeedY = -moveAmount * 1.3f - 5; //CurrentSpeedY helps with gravity and will decrease over time until a certain threshold
        }

        //When player is jumping
        if (isJumping)
        {
            //float moveAmountY = moveAmount;
            y += currentSpeedY;  //moveAmountY;  //currentSpeedY; 
            if (currentSpeedY > -jumpHeightAndSpeed - 10)
            {
                currentSpeedY = currentSpeedY + 0.15f;

                if (currentSpeedY > 0)
                {
                    if (this is Player)
                    {
                        if (id == "Player1")
                        {
                            GameData.playerIsFallingJump1 = true;
                            trueJumpFalling = true;
                        }
                        if (id == "Player2")
                        {
                            GameData.playerIsFallingJump2 = true;
                            trueJumpFalling = true;
                        }

                    }
                }

                else
                {
                    if (id == "Player1")
                    {
                        GameData.playerIsFallingJump1 = false;
                    }

                    if (id == "Player2")
                    {
                        GameData.playerIsFallingJump2 = false;
                    }
                }

                if (currentSpeedY > 30)
                {
                    currentSpeedY = 20;
                }
            }
        }

        //If player isn't jumping, adding more value to y would fix player floating (whening moving left and right)
        else
        {
            trueJumpFalling = false;
            currentSpeedY = theSpeed;
            y += jumpHeightAndSpeed;
        }

        UpdateCollisions();
       
        //Collider with wall
        if (CheckIsColliding(1))
        {
            y = oldY;
            Collision gravityCollision = MoveUntilCollision(0, 2);

            if (gravityCollision != null)
            {
                if (gravityCollision.normal.y < 0 || canAllowTopJump)
                {
                    isJumping = false;
                    y = oldY;
                }
            }

            if (this is Player)
            {
                if (id == "Player1")
                {
                    GameData.thePlatformSpawn1 = new Platform("colors.png", 1, 1, 0, 64, 48, -1, 0, 30, false, true);
                }

                if (id == "Player2")
                {
                    GameData.thePlatformSpawn2 = new Platform("colors.png", 1, 1, 0, 64, 48, -1, 0, 30, false, true);
                }

            }
        }


        if (CheckIsColliding(4) && this is Player)
        {
            Collision gravityCollision = MoveUntilCollision(0, 2);

            if (gravityCollision != null)
            {
                if (thePlayernumber == 1)
                {
                    if (GameData.thePlatform1 != null)
                    {
                        GameData.detectSpawn1 = false;
                        GameData.player1.y += 30;
                        GameData.CheckPlat(1);
                        GameData.player1.y -= 30;

                        if (!GameData.detectSpawn1 && GameData.playerIsFallingJump1 && (GameData.thePlatform1.y - (GameData.player1.height / 2)
                            > GameData.player1.y))
                        {
                            isJumping = false;
                            y = oldY;
                        }
                    }

                    if (GameData.thePlatformSpawn1 != null)
                    {

                        GameData.detectSpawn1 = false;
                        GameData.player1.y += 30;
                        GameData.CheckPlatSpawned(1);
                        GameData.player1.y -= 30;

                        if (GameData.detectSpawn1 && GameData.playerIsFallingJump1 && (GameData.thePlatformSpawn1.y + GameData.thePlatformSpawn1.height
                            > GameData.player1.y) && (((GameData.thePlatformSpawnOld1 == GameData.thePlatformSpawn1) || (GameData.thePlatformSpawnOld1 == null))))
                        {
                            isJumping = false;
                            y = oldY;
                            //      GameData.thePlatformSpawnOld = GameData.thePlatformSpawn;

                            if (GameData.theNumberReached < GameData.thePlatformSpawn1.theNumber)
                            {
                                GameData.platformSpawnAmount = GameData.thePlatformSpawn1.theNumber - GameData.theNumberReached;
                                GameData.theNumberReached = GameData.thePlatformSpawn1.theNumber;
                            }
                        }

                        else
                        {
                            GameData.thePlatformSpawnOld1 = GameData.thePlatformSpawn1;
                        }

                        if (GameData.thePlatformListSpawned != null)
                        {
                            return;
                        }
                    }
                }

                else
                {
                    if (GameData.thePlatform2 != null)
                    {
                        GameData.detectSpawn2 = false;
                        GameData.player2.y += 30;
                        GameData.CheckPlat(2);
                        GameData.player2.y -= 30;

                        if (!GameData.detectSpawn2 && GameData.playerIsFallingJump2 && (GameData.thePlatform2.y - (GameData.player2.height / 2)
                            > GameData.player2.y))
                        {
                            isJumping = false;
                            y = oldY;
                        }
                    }

                    if (GameData.thePlatformSpawn2 != null)
                    {

                        GameData.detectSpawn2 = false;
                        GameData.player2.y += 30;
                        GameData.CheckPlatSpawned(2);
                        GameData.player2.y -= 30;

                        if (GameData.detectSpawn2 && GameData.playerIsFallingJump2 && (GameData.thePlatformSpawn2.y + GameData.thePlatformSpawn2.height
                            > GameData.player2.y) && (((GameData.thePlatformSpawnOld2 == GameData.thePlatformSpawn2) || (GameData.thePlatformSpawnOld2 == null))))
                        {
                            isJumping = false;
                            y = oldY;
                            //      GameData.thePlatformSpawnOld = GameData.thePlatformSpawn;

                            if (GameData.theNumberReached < GameData.thePlatformSpawn2.theNumber)
                            {
                                GameData.platformSpawnAmount = GameData.thePlatformSpawn2.theNumber - GameData.theNumberReached;
                                GameData.theNumberReached = GameData.thePlatformSpawn2.theNumber;
                            }
                        }

                        else
                        {
                            GameData.thePlatformSpawnOld2 = GameData.thePlatformSpawn2;
                        }

                        if (GameData.thePlatformListSpawned != null)
                        {
                            return;
                        }
                    }
                }
            }
        }

        if (CheckIsColliding(5) && !CheckIsColliding(4))
        {
            {
                isJumping = false;
                y = oldY;
            }
        }
    }

    public void HozMovement(bool isRight)
    { 
        if (temp == 0)
        {
            lastState = isRight;
            temp = 1;
        }

        isMovingHoz = true;
        float oldX = x;

        if (lastState != isRight || !isMovingHoz && !isJumping)
        {
            acceleration = 1; 
            lastState = isRight;
        }
        else
        {
            if (acceleration < 3) acceleration *= 1.02f;
        }

        if (isRight)
        {
            moveAmount = currentSpeedX + acceleration;
            x += moveAmount; 
        }
        else
        {
            moveAmount = currentSpeedX + acceleration;
            x -= moveAmount;
        }

        UpdateCollisions();

        if (CheckIsColliding(1))
        {
            x = oldX;
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

}
