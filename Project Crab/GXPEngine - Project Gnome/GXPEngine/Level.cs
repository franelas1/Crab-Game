using System;
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
    Player player1;
    Player player2;

    //Determine the position the player will be displayed in the game camera
    float boundaryValueX; //Should be width / 2 to display the player at the center of the screen
    float boundaryValueY; //Should be height / 2 to display the player at the center of the screen

    Dictionary<string, TextCanvas> textCanvasListHash = new Dictionary<string, TextCanvas>();
    int pickUpCheckTimer;

    const int PICKUPCHECKTIME = 15;
    const int PLATFORMGENMIN = 1670;

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

            GameData.playerAll = new Player("playerFile.png", 1, 1, 0, 64, 48, -1, 0, 30, false, true);

            //Manually create some of the game objects to make things working
            SpawnObjects(mapData);

            
            foreach (Platform thePlatform in FindObjectsOfType<Platform>())
            {
                GameData.thePlatformList.Add(thePlatform);
            }

            foreach (Player thePlayer in FindObjectsOfType<Player>())
            {
                if (thePlayer.id == "Player1")
                {
                    player1 = thePlayer;
                    player1.setSpeed(GameData.playerSpeed);
                    player1.setJumpHeightAndSpeed(GameData.playerJumpHeightAndSpeed);
                    GameData.player1 = player1;
                }

                if (thePlayer.id == "Player2")
                {
                    player2 = thePlayer;
                    player2.setSpeed(GameData.playerSpeed);
                    player2.setJumpHeightAndSpeed(GameData.playerJumpHeightAndSpeed);
                    GameData.player2 = player2;
                }
            }

            if (player1 != null && player2 != null)
            {
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
            }

            //Adjusting the position if a gameobject is found and add it to game
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
        if (GameData.playerAll != null)
        {
            UseCamera();
        }

        if (player1 != null && player2 != null && !GameData.isMenu)
        {
            GameData.playerAll.x = (player1.x + player2.x) / 2;
            GameData.playerAll.y = (player1.y + player2.y) / 2;
        }

        CheckPlatFormsSpawn(1); //check collision of generated platforms
        CheckPlatforms(1); //check collision of placed platforms
        CheckPlatFormsSpawn(2); //check collision of generated platforms
        CheckPlatforms(2); //check collision of placed platforms

        //Check if player touches a coin, enters a door, and other things. With some cooldown.
        if (Time.time - pickUpCheckTimer >= PICKUPCHECKTIME)
        {
            pickUpCheckTimer = Time.time;
            if (GameData.thePlatformList != null && GameData.playerAll != null)
            {

                if (GameData.platformSpawnAmount > 0)
                {
                    SpawnPlatform(0);
                    GameData.platformSpawnAmount--;
                }
            }
        }

        if (GameData.playerAll != null && GameData.oldPlayerY != -1)
        {
            if (GameData.player1.isJumping && !GameData.playerIsFallingJump1)
            {
                GameData.theBackground.y -= Math.Abs(GameData.player1.y - GameData.oldPlayerY);
            }
            
            if (GameData.oldPlayerY < GameData.player1.y)
            {
                GameData.theBackground.y += Math.Abs(GameData.oldPlayerY - GameData.player1.y);
            }
        }

            /*
            if (GameData.playerAll != null && GameData.oldPlayerY != -1)
            {
                if (GameData.playerAll.isJumping && (!GameData.playerIsFallingJump1 || !GameData.playerIsFallingJump2))
                {
                    GameData.theBackground.y -= Math.Abs(GameData.playerAll.y - GameData.oldPlayerY) / 2;
                }

                if (player1.y > player2.y)
                {
                    if (GameData.oldPlayerY < player1.y)
                    {
                        GameData.theBackground.y += Math.Abs(GameData.oldPlayerY - GameData.playerAll.y) / 2;
                    }
                }

                else
                {
                    if (GameData.oldPlayerY < player2.y)
                    {
                        GameData.theBackground.y += Math.Abs(GameData.oldPlayerY - GameData.playerAll.y) / 2;
                    }
                }

            }
            */
        }


    //Sets the game area player can look. AKA the game camera
    //Can set how far right and down player can see. (left stops at x < 0, top stops at y < 0)
    void UseCamera()
    {
        //handling player moving up
        if (GameData.playerAll.y + y < game.height - boundaryValueY && y < 999999999999)
        {
            y = game.height - boundaryValueY - GameData.playerAll.y;
        }

        //handling player moving down
        if (GameData.playerAll.y + y > boundaryValueY && y > -1 - (game.height * 2) - 100)
        {
            y = boundaryValueY - GameData.playerAll.y;
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


    void CheckPlatforms(int thePlayerNumber)
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

        foreach (Platform thePlatform in GameData.thePlatformList)
        {
            if (thePlayer != null)
            {
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
            }
        }     
    }

    void CheckPlatFormsSpawn(int thePlayerNumber)
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
                            GameData.detectSpawn1 = true;
                            GameData.thePlatformSpawn1 = thePlatform;
                            thePlayer.x += thePlatform.width / 2;
                        }

                        else
                        {
                            GameData.detectSpawn2 = true;
                            GameData.thePlatformSpawn2 = thePlatform;
                            thePlayer.x += thePlatform.width / 2;
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

    //thePlatformListSpawned
    void SpawnPlatform(int theType)
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

            GameData.deathY -= theYCrood;
            GameData.deathYPlayer -= theYCrood;
 
            ySapwnValue -= theYCrood;
            theSpawnPlatform.x = theXCrood;
            theSpawnPlatform.y = ySapwnValue;
            theSpawnPlatform.theNumber = GameData.theNumberSpawn;
            GameData.thePlatformListSpawned.Add(theSpawnPlatform);

            AddChild(theSpawnPlatform);
        }
    }
}