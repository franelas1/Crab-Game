using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

/*
 *A platform like in the game icy tower
 */
public class Platform : AnimationSpriteCustom
{
    public int defaultLength = 10;
    public int theType = -1;
    public Platform(string filenName, int rows, int columns, TiledObject obj = null) : base(filenName, rows, columns, obj)
    {
        scaleX = obj.GetFloatProperty("f_theLength", -1);
        theType = obj.GetIntProperty("f_theType", -1);
    }

    public Platform(string filenName, float scaleX, float scaleY, int singleFrameID, int columns, int rows,
        int numberOfFrames, int startFrame, int nextFrameDelay, bool textureKeepInCache, bool hasCollision) :
        base (filenName, scaleX, scaleY, singleFrameID, columns, rows,
         numberOfFrames, startFrame, nextFrameDelay, textureKeepInCache, hasCollision)
    {
    }

    public void changeScaleX(float scale)
    {
        scaleX = scale;
    }
}