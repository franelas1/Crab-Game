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
    float theSpeed;
    float currentSpeedX;
    float currentSpeedY; 
    protected bool canAllowTopJump = false; //Can jump to ceiling and keep jumping into the ceiling if holding jump (not working anymore)
    protected bool isJumping = false;
    float jumpHeightAndSpeed;
    protected bool isMovingHoz = false;
    List<Behavior> behaviorList = new List<Behavior>(); //storing all behaviors (tasks) the character has
    TiledObject obj = null; //Map parser data

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

            currentSpeedY = -jumpHeightAndSpeed; //CurrentSpeedY helps with gravity and will decrease over time until a certain threshold
        }

        //When player is jumping
        if (isJumping)
        {
            y += currentSpeedY; 
            if (currentSpeedY > -jumpHeightAndSpeed - 10)
            {
                currentSpeedY = currentSpeedY + 0.75f;

                if (currentSpeedY > 0)
                {
                    if (this is Player)
                    {
                        GameData.playerIsFallingJump = true;
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
            currentSpeedY = theSpeed;
            y += jumpHeightAndSpeed;
        }

        UpdateCollisions();
        
        //If touched by 'deadly' tile
        if (CheckIsColliding(5))
        {
            if (this is Player)
            {
                SoundChannel theSound = new Sound("playerDead.wav", false, false).Play();
                GameData.playerDead = true;
            }
        }

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
        }


        if (CheckIsColliding(4) && this is Player)
        {
            Collision gravityCollision = MoveUntilCollision(0, 2);

            if (gravityCollision != null)
            {
                if (GameData.thePlatform != null)
                {
                    GameData.thePlayer.y += 30;
                    GameData.checkPlat();
                    GameData.thePlayer.y -= 30;
                    if (GameData.playerIsFallingJump && (GameData.thePlatform.y - (GameData.thePlayer.height / 2)
                        > GameData.thePlayer.y))
                    {
                        isJumping = false;
                        y = oldY;
                    }
                }
            }
        }
    }

    public void HozMovement(bool isRight)
    {
        isMovingHoz = true;

        float oldX = x;

        if (isRight)
        {
            x += currentSpeedX;
        }

        else
        {
            x -= currentSpeedX;
        }

        UpdateCollisions();

        if (CheckIsColliding(1))
        {
            x = oldX;
        }

        if (CheckIsColliding(5))
        {
            if (this is Player)
            {
                SoundChannel theSound = new Sound("playerDead.wav", false, false).Play();
                GameData.playerDead = true;
                x = oldX;
                return;
            }
        }

        /*
        if (CheckIsColliding(4) && this is Player && GameData.playerPlatormColliderValue == 1)
        {
            x = oldX;
        }
        */
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void AddBehaviorByName(string theName)
    {
        switch (theName)
        {
        }
    }

}
