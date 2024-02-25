using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;	// Adding lists
//using System.IO.Ports;

public class MyGame : Game
{
    

    // Declare variables:
    Player player1, player2;

    public bool jumped = false;

    List<Platform> platforms = new List<Platform>();

    public MyGame() : base(800, 600, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
        Sprite background = new Sprite("crab_bg.png");
        AddChild(background);


        //Spawning and adding players
        player1 = new Player(1, 200, height - 50, 140);
        AddChild(player1);
        Gamedata.player1 = player1;

        player2 = new Player(2, 200, height - 50, 140);
        AddChild(player2);
        Gamedata.player2 = player2;

        platforms.Add(new Platform("square.png", 100, 50, 0, "A"));
        platforms.Add(new Platform("square.png", 300, 100, 1, "B"));
        platforms.Add(new Platform("square.png", 325, 100, 1, "B.1"));
        platforms.Add(new Platform("square.png", 400, 100, 1, "C"));
        AddChild(platforms[0]);
        AddChild(platforms[1]);
        AddChild(platforms[2]);
        AddChild(platforms[3]);
        Gamedata.platforms = platforms;


        //test platform
        //Platform plat1 = new Platform[];
    }

    void Update()
    {
        if (Input.GetKey(Key.M))
        {
            Gamedata.platformStartFalling = true;
        }
        //Updating player movement with human imput
        player1.updatePlayer(Input.GetKey(Key.D), Input.GetKey(Key.A), Input.GetKeyDown(Key.W));
        player2.updatePlayer(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT), Input.GetKeyDown(Key.UP));

     //   if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.UP)) jumped = true;

        /*
        if (jumped)
        {
            foreach (Platform plat in platforms)
            {
                plat.updatePlatform();
            }
        }
        */

        Gamedata.detectPlatformPlayer1 = false;
    }

    // Main is the first method that's called when the program is run
    static void Main()
    {
        // Create a "MyGame" and start it:
        new MyGame().Start();
    }
}