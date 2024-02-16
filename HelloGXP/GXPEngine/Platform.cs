namespace GXPEngine
{
    class Platform : Sprite
    {
        int margin = 95; // how many pixels away the walls are from the sides of the screen

        public float fallSpeed = 1.2f; //Platform falling speed
        public Platform(float tempY) : base("square.png")
        {
            SetOrigin(width / 2, 0);
            scaleX = Utils.Random(1.5f, 3f);
            scaleY = 0.5f;
            x = Utils.Random(margin + width, game.width - margin - width);
            y = tempY;
        }

        public void updatePlatform()
        {
            fall();
            destroyPlatform();
        }

        void fall()
        {
            y += fallSpeed;
        }

        void destroyPlatform()
        {
            if (y > game.height)
            {
                this.Remove();
                this.Destroy();
            }
        }

    }
}
