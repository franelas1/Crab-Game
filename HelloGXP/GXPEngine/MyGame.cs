using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;
using System.Drawing.Printing;
using System.Threading;

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
    SerialPort port = new SerialPort();

    bool inMainMenu = true;

    string message;
    string[] data;

    SoundChannel deathSound;
    SoundChannel menuSound;
    SoundChannel gameSound;


    bool deathSoound;
    bool gameASound;
    public MyGame() : base(1366, 768, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
          ResetGame();
        OpenConnection(port, "COM11");
    }

    void OpenConnection(SerialPort portTemp, string portNumber)
    {
        //Arduino input stuff 
        portTemp.PortName = portNumber;
        portTemp.BaudRate = 9600;
        portTemp.RtsEnable = true;
        portTemp.DtrEnable = true;
        portTemp.Open();
    }

    void ResetGame()
    {
        Gamedata.ResetData();

        abilityPickupTimer = Time.time;
        abilityPickupTime = 0;

        deathSoound = true;
        gameASound = true;

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
   //     Console.WriteLine(player1);


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

        if (Gamedata.restartStage == 1 && gameASound)
        {
            gameSound = null;
            gameSound = new Sound("ENV_Boil-Loop_001.wav", true, false).Play();
            gameASound = false;
        }
        
        //if ((Time.time % 1000) / 60 == 0)
        ReadArduinoInput(port);

        if (Gamedata.restartStage == -1)
        {
            theBackgroundEndOrStart = null;
            theBackgroundEndOrStart = new Sprite("menu_start.png");
            AddChild(theBackgroundEndOrStart);
            Gamedata.restartStage = 0;
            
            menuSound = new Sound("MUSIC_MainMenu_001.wav", true, false).Play(); //this is the level sound, looping
            menuSound.Volume = 0.8f;

        }

        if (Gamedata.restartStage == 0)
        {
            Console.WriteLine("start");

            
            if (inMainMenu == false)
            {
                menuSound.Stop();
                ResetGame();
                Gamedata.restartStage = 1;
                return;
            }

            if (Input.GetKeyDown('G'))
            {
                menuSound.Stop();
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
            if (gameSound != null)
            {
                gameSound.Stop();
            }
        
            if (deathSoound)
            {
                int theDeathSound = Utils.Random(1, 4);
                if (theDeathSound == 1)
                {
                    SoundChannel theSound = new Sound("SFX_Death_001.wav", false, false).Play();
                }

                else if (theDeathSound == 2)
                {
                    SoundChannel theSound = new Sound("SFX_Death_002.wav", false, false).Play();
                }

                else
                {
                    SoundChannel theSound = new Sound("SFX_Death_003.wav", false, false).Play();
                }

                deathSoound = false;
            }



            //   Console.WriteLine("restarting");
            if (Time.time - restartTimer >= 7000)
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
                        SoundChannel theSound = new Sound("SFX_Pick-up_001_Crab.wav", false, false).Play();
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
                        SoundChannel theSound = new Sound("SFX_Pick-up_001_Lobster.wav", false, false).Play();
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
            Console.WriteLine("producing");
            SpawnPlatform();


        }
    }

    void ReadArduinoInput(SerialPort portTemp)
    {
        message = portTemp.ReadLine();
        data = message.Split(' ');
        try
        {
            player1.moveXAmount = int.Parse(data[0]);
            player1.jumpButton = int.Parse(data[1]);
            player1.powerButton = int.Parse(data[2]);
            player2.moveXAmount = int.Parse(data[3]);
            player2.jumpButton = int.Parse(data[4]);
            player2.powerButton = int.Parse(data[5]);
            Console.WriteLine("{0} {1} {2} {3} {4} {5}", data[0], data[1], data[2], data[3], data[4], data[5]);
        }
        catch { }
        
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
        int thePlatform = (int) Utils.Random(1, 6);

        int detectionValue = 10;
        int heightAdjustPlayer1 = 6;
        int heightAdjustPlayer2 = 6;

    //    Console.WriteLine(thePlatform);

        if (thePlatform == 1)
        {
            theYCrood = Utils.Random(125, 126);
            theMargin = 180;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_onion.png";
            theYScale = 1f;
            detectionValue = 25;
            heightAdjustPlayer1 = 8; //8
            heightAdjustPlayer2 = 10; //10
        }

        else if (thePlatform == 2)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 165;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_broccoli.png";
            theYScale = 0.4f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6; //6
            heightAdjustPlayer2 = 6; //6
        }

        else if (thePlatform == 3)
        {
            theYCrood = Utils.Random(100, 100);
            theMargin = 170;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_cheese.png";
            theYScale = 0.5f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6; //6
            heightAdjustPlayer2 = 7; //7
        }

        else if (thePlatform == 4)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 140;
            theXScale = Utils.Random(0.33f, 0.7f);
            theImage = "plat_corn.png";
            theYScale = 0.33f;
            detectionValue = 20;
            heightAdjustPlayer1 = 4; //4
            heightAdjustPlayer2 = 5; //5
        }


        else if (thePlatform == 5)
        {
            theYCrood = Utils.Random(100, 120);
            theMargin = 65;
            theXScale = Utils.Random(0.6f, 0.9f);
            theImage = "plat_carrot.png";
            theYScale = 0.4f;
            detectionValue = 20;
            heightAdjustPlayer1 = 4; //4
            heightAdjustPlayer2 = 2; //2
        }

        else
        {
            theYCrood = Utils.Random(125, 125);
            theMargin = 100;
            theXScale = Utils.Random(1f, 1.7f);
            theImage = "plat_eggplant.png";
            theYScale = 0.7f;
            detectionValue = 20;
            heightAdjustPlayer1 = 6; //3
            heightAdjustPlayer2 = 3; //6
            
        }

        if (theSpawnNumber > STARTERPLATFORMS * 2)
        {
            theYCrood = theYCrood / 2.5f;
        }


        platformYSpawnValue -= theYCrood;

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

            int theAbilityNum = Utils.Random(1, 4);
            Pickup thePickup;

            theMargin = 174;
            int theAbilityWidth = 64;
            int theAbilityHeight = 64;
            

            if (theAbilityNum == 1)
            {
                 thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth * 2), -theAbilityHeight, "pepper.png", "ability_chiliPepperPiece", 10000);
               // thePickup = new Pickup(Utils.Random(game.width - theMargin - theAbilityWidth * 2, game.width - theMargin - theAbilityWidth * 2), -theAbilityHeight, "pepper.png", "ability_chiliPepperPiece", 10000);
                thePickup.scale = 0.22f;
            }
            
            else if (theAbilityNum == 2)
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth * 2), -theAbilityHeight, "basil_leave.png", "ability_basilLeaf", 15000);
                thePickup.scale = 0.22f;
            }

            else if (theAbilityNum == 3)
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth * 2), -theAbilityHeight, "lavender.png", "ability_lavenderFlower", 10000);
                thePickup.scale = 0.22f;
            }

            else
            {
                thePickup = new Pickup(Utils.Random(theMargin + theAbilityWidth, game.width - theMargin - theAbilityWidth * 2), -theAbilityHeight, "garlic.png", "ability_gralicPiece", 10000);
                thePickup.scale = 0.22f;
            }

            SoundChannel theSound = new Sound("SFX_Power-up-Spawn_001.wav", false, false).Play();

            AddChild(thePickup);
            Gamedata.pickupList.Add(thePickup);
        }

        AddChild(theSpawnPlatform);
    }
}