using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;

public class MyGame : Game
{

    const int PLATFORMSPAWNAMOUNT = 20;
    // Declare variables:
    Player player1, player2;


    //control crood, rotation, render order, etc of certain object groups
    Pivot pivotAll;
    Pivot playerPivot;
    Pivot backgroundPivot;

    Button startButton;

    float platformYSpawnValue;


    int restartTimer;

    AnimationSprite water;
    TextCanvas winScreenText;

    int theSpawnNumber;

    public MyGame() : base(800, 600, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
      //  ResetGame();
    }

    void ResetGame()
    {
        Gamedata.ResetData();

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

        //Sprite background = new Sprite("finalBG.png");
        //backgroundPivot.AddChild(background);

        water = new AnimationSprite("full-bg.png", 3, 3);
        water.SetCycle(0, 7, 5);
        backgroundPivot.AddChild(water);

        //Spawning and adding players
        player1 = new Player(1, 300, height - 120, 140, "circle.png");
        Gamedata.player1 = player1;
        playerPivot.AddChild(player1);


        player2 = new Player(2, 500, height - 120, 140, "circle1.png");
        Gamedata.player2 = player2;
        playerPivot.AddChild(player2);
        platformYSpawnValue = 500;

        //starter platforms
        Platform spawnPlatform1 = new Platform(300, height - 105, "square.png", 1.5f);
        Gamedata.platforms.Add(spawnPlatform1);
        AddChild(spawnPlatform1);

        Platform spawnPlatform2 = new Platform(500, height - 105, "square.png", 1.5f);
        Gamedata.platforms.Add(spawnPlatform2);
        AddChild(spawnPlatform2);
    }

    void Update()
    {

        if (Gamedata.restartStage == -1)
        {
            startButton = new Button(width / 2, height / 2, "colors.png");
            AddChild(startButton);
            Gamedata.restartStage = 0;
        }

        if (Gamedata.restartStage == 0)
        {
            if (startButton == null)
            {
                return;
            }

            if (Input.GetMouseButton(0) && startButton.checkActivate())
            {
                Gamedata.restartStage = 1;
        //p        Console.WriteLine("activate");
                ResetGame();
            }
        }

        if (water != null)
        {
            water.Animate();
        }
        

        if (Input.GetKey(Key.M))
        {
            Gamedata.platformStartFalling = true;
        }

        if (Gamedata.restartStage != 2 && Gamedata.restartStage != 3 && player1 != null && player2 != null)
        {
            //Updating player movement with human imput
            player1.updatePlayer();
            player2.updatePlayer();
        }


        Gamedata.detectPlatformPlayer1 = false;

        if (Gamedata.platforms.Count < PLATFORMSPAWNAMOUNT && Gamedata.playerMoved == true)
        {
        //    Console.WriteLine("producing");
            SpawnPlatform();
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
            winScreenText = new TextCanvas("", "SwanseaBold-D0ox.ttf", 20, 200, 200, 255, 255, 255, false);
            winScreenText.SetPoint((width / 2) - 100, (height / 2) - 100);
            if (Gamedata.playerWin == -1)
            {
                winScreenText.ChangeText("loading");
            }

            else
            {
                winScreenText.ChangeText("Player " + Gamedata.playerWin + " wins");
            }

            winScreenText.visible = true;
            AddChild(winScreenText);
        }

        if (Gamedata.restartStage == 3)
        {

            if (Time.time - restartTimer >= 3000)
            {
                ResetGame();
                Gamedata.restartStage = 0;
            }
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

        

        int theYCrood;
        theYCrood = (int) Utils.Random(80, 110);
        int theMargin = (int) Utils.Random(50, 101);
        platformYSpawnValue -= theYCrood;

  //      Console.WriteLine(platformYSpawnValue);
        String theImage;
        float theXScale = Utils.Random(1.5f, 3f);

        if (theXScale > 2)
        {
            theImage = "square.png";
        }

        else if (theXScale > 2.5f)
        {
            theImage = "square.png";
        }

        else
        {
            theImage = "square.png";
        }



        if (theSpawnNumber > PLATFORMSPAWNAMOUNT)
        {
            platformYSpawnValue = -100 - theYCrood;
        }

        float platformSpeed = 1.5f * (theSpawnNumber / 10);

        Platform theSpawnPlatform = new Platform(theImage, platformYSpawnValue, theMargin, theXScale, platformSpeed);
        Gamedata.platforms.Add(theSpawnPlatform);
        AddChild(theSpawnPlatform);
    }
}