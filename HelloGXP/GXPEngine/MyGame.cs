using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;
using System.Drawing.Printing;

public class MyGame : Game
{
    const int STARTERPLATFORMS = 7;
    const int PLATFORMSPAWNAMOUNT = 15;
    // Declare variables:
    Player player1, player2;


    //control crood, rotation, render order, etc of certain object groups
    Pivot pivotAll;
    Pivot playerPivot;
    Pivot backgroundPivot;

    float platformYSpawnValue;


    int restartTimer;

    AnimationSprite water;
    TextCanvas winScreenText;
    int theSpawnNumber;

    int abilityPickupTimer = Time.time;
    int abilityPickupTime;

    Sprite theBackgroundEndOrStart;

    bool inMainMenu = true;

    public MyGame() : base(1366, 768, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
     //   ResetGame();
    }

    void ResetGame()
    {
        Gamedata.ResetData();

        abilityPickupTimer = Time.time;
        abilityPickupTime = 0;



        winScreenText = null;
        theSpawnNumber = 0;

        platformYSpawnValue = 300;

        pivotAll = new Pivot();
        playerPivot = new Pivot();
        backgroundPivot = new Pivot();


        //destroy all gameobjects
        List<GameObject> children = GetChildren();
        foreach (GameObject child in children)
        {
            child.LateDestroy();
        }

        AddChild(pivotAll);
        pivotAll.AddChild(backgroundPivot);
        pivotAll.AddChild(playerPivot);

        water = new AnimationSprite("full-bg.png", 3, 3);
        water.SetCycle(0, 7, 5);
        water.scaleX = 1.7075f;
        water.scaleY = 1.28f;
        backgroundPivot.AddChild(water);

        //Spawning and adding players

        //0.35, 0.5

        //player 1
        //jumping: 0 - 6 frame (cycle: 0, 7, 10)
        //idle: frame 1 (cycle: 1, 1, 60)
        //walk: (cycle: 9, 2, 10)
        player1 = new Player(1, width / 2 - 200, height - 120, 0.35f, 0.5f, 140, "player1.png", 4, 3, -1, 1, 1, 60, 0, 7, 10, 9, 2, 10);
        Gamedata.player1 = player1;
        playerPivot.AddChild(player1);


        player2 = new Player(2, width / 2 + 200, height - 120, 0.35f, 0.39f, 140, "player2.png", 4, 4, -1, 5, 1, 60, 0, 5, 10, 9, 3, 10);
        Gamedata.player2 = player2;
        playerPivot.AddChild(player2);
        platformYSpawnValue = height - 150;

        //starter platforms
        Platform spawnPlatform1 = new Platform(width / 2, height - 105, "plat_eggplant.png", 8f);
        Gamedata.platforms.Add(spawnPlatform1);
        AddChild(spawnPlatform1);

        //Platform spawnPlatform2 = new Platform(width / 2 - 200, height - 105, "square.png", 1.5f);
        //Gamedata.platforms.Add(spawnPlatform2);
        //AddChild(spawnPlatform2);
    }

