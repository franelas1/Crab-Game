using System;

namespace GXPEngine
{
    public class Platform : Sprite
    {
        int margin;
        public int theIndex;

        //filename: the image name of the platform's image
        //posYStart: y position of the platform at spawn
        //margin: how many pixels away the walls are from the sides of the screen
        public Platform(string filename, float posYStart, int margin, int theIndex, float theXScale) : base(filename)
        {
            SetOrigin(width / 2, 0);
            scaleX = theXScale;
            scaleY = 0.5f;
            this.margin = margin;
            x = Utils.Random(margin + width, game.width - margin - width);
            y = posYStart;
            this.theIndex = theIndex;
        }


        //filename: the image name of the platform's image
        //posYStart: y position of the platform at spawn
        //margin: how many pixels away the walls are from the sides of the screen
        public Platform(float posX, float posY, string filename, int theIndex) : base(filename)
        {
            SetOrigin(width / 2, 0);
            scaleX = Utils.Random(1.5f, 3f);
            scaleY = 0.5f;
            x = posX;
            y = posY;
            this.theIndex = theIndex;
        }

        //Function is called every frame once the platform is created
        public void Update()
        {
           
            if (Gamedata.playerMoved)
            {
                y += 1f;
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
