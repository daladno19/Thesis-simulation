using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_grid
{
    // Start is called before the first frame update

    //public int tile_size = 1;

    //public Furniture_tile[,] grid;

    public Furniture_grid(Vector2 room_center, Vector2 room_dimensions, string room_type)
    {
        Furniture_tile[,] grid = new Furniture_tile[(int)room_dimensions[0],(int)room_dimensions[1]];

        int pos_x = (int)room_center[0] - (int)room_dimensions[0] / 2;
        int pos_z = (int)room_center[1] - (int)room_dimensions[1] / 2;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                bool available = true;
                if (Physics.CheckSphere(new Vector3(pos_x, 3, pos_z), 0.5f))
                {
                    available = false;
                }
                grid[i, j] = new Furniture_tile(pos_x, pos_z, available);
                if (!available)
                {
                    Debug.DrawLine(new Vector3(pos_x, 0, pos_z), new Vector3(pos_x, 20, pos_z), Color.red, 60f);
                }
                else
                {
                    Debug.DrawLine(new Vector3(pos_x, 0, pos_z), new Vector3(pos_x, 20, pos_z), Color.gray, 60f);
                }
                pos_z++;
            }
            pos_z = (int)room_center[1] - (int)room_dimensions[1] / 2;
            pos_x++;
        }
        //foreach (Furniture_tile tile in grid)
        //{
        //    if (tile.available)
        //    {
        //        Debug.DrawLine(new Vector3(tile.pos_x, 0, tile.pos_z), new Vector3(tile.pos_x, 20, tile.pos_z), Color.red, 60f);
        //    }
        //    else
        //    {
        //        Debug.DrawLine(new Vector3(tile.pos_x, 0, tile.pos_z), new Vector3(tile.pos_x, 20, tile.pos_z), Color.gray, 60f);

        //    }
        //}
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
