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

    public static Player playerAll;
    public static Player player1;
    public static Player player2;
    public static Player thePlayer;
    public static float playerSpeed = 2f;
    public static float playerJumpHeightAndSpeed = 10;

    public static int playerMaxHealth = 3;
    public static int playerHealth = 3;

    public static bool isMenu = true;

    public static int[] jumpUpgradeList = new int[] { 1, 1, 1, 1, 1 };
    public static int[] speedUpgradeList = new int[] { 1, 1, 1, 1, 1 };

    public static Platform thePlatform1;
    public static Platform thePlatformSpawn1;
    public static Platform thePlatform2;
    public static Platform thePlatformSpawn2;

    public static Platform thePlatformSpawnOld1;
    public static Platform thePlatformSpawnOld2;
    public static List<Platform> thePlatformList = new List<Platform>();
    public static List<Platform> thePlatformListSpawned = new List<Platform>(); 

    public static Boolean playerIsFallingJump1 = false;
    public static Boolean playerIsFallingJump2 = false;

    public static Boolean detectSpawn1 = false;
    public static Boolean detectSpawn2 = false;

    public static int theNumberReached = 0;
    public static int theNumberSpawn = 0;

    public static Pivot theBackground;


    public static int deathY = 2474 + (80 * 7);
    public static int deathYPlayer = 2374 + (80 * 7);

    public static int platformSpawnAmount = 7;

    public static float oldPlayerY = -1;
    public static void CheckPlat(int thePlayerNumber)
    {
        Player thePlayer = null;

        if (thePlayerNumber == 1)
        {
            thePlayer = GameData.player1;
        }

        else
        {
            thePlayer = GameData.player2;
        }

        foreach (Platform thePlatform in GameData.thePlatformListSpawned)
        {
            thePlayer.x -= thePlatform.width / 2;
            if (thePlayer != null)
            {
                // && thePlayer.collider.GetCollisionInfo(thePlatform.collider) != null
                if (CustomUtil.IntersectsSpriteCustomAndAnimationSpriteCustom(thePlatform, thePlayer))
                {
                    if (thePlatform.collider != null)
                    {
                        if (thePlayerNumber == 1)
                        {
                            GameData.thePlatform1 = thePlatform;
                        }

                        else
                        {
                            GameData.thePlatform2 = thePlatform;
                        }
                    }
                }

                else
                {
                    thePlayer.x += thePlatform.width / 2;
                }
            }
        }
    }

    public static void CheckPlatSpawned(int thePlayerNumber)
    {
        Player thePlayer = null;

        if (thePlayerNumber == 1)
        {
            thePlayer = GameData.player1;
        }

        else
        {
            thePlayer = GameData.player2;
        }

        foreach (Platform thePlatform in GameData.thePlatformListSpawned)
        {
            thePlayer.x -= thePlatform.width / 2;
            if (thePlayer != null)
            {
                // && thePlayer.collider.GetCollisionInfo(thePlatform.collider) != null
                if (CustomUtil.IntersectsSpriteCustomAndAnimationSpriteCustom(thePlatform, thePlayer))
                {
                    if (thePlatform.collider != null)
                    {
                        if (thePlayerNumber == 1)
                        {
                            GameData.thePlatformSpawn1 = thePlatform;
                            thePlayer.x += thePlatform.width / 2;
                            GameData.detectSpawn1 = true;
                        }

                        else
                        {
                            GameData.thePlatformSpawn2 = thePlatform;
                            thePlayer.x += thePlatform.width / 2;
                            GameData.detectSpawn2 = true;
                        }
                    }

                    else
                    {
                        thePlayer.x += thePlatform.width / 2;
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
        deathYPlayer = 2374 + (80 * 7);
        deathY = 2474 + (80 * 7);
        thePlatformList.Clear();
        thePlatformListSpawned.Clear();
        thePlatformSpawn1 = null;
        thePlatform1 = null;
        thePlatformSpawn2 = null;
        thePlatform2 = null;
        theLevel = null;
        playerHealth = playerMaxHealth;
        playerDead = false;
        player1 = null;
        player2 = null;
        playerAll = null;
        theNumberReached = 0;
        theNumberSpawn = 0;
        detectSpawn1 = false;
        detectSpawn2 = false;
        platformSpawnAmount = 7;
        theBackground = null;
        oldPlayerY = -1;
    }

}
