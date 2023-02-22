using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture_grid
{
    public Furniture_tile[,] grid;

    public Furniture_grid(Vector2 room_center, Vector2 room_dimensions, string room_type, string seed)
    {
        this.grid = new Furniture_tile[(int)room_dimensions[0]+1,(int)room_dimensions[1]+1];

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
        while (density < Get_current_density(grid))
        {
            switch (room_type)
            {
                case "kitchen":
                    List<string> furniture_list = new List<string> { "Kitchen_table", "Kitchen_stove", "Kitchen_freezer", "Chair", "Kitchen_drawer" };

                    break;
                case "bedroom":
                    break;
                case "officeroom":
                    break;
                case "livingroom":
                    break;
                case "batroom":
                    break;
            }
        }
        
    }
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
