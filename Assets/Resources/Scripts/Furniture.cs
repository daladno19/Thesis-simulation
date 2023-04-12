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
    }//

    public static Furniture debugLongBox()
    {
        return new Furniture(new Vector2(20, 2), 2f, "Prefabs/Debug_Long_Box", "any");
    }//

    public static Furniture debugBigBox()
    {
        return new Furniture(new Vector2(14, 12), 2f, "Prefabs/Debug_Big_Box", "any");
    }//

    public static Furniture door()
    {
        return new Furniture(new Vector2(8, 8), -2f, "none", "any");
    }//

    // kitchen furniture
    public static Furniture Fridge()
    {
        return new Furniture(new Vector2(6, 6), 10f, "Prefabs/Fridge", "wall");
    }//

    public static Furniture BigFridge()
    {
        return new Furniture(new Vector2(12, 6), 10f, "Prefabs/Big_Fridge", "wall");
    }//

    public static Furniture KitchenTable()
    {
        return new Furniture(new Vector2(11, 8), 5f, "Prefabs/Kitchen_Table", "any");
    }//

    public static Furniture SmallCounter()
    {
        return new Furniture(new Vector2(12, 6), 5f, "Prefabs/Small_Counter", "wall");
    }//

    public static Furniture Stove()
    {
        return new Furniture(new Vector2(6, 6), 5f, "Prefabs/Stove", "wall");
    }//

    public static Furniture MediumCounter()
    { 
        return new Furniture(new Vector2(18, 6), 5f, "Prefabs/Medium_Counter", "wall");
    }//

    public static Furniture KitchenShelf()
    {
        return new Furniture(new Vector2(8, 3), 6f, "Prefabs/Kitchen_Shelf", "wall");
    }//

    public static Furniture BigCounter()
    {
        return new Furniture(new Vector2(23, 6), 5f, "Prefabs/Big_Counter", "wall");
    }//

    // misc furniture
    public static Furniture SmallTable()
    {
        return new Furniture(new Vector2(8, 5), 5f, "Prefabs/Small_Table", "any");
    }//

    public static Furniture Stool()
    {
        return new Furniture(new Vector2(5, 5), 4.5f, "Prefabs/Stool", "any");
    }//

    public static Furniture ShoeRack()
    {
        return new Furniture(new Vector2(8, 3), 3f, "Prefabs/Shoe_Rack", "wall");
    }//

    public static Furniture BigShelf()
    {
        return new Furniture(new Vector2(8, 7), 10f, "Prefabs/Big_Shelf", "wall");
    }//
    // Livingroom furniture
    public static Furniture Buffet()
    {
        return new Furniture(new Vector2(20, 6), 5f, "Prefabs/Buffet", "wall");
    }//

    public static Furniture Sofa()
    {
        return new Furniture(new Vector2(8, 8), 1.5f, "Prefabs/Sofa", "any");
    }//

    public static Furniture Couch()
    {
        return new Furniture(new Vector2(20, 9), 1.5f, "Prefabs/Couch", "any");
    }

    public static Furniture SquareTable()
    {
        return new Furniture(new Vector2(10, 10), 5f, "Prefabs/Square_Table", "any");
    }//

    // TODO no prefabs, adjust float height
    public static Furniture GlassShelf()
    {
        return new Furniture(new Vector2(15, 5), 6f, "Prefabs/Glass_Shelf", "any");
    }//

    public static Furniture GlassTable()
    {
        return new Furniture(new Vector2(10, 6), 5f, "Prefabs/Glass_Table", "any");
    }//

    public static Furniture TvTable()
    {
        return new Furniture(new Vector2(12, 5), 4f, "Prefabs/Tv_Table", "any");
    }//

    public static Furniture Speaker()
    {
        return new Furniture(new Vector2(4, 3), 5f, "Prefabs/Speaker", "any");
    }//

    public static Furniture Puff()
    {
        return new Furniture(new Vector2(5, 4), 2f, "Prefabs/Puff", "any");
    }//

    public static Furniture BigCouch()
    {
        return new Furniture(new Vector2(22, 11), 1.5f, "Prefabs/Big_Couch", "any");
    }//

    public static Furniture BookShelf()
    {
        return new Furniture(new Vector2(12, 4), 5f, "Prefabs/Book_Shelf", "wall");
    }//
    //
    public static Furniture BedroomCounter()
    {
        return new Furniture(new Vector2(10, 5), 5f, "Prefabs/Bedroom_Counter", "wall");
    }///
    //
    public static Furniture BigBed()
    {
        return new Furniture(new Vector2(22, 17), 3.5f, "Prefabs/Big_Bed", "any");
    }//?
    //
    public static Furniture Shelf()
    {
        return new Furniture(new Vector2(10, 4), 5f, "Prefabs/Shelf", "any");
    }//
    //
    public static Furniture MediumBed()
    {
        return new Furniture(new Vector2(22, 14), 3.5f, "Prefabs/Medium_Bed", "any");
    }//

    public static Furniture OfficeTable()
    {
        return new Furniture(new Vector2(14,6), 5f, "Prefabs/Office_Table", "any");
    }//

    public static Furniture Bathtub()
    {
        return new Furniture(new Vector2(17, 7), 0.5f, "Prefabs/Bathtub", "any");
    }//

    public static Furniture Shower()
    {
        return new Furniture(new Vector2(14, 10), 0.5f, "Prefabs/Shower", "wall");
    }//

    public static Furniture Toilet()
    {
        return new Furniture(new Vector2(6, 4), 3f, "Prefabs/Toilet", "wall");
    }// 




}
