using System;
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
    public static float playerSpeed = 0.5f;
    public static float playerJumpHeightAndSpeed = 10;

    public static int playerMaxHealth = 3;
    public static int playerHealth = 3;

    public static bool isMenu = true;

    public static int[] jumpUpgradeList = new int[] { 1, 1, 1, 1, 1 };
    public static int[] speedUpgradeList = new int[] { 1, 1, 1, 1, 1 };

    public static Platform thePlatform;
    public static Platform thePlatformSpawn;
    public static Platform thePlatformSpawnOld = null;
    public static List<Platform> thePlatformList = new List<Platform>();
    public static List<Platform> thePlatformListSpawned = new List<Platform>();
    public static float playerPlatormColliderValue = 999;

    public static Boolean playerIsFallingJump = false;

    public static Boolean detectSpawn = false;

    public static void CheckPlat()
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
                    }
                }
            }
        }
    }

    public static void CheckPlatSpawned()
    {
        foreach (Platform thePlatform in GameData.thePlatformListSpawned)
        {
            thePlayer.x -= thePlatform.width / 2;
            if (thePlayer != null)
            {
                if (CustomUtil.IntersectsSpriteCustomAndAnimationSpriteCustom(thePlatform, thePlayer))
                {
                    thePlayer.x += thePlatform.width / 2;
                    GameData.thePlatformSpawn = thePlatform;
                    detectSpawn = true;
                    GameData.playerPlatormColliderValue = thePlayer.collider.GetCollisionInfo(thePlatform.collider).normal.x;
                }

                else
                {
                    thePlayer.x += thePlatform.width / 2;
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
        thePlatformListSpawned.Clear();
        theLevel = null;
        playerHealth = playerMaxHealth;
        playerDead = false;
        thePlayer = null;
    }

}
