using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;

public class MyGame : Game
{

    const int PLATFORMSPAWNAMOUNT = 7;
    // Declare variables:
    Player player1, player2;


    //control crood, rotation, render order, etc of certain object groups
    Pivot pivotAll;
    Pivot playerPivot;
    Pivot backgroundPivot;

    float platformYSpawnValue;

    int theIndex = 2;

    int restartTimer;




    TextCanvas winScreenText;

    public MyGame() : base(800, 600, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
        ResetGame();
    }

    void ResetGame()
    {
        Gamedata.ResetData();

        winScreenText = null;

        platformYSpawnValue = 500;
        theIndex = 2;

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

        Sprite background = new Sprite("crab_bg.png");
        backgroundPivot.AddChild(background);

        //Spawning and adding players
        player1 = new Player(1, 300, height - 120, 140, "circle.png");
        Gamedata.player1 = player1;
        playerPivot.AddChild(player1);


        player2 = new Player(2, 500, height - 120, 140, "circle1.png");
        Gamedata.player2 = player2;
        playerPivot.AddChild(player2);
        platformYSpawnValue = 500;

        //starter platforms
        Platform spawnPlatform1 = new Platform(300, height - 105, "square.png", 0);
        Gamedata.platforms.Add(spawnPlatform1);
        AddChild(spawnPlatform1);

        Platform spawnPlatform2 = new Platform(500, height - 105, "square.png", 1);
        Gamedata.platforms.Add(spawnPlatform2);
        AddChild(spawnPlatform2);
    }

    void Update()
    {
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


        for (int i = 0; i < Gamedata.platforms.Count; i++)
        {
            for (int j = 0; j < Gamedata.platforms.Count; j++)
            {
                if (i != j)
                {
                    if (Gamedata.platforms[i].theIndex == Gamedata.platforms[j].theIndex)
                    {
                        Console.WriteLine("same index. SHOULD NOT APPEAR");
                    }

                }
            }
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
         //   Console.WriteLine("restarting");
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
        if (theIndex > 22)
        {
            theIndex = 0;
        }


        int theYCrood;
        theYCrood = (int) Utils.Random(65, 110);
        int theMargin = (int) Utils.Random(50, 101);
        platformYSpawnValue -= theYCrood;
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

        Platform theSpawnPlatform = new Platform(theImage, platformYSpawnValue, theMargin, theIndex, theXScale);
        Gamedata.platforms.Add(theSpawnPlatform);
        AddChild(theSpawnPlatform);
        theIndex++;
    }
}