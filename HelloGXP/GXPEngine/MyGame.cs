using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.Collections.Generic;	// Adding lists

public class MyGame : Game
{
    Player player1, player2;    //the two players the game will use
    List<Platform> platforms = new List<Platform>();

    public MyGame() : base(800, 600, false)     // Create a window that's 800x600 and NOT fullscreen
    {

        targetFps = 60;
        //Spawning and adding players
        player1 = new Player(1, 200, height - 50);
        AddChild(player1);
        Gamedata.player1 = player1;

        player2 = new Player(2, 200, height - 50);
        AddChild(player2);
        Gamedata.player2 = player2;

        platforms.Add(new Platform("square.png", 0, 50));
        platforms.Add(new Platform("square.png", 200, 50));
        platforms.Add(new Platform("square.png", 400, 50));
        platforms.Add(new Platform("square.png", 550, 50));
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


        Gamedata.detectPlatformPlayer1 = false;
    }

    // Main is the first method that's called when the program is run
    static void Main()
    {
        // Create a "MyGame" and start it:
        new MyGame().Start();
    }
}