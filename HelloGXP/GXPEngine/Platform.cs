namespace GXPEngine
{
    public class Platform : Sprite
    {
        int margin;
        int theNumber;

        public string debugString;

        //filename: the image name of the platform's image
        //posYStart: y position of the platform at spawn
        //margin: how many pixels away the walls are from the sides of the screen
        public Platform(string filename, float posYStart, int margin, int theNumber, string debugString) : base(filename)
        {
            SetOrigin(width / 2, 0);
            scaleX = Utils.Random(1.5f, 3f);
            scaleY = 0.5f;
            this.margin = margin;
            x = Utils.Random(margin + width, game.width - margin - width);
            y = posYStart;
            this.debugString = debugString;
        }

        //Function is called every frame once the platform is created
        public void Update()
        {
            if (Gamedata.theNumberReached - theNumber > 5)
            {
                y++;
                CheckPlatformOutOfScreen();
            }
        }

        void CheckPlatformOutOfScreen()
        {
            if (y > game.height)
            {
                this.Destroy();
            }
        }
    }
}
