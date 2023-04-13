using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coverage_grid
{

}

public class tile
{ 
    public float x;
    public float z;
    public bool covered;
    public Color color;

    public tile(float x, float z)
    { 
        this.x = x;
        this.z = z;

        this.covered = false;
        color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f); // half transparent white
        GameObject tile_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}
