using System;                   // System contains a lot of default C# libraries 
using GXPEngine;                // GXPEngine contains the engine

public class MyGame : Game
{
	// Declare other variables:
	Player player1, player2;
	Platform plat1;
	//List<Platform> platforms = new List<Platform>;
	public MyGame() : base(800, 600, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		//Spawning and adding players
		player1 = new Player(width/2, height);
		AddChild(player1);

		player2 = new Player(width - 200, height / 2);
		AddChild(player2);

		plat1 = new Platform();
		AddChild(plat1);

		//test platform
		//Platform plat1 = new Platform[];
	}

	void Update() {
		//Updating player movement with human imput
		player1.updatePlayer(Input.GetKey(Key.D), Input.GetKey(Key.A), Input.GetKeyDown(Key.W));
		player2.updatePlayer(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT), Input.GetKeyDown(Key.UP));
		//foreach ()
		plat1.updatePlatform();
	}

	// Main is the first method that's called when the program is run
	static void Main() {
		// Create a "MyGame" and start it:
		new MyGame().Start();
	}

	void playerOnPlatform(Player player)
    {
	
    }
}