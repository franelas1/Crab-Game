using GXPEngine;                    // GXPEngine contains the engine
using System;
using System.IO.Ports;

public class MyGame : Game
{

    const int PLATFORMSPAWNAMOUNT = 22;
    // Declare variables:
    Player player1, player2;

    public bool jumped = false;

    int theNumberSpawn; //decides the 'number' platform will be spawned with


    Pivot pivotAll = new Pivot();
    Pivot platformsPivot = new Pivot();
    Pivot playerPivot = new Pivot();
    Pivot backgroundPivot = new Pivot();

    public SerialPort port1 = new SerialPort();

    float platformYSpawnValue;

    //Determine the position the player will be displayed in the game camera
    float boundaryValueX; //Should be width / 2 to display the player at the center of the screen
    float boundaryValueY; //Should be height / 2 to display the player at the center of the screen

    int restartStage = 0; //0 = game start, 1 = ongoing, 2 = win restart
    string message;
    string[] data;

    public MyGame() : base(800, 600, false, false)     // Create a window that's 800x600 and NOT fullscreen
    {
        Connect(port1, "COM3");

        AddChild(pivotAll);
        pivotAll.AddChild(backgroundPivot);
        pivotAll.AddChild(playerPivot);
        pivotAll.AddChild(platformsPivot);


        Sprite background = new Sprite("crab_bg.png");
        backgroundPivot.AddChild(background);

        //Spawning and adding players
        player1 = new Player(1, 300, height - 120, 140);
        Gamedata.player1 = player1;
        playerPivot.AddChild(player1);


        player2 = new Player(2, 500, height - 120, 140);
        Gamedata.player2 = player2;
        playerPivot.AddChild(player2);

        platformYSpawnValue += 500;

        //starter platforms
        Platform spawnPlatform1 = new Platform(300, height - 105, 0, "square.png", 0);
        Gamedata.platforms.Add(spawnPlatform1);
        AddChild(spawnPlatform1);

        Platform spawnPlatform2 = new Platform(500, height - 105, 0, "square.png", 1);
        Gamedata.platforms.Add(spawnPlatform2);
        AddChild(spawnPlatform2);

        boundaryValueX = game.width / 2;
        boundaryValueY = game.height / 2;
    }

    void Update()
    {
        message = port1.ReadLine();
        data = message.Split(' ');

        if (Input.GetKey(Key.M))
        {
            Gamedata.platformStartFalling = true;
        }
        //Updating player movement with human imput
        player1.updatePlayer(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]));
        Console.WriteLine("{0} {1}", int.Parse(data[0]), int.Parse(data[1]));
        //player2.updatePlayer(Input.GetKey(Key.RIGHT), Input.GetKey(Key.LEFT), Input.GetKeyDown(Key.UP));


        Gamedata.detectPlatformPlayer1 = false;

        if (Gamedata.platforms.Count < PLATFORMSPAWNAMOUNT && Gamedata.playerMoved == true)
        {
            Console.WriteLine("producing");
            SpawnPlatform();
            Gamedata.platformSpawnAmount--;
        }

        //  CheckCamera();
        //    y++;
        //    pivotAll.y--;

        //   levelPivot.y--;
        // playerPivot.y--;
    }

    // Main is the first method that's called when the program is run
    static void Main()
    {
        // Create a "MyGame" and start it:
        new MyGame().Start();
    }


    /*
    void CheckCamera()
    {
        Console.WriteLine(y);
        //handling player moving up
        if ((player1.y + player2.y) / 2 + y < game.height - boundaryValueY && y < 999999999999)
        {
            y = game.height - boundaryValueY - (player1.y + player2.y) / 2;
        }
    }
    */


    void SpawnPlatform()
    {
        theNumberSpawn++;
        float theYCrood;
        theYCrood = Utils.Random(100, 101);
        int theMargin = Utils.Random(50, 101);
        platformYSpawnValue -= theYCrood;
        Platform theSpawnPlatform = new Platform("square.png", platformYSpawnValue, theMargin, theNumberSpawn, PLATFORMSPAWNAMOUNT -
            (PLATFORMSPAWNAMOUNT - Gamedata.platforms.Count));

        Gamedata.platforms.Add(theSpawnPlatform);
        AddChild(theSpawnPlatform);
    }

    void Connect(SerialPort portTemp, string port_nr)
    {
        //Arduino input stuff
        portTemp.PortName = port_nr;
        portTemp.BaudRate = 9600;
        portTemp.RtsEnable = true;
        portTemp.DtrEnable = true;
        portTemp.Open();

    }

/*    int Read(SerialPort portTemp)
    {
        try
        {
            int message = int.Parse(portTemp.ReadLine());
            Console.WriteLine(message);
            System.Threading.Thread.Sleep(0);
            return message;
        }
        catch (TimeoutException) { return 0; }
    }*/
}