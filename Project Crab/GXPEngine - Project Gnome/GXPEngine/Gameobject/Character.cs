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
    List<Behavior> behaviorList = new List<Behavior>(); //storing all behaviors (tasks) the character has
    TiledObject obj = null; //Map parser data
    int temp = 0;
    bool lastState;
    float lastStateX;
    protected float acceleration = 1;
    protected float moveAmount;

    public bool trueJumpFalling = false;

    public Character(string theImageName, int columns, int rows, TiledObject obj=null) :
    base(theImageName, columns, rows, obj)
    {
        theSpeed = obj.GetFloatProperty("f_theSpeed", 4); //The moving left and right speed
        jumpHeightAndSpeed = obj.GetFloatProperty("f_jumpHeightAndSpeed", 1); //The jump height
        currentSpeedX = theSpeed;
        currentSpeedY = jumpHeightAndSpeed;
        this.obj = obj;
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

        //do all behaviors the character has
        foreach (Behavior b in behaviorList)
        {
            b.Action();
        }

        if (y > GameData.deathYPlayer)
        {
            if (this is Player)
            {
                SoundChannel theSound = new Sound("playerDead.wav", false, false).Play();
                GameData.playerDead = true;
            }

            else
            {
                LateDestroy();
            }
        }
    }

    public void AddBehavior(Behavior behavior)
    {
        behaviorList.Add(behavior);
    }

    public virtual void VerticalMovement(bool hasJumpIntent)
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
                        GameData.playerIsFallingJump = true;
                        trueJumpFalling = true;
                    }
                }

                else
                {
                    GameData.playerIsFallingJump = false;
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
                GameData.thePlatformSpawn = new Platform("colors.png", 1, 1, 0, 64, 48, -1, 0, 30, false, true);
            }
        }


        if (CheckIsColliding(4) && this is Player)
        {
            Collision gravityCollision = MoveUntilCollision(0, 2);

            if (gravityCollision != null)
            {

                if (GameData.thePlatform != null)
                {
                    GameData.detectSpawn = false;
                    GameData.thePlayer.y += 30;
                    GameData.CheckPlat();
                    GameData.thePlayer.y -= 30;

                    

                    if (!GameData.detectSpawn && GameData.playerIsFallingJump && (GameData.thePlatform.y - (GameData.thePlayer.height / 2)
                        > GameData.thePlayer.y))
                    {
                        isJumping = false;
                        y = oldY;
                    }

                }

                if (GameData.thePlatformSpawn != null)
                {

                    GameData.detectSpawn = false;
                    GameData.thePlayer.y += 30;
                    GameData.CheckPlatSpawned();
                    GameData.thePlayer.y -= 30;


                    //       Console.WriteLine("Player Y: {0} | Platform Y: {1}", GameData.thePlayer.y, GameData.thePlatformSpawn.y);

              //      Console.WriteLine((GameData.thePlatformSpawnOld != GameData.thePlatformSpawn) || (GameData.thePlatformSpawnOld == null));

                    if (GameData.detectSpawn && GameData.playerIsFallingJump && (GameData.thePlatformSpawn.y + GameData.thePlatformSpawn.height
                        > GameData.thePlayer.y) && (((GameData.thePlatformSpawnOld == GameData.thePlatformSpawn) || (GameData.thePlatformSpawnOld == null))))
                    {
                        isJumping = false;
                        y = oldY;
                 //      GameData.thePlatformSpawnOld = GameData.thePlatformSpawn;
                    }

                    else
                    {
                        GameData.thePlatformSpawnOld = GameData.thePlatformSpawn;
                    }

                    if (GameData.thePlatformListSpawned != null)
                    {
                        return;
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
