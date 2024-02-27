using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;   // Adding lists
using System.Runtime.Remoting.Activation;
using System.IO.Ports;

public class MyGame : Game
{

    const int PLATFORMSPAWNAMOUNT = 22;
    // Declare variables:
    Player player1, player2;


    //control crood, rotation, render order, etc of certain object groups
    Pivot pivotAll = new Pivot();
    Pivot playerPivot = new Pivot();
    Pivot backgroundPivot = new Pivot();

    float platformYSpawnValue;

    int theIndex = 2;

    public MyGame() : base(800, 600, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
        ResetGame();
        Gamedata.restartStage = 0;
    }

    void ResetGame()
    {
        Gamedata.ResetData();

        platformYSpawnValue = 500;


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
        //Updating player movement with human imput
        player1.updatePlayer();
        player2.updatePlayer();


        Gamedata.detectPlatformPlayer1 = false;

        if (Gamedata.platforms.Count < PLATFORMSPAWNAMOUNT && Gamedata.playerMoved == true)
        {
            SpawnPlatform();
        }

        
        for (int i = 0; i < Gamedata.platforms.Count; i++) { 
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
        

        /*
        Console.WriteLine("--------------------------");

        for (int i = 0; i < Gamedata.platforms.Count; i++)
        {
            Console.WriteLine(Gamedata.platforms[i].theIndex);
        }
        */
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
        theYCrood = (int) Utils.Random(50, 101);
        int theMargin = (int) Utils.Random(50, 101);
        platformYSpawnValue -= theYCrood;
        Platform theSpawnPlatform = new Platform("square.png", platformYSpawnValue, theMargin, theIndex);

        Gamedata.platforms.Add(theSpawnPlatform);
        AddChild(theSpawnPlatform);
        theIndex++;
    }
}