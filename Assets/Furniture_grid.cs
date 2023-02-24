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
    public Furniture_grid(Vector2 room_center, Vector2 room_dimensions, string room_type, string seed)
    {
        this.grid = new Furniture_tile[(int)room_dimensions[0] + 1, (int)room_dimensions[1] + 1];
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

        float density = 0.7f;

        Furniture_delegate[] kitchen_furniture = new Furniture_delegate[5];
        kitchen_furniture[0] = new Furniture_delegate(Furniture.kitchenTable);
        Debug.Log("density: " + density + " || curr density" + Get_current_density(this.grid));
        while (density > Get_current_density(this.grid))
        {
            //Debug.Log("went inside");
            // choose new furniture piece to place
            Furniture furniture_piece;
            switch (room_type)
            {
                case "kitchen":
                    furniture_piece = Furniture.debugBox(); // TODO random number instead of zero

                    break;
                default:
                    furniture_piece = Furniture.debugBox();
                    break;
            }

            // find all potential placements
            List<Vector2> pot_placements = findPotPoints(furniture_piece);

            if (pot_placements.Count == 0)
            {
                Debug.Log("cant place any");
                break;
            }
            Vector2 center = pot_placements[rnd.Next(0, pot_placements.Count)];
            Occupy_grid(center, furniture_piece);
            Vector2 global_center = array_to_global(center);
            GameObject.Instantiate(Resources.Load(furniture_piece.path), new Vector3(global_center[0], 0, global_center[1]), Quaternion.identity);
            // chose random place and place prefab there

        }
        Draw_obstacles(this.grid);


    }

    // debugging function
    private void OnDrawGizmos()
    {
        // TODO
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
                if (i < 0 || j < 0 || i > grid.GetLength(0) || j > grid.GetLength(1))
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

    // function to find all potential furniture placements
    public List<Vector2> findPotPoints(Furniture piece)
    {
        List<Vector2> pot_places = new List<Vector2>();
        foreach (Furniture_tile tile in this.grid)
        {
            Vector2 LB_corner = new Vector2(tile.pos_x - piece.dimensions[0] / 2, tile.pos_z - piece.dimensions[0] / 2);
            Vector2 RB_corner = new Vector2(tile.pos_x + piece.dimensions[0] / 2, tile.pos_z - piece.dimensions[0] / 2);
            Vector2 LU_corner = new Vector2(tile.pos_x - piece.dimensions[0] / 2, tile.pos_z + piece.dimensions[0] / 2);
            Vector2 RU_corner = new Vector2(tile.pos_x + piece.dimensions[0] / 2, tile.pos_z + piece.dimensions[0] / 2);

            switch (piece.placement)
            {
                default:
                    if (!tile.available) continue;
                    if (Out_of_bounds(piece, new Vector2(tile.pos_x, tile.pos_z)))
                    {
                        Debug.Log("Out of bounds");
                        continue;
                    }
                    if (Overlaps(new Vector2(tile.array_i, tile.array_j), piece))
                    {
                        Debug.Log("Overlaps");
                        continue;
                    }
                    pot_places.Add(new Vector2(tile.pos_x, tile.pos_z));
                    break;

            }
        }
        return new List<Vector2>();
    }

    // function to check overlaping
    public bool Overlaps(Vector2 array_pos, Furniture piece)
    {
        for (int i = (int)array_pos[0] - (int)piece.dimensions[0] / 2;
            i < (int)array_pos[0] + (int)piece.dimensions[0] / 2; i++)
        {
            for (int j = (int)array_pos[1] - (int)piece.dimensions[1] / 2;
                j < (int)array_pos[1] + (int)piece.dimensions[1] / 2; j++)
            {
                if (i < 0 || j < 0 || i > this.grid.GetLength(0) || j > this.grid.GetLength(1))
                {
                    continue;
                }
                if (!this.grid[i, j].available)
                {
                    return true;
                }
            }
        }
        return false;
    }
    // vroken sed: 954445584575241534256184552323454
    public bool Overlaps(Vector2 array_point)
    {
        array_point = global_to_array(array_point); // now it's actually array coords and not global
        Debug.Log("||point: " + array_point + "bounds: " + this.grid.GetLength(0) + " x " + this.grid.GetLength(1));
        if (array_point[0] < 0 || array_point[1] < 0 ||
            array_point[0] > this.grid.GetLength(0) ||
            array_point[1] > this.grid.GetLength(1))
        {
            return true;
        }
        if (!this.grid[(int)array_point[0], (int)array_point[1]].available) // TODO
        {
            return true;
        }
        return false;
    }

    // function to check out of bounds
    public bool Out_of_bounds(Furniture piece, Vector2 pos_arr)
    {
        Vector2 LB_corner = new Vector2(pos_arr[0] - piece.dimensions[0] / 2, pos_arr[1] - piece.dimensions[1] / 2);
        Vector2 RB_corner = new Vector2(pos_arr[0] + piece.dimensions[0] / 2, pos_arr[1] - piece.dimensions[1] / 2);
        Vector2 LU_corner = new Vector2(pos_arr[0] - piece.dimensions[0] / 2, pos_arr[1] + piece.dimensions[1] / 2);
        Vector2 RU_corner = new Vector2(pos_arr[0] + piece.dimensions[0] / 2, pos_arr[1] + piece.dimensions[1] / 2);
        //Debug.Log("||LB=" + LB_corner + "||LU=" + LU_corner+"||LB=" + RB_corner+"||LB=" + RU_corner);
        //Debug.Log(this.grid.GetLength(0) + " x " + this.grid.GetLength(1));
        if (LB_corner[0] < 0 ||
            LB_corner[1] < 0 ||
            RB_corner[0] > this.grid.GetLength(0) ||
            RU_corner[1] > this.grid.GetLength(1))
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
