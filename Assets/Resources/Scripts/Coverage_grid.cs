using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coverage_grid
{
    //public List<Tile> coverage;

    private float sample_step = 0.5f;

    public Coverage_grid(Environment environment)
    {
        // find max X & Z

        int max_x = 0;
        int max_z = 0;

        foreach (Room room in environment.room_list)
        {
            foreach (Vector2 corner in room.room_corners)
            {
                if (corner[0] > max_x)
                    max_x = (int)corner[0];
                if (corner[1] > max_z)
                    max_z = (int)corner[1];
            }
        }

        // fill coverage list with tiles 
        for (float x = 0; x <= max_x; x += sample_step)
        {
            for (float z = 0; z <= max_z; z += sample_step)
            {
                Vector3 tile_pos = new Vector3(x, - 0.5f, z);
                RaycastHit[] hit_arr = Physics.SphereCastAll(tile_pos, sample_step/2, Vector3.up, 1f);
                
                bool available = true;
                /*RaycastHit wall_hit = new RaycastHit();
                if (Physics.Raycast(new Vector3(x, 25f, z), Vector3.down, out wall_hit, 5f) && wall_hit.transform.tag == "Wall")
                { 
                    available = false;
                }*/
                
                // TODO fix sperecast bullshit

                bool floored = false;
                foreach (RaycastHit hit in hit_arr)
                {
                    //Debug.Log(hit.transform.tag + " at X: " + x + " Y:" + hit.transform.position.y + " Z: " + z + " || " + hit.transform.name);
                    if (hit.transform.tag == "Floor")
                    {
                        floored = true;
                    }
                    if (hit.transform.tag == "Obstacle" || hit.transform.tag == "Wall")
                    {
                        available = false;
                        //Debug.Log("X = " + x + " || Z = " + z + " || Obstacle or wall");
                        break;
                    }

                }
          
                    
                if (available && floored)
                {
                    GameObject.Instantiate(Resources.Load("Prefabs/Invisible_Tile"), new Vector3(x, 0, z), Quaternion.identity);
                    //Debug.DrawLine(new Vector3(x, 0.5f, z), new Vector3(x, -0.5f, z), Color.green, 60f);
                }
                  
            }
        }
    }
}

