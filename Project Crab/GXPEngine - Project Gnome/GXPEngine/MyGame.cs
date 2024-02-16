using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using System.Collections.Generic;
using System.Reflection.Emit;

/*
 * The entire game itself. Game starts from here
 */
public class MyGame : Game 
{
    Level theLevel; //A game should only have one level
    SoundChannel mainMenuSound;
    bool isMenu = true;
    const int RESETTIME = 1000;
    int resetTimer = Time.time;
    bool isResetting = false;
    public MyGame() : base(800, 640, false, false)     // Create a window that's 800x600 and NOT fullscreen
	{
        targetFps = 60;
        loadLevel();
    }

	//For every game object, Update is called every frame, by the engine:
	void Update() 
    {
        GameData.theFPS = currentFps;   //for debug purpose

        //Level reset condition - load level again to reset
        if ((Input.GetKeyDown(Key.R) || GameData.playerDead) && !GameData.isMenu)         
        {
            if (GameData.playerHealth <= 0){
                SoundChannel newSound = new Sound("playerDead.wav", false, false).Play();
            }

            if (Input.GetKeyDown(Key.R) && !GameData.playerDead)
            {
                GameData.playerWin = -1;
            }

            Console.WriteLine("dish");
            isResetting = true;
            loadLevel();
            resetTimer = Time.time;
        }

        //Allowing player to go to game menu during game. Will exit the level
        if (Input.GetKey(Key.M))
        {
            GameData.theLevelName = "mainMenu.tmx";

            GameData.isMenu = true;
            isResetting = false;
            theLevel.visible = true;
            loadLevel();
        }


        if (Input.GetKeyDown(Key.G))
        {
          //  Console.WriteLine(this.GetDiagnostics());
        }


        if (isResetting && Time.time - resetTimer >= RESETTIME)
        {
            isResetting = false;
            loadLevel();
        }
    }

    //Main() is the first method that's called when the program is run.
    static void Main()                          
	{
        new MyGame().Start(); //Start the game execution
    }

    //Loading a new level
	void loadLevel()
	{
        //Destroy old level
        List<GameObject> children = GetChildren();
        foreach (GameObject child in children)
        {
            child.LateDestroy();
        }

        //If loading a menu level (main menu, level select menu, etc)
        if (GameData.isMenu != isMenu)
        {
            if (mainMenuSound != null)
            {
                mainMenuSound.Stop();
            }
            isMenu = !isMenu;
        }

        //If loading a game level (level 1, level 2, etc)
        if (GameData.isMenu == false)
        {
            GameData.ResetLevelData();
          //  mainMenuSound.Stop();
           // mainMenuSound = new Sound("MainGameMusic.wav", true, false).Play(); //this is the level sound, looping
            //mainMenuSound.Volume = 0.4f;
        }

        GameData.playerDead = false;

        //Loading the level itself
        theLevel = new Level(GameData.theLevelName);
        if (isResetting)
        {
            GameData.displayWinText = true;
            theLevel.visible = false;
            
        }

        else
        {
            GameData.displayWinText = false;
        }

        AddChild(theLevel);


        //Load the HUD if loading a game level. Render order higher than the level
        if (GameData.isMenu == false)
        {
            HUD theHud = new HUD();
            AddChild(theHud);
        }
    }
}