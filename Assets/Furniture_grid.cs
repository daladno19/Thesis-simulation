using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_grid
{
    public Furniture_tile[,] grid;
    public Vector2 room_dimensions;
    public Vector2 room_center;

    // TODO create seed, overlap function 
    public Furniture_grid(Vector2 room_center, Vector2 room_dimensions, string room_type)
    {
        this.grid = new Furniture_tile[(int)room_dimensions[0] + 1, (int)room_dimensions[1] + 1];
        this.room_dimensions = room_dimensions;
        this.room_center = room_center;

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

        float density = 0.2f;

        Furniture_delegate[] kitchen_furniture = new Furniture_delegate[5];
        kitchen_furniture[0] = new Furniture_delegate(Furniture.kitchenTable);

        //Debug.Log("density: " + density + " || curr density" + Get_current_density(this.grid));

        Furniture piece = Furniture.debugBox();

        List<Vector2> pot = findPotPoints(piece);

        Vector2 center = pot[rnd.Next(0,pot.Count)];
        Debug.Log(pot.Count);
        foreach (Vector2 point in pot)
        {
            Debug.DrawLine(new Vector3(point[0], 0, point[1]), new Vector3(point[0], 5, point[1]), Color.yellow, 60f);
        }

        GameObject.Instantiate(Resources.Load(piece.path), new Vector3(center[0], 2, center[1]), Quaternion.identity);
        Occupy_grid(global_to_array(center), piece);

        /*int k = 0;
        while (density > Get_current_density(this.grid) || k < 10)
        {
            // choose new furniture piece to place
            Furniture furniture_piece;
            switch (room_type)
            {
                case "kitchen":
                    furniture_piece = Furniture.debugBox();

                    break;
                default:
                    furniture_piece = Furniture.debugBox();
                    break;
            }

            // find all potential placements
            List<Vector2> pot_placements = findPotPoints(furniture_piece);

            // if cant place specific furniture, give up
            if (pot_placements.Count == 0 || k == 10)
            {
                Debug.Log("cant place any");
                break;
            }

            // pick a random valid placement
            Vector2 center = pot_placements[rnd.Next(0, pot_placements.Count)];

            // occupy grid under new object
            Occupy_grid(global_to_array(center), furniture_piece);

            // place a prefab
            GameObject.Instantiate(Resources.Load(furniture_piece.path), new Vector3(center[0], 0, center[1]), Quaternion.identity);
            k++;
        }*/


        Draw_obstacles(this.grid);


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
            default: // can be placed anywhere (placeholder)
                foreach (Furniture_tile tile in this.grid)
                {
                    if (!IsValid(tile, piece)) continue;
                    pot_places.Add(new Vector2(tile.pos_x, tile.pos_x));
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
        float occupied = 0;
        float all = grid.Length;
        //Debug.Log("occupied: " + occupied + " || all" + all) ;

        foreach (Furniture_tile tile in grid)
        {
            if (!tile.available) occupied++;
            //Debug.Log("YEp");
        }
        return occupied/all;
    }
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
