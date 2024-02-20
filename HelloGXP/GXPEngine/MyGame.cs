using GXPEngine;				    // GXPEngine contains the engine
using System.Collections.Generic;	// Adding lists
using System.IO.Ports;

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
        player1 = new Player(200, height);
        AddChild(player1);

        player2 = new Player(width - 200, height);
        AddChild(player2);


        platforms.Add(new Platform(0));
        platforms.Add(new Platform(200));
        platforms.Add(new Platform(400));
        AddChild(platforms[0]);
        AddChild(platforms[1]);
        AddChild(platforms[2]);


        //test platform
        //Platform plat1 = new Platform[];
    }

    void Update()
    {
        //Updating player movement with human imput
        player1.updatePlayer(Input.GetKey(Key.D), Input.GetKey(Key.A), Input.GetKeyDown(Key.W));
        player2.updatePlayer(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT), Input.GetKeyDown(Key.UP));

        if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.UP)) jumped = true;

        if (jumped)
        {
            foreach (Platform plat in platforms)
            {
                player1.onPlatform(plat);
                player2.onPlatform(plat);
                plat.updatePlatform();
            }
        }

    }

    // Main is the first method that's called when the program is run
    static void Main()
    {
        // Create a "MyGame" and start it:
        new MyGame().Start();
    }

    void playerOnPlatform(Player player)
    {

    }

    void platformSpawner()
    {

    }
}