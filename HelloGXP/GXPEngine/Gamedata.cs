﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    //for storing, extracting, and using variables all the classes can access
    public static class Gamedata
    {
        //this class has no contructor since it is a static class (we will use the class itself as an object)
        //since we want every class to use it, everything may need to be public static

        public static bool platformStartFalling = false;

        public static Player player1;
        public static Player player2;
        public static List<Platform> platforms;
        public static Platform currentPlayer1Platform; //the last platform player1 touched or the next platform player1 will touch
        public static Platform currentPlayer2Platform; //the last platform player2 touched or the next platform player2 will touch
        public static bool detectPlatformPlayer1;
        //used when the game restarts
        public static void ResetData()
        {
            platformStartFalling = false;
        }

        public static void CheckPlat(int thePlayerNumber)
        {
            if (player1 == null || player2 == null) return;

            Player thePlayer = null;

            if (thePlayerNumber == 1)
            {
                thePlayer = player1;
            }

            else
            {
                thePlayer = player2;
            }

            foreach (Platform thePlatform in platforms)
            {

                if (thePlayer != null)
                {
                    if (CustomUtil.hasIntersectionSprites(thePlatform, thePlayer))
                    {
                        if (thePlatform.collider != null)
                        {
                            if (thePlayerNumber == 1)
                            {
                                Gamedata.currentPlayer1Platform = thePlatform;
                                Gamedata.detectPlatformPlayer1 = true;
                            }

                            else
                            {
                                Gamedata.currentPlayer2Platform = thePlatform;
                            }
                        }
                    }
                }
            }
        }
    }


}