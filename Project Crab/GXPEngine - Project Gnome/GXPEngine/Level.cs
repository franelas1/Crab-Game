﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Messaging;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

/*
 * The level Gameobject. For generating the game itself as a menu screen or a game level
 * The game should only have one level
 */
public class Level : GameObject
{
    TiledLoader loader;
    Player thePlayer;

    //Determine the position the player will be displayed in the game camera
    float boundaryValueX; //Should be width / 2 to display the player at the center of the screen
    float boundaryValueY; //Should be height / 2 to display the player at the center of the screen

    Dictionary<string, TextCanvas> textCanvasListHash = new Dictionary<string, TextCanvas>();
    int pickUpCheckTimer;

    //88
    const int PICKUPCHECKTIME = 15;
    const int PLATFORMGENMIN = 1670;
    const int PLATFORMGENMAX = 1054;

    List<TriggerAction> triggerActionList = new List<TriggerAction>();

    int ySapwnValue = PLATFORMGENMIN;

    Pivot theWallsAndBackgroud = new Pivot();

    public Level(string theMapfileName)
    {
        GameData.theBackground = theWallsAndBackgroud;
        AddChild(theWallsAndBackgroud);
        Map mapData = MapParser.ReadMap(theMapfileName);
        GameData.theLevel = this;
        loader = new TiledLoader(theMapfileName);

        //If the game lvel is a menu
        if (GameData.isMenu)
        {
            //Automatically generates the game objects
            loader.autoInstance = true;
            loader.rootObject = this;
            loader.LoadImageLayers();
            loader.LoadObjectGroups(0);
            loader.LoadObjectGroups(1);
            loader.LoadObjectGroups(2);
            //Manually generates some of the game objects
            SpawnObjects(mapData);
        }

        //If the game level is not a menu
        else
        {
            //Manually generates the tile layers.
            CreateTile(mapData, 0); //Create the walls
            CreateTile(mapData, 1); //Create background
            CreateTile(mapData, 2); //Create traps
            //Automatically generates the game objects
            loader.autoInstance = true;
            loader.rootObject = this;
            loader.LoadImageLayers();
            loader.LoadObjectGroups(0);

            //A game level should have a player, so find the player and allowing the program to reference it via gamedata
            thePlayer = FindObjectOfType<Player>();
            GameData.thePlayer = thePlayer;
            thePlayer.setSpeed(GameData.playerSpeed);
            thePlayer.setJumpHeightAndSpeed(GameData.playerJumpHeightAndSpeed);

            //Manually create some of the game objects to make things working
            SpawnObjects(mapData);

            
            foreach (Platform thePlatform in FindObjectsOfType<Platform>())
            {
                GameData.thePlatformList.Add(thePlatform);
            }
            
        }

        //Setting up the camera boundary (player at center for these values)
        boundaryValueX = game.width / 2;
        boundaryValueY = game.height / 2;

    }

    //Some game objects can't be automacally generate (sometime program needs their reference), so
    //manually generate some of the gameobjects. 
    //If is a menu level, extract layer 3, if is not a menu level, extract layer 1
    void SpawnObjects(Map mapData)
    {
        //Check data is not empty
        if (mapData.ObjectGroups == null || mapData.ObjectGroups.Length == 0)
        {
            return;
        }

        ObjectGroup objectGroup; //Holding the list of gameobject we will extract

        //A menu level has a different layer for the gameobjects that need to be manually generated
        if (GameData.isMenu)
        {
            objectGroup = mapData.ObjectGroups[3];
        }

        else
        {
            objectGroup = mapData.ObjectGroups[1];
        }

        //Check if the selected layer has objects
        if (objectGroup.Objects == null || objectGroup.Objects.Length == 0)
        {
            return;
        }

        //Extracting the game objects layer
        foreach (TiledObject theObject in objectGroup.Objects)
        {
            Sprite nextGameObject = null;
            switch (theObject.Name)
            {
                //Detect is it's a text canvas. A text display
                case "TextCanvas":
                    //Extracting properties from what's exported from tiled
                    TextCanvas theTextCanvas = new TextCanvas(theObject.GetStringProperty("theTextContent", ""),
                        theObject.GetStringProperty("theTextFont", ""), theObject.GetIntProperty("theTextSize", 0),
                         theObject.GetIntProperty("theCanvasWidth", 0), theObject.GetIntProperty("theCanvasHeight", 0),
                         theObject.GetIntProperty("theTextColorR", 0), theObject.GetIntProperty("theTextColorG", 0),
                         theObject.GetIntProperty("theTextColorB", 0), theObject.GetBoolProperty("transparentBackground", false));

                    theTextCanvas.visible = theObject.GetBoolProperty("visibleAtStart", false);

                    //Detecting if the text canvas is one of the specific text canvases
/*                    switch (theObject.GetStringProperty("f_displayID", ""))
                    {

                    }*/

                    textCanvasListHash.Add(theObject.GetStringProperty("theTextCanvasID", ""), theTextCanvas);
                    nextGameObject = theTextCanvas;
                    break;
            }

            //Adjusting the position if a gameobject is found and add it to game
            if (nextGameObject != null)
            {
                nextGameObject.x = theObject.X;
                nextGameObject.y = theObject.Y;
                AddChild(nextGameObject);
            }
        }

        /* 
         * Some gameobjects must be loaded after some other gameobject to make things work properly,
         * so repeat the extraction process again for some gameobjects
         */

        foreach (TiledObject theObject in objectGroup.Objects)
        {
            Sprite nextGameObject = null;
            switch (theObject.Name)
            {
                //Trigger region
                case "RegionActivateAtOnce":

                    RegionActivateAtOnce theRegion = new RegionActivateAtOnce(thePlayer, theObject.GetBoolProperty("triggerAction1Parameter1Bool", false),
                        theObject.GetIntProperty("i_scaleX", 1), theObject.GetIntProperty("i_scaleY", 1), theObject.GetIntProperty("f_amountOfTimes", -1));

                    switch (theObject.GetStringProperty("triggerActionName1", ""))
                    {
                        case "enableDisableGameObjectRender":
                            theRegion.AddTriggerAction(new TriggerActionVisbility(textCanvasListHash[theObject.GetStringProperty("triggerAction1Parameter2String", "")],
                                false));
                            break;
                    }

                    nextGameObject = theRegion;
                    break;
            }

            if (nextGameObject != null)
            {
                nextGameObject.x = theObject.X;
                nextGameObject.y = theObject.Y;
                AddChild(nextGameObject);
            }
        }
    }

