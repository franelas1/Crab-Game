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
    public static Platform thePlatformSpawnOld;
    public static List<Platform> thePlatformList = new List<Platform>();
    public static List<Platform> thePlatformListSpawned = new List<Platform>();
    public static float playerPlatormColliderValue = 999;

    public static Boolean playerIsFallingJump = false;

    public static Boolean detectSpawn = false;

    public static int theNumberReached = 0;
    public static int theNumberSpawn = 0;

    public static Pivot theBackground;


    public static int deathY = 2374 + (80 * 7);
    public static int deathYPlayer = 2034 + (80 * 7);

    public static int platformSpawnAmount = 7;

    public static float oldPlayerY = -1;
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

    

                    if (GameData.thePlatformSpawn.theNumber > GameData.theNumberReached)
                    {
                        
                        platformSpawnAmount = thePlatformSpawn.theNumber - theNumberReached;
                     //   Console.WriteLine(thePlatformSpawn);
                        GameData.theNumberReached = GameData.thePlatformSpawn.theNumber;
                        
                    }
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
        oldPlayerY = 0;
        deathYPlayer = 1934 + (80 * 7);
        deathY = 2274 + (80 * 7);
        thePlatformList.Clear();
        thePlatformListSpawned.Clear();
        thePlatformSpawn = null;
        thePlatform = null;
        theLevel = null;
        playerHealth = playerMaxHealth;
        playerDead = false;
        thePlayer = null;
        theNumberReached = 0;
        theNumberSpawn = 0;
        detectSpawn = false;
        platformSpawnAmount = 7;
        theBackground = null;
        oldPlayerY = -1;
    }

}
