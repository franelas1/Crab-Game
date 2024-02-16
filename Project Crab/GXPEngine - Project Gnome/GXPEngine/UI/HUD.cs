using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;

/*
 * UI to be displayed in-game during a level
 */
class HUD : Canvas
{
    EasyDraw scoreDisplayCanvas;
    EasyDraw fpsDisplay;
    EasyDraw winDisplay;

    public HUD() : base(100, 100)
    {
        scoreDisplayCanvas = new EasyDraw(200, 50);

        Font theFont = Utils.LoadFont("SwanseaBold-D0ox.ttf", 10);
        scoreDisplayCanvas.TextFont(theFont);
        //(hoz, vert)
        scoreDisplayCanvas.TextAlign(CenterMode.Center, CenterMode.Center);
        scoreDisplayCanvas.Fill(0, 0, 0);
        scoreDisplayCanvas.Text("Score: ", -5, -5);
        scoreDisplayCanvas.Text("Health: ");
        AddChild(scoreDisplayCanvas);
    //    fpsDisplay = new EasyDraw(300, 50);
     //   fpsDisplay.SetXY(600, 20);
      //  AddChild(fpsDisplay);
        winDisplay = new EasyDraw(300, 50);
        winDisplay.SetXY(game.width / 2, game.height / 2);
        AddChild(winDisplay);
    }

    void Update()
    {
        scoreDisplayCanvas.Clear(252, 186, 3);
        scoreDisplayCanvas.Text("Height reached: " + GameData.theNumberReached, 100, 15);
      //  scoreDisplayCanvas.Text("Health: " + GameData.playerHealth, 50, 35);
        graphics.Clear(Color.Empty);
      //   graphics.DrawString("Score: " + MyGame.theGameData.TheScore, SystemFonts.DefaultFont, Brushes.White, 20, 50);
      //    fpsDisplay.Clear(252, 186, 3);
       //   fpsDisplay.Text("Fps: " + GameData.theFPS);

        if (GameData.displayWinText)
        {
            //graphics.Clear(Color.Empty);
            winDisplay.Clear(0, 0, 0, 255);

            if (GameData.playerWin == 1)
            {
                winDisplay.Text("Player 1 win");
            }

            else if (GameData.playerWin == 2)
            {
                winDisplay.Text("Player 2 win");
            }

            else
            {
                winDisplay.Text("loading...");
            }
        }

    }

}


// graphics.DrawString("Score: " + GameData.theNumberReached, SystemFonts.DefaultFont, Brushes.White, 20, 5);