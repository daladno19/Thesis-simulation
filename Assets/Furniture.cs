using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture
{
    public Vector2 dimensions;
    public string path;
    public string placement;

    public Furniture(Vector2 dimensions, string path, string placement)
    {
        this.dimensions = dimensions;
        this.path = path;
        this.placement = placement; // middle || wall
    }

    public static Furniture kitchenTable()
    {
        return new Furniture(new Vector2(18,14), "Prefabs/Kitchen_Table", "any");
    }

    public static Furniture debugBox()
    {
        return new Furniture(new Vector2(4, 4), "Prefabs/Debug_Box", "any");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
