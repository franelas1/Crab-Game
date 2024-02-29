using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Ability
    {
        public string theAbility;
        int abilityTime;
        int theTimer = Time.time;
        public bool isOver = false;

        public int markedForDeathPrep = 0;

        public bool hasRestored = false;

        public Ability(string theAbility, int abilityTime) 
        {
            this.theAbility = theAbility;
            this.abilityTime = abilityTime;
        }

        public void UpdateAbility()
        {
            if (Time.time - theTimer  >= abilityTime)
            {
                isOver = true;
            }

            /*
            if (markedForDeath == 1)
            {
                Gamedata.player1.theAbilities.Remove(this);
            }

            if (markedForDeath == 2)
            {
                Gamedata.player2.theAbilities.Remove(this);
            }
            */

        }
    }
}
