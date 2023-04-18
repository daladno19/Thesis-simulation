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
        int seed = 7873; //1978 - small seed // 7873 - broken seed
        if (seed == 0)
        {
            seed = rnd.Next(1, 10000);
        }
        Debug.Log(seed);

        System.Random rnd_seed = new System.Random(seed);
        // build environment skeleton
        Environment environment = new Environment(seed);

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



        // populate each room with furniture
        foreach (Room room in environment.room_list)
        {
            Furniture_grid grid = new Furniture_grid(room);
        }





        //spawn agents
        int number_of_agents = 5;
        Quaternion[] rotations = { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 270, 0) };

        do
        {
            Vector3 pot_pos = new Vector3(rnd_seed.Next(0, max_x), 35, rnd_seed.Next(0, max_z));

            if (viablePos(pot_pos, number_of_agents))
            {
                GameObject agent = GameObject.Instantiate(Resources.Load("Prefabs/Agent"), new Vector3(pot_pos.x, 1, pot_pos.z), rotations[rnd.Next(0, 4)]) as GameObject;
                agent.GetComponent<Agent>().id = number_of_agents;
                number_of_agents--;
            }

        }
        while (number_of_agents != 0);

        Coverage_grid coverage = new Coverage_grid(environment);
    }

    public bool viablePos(Vector3 pos, int id)
    {
        RaycastHit hit = new RaycastHit();
        bool floored = false;
        if ((Physics.Raycast(pos + new Vector3(0, 0, -2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(0, 0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(0, 0, +2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0, -2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0, +2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2, 0, -2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2, 0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2, 0, +2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor"))
        {
            floored = true;
        }
        bool obstacle = false;

        RaycastHit[] hit_arr = Physics.BoxCastAll(pos, new Vector3(2, 1, 2), Vector3.up * -1);
        foreach (RaycastHit hitt in hit_arr)
        {
            if (hitt.transform.tag == "Obstacle" || hitt.transform.tag == "Wall" || hitt.transform.tag == "Agent")
                obstacle = true;
        }
        //Debug.Log("id: " + id + " || floored: " + floored + " || obstacle: " + obstacle + " || pos: " + pos);

        if (floored && !obstacle)
            return true;
        return false;
    }
}
