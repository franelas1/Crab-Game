using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;

public class MyGame : Game
{

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

    public MyGame() : base(1366, 768, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
        ResetGame();
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

        water = new AnimationSprite("full-bg.png", 3, 3);
        water.SetCycle(0, 7, 5);
        water.scaleX = 1.7075f;
        water.scaleY = 1.28f;
        backgroundPivot.AddChild(water);

        //Spawning and adding players
        player1 = new Player(1, width / 2 - 200, height - 120, 140, "circle.png");
        Gamedata.player1 = player1;
        playerPivot.AddChild(player1);


        player2 = new Player(2, width / 2 + 200, height - 120, 140, "circle1.png");
        Gamedata.player2 = player2;
        playerPivot.AddChild(player2);
        platformYSpawnValue = height - 150;

        //starter platforms
        Platform spawnPlatform1 = new Platform(width / 2, height - 105, "eggplantTest.png", 8f);
        Gamedata.platforms.Add(spawnPlatform1);
        AddChild(spawnPlatform1);

        //Platform spawnPlatform2 = new Platform(width / 2 - 200, height - 105, "square.png", 1.5f);
        //Gamedata.platforms.Add(spawnPlatform2);
        //AddChild(spawnPlatform2);
    }

    void Update()
    {
        water.Animate(1);

        if (Input.GetKey(Key.M))
        {
            Gamedata.platformStartFalling = true;
        }

        if (Gamedata.restartStage != 2 && Gamedata.restartStage != 3)
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
            winScreenText = new TextCanvas("Player X win", "SwanseaBold-D0ox.ttf", 20, 200, 200, 255, 255, 255, false);
            winScreenText.SetPoint((width / 2) - 100, (height / 2) - 100);
            winScreenText.ChangeText("Player " + Gamedata.playerWin + " wins");
            winScreenText.visible = true;
            AddChild(winScreenText);
        }

        if (Gamedata.restartStage == 3)
        {
            Console.Clear();
            Console.WriteLine("restarting");
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

        int theYCrood = (int) Utils.Random(30, 60) + 5 * theSpawnNumber;
        int theMargin = 100; // (int) Utils.Random(50, 100);
        platformYSpawnValue -= theYCrood;

        Console.WriteLine(platformYSpawnValue);
        String theImage;
        float theXScale = Utils.Random(1f, 2f);

        if(theXScale >= 1f && theXScale < 1.5f)
        {
            theImage = "eggplantTest.png";
        }

        else if (theXScale >= 1.5f && theXScale < 2f)
        {
            theImage = "eggplantTest.png";
        }

        else if (theXScale >= 2f && theXScale < 2.5f)
        {
            theImage = "eggplantTest.png";
        }

        else
        {
            theImage = "eggplantTest.png";
        }


        if (theSpawnNumber > PLATFORMSPAWNAMOUNT)
        {
            platformYSpawnValue = -100 - theYCrood;
        }

        float platformSpeed = 1.5f * Math.Max(1f,(theSpawnNumber / 10));

        Platform theSpawnPlatform = new Platform(theImage, platformYSpawnValue, theMargin, theXScale, platformSpeed);
        Gamedata.platforms.Add(theSpawnPlatform);
        AddChild(theSpawnPlatform);
    }
}