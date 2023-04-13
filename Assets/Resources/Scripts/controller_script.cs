using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        System.Random rnd = new System.Random();

        // get new  seed
        int seed = 0;
        if (seed == 0)
        {
            seed = rnd.Next(1, 10000);
        }
        Debug.Log(seed);

        // build environment skeleton
        Environment environment = new Environment(seed);

        // populate each room with furniture
        foreach (Room room in environment.room_list)
        {
            Furniture_grid grid = new Furniture_grid(room);
        }
        // find max x and max z

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

        //spawn agents
        int number_of_agents = 5;
        RaycastHit floor_check = new RaycastHit();

        for (int spawned_agents = 0; spawned_agents < number_of_agents; spawned_agents++)
        {
            Vector2Int spawn_location = new Vector2Int(rnd.Next(0, max_x), rnd.Next(0, max_z));

            if ((Physics.Raycast(new Vector3(spawn_location.x,   31, spawn_location.y),   Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x-2, 31, spawn_location.y),   Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x,   31, spawn_location.y-2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x+2, 31, spawn_location.y),   Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x,   31, spawn_location.y+2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x-2, 31, spawn_location.y-2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x-2, 31, spawn_location.y+2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x+2, 31, spawn_location.y-2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor") ||
                (Physics.Raycast(new Vector3(spawn_location.x+2, 31, spawn_location.y+2), Vector3.down, out floor_check, 31f) && floor_check.transform.tag != "Floor"))
            {
                spawned_agents--;
                continue;
            }
            GameObject.Instantiate(Resources.Load("Prefabs/Agent"), new Vector3(spawn_location.x, 1, spawn_location.y), Quaternion.identity);
        }

        //GameObject.Instantiate(Resources.Load("Prefabs/Agent"), new Vector3(5,30,5), Quaternion.identity);
    }

    // Update is called once per frame

}
