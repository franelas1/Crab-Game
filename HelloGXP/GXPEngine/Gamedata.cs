using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    //for storing, extracting, and using variables all the classes can access
    public static class Gamedata
    {
        //this class has no contructor since it is a static class (we will use the class itself as an object)
        //since we want every class to use it, everything may need to be public static

      //  public static bool platformStartFalling = false;

        public static Player player1;
        public static Player player2;
        public static List<Platform> platforms = new List<Platform> ();
        public static List<Pickup> pickupList = new List<Pickup>();
        public static Platform currentPlayer1Platform; //the last platform player1 touched or the next platform player1 will touch
        public static Platform currentPlayer2Platform; //the last platform player2 touched or the next platform player2 will touch
        public static bool detectPlatformPlayer1;
        public static bool detectPlatformPlayer2;
        //used when the game restarts

        public static int theNumberReached = 0;
        //public static int platformSpawnAmount = 20;

        public static bool playerMoved = false;

        public static int restartStage = -1;
        public static int playerWin;

        public static float platformSpeed = 2f;


        public static bool inBasilLEffect;

        public static bool countdownOver = false;

        public static void ResetData()
        {
            countdownOver = false;
            playerWin = 0;
            platformSpeed = 2f;
            inBasilLEffect = false;
         //   platformStartFalling = false;
            player1 = null;
            player2 = null;
            platforms.Clear();
            currentPlayer1Platform = null;
            currentPlayer2Platform = null;
            detectPlatformPlayer1 = false;
            detectPlatformPlayer2 = false;
            theNumberReached = 0;
     //       platformSpawnAmount = 20;
            playerMoved = false;
            restartStage = -1;
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
                               Console.WriteLine(currentPlayer1Platform.y);
                            }

                            else
                            {
                                Gamedata.currentPlayer2Platform = thePlatform;
                                detectPlatformPlayer2 = true;
                         //       Console.WriteLine("P2: " + thePlatform.debugString);
                            }
                        }
                    }
                }
            }
        }
    }


}
