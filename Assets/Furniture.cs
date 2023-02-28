using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture
{
    public Vector2 dimensions;
    public string path;
    public string placement;
    public Quaternion rotation;
    public float height;

    public Furniture(Vector2 dimensions,float height, string path, string placement)
    {
        this.dimensions = dimensions;
        this.path = path;
        this.placement = placement; // any || wall
        this.rotation = Quaternion.identity;
        this.height = height;
    }

    public static Furniture kitchenTable()
    {
        return new Furniture(new Vector2(18,14),1f, "Prefabs/Kitchen_Table", "any");
    }

    public static Furniture debugBox()
    {
        return new Furniture(new Vector2(4, 4), 2f, "Prefabs/Debug_Box", "wall");
    }

    public static Furniture debugLongBox()
    {
        return new Furniture(new Vector2(20, 2), 2f, "Prefabs/Debug_Long_Box", "any");
    }

    public static Furniture debugBigBox()
    {
        return new Furniture(new Vector2(14, 12), 2f, "Prefabs/Debug_Big_Box", "any");
    }

    public static Furniture door()
    {
        return new Furniture(new Vector2(8, 8), -2f, "none", "any");
    }

}
