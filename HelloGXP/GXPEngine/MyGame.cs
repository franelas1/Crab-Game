using System;                   // System contains a lot of default C# libraries 
using GXPEngine;                // GXPEngine contains the engine

public class MyGame : Game {
	// Declare the Sprite variables:
	Sprite spaceShip;
	AnimationSprite character;
	EasyDraw background;
	EasyDraw button;
	EasyDraw information;
	//add some comments
	//test1

	// Declare other variables:

	float turnSpeedShip = 3;
	float moveSpeedShip = 5;
	float characterSpeed = 3;

	SoundChannel soundTrack;*/
	Player player1;
	public MyGame() : base(800, 600, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		player1 = new Player(width/2, height);
		AddChild(player1);


	}
	void mainMenuScreen()
    {
		
    }

	void doGameLoop()
    {

    }

	void Update() {
		player1.updatePlayer();
	}

	// Main is the first method that's called when the program is run
	static void Main() {
		// Create a "MyGame" and start it:
		new MyGame().Start();
	}
}