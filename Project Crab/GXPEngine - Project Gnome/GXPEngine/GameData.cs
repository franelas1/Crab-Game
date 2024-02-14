﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

/*
 * Holds all kind of data all other classes can access and edit
 * thid holds data
 */
public static class GameData
{
    public static string lastLevelPlayed = "map1.tmx"; //determine what level will be loaded
    public static string theLevelName = "mainMenu.tmx"; //determine what level will be loaded
    public static Level theLevel; //the level object itself
    public static bool playerDead = false;

    public static int levelCleared = 0;
    public static int theFPS;

    public static Player thePlayer;
    public static float playerSpeed = 4;
    public static float playerJumpHeightAndSpeed = 14;

    public static int playerMaxHealth = 3;
    public static int playerHealth = 3;

    public static bool isMenu = true;

    public static int bossHealth = 4;
    
    public static Platform thePlatform = null;
    public static List<Platform> thePlatformList = new List<Platform>();
    public static float playerPlatormColliderValue = 999;

    public static Boolean playerIsFallingJump = false;

    public static void checkPlat()
    {
        foreach (Platform thePlatform in GameData.thePlatformList)
        {
            if (thePlayer != null)
            {
                if (CustomUtil.IntersectsSpriteCustomAndAnimationSpriteCustom(thePlatform, thePlayer))
                {
                    if (thePlatform.collider != null)
                    {
                        GameData.thePlatform = thePlatform;
                        GameData.playerPlatormColliderValue = thePlayer.collider.GetCollisionInfo(thePlatform.collider).normal.x;
                        Console.WriteLine(GameData.playerPlatormColliderValue);
                    }


                }
            }

        }
    }
    public static void CheckNewLevelCleared()
    {
        if (levelCleared == 0 && theLevelName == "map1.tmx")
        {
            levelCleared++;
        }

        if (levelCleared == 1 && theLevelName == "map2.tmx")
        {
            levelCleared++;
        }
    }

    public static void ResetLevelData()
    {
        bossHealth = 6;
       thePlatformList.Clear();
        theLevel = null;
        playerHealth = playerMaxHealth;
        playerDead = false;
        thePlayer = null;
    }

}
