﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Managers;
using TiledMapParser;

/*
 * The player character and movements
 */
public class Player : Character
{
    int runStartFrame;
    int runAmountOfFrames;
    int idleStartFrame;
    int idleAmountOfFrames;
    int jumpStartFrame;
    int jumpAmountOfFrames;
    bool isJumpPrev = false;

    bool ableToJump = true;
    int jumpTimer = Time.time;

    public Player(string fileName, int rows, int columns, TiledObject obj = null) : base(fileName, rows, columns, obj)
    {
        idleStartFrame = obj.GetIntProperty("a_idleStartFrame", 0);
        idleAmountOfFrames = obj.GetIntProperty("a_idleNumberOfFrames", 0);
        runStartFrame = obj.GetIntProperty("a_runStartFrame", 0);
        runAmountOfFrames = obj.GetIntProperty("a_runNumberOfFrames", 0);
        jumpStartFrame = obj.GetIntProperty("a_jumpStartFrame", 0);
        jumpAmountOfFrames = obj.GetIntProperty("a_jumpNumberOfFrames", 0);
        SetAnimationCycle(idleStartFrame, idleAmountOfFrames);
        GameData.theLevel.AddChild(this);
    }

    public Player(string filenName, float scaleX, float scaleY, int singleFrameID, int columns, int rows,
    int numberOfFrames, int startFrame, int nextFrameDelay, bool textureKeepInCache, bool hasCollision) :
    base(filenName, scaleX, scaleY, singleFrameID, columns, rows,
     numberOfFrames, startFrame, nextFrameDelay, textureKeepInCache, hasCollision)
    {
    }

    protected override void Update()
    {
        if (id == "Player1")
        {
            GameData.oldPlayerY = y;
        }
        
        base.Update();
        CheckPlayerControl();

        if (ableToJump == false)
        {
            if (Time.time - jumpTimer >= 100)
            {
                ableToJump = true;
            }
        }
    }

    public void SetAnimation(int theAnimation)
    {
        switch (theAnimation)
        {
            case 0:
                SetAnimationCycle(idleStartFrame, idleAmountOfFrames);
                break;
            case 1:
                SetAnimationCycle(jumpStartFrame, jumpAmountOfFrames);
                break;
            default:
                SetAnimationCycle(runStartFrame, runAmountOfFrames);
                break;
        }
    }

    void CheckPlayerControl()
    {
        UpdateCollisions();

        isMovingHoz = false;

        if (id == "Player1")
        {
            if (Input.GetKey('D'))
            {
                HozMovement(true);
            }
            
            if (Input.GetKey('A'))
            {
                HozMovement(false);
            }
            
            if (Input.GetKeyDown('W'))
            {
                VerticalMovement(true, 1);
                ableToJump = false;
                jumpTimer = Time.time;
            }
            
            if (Input.GetKeyUp('A') || Input.GetKeyUp('D'))
            {
                acceleration = 1;
            }
            
            else
            {
                if (!CheckIsColliding(2))
                {
                    VerticalMovement(false, 1);
                }
            }
        }

        if (id == "Player2")
        {
            if (Input.GetKey('L') || Input.GetKey(Key.RIGHT))
            {
                HozMovement(true);
            }

            if (Input.GetKey('J') || Input.GetKey(Key.LEFT))
            {
                HozMovement(false);
            }

            if (Input.GetKeyDown('I') || Input.GetKeyDown(Key.UP))
            {
                VerticalMovement(true, 2);
                ableToJump = false;
                jumpTimer = Time.time;
            }

            if (Input.GetKeyUp('J') || Input.GetKeyUp('L') || Input.GetKeyUp(Key.LEFT) || Input.GetKeyUp(Key.RIGHT))
            {
                acceleration = 1;
            }

            else
            {
                if (!CheckIsColliding(2))
                {
                    VerticalMovement(false, 2);
                }
            }
        }


        //If player got stuck into wall (from stomping enemy)
        if (CheckIsColliding(1))
        {
            y -= 1;
        }
    }

    public override void VerticalMovement(bool hasJumpIntent, int thePlayerNumber)
    {

        if (id == "Player1")
        {
            thePlayerNumber = 1;
        }

        if (id == "Player2")
        {
            thePlayerNumber = 2;
        }

        if (isJumping && isJumping != isJumpPrev)
        {
            SetAnimationCycle(jumpStartFrame, jumpAmountOfFrames);
            isJumpPrev = true;
        }

        if (isJumpPrev != isJumping && !isMovingHoz)
        {
            SetAnimationCycle(idleStartFrame, idleAmountOfFrames);
            isJumpPrev = false;
        }

        if (isMovingHoz && !isJumping)
        { 
            SetAnimationCycle(runStartFrame, runAmountOfFrames);
        }

        if (isMovingHoz && isJumping)
        {
            SetAnimationCycle(jumpStartFrame, jumpAmountOfFrames);
        }

        if (!isMovingHoz && !isJumping)
        {
            SetAnimationCycle(idleStartFrame, idleAmountOfFrames);
        }

        base.VerticalMovement(hasJumpIntent, thePlayerNumber);
    }

}
