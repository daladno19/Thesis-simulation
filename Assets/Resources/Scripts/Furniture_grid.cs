using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_grid
{
    public Furniture_tile[,] grid;
    public Vector2 room_dimensions;
    public Vector2 room_center;

    // furniture grid constructor
    public Furniture_grid(Room room)
    {
        this.grid = new Furniture_tile[(int)room.room_dimensions[0] + 1, (int)room.room_dimensions[1] + 1];
        this.room_dimensions = room.room_dimensions;
        this.room_center = room.room_center;

        // init seeded random
        System.Random rnd = new System.Random((int)room_center[1] + (int)room_center[0]);

        // populate grid with tiles
        int pos_x = (int)room_center[0] - (int)room_dimensions[0] / 2;
        int pos_z = (int)room_center[1] - (int)room_dimensions[1] / 2;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (pos_x == (int)room_center[0] - (int)room_dimensions[0] / 2 ||
                    pos_x == (int)room_center[0] + (int)room_dimensions[0] / 2 ||
                    pos_z == (int)room_center[1] - (int)room_dimensions[1] / 2 ||
                    pos_z == (int)room_center[1] + (int)room_dimensions[1] / 2)
                {
                    this.grid[i, j] = new Furniture_tile(pos_x, pos_z, false,i,j);
                }
                else
                {
                    this.grid[i, j] = new Furniture_tile(pos_x, pos_z, true,i,j);
                }
                pos_z++;
            }
            pos_z = (int)room_center[1] - (int)room_dimensions[1] / 2;
            pos_x++;
        }

        // occupy grid near doors
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            // check if door is in this room
            if (door.transform.position.x < this.room_center[0] - this.room_dimensions[0] / 2 ||
                door.transform.position.x > this.room_center[0] + this.room_dimensions[0] / 2 ||
                door.transform.position.z < this.room_center[1] - this.room_dimensions[1] / 2 ||
                door.transform.position.z > this.room_center[1] + this.room_dimensions[1] / 2)
            {
                continue;
            }
            Occupy_grid(global_to_array(new Vector2(door.transform.position.x, door.transform.position.z)), Furniture.door());
            //occupy grid around
        }
        //float density = 0.2f;

        Furniture_delegate[] debug_furniture = new Furniture_delegate[3];
        debug_furniture[0] = new Furniture_delegate(Furniture.debugBox);
        debug_furniture[1] = new Furniture_delegate(Furniture.debugBigBox);
        debug_furniture[2] = new Furniture_delegate(Furniture.debugLongBox);

        Furniture_delegate[] kitchen_furniture = new Furniture_delegate[10];
        kitchen_furniture[0] = new Furniture_delegate(Furniture.Fridge);
        kitchen_furniture[1] = new Furniture_delegate(Furniture.BigFridge);
        kitchen_furniture[2] = new Furniture_delegate(Furniture.KitchenTable);
        kitchen_furniture[3] = new Furniture_delegate(Furniture.SmallCounter);
        kitchen_furniture[4] = new Furniture_delegate(Furniture.Stove);
        kitchen_furniture[5] = new Furniture_delegate(Furniture.MediumCounter);
        kitchen_furniture[6] = new Furniture_delegate(Furniture.KitchenShelf);
        kitchen_furniture[7] = new Furniture_delegate(Furniture.BigCounter);
        kitchen_furniture[8] = new Furniture_delegate(Furniture.Stool);
        kitchen_furniture[9] = new Furniture_delegate(Furniture.SquareTable);

        Furniture_delegate[] bathroom_furniture = new Furniture_delegate[3];
        bathroom_furniture[0] = new Furniture_delegate(Furniture.Bathtub);
        bathroom_furniture[1] = new Furniture_delegate(Furniture.Toilet);
        bathroom_furniture[2] = new Furniture_delegate(Furniture.Shower);

        Furniture_delegate[] office_furniture = new Furniture_delegate[6];
        office_furniture[0] = new Furniture_delegate(Furniture.OfficeTable);
        office_furniture[1] = new Furniture_delegate(Furniture.Puff);
        office_furniture[2] = new Furniture_delegate(Furniture.BookShelf);
        office_furniture[3] = new Furniture_delegate(Furniture.GlassShelf);
        office_furniture[4] = new Furniture_delegate(Furniture.Couch);
        office_furniture[5] = new Furniture_delegate(Furniture.GlassTable);

        Furniture_delegate[] livingroom_furniture = new Furniture_delegate[10];
        livingroom_furniture[0] = new Furniture_delegate(Furniture.Buffet);
        livingroom_furniture[1] = new Furniture_delegate(Furniture.Sofa);
        livingroom_furniture[2] = new Furniture_delegate(Furniture.BigShelf);
        livingroom_furniture[3] = new Furniture_delegate(Furniture.Puff);
        livingroom_furniture[4] = new Furniture_delegate(Furniture.BigCouch);
        livingroom_furniture[5] = new Furniture_delegate(Furniture.ShoeRack);
        livingroom_furniture[6] = new Furniture_delegate(Furniture.Speaker);
        livingroom_furniture[7] = new Furniture_delegate(Furniture.TvTable);
        livingroom_furniture[8] = new Furniture_delegate(Furniture.Shelf);
        livingroom_furniture[9] = new Furniture_delegate(Furniture.SmallTable);
        
        Furniture_delegate[] bedroom_furniture = new Furniture_delegate[5];
        bedroom_furniture[0] = new Furniture_delegate(Furniture.MediumBed);
        bedroom_furniture[1] = new Furniture_delegate(Furniture.BigBed);
        bedroom_furniture[2] = new Furniture_delegate(Furniture.BedroomCounter);
        bedroom_furniture[3] = new Furniture_delegate(Furniture.Stool);
        bedroom_furniture[4] = new Furniture_delegate(Furniture.BigShelf);
       

        // choose correct furniture pool according to room type
        Furniture_delegate[] furniture_array;
        switch (room.room_type)
        {
            case "bedroom":
                furniture_array = bedroom_furniture;
                break;
            case "bathroom":
                furniture_array = bathroom_furniture;
                break;
            case "officeroom":
                furniture_array = office_furniture;
                break;
            case "livingroom":
                furniture_array = livingroom_furniture;
                break;
            case "kitchen":
                furniture_array = kitchen_furniture;
                break;
            default:
                furniture_array = debug_furniture;
                break;
        }

        int furniture_num = rnd.Next((int)(room_dimensions[0] * room_dimensions[1]) / 500, (int)(room_dimensions[0] * room_dimensions[1]) / 300);

        for (int i = 0; i < furniture_num; i++)
        {
            // pick random piece from pool
            Furniture piece = furniture_array[rnd.Next(0, furniture_array.Length)]();

            //randomly rotate if furniture can be placed anywhere
            if (rnd.Next(2) == 1)
            {
                piece.rotation = Quaternion.Euler(0,90,0);
                float temp_dim = piece.dimensions[0];
                piece.dimensions[0] = piece.dimensions[1];
                piece.dimensions[1] = temp_dim;
            }

            // get random point to place
            List<Vector2> pot_points = findPotPoints(piece);
            if (pot_points.Count == 0)
            {
                continue;
            }
            Vector2 placement = pot_points[rnd.Next(0, pot_points.Count)];

            // place prefab
            GameObject.Instantiate(Resources.Load(piece.path),new Vector3(placement[0],piece.height,placement[1]), piece.rotation);

            // occupy grid
            Occupy_grid(global_to_array(placement), piece);
        }
        //Draw_obstacles(this.grid);
    }

    // function to occupy tiles taken by new furniture piece
    public void Occupy_grid(Vector2 center_arr, Furniture piece)
    {
        for (int i = (int)center_arr[0] - (int)piece.dimensions[0] / 2 - 3;
            i < (int)center_arr[0] + (int)piece.dimensions[0] / 2 + 3; i++)
        {
            for (int j = (int)center_arr[1] - (int)piece.dimensions[1] / 2 - 3;
            j < (int)center_arr[1] + (int)piece.dimensions[1] / 2 + 3; j++)
            {
                if (i < 0 || j < 0 || i >= grid.GetLength(0) || j >= grid.GetLength(1))
                {
                    continue;
                }
                else
                {
                    this.grid[i, j].available = false;
                }
            }
        }
    }

    // function to find all potential furniture placements | returns  local coords for some dumb reason
    public List<Vector2> findPotPoints(Furniture piece)
    {
        List<Vector2> pot_places = new List<Vector2>();

        switch (piece.placement)
        {
            case "wall":
                foreach (Furniture_tile tile in this.grid)
                {
                    if (tile.pos_x + piece.dimensions[0] / 2 != room_center[0] + room_dimensions[0] / 2 &&
                        tile.pos_x - piece.dimensions[0] / 2 != room_center[0] - room_dimensions[0] / 2 + 1 &&
                        tile.pos_z + piece.dimensions[1] / 2 != room_center[1] + room_dimensions[1] / 2 &&
                        tile.pos_z - piece.dimensions[1] / 2 != room_center[1] - room_dimensions[1] / 2 + 1)
                    {
                        continue;
                    }
                    if (!IsValid(tile, piece))
                    {
                        continue;
                    }
                    pot_places.Add(new Vector2(tile.pos_x, tile.pos_z));
                    //Debug.DrawLine(new Vector3(tile.pos_x, 0, tile.pos_z), new Vector3(tile.pos_x, 15, tile.pos_z), Color.yellow, 60f);
                }
                break;

            default: // can be placed anywhere (placeholder)
                foreach (Furniture_tile tile in this.grid)
                {
                    if (!IsValid(tile, piece)) continue;
                    pot_places.Add(new Vector2(tile.pos_x, tile.pos_z));
                    //Debug.DrawLine(new Vector3(tile.pos_x, 0, tile.pos_z), new Vector3(tile.pos_x, 5, tile.pos_z), Color.yellow, 60f);
                }
                break;
        }

        return pot_places;
    }

    // function to check wether placement is valid
    public bool IsValid(Furniture_tile tile, Furniture piece)
    {
        int length = (int)piece.dimensions[0]; // x
        int width  = (int)piece.dimensions[1]; // z
        int x_arr_bounds = this.grid.GetLength(0);
        int z_arr_bounds = this.grid.GetLength(1);

        // check out of bounds
        if (tile.array_i - length / 2 < 0 ||
            tile.array_j - width / 2  < 0 ||
            tile.array_i + length / 2 >= x_arr_bounds ||
            tile.array_j + width / 2  >= z_arr_bounds)
        {
            //Debug.Log("out of bounds");
            return false;
        }
        // check overlaping
        for (int i = tile.array_i - length / 2; i < tile.array_i + length / 2; i++)
        {
            for (int j = tile.array_j - width / 2; j < tile.array_j + width / 2; j++)
            {
                if (!grid[i, j].available)
                {
                    //Debug.Log("overlaps");
                    return false;
                }
            }
        }
        return true;
    }

    // global coords to array coords
    public Vector2 global_to_array(Vector2 global_coords)
    {
        int array_i = (int)global_coords[0] - (int)this.room_center[0] + (int)this.room_dimensions[0] / 2;
        int array_j = (int)global_coords[1] - (int)this.room_center[1] + (int)this.room_dimensions[1] / 2;
        return new Vector2(array_i, array_j);
    }

    // fucking magic 0 idea what a delegate is
    public delegate Furniture Furniture_delegate();
}

public class Furniture_tile
{
    public int pos_x; // global x value
    public int pos_z; // global z value

    public int array_i; // local x value
    public int array_j; // local z value

    public bool available;

    public Furniture_tile(int x, int z, bool available, int i, int j)
    {
        this.pos_x = x;
        this.pos_z = z;
        this.available = available;
        this.array_i = i;
        this.array_j = j;
    }

}
