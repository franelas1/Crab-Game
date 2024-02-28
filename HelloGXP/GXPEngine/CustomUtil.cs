using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GXPEngine
{
    //providing functions all classes can use
    public static class CustomUtil
    {
        //Check if two sprite objects intersect
        public static bool hasIntersectionSprites(Sprite thisObject, Sprite thatObject)
        {
            if (Math.Abs(thisObject.x - thatObject.x) <= (thisObject.width / 2) + (thatObject.width / 2) &&
                Math.Abs(thisObject.y - thatObject.y) <= (thisObject.height / 2) + (thatObject.height / 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check if two animation sprite objects intersect
        public static bool hasIntersectionAnimationSprites(AnimationSprite thisObject, AnimationSprite thatObject)
        {
            if (Math.Abs(thisObject.x - thatObject.x) <= (thisObject.width / 2) + (thatObject.width / 2) &&
                Math.Abs(thisObject.y - thatObject.y) <= (thisObject.height / 2) + (thatObject.height / 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double GetDistance(Sprite thisObject, Sprite thatObject)
        {
            return Math.Sqrt(((thisObject.x - thatObject.x) * (thisObject.x - thatObject.x)) +
                ((thisObject.y - thatObject.y) * (thisObject.y - thatObject.y))
                );
        }

        //Check if a point is within a rectangle shape
        public static bool CheckPointWithRect(Vector2 boxCrood, int boxWidth, int boxHeight, Vector2 otherCrood)
        {
            if (boxCrood.x - boxWidth / 2 <= otherCrood.x && boxCrood.x + boxWidth / 2 >= otherCrood.x)
            {
                if (boxCrood.y - boxHeight / 2 <= otherCrood.y && boxCrood.y + boxHeight / 2 >= otherCrood.y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
