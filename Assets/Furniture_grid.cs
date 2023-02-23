using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_grid
{
    public Furniture_tile[,] grid;
    public Vector2 room_dimensions;
    public Vector2 room_center;
    public string seed;
    public int iterator;

    // TODO create seed, overlap function 
    public Furniture_grid(Vector2 room_center, Vector2 room_dimensions, string room_type, string seed)
    {
        this.grid = new Furniture_tile[(int)room_dimensions[0]+1,(int)room_dimensions[1]+1];
        this.room_dimensions = room_dimensions;
        this.room_center = room_center;
        System.Random rnd = new System.Random((int)room_center[1]);

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
                    this.grid[i, j] = new Furniture_tile(pos_x, pos_z, false);
                }
                else
                {
                    this.grid[i, j] = new Furniture_tile(pos_x, pos_z, true);
                }
                pos_z++;
            }
            pos_z = (int)room_center[1] - (int)room_dimensions[1] / 2;
            pos_x++;
        }
        // debugging purposes
        Draw_obstacles(this.grid);


        //string[] kitchen_furniture = { "Kitchen_table", "Kitchen_stove", "Kitchen_freezer", "Chair", "Kitchen_drawer"};
        float density = Room_generator.Map(float.Parse(seed.Substring(15, 1)), 1f, 9f, 0.3f, 0.5f);

        Furniture_delegate[] kitchen_furniture = new Furniture_delegate[5];
        kitchen_furniture[0] = new Furniture_delegate(Furniture.kitchenTable);

        while (density < Get_current_density(grid))
        {
            // choose new furniture piece to place
            Furniture furniture_piece;
            switch (room_type)
            {
                case "kitchen":
                    furniture_piece = kitchen_furniture[0](); // TODO random number instead of zero
                    break;
                default:
                    furniture_piece = Furniture.debugBox();
                    break;
            }

            // find all potential placements
            List<Vector2> pot_placements = findPotPoints(furniture_piece);

            // chode random place and place prefab there
            // rewrite nodes as occupied
        }
        
    }
    // function to find all potential furniture placements
    public List <Vector2> findPotPoints(Furniture piece)
    {
        foreach (Furniture_tile tile in this.grid)
        {
            
        }
        return new List<Vector2>();
    }
    public bool Overlaps(Vector2 array_point)
    {
        if (!this.grid[(int)array_point[0], (int)array_point[1]].available)
        {
            return true;
        }
        return false;
    }

    // global coords to array coords
    public Vector2 global_to_array(Vector2 global_coords)
    {
        int array_i = (int)global_coords[0] - (int)this.room_center[0] + (int)this.room_dimensions[0] / 2;
        int array_j = (int)global_coords[1] - (int)this.room_center[1] + (int)this.room_dimensions[1] / 2;
        return new Vector2(array_i, array_j);
    }

    // array coords to global coords
    public Vector2 array_to_global(Vector2 global_coords)
    {
        int pos_x = (int)global_coords[0] + (int)this.room_center[0] - (int)this.room_dimensions[0] / 2;
        int pos_y = (int)global_coords[1] + (int)this.room_center[1] - (int)this.room_dimensions[1] / 2;
        return new Vector2(pos_x, pos_y);
    }

    // fucking magic 0 idea what a delegate is
    public delegate Furniture Furniture_delegate();

    // debugging function to draw rays on all occupied tiles
    public void Draw_obstacles(Furniture_tile[,] grid)
    {
        foreach (Furniture_tile tile in grid)
        {
            if (!tile.available)
            {
                Debug.DrawLine(new Vector3(tile.pos_x, 0, tile.pos_z), new Vector3(tile.pos_x, 20, tile.pos_z), Color.red, 60f);
            }
        }
    }

    // function to calculate current density
    public float Get_current_density(Furniture_tile[,] grid)
    {
        int occupied = 0;
        int all = grid.Length;
        foreach (Furniture_tile tile in grid)
        {
            if (!tile.available) occupied++;
        }
        
        return occupied/all;
    }
}

public class Furniture_tile
{
    public int pos_x;
    public int pos_z;
    public bool available;

    public Furniture_tile(int x, int z, bool available)
    {
        this.pos_x = x;
        this.pos_z = z;
        this.available = available;
    }

}