    void Update()
    {
        if (Gamedata.restartStage == -1)
        {
            theBackgroundEndOrStart = null;
            theBackgroundEndOrStart = new Sprite("menu_start.png");
            AddChild(theBackgroundEndOrStart);
            Gamedata.restartStage = 0;
   
        }

        if (Gamedata.restartStage == 0)
        {
            if (inMainMenu == false)
            {
                ResetGame();
                return;
            }

            if (Input.GetKeyDown('G'))
            {
                Gamedata.restartStage = 1;
                inMainMenu = false;
                ResetGame();
                
            }

            return;
        }

        if (Gamedata.restartStage == 2)
        {
            List<GameObject> children = GetChildren();
            foreach (GameObject child in children)
            {
                child.LateDestroy();
            }

            player1 = null;
            player2 = null;
            pivotAll = null;
            backgroundPivot = null;
            playerPivot = null;

            restartTimer = Time.time;
            Gamedata.restartStage = 3;
            winScreenText = new TextCanvas("Player X win", "SwanseaBold-D0ox.ttf", 20, 200, 200, 255, 255, 255, false);
            winScreenText.SetPoint((width / 2) - 100, (height / 2) - 100);

            if (Gamedata.playerWin == 1)
            {
                theBackgroundEndOrStart = null;
                theBackgroundEndOrStart = new Sprite("end_screen_crab_wins.png");
                AddChild(theBackgroundEndOrStart);
            }

            else
            {
                theBackgroundEndOrStart = null;
                theBackgroundEndOrStart = new Sprite("end_screen_lobster_win.png");
                AddChild(theBackgroundEndOrStart);
            }
        }

        if (Gamedata.restartStage == 3)
        {
            Console.Clear();
            //   Console.WriteLine("restarting");
            if (Time.time - restartTimer >= 3000)
            {
                ResetGame();
                Gamedata.restartStage = 0;
            }
        }

        if (water == null || player1 == null || player2 == null)
        {
            return;
        }

        water.Animate(1);

        //detect if  player touches a pickup, if touches and the player already has the same effect of the ability of the pickup, the player would not get the pickup
        foreach (Pickup thePickupUp in Gamedata.pickupList)
        {
            if (CustomUtil.hasIntersectionSprites(thePickupUp, player1))
            {
                bool theAbilityNotRepeat = true;

                foreach (Ability theAbility in player1.theAbilities)
                {
                    if (theAbility.theAbility == thePickupUp.theAbility.theAbility)
                    {
                        theAbilityNotRepeat = false;
                    }
                }

                if (theAbilityNotRepeat)
                {
                    if (player1.theAbilities.Count == 0)
                    {
                        player1.theAbilities.Add(thePickupUp.theAbility);
                        thePickupUp.gotPicked = true;
                    }
                }
            }


            if (CustomUtil.hasIntersectionSprites(thePickupUp, player2))
            {
                bool theAbilityNotRepeat = true;

                foreach (Ability theAbility in player2.theAbilities)
                {
                    if (theAbility.theAbility == thePickupUp.theAbility.theAbility)
                    {
                        theAbilityNotRepeat = false;
                    }
                }

                if (theAbilityNotRepeat)
                {
                    if (player2.theAbilities.Count == 0)
                    {
                        player2.theAbilities.Add(thePickupUp.theAbility);
                        thePickupUp.gotPicked = true;
                    }
                }
            }                   
        }

        if (Gamedata.restartStage != 2 && Gamedata.restartStage != 3)
        {
            //Updating player movement with human imput
            player1.updatePlayer();
            player2.updatePlayer();
        }

        Gamedata.detectPlatformPlayer1 = false;

        if ((Gamedata.platforms.Count < PLATFORMSPAWNAMOUNT && Gamedata.playerMoved == true) || Gamedata.platforms.Count < STARTERPLATFORMS)
        {
        //    Console.WriteLine("producing");
            SpawnPlatform();


        }
    }

    // Main is the first method that's called when the program is run
    static void Main()
    {
        // Create a "MyGame" and start it:
        new MyGame().Start();
    }
    void SpawnPlatform()
    {
        theSpawnNumber++;

        
        if (platformYSpawnValue > 52 * (PLATFORMSPAWNAMOUNT - STARTERPLATFORMS) * 2)
        {
            platformYSpawnValue = -100;
        }
        

        //(int) Utils.Random(10, 10) + 5 * theSpawnNumber;
        float theYCrood;
        int theMargin; // (int) Utils.Random(50, 100);
        float theYScale;


   //     Console.WriteLine(platformYSpawnValue);
        String theImage;
        float theXScale;
        int thePlatform = (int) Utils.Random(6, 7);

        int detectionValue = 10;
        int heightAdjustPlayer1 = 6;
        int heightAdjustPlayer2 = 6;

    //    Console.WriteLine(thePlatform);

        if (thePlatform == 1)
        {
            theYCrood = Utils.Random(125, 126);
            theMargin = 100;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_onion.png";
            theYScale = 1f;
            detectionValue = 20;
            heightAdjustPlayer1 = 8;
            heightAdjustPlayer2 = 10;
        }

        else if (thePlatform == 2)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 100;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_broccoli.png";
            theYScale = 0.4f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6;
            heightAdjustPlayer2 = 6;
        }

