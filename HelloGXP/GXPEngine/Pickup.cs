using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Pickup : Sprite
    {
        public string theAbility;
        public Pickup(float posX, float posY, string filename, string theAbility) : base(filename)
        {
            x = posX;
            y = posY;
            this.theAbility = theAbility;
        }
    }
}