    void Update()
    {
        //Use camera if player is found
        if (thePlayer != null)
        {
            UseCamera();
        }

        CheckTriggerAction();

        CheckPlatFormsSpawn();
        CheckPlatforms();

        //Check if player touches a coin, enters a door, and other things. With some cooldown.
        if (Time.time - pickUpCheckTimer >= PICKUPCHECKTIME)
        {
            pickUpCheckTimer = Time.time;
            if (GameData.thePlatformList != null && GameData.thePlayer != null)
            {

                if (GameData.platformSpawnAmount > 0)
                {
                    spawnPlatform(0);
                    GameData.platformSpawnAmount--;
                }
            }
        }

        if (thePlayer != null)
        {
            if (thePlayer.isJumping && !GameData.playerIsFallingJump)
            {
                GameData.theBackground.y -= Math.Abs(thePlayer.y - GameData.oldPlayerY);
            }

            if (thePlayer.trueJumpFalling)
            {
                GameData.theBackground.y += Math.Abs(GameData.oldPlayerY - thePlayer.y);
            }
        }
        
    }


    //Sets the game area player can look. AKA the game camera
    //Can set how far right and down player can see. (left stops at x < 0, top stops at y < 0)
    void UseCamera()
    {

        //first determine if the camera moves, then determine the max distance the camera can move
        //handling player moving right
        if (thePlayer.x + x > boundaryValueX && x > -1 * ((game.width * 6) - 800))
        {
            x = boundaryValueX - thePlayer.x;
        }

        //handling player moving left
        if (thePlayer.x + x < game.width - boundaryValueX && x < 0)
        {
            x = game.width - boundaryValueX - thePlayer.x;
        }

        //handling player moving up
        if (thePlayer.y + y < game.height - boundaryValueY && y < 999999999999)
        {
            y = game.height - boundaryValueY - thePlayer.y;
        }

        //handling player moving down
        if (thePlayer.y + y > boundaryValueY && y > -1 - (game.height * 2) - 100)
        {
            y = boundaryValueY - thePlayer.y;
        }
    }