        else if (thePlatform == 3)
        {
            theYCrood = Utils.Random(100, 100);
            theMargin = 100;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_cheese.png";
            theYScale = 0.5f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6;
            heightAdjustPlayer2 = 7;
        }

        else if (thePlatform == 4)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 100;
            theXScale = Utils.Random(0.33f, 0.7f);
            theImage = "plat_corn.png";
            theYScale = 0.33f;
            detectionValue = 20;
            heightAdjustPlayer1 = 4;
            heightAdjustPlayer2 = 5;
        }


        else if (thePlatform == 5)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 100;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_carrot.png";
            theYScale = 0.4f;
            detectionValue = 20;
            heightAdjustPlayer1 = 4;
            heightAdjustPlayer2 = 2;
        }

        else
        {
            theYCrood = Utils.Random(125, 125);
            theMargin = 100;
            theXScale = Utils.Random(1f, 1.7f);
            theImage = "plat_eggplant.png";
            theYScale = 0.5f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6;
            heightAdjustPlayer2 = 3;
            //Math.Abs(Gamedata.currentPlayer1Platform.y - (Gamedata.currentPlayer1Platform.height / 2)
            //-(y - height / 2))
        }

        /*

        else if (theXScale >= 1.5f && theXScale < 2f)
        {
            theImage = "eggplantTest.png";
        }

        else if (theXScale >= 2f && theXScale < 2.5f)
        {
            theImage = "eggplantTest.png";
        }
        */

        if (theSpawnNumber > STARTERPLATFORMS * 2)
        {
            theYCrood = theYCrood / 2.5f;
        }


        platformYSpawnValue -= theYCrood;

        /*
        if (platformYSpawnValue < -100)
        {
            platformYSpawnValue = -100;
        }

        if (theSpawnNumber > PLATFORMSPAWNAMOUNT)
        {
            platformYSpawnValue = -100 - theYCrood;
        }
        */

        float platformSpeed = 1.5f * Math.Max(1f,(theSpawnNumber / 10));

        Platform theSpawnPlatform = new Platform(theImage, platformYSpawnValue, theMargin, theXScale, theYScale, ((int)platformSpeed),
             detectionValue, heightAdjustPlayer1, heightAdjustPlayer2);
        Gamedata.platforms.Add(theSpawnPlatform);

        if (theSpawnNumber == 1 || Time.time - abilityPickupTimer >= abilityPickupTime)
        {
            //ability_chiliPepperPiece //10000
            //ability_basilLeaf //15000
            //ability_gralicPiece //10000

            abilityPickupTimer = Time.time;
            abilityPickupTime = Utils.Random(10000, 20001);

            int theAbilityNum = Utils.Random(1, 5);
            Pickup thePickup;

            theMargin = 100;
            int theAbilityWidth = 64;
            int theAbilityHeight = 64;
            

            if (theAbilityNum == 1)
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth), -theAbilityHeight, "pepper.png", "ability_chiliPepperPiece", 10000);
                thePickup.scale = 0.22f;
            }
            
            else if (theAbilityNum == 2)
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth), -theAbilityHeight, "basil_leave.png", "ability_basilLeaf", 15000);
                thePickup.scale = 0.22f;
            }

            else if (theAbilityNum == 3)
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth), -theAbilityHeight, "lavender.png", "ability_lavenderFlower", 10000);
                thePickup.scale = 0.22f;
            }

            else
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth), -theAbilityHeight, "garlic.png", "ability_gralicPiece", 10000);
                thePickup.scale = 0.22f;
            }

            AddChild(thePickup);
            Gamedata.pickupList.Add(thePickup);
        }

        AddChild(theSpawnPlatform);
    }
}