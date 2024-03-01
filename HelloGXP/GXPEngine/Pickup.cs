using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Pickup : Sprite
    {
        public Ability theAbility;
        //since pickup is always on top of platform, we will need the reference of the platform
        public bool gotPicked;
        public Pickup(float posX, float posY, string filename, string theAbilityName, int theAbilityTime) : base(filename)
        {
            x = posX;
            y = posY;

            theAbility = new Ability(theAbilityName, theAbilityTime);
        }

        public void Update()
        {
            if (gotPicked)
            {
                Gamedata.pickupList.Remove(this);
                this.Destroy();
                return;
            }
            if (Gamedata.playerMoved)
            {
                y += Gamedata.platformSpeed;
            }

            if (y > game.height + height / 2)
            {
                Gamedata.pickupList.Remove(this);
                this.Destroy();
            }
        }
    }
}