    //Spawns all the Tiles of the level
    void CreateTile(Map mapData, int theLayer)
    {
        //Check if map data is not empty
        if (mapData.Layers == null || mapData.Layers.Length == 0)
        {
            return;
        }
        Layer layer = mapData.Layers[theLayer];


        //Helps to render all the background (layer 1) sprites in one call.
        SpriteBatch background = new SpriteBatch();
        theWallsAndBackgroud.AddChild(background);

        short[,] tileNumbers = layer.GetTileArray(); //holding the tile data

        //Generating the tiles depends on the map data.
        //Extracting the 2d array extracted from the map data. Each number represent a specific tile
        for (int i = 0; i < layer.Height; i++)
        {
            for (int j = 0; j < layer.Width; j++)
            {
                int theTileNumber = tileNumbers[j, i]; //extracting the tile number
                TileSet theTilesSet = mapData.GetTileSet(theTileNumber); //what tile set the number comes from

                //A number with value 0 means no tile, so ignore number 0
                if (theTileNumber != 0)
                {
                    Tile theTile; //the tile object to be added to the game level

                    /*
                     * Determining what type of tile the tile object will be based on the tile layer.
                     * Layer 0 represents wall tiles, layer 1 represents background tiles, layer 2 represent wall tiles that player will die if touched
                     */

                    // A wall tile. isDeadly == false. collision on
                    if (theLayer == 0)
                    {
                        theTile = new Tile(false, theTilesSet.Image.FileName, 1, 1, theTileNumber - theTilesSet.FirstGId,
                            theTilesSet.Columns, theTilesSet.Rows, -1, 1, 10, false, true);
                        theTile.x = j * theTile.width;
                        theTile.y = i * theTile.height;
                        theWallsAndBackgroud.AddChild(theTile);

                    }

                    //Background. isDeadly == false. collision off
                    else if (theLayer == 1)
                    {
                        theTile = new Tile(false, theTilesSet.Image.FileName, 1, 1, theTileNumber - theTilesSet.FirstGId,
                            theTilesSet.Columns, theTilesSet.Rows, -1, 1, 10, false, false);
                        theTile.x = j * theTile.width;
                        theTile.y = i * theTile.height;
                        background.AddChild(theTile);
                    }


                    //A wall tile with isDeadly == false. collision on
                    else if (theLayer == 2)
                    {
                        theTile = new Tile(true, theTilesSet.Image.FileName, 1, 1, theTileNumber - theTilesSet.FirstGId,
                            theTilesSet.Columns, theTilesSet.Rows, -1, 1, 10, false, true);
                        theTile.SetFrame(theTileNumber - theTilesSet.FirstGId);
                        theTile.x = j * theTile.width;
                        theTile.y = i * theTile.height;
                        theWallsAndBackgroud.AddChild(theTile);
                    }
                }
            }
        }
        background.Freeze(); //Freeze all the background tiles by destroying the sprite and their collider. Creating better performance
    }


    void CheckPlatforms()
    {   
        foreach (Platform thePlatform in GameData.thePlatformList)
        {
            if (thePlayer != null)
            {
                if (CustomUtil.IntersectsSpriteCustomAndAnimationSpriteCustom(thePlatform, thePlayer))
                {
                    if (thePlatform.collider != null)
                    {
                        GameData.thePlatformSpawn = thePlatform;
                        GameData.playerPlatormColliderValue = thePlayer.collider.GetCollisionInfo(thePlatform.collider).normal.x;
                    }
                }
            }
        }     
    }

    void CheckPlatFormsSpawn()
    {
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
                        thePlayer.x += thePlatform.width / 2;
                        GameData.thePlatformSpawn = thePlatform;
                        GameData.detectSpawn = true;
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

    //thePlatformListSpawned
    void spawnPlatform(int theType)
    {
        //left:   98 + platform width 
        //right:  704 - platfrom width
        Platform theSpawnPlatform = null;

        float theScale = Utils.Random(1, 8);

        foreach (Platform thePlatform in GameData.thePlatformList)
        {
            if (thePlatform.theType == theType)
            {
                theSpawnPlatform = new Platform(thePlatform.theFilename, 1, 1, 0 , 64, 48, -1, 0, 30, false, true);
                theSpawnPlatform.changeFrame(thePlatform.singleFrameID);
                theSpawnPlatform.changeScaleX(theScale);
                break;
            }
        }

        if (theSpawnPlatform != null)
        {

            int theXCrood = (int) Utils.Random(98 + (theSpawnPlatform.width), (704 - (theSpawnPlatform.width) + 1));
            int theYCrood;
            GameData.theNumberSpawn++;
            if (GameData.theNumberSpawn == 1)
            {
                theYCrood = 10;
            }

            else
            {
                theYCrood = Utils.Random(30, 88);
            }

            GameData.deathY -= theYCrood / 2;
            GameData.deathYPlayer -= theYCrood / 2;

            ySapwnValue -= theYCrood;
            theSpawnPlatform.x = theXCrood;
            theSpawnPlatform.y = ySapwnValue;
            theSpawnPlatform.theNumber = GameData.theNumberSpawn;
            GameData.thePlatformListSpawned.Add(theSpawnPlatform);

            AddChild(theSpawnPlatform);
        }
    }


    void CheckTriggerAction()
    {
        foreach (TriggerAction theTriggerAction in triggerActionList)
        {
            if (theTriggerAction.triggerActionID == "TriggerActionVisbilityTimed")
            {
                theTriggerAction.Action();
                if (theTriggerAction.finished)
                {
                    triggerActionList.Remove(theTriggerAction);
                    break;
                }
            }
        }
    }
}