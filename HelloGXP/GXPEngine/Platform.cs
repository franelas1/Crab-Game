using System;

namespace GXPEngine
{
    public class Platform : Sprite
    {
        int margin;
        public float theSpeed;
        //filename: the image name of the platform's image
        //posYStart: y position of the platform at spawn
        //margin: how many pixels away the walls are from the sides of the screen

        public int detectionValue = 10;

        public int heightAdjustPlayer1 = 6;
        public int heightAdjustPlayer2 = 3;
        public Platform(string filename, float posYStart, int margin, float theXScale, float scaleY, float theSpeed,
            int detectionValue, int heightAdjustPlayer1, int heightAdjustPlayer2) : base(filename)
        {
            SetOrigin(width / 2, 0);
            scaleX = theXScale;
            this.scaleY = scaleY;
            this.margin = margin;
            this.detectionValue = detectionValue;
            this.heightAdjustPlayer1 = heightAdjustPlayer1;
            this.heightAdjustPlayer2 = heightAdjustPlayer2;

            /*
            Console.WriteLine("-------------");
            Console.WriteLine(margin + width);
            Console.WriteLine(game.width - margin - width);
            */

            x = Utils.Random(margin + width, game.width - margin - width);

        //    x = Utils.Random(margin + width, margin + width + 1);

        //   x = Utils.Random(game.width - margin - width, game.width - margin - width + 1);


            y = posYStart;
            this.theSpeed = theSpeed;
        }

        //filename: the image name of the platform's image
        //posYStart: y position of the platform at spawn
        //margin: how many pixels away the walls are from the sides of the screen
        public Platform(float posX, float posY, string filename, float theSpeed) : base(filename)
        {
            SetOrigin(width / 2, 0);
            scaleX = Utils.Random(1.5f, 3f);
            scaleY = 0.5f;
            x = posX;
            y = posY;
            this.theSpeed = theSpeed;

        }

        //Function is called every frame once the platform is created
        public void Update()
        {
           
            if (Gamedata.playerMoved && Gamedata.countdownOver)
            {
                if (Gamedata.inBasilLEffect)
                {
                    y += Gamedata.platformSpeed - (float) (Gamedata.platformSpeed * 0.25);
                }

                else
                {
                   y += Gamedata.platformSpeed;
                }
                CheckPlatformOutOfScreen();
            }
        }

        void CheckPlatformOutOfScreen()
        {
            if (y > game.height + height / 2)
            {
                Gamedata.platforms.Remove(this);
                this.Destroy();
            }
        }
    }
}
