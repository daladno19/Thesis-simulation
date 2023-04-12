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
    }

    // Update is called once per frame

}
