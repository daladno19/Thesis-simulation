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

    // kitchen furniture
    public static Furniture Fridge()
    {
        return new Furniture(new Vector2(6, 6), 10f, "Prefabs/Fridge", "wall");
    }

    public static Furniture BigFridge()
    {
        return new Furniture(new Vector2(12, 6), 10f, "Prefabs/Big_Fridge", "wall");
    }

    public static Furniture KitchenTable()
    {
        return new Furniture(new Vector2(11, 8), 5f, "Prefabs/Kitchen_Table", "any");
    }

    public static Furniture SmallCounter()
    {
        return new Furniture(new Vector2(12, 6), 5f, "Prefabs/Small_Counter", "wall");
    }

    public static Furniture Stove()
    {
        return new Furniture(new Vector2(6, 6), 5f, "Prefabs/Stove", "wall");
    }

    public static Furniture MediumCounter()
    { 
        return new Furniture(new Vector2(18, 6), 5f, "Prefabs/Medium_Counter", "wall");
    }

    public static Furniture KitchenShelf()
    {
        return new Furniture(new Vector2(8, 3), 2.5f, "Prefabs/Kitchen_Shelf", "wall");
    }

    public static Furniture BigCounter()
    {
        return new Furniture(new Vector2(23, 6), 5f, "Prefabs/Big_Counter", "wall");
    }

    // misc furniture
    public static Furniture SmallTable()
    {
        return new Furniture(new Vector2(8, 5), 5f, "Prefabs/Small_Table", "any");
    }

    public static Furniture Stool()
    {
        return new Furniture(new Vector2(5, 5), 4f, "Prefabs/Stool", "any");
    }

    public static Furniture ShoeRack()
    {
        return new Furniture(new Vector2(8, 3), 3f, "Prefabs/Shoe_Rack", "wall");
    }

    public static Furniture BigShelf()
    {
        return new Furniture(new Vector2(8, 7), 10f, "Prefabs/Big_Shelf", "wall");
    }
    // Livingroom furniture
    public static Furniture Buffet()
    {
        return new Furniture(new Vector2(20, 6), 5f, "Prefabs/Buffet", "wall");
    }

    public static Furniture Sofa()
    {
        return new Furniture(new Vector2(8, 8), 1.5f, "Prefabs/Sofa", "any");
    }

    public static Furniture Couch()
    {
        return new Furniture(new Vector2(20, 9), 1.5f, "Prefabs/Couch", "any");
    }

    public static Furniture SquareTable()
    {
        return new Furniture(new Vector2(10, 10), 5f, "Prefabs/Square_Table", "any");
    }
}
