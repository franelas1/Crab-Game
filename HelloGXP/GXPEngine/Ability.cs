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
        public Ability(string theAbility, int abilityTime) 
        {
            this.theAbility = theAbility;
            this.abilityTime = abilityTime;
        }

        public void UpdateAbility()
        {
            Console.WriteLine(Time.time - theTimer);
            if (Time.time - theTimer  >= abilityTime)
            {
                isOver = true;
            }
        }
    }
}
