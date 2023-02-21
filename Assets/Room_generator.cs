using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Room_generator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // generating 32-digit long seed
        string seed = "";
        seed = "933632431337312885764214731722567";   // for debugging purposes
        short seed_length = 32;
        if (seed == "")
        { 
            for (int i = 0; i < seed_length; i++)
            {
                if (i == 0)
                {
                    seed = seed + "9"; // debug max room num
                }
                seed = seed + UnityEngine.Random.Range(1, 9);
            }
        }
        Debug.Log("Seed: " + seed);
        /* seed transcript:
        =====================                                 
        1  - number of rooms                                           north          
        2  - 1st room x size                                             +Z  
        3  - 2nd room x size                                             |              
        4  - 3rd room x size                                  west -X -- + -- +X east     
        5  - 4th room x size                                             |              
        6  - 5th room x size                                             -Z          
        7  - 1st room z size                                           south
        8  - 2nd room z size & 1st random room type
        9  - 3rd room z size & 2nd random room type   
        10 - 4th room z size & 3rd random room type
        11 - 5th room z size & 4th random room type
        12 - 2nd room center & 5th random room type
        13 - 3rd room center
        14 - 4th room center
        15 - 5th room center
        16 -
        17 - 
        18 - 
        19 - 
        20 - 
        21 -
        22 -
        23 - 
        24 -
        25 - 
        26 - 
        27 - 
        28 - 
        29 - 
        30 - 
        31 - 
        32 -
        =====================
        */
        //int door_width = 10;
        
        int number_of_rooms = Random_int_in_scale(seed, 0, 1, 9, 2, 5);
        string[] room_types = { "bedroom", "bathroom", "kitchen", "livingroom", "officeroom" };
        Vector2[] room_type_dimensions = { new Vector2(30, 70), new Vector2(20, 40), new Vector2(30, 60), new Vector2(40, 80), new Vector2(30, 50) };
        List<Room> room_list = new List<Room>();

        // generating first room
        Vector2 new_dimensions = new Vector2(Round_to_tens(Random_int_in_scale(seed, 0, 1, 9, (int)room_type_dimensions[0][0], (int)room_type_dimensions[0][1])),
                                            Round_to_tens(Random_int_in_scale(seed, 6, 1, 9, (int)room_type_dimensions[0][0], (int)room_type_dimensions[0][1])));
        Vector2 new_center = new Vector2(new_dimensions[0] / 2, new_dimensions[1] / 2);
        room_list.Add(new Room(new_center, new_dimensions, 1, "bedroom"));

        // generating other rooms
        for (int i = 1; i < number_of_rooms; i++)
        {
            // generate random room dimensions and type
            int room_type = (int)Mathf.Round(Map(float.Parse(seed.Substring(i+6,1)),1,9,0,4));
            new_dimensions = new Vector2(Round_to_tens(Random_int_in_scale(seed, i,   1, 9, (int)room_type_dimensions[room_type][0], (int)room_type_dimensions[room_type][1])),
                                         Round_to_tens(Random_int_in_scale(seed, i+6, 1, 9, (int)room_type_dimensions[room_type][0], (int)room_type_dimensions[room_type][1])));
            
            // find a list of all potential room placement center points
            List<Vector2> pot_points = Find_Pot_Points(room_list, new_dimensions);
            
            // get random point to build room from
            new_center = pot_points[Random_int_in_scale(seed, i + 10, 1, 9, 0, pot_points.Count - 1)];
            
            // place a room on that point
            // add room to room array
            room_list.Add(new Room(new_center, new_dimensions, i+1, room_types[room_type]));


            Debug.Log("|| room ID: " + room_list[i].room_ID + "|| room type: " + room_list[i].room_type);
            // get new room
            Room new_room  = room_list[i];
            
            //find prev room, to which new one got connected
            Room prev_room = Find_connected_room(new_room, room_list);
            Debug.DrawLine(new Vector3(new_room.room_center[0], 5, new_room.room_center[1]), new Vector3(prev_room.room_center[0], 5, prev_room.room_center[1]), Color.red, 60f);
            
            //make a door between new_room and prev_room |===============| ToFix |===============|
            Cut_door(new_room, prev_room);
        }
        // TODO populate with furniture
    }

    // function to make door between two rooms
    public void Cut_door(Room room1, Room room2)
    {
        if (room1.room_corners[0][0] == room2.room_corners[2][0] || room2.room_corners[0][0] == room1.room_corners[2][0])
        {
            // room connection is horizontal
            int x_bounds;
            if (room1.room_corners[0][0] == room2.room_corners[1][0])
            {
                // room1 => right || room2 => left
                x_bounds = (int)room1.room_corners[0][0];

            }
            else
            {
                // room1 => left || room2 => right
                x_bounds = (int)room1.room_corners[1][0];
            }
            int[] z_points = { (int)room1.room_corners[0][1], (int)room1.room_corners[3][1], (int)room2.room_corners[0][1], (int)room2.room_corners[3][1]};
            Array.Sort(z_points);

            Debug.DrawLine(new Vector3(x_bounds, 11, z_points[0]),new Vector3(x_bounds, 11, z_points[3]), Color.green, 60f);
            Debug.DrawLine(new Vector3(x_bounds, 0, z_points[1]), new Vector3(x_bounds, 30, z_points[1]), Color.blue, 60f);
            Debug.DrawLine(new Vector3(x_bounds, 0, z_points[2]), new Vector3(x_bounds, 30, z_points[2]), Color.blue, 60f);
            
            Vector2 door = new Vector2(x_bounds, (z_points[1]+z_points[2])/2);
            Debug.DrawLine(new Vector3(door[0], 0, door[1]), new Vector3(door[0], 30, door[1]), Color.yellow, 60f);
            Room.Build_wall_w_door(new Vector2(x_bounds, z_points[0]), new Vector2(x_bounds, z_points[3]), door, "vertical");
        }
        else 
        {
            int z_bounds;
            //rooms connection is vertical
            if (room1.room_corners[0][1] == room2.room_corners[3][1])
            {
                // room 1 => top || room2 => bottom
                z_bounds = (int)room1.room_corners[0][1];
            }
            else 
            {
                // room1 => bottom || room2 => top
                z_bounds = (int)room2.room_corners[0][1];
            }
            int[] x_points = {(int)room1.room_corners[0][0], (int)room1.room_corners[1][0], (int)room2.room_corners[0][0], (int)room2.room_corners[1][0]};
            Array.Sort(x_points);

            Debug.DrawLine(new Vector3(x_points[0], 11, z_bounds), new Vector3(x_points[3], 11, z_bounds), Color.green, 60f);
            Debug.DrawLine(new Vector3(x_points[1], 0, z_bounds), new Vector3(x_points[1], 30, z_bounds), Color.blue, 60f);
            Debug.DrawLine(new Vector3(x_points[2], 0, z_bounds), new Vector3(x_points[2], 30, z_bounds), Color.blue, 60f);

            Vector2 door = new Vector2((x_points[1] + x_points[2])/2,z_bounds);
            Debug.DrawLine(new Vector3(door[0], 0, door[1]), new Vector3(door[0], 30, door[1]), Color.yellow, 60f);
            Room.Build_wall_w_door(new Vector2(x_points[0], z_bounds), new Vector2(x_points[3], z_bounds), door, "horizontal");
        }
    }

    // function to find a room which is connected (sharing a wall)
    public Room Find_connected_room(Room new_room, List<Room> room_list)
    {
        foreach (Room pot_room in room_list)
        {
            if (pot_room.room_ID == new_room.room_ID)
            {
                continue;
            }
            if (Room_close(new_room, pot_room)) 
            {
                continue;
            }
            if (Sharing_wall(new_room, pot_room))
            {
                return pot_room;
            }
        }

        Debug.Log("Found none !!!!!!!!! Huge Issue");
        return new_room;
    }

    // function to check if rooms are close
    public bool Room_close(Room room1, Room room2)
    {
        int x_dist = Mathf.Abs((int)room1.room_center[0] - (int)room2.room_center[0]);
        int z_dist = Mathf.Abs((int)room1.room_center[1] - (int)room2.room_center[1]);
        if (x_dist < ((int)room1.room_dimensions[0] / 2) + ((int)room2.room_dimensions[0] / 2))
        {
            return false;
        }
        if (z_dist < ((int)room1.room_dimensions[1] / 2) + ((int)room2.room_dimensions[1] / 2))
        {
            return false;
        }
        return true;
    }

    // function to check if walls are touching
    public bool Sharing_wall(Room room1, Room room2)
    {
        if ((room1.room_corners[1][0] > room2.room_corners[0][0] || 
            room1.room_corners[0][0] < room2.room_corners[1][0]) &&
            room1.room_corners[1][1] == room2.room_corners[2][1])
        {
            return true; // south
        }
        if ((room1.room_corners[1][0] > room2.room_corners[0][0] ||
            room1.room_corners[0][0] < room2.room_corners[1][0]) &&
            room1.room_corners[2][1] == room2.room_corners[1][1])
        {
            return true; // north
        }
        if ((room1.room_corners[3][1] > room2.room_corners[0][1] ||
            room1.room_corners[0][1] < room2.room_corners[3][1]) &&
            room1.room_corners[1][0] == room2.room_corners[0][0])
        {
            return true; // west
        }
        if ((room1.room_corners[3][1] > room2.room_corners[0][1] ||
            room1.room_corners[0][1] < room2.room_corners[3][1]) &&
            room1.room_corners[0][0] == room2.room_corners[1][0])
        {
            return true; // east
        }


        return false;
    }

    // function to find all possible room placements
    public List<Vector2> Find_Pot_Points (List<Room> room_list, Vector2 new_dimensions)
    {
        List<Vector2> PotPoints = new List<Vector2>();
        //Debug.Log("trying to find pot points");
        int x = (int)new_dimensions[0];
        int z = (int)new_dimensions[1];

        for (int i = 0; i < room_list.Count; i++)
        {
            Room room = room_list[i];
            int pot_x = 0;
            int pot_z = 0;

            // north
            for (int j = (int)room.room_corners[3][0] - (x / 2); j <= (int)room.room_corners[2][0] + (x / 2); j += 10) // points above room
            {
                pot_x = j;
                pot_z = (int)(room.room_center[1] + (z / 2) + (room.room_dimensions[1] / 2));
                Vector2 pot_left_bottom_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner= new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner  = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);

                if (pot_left_bottom_corner[0] < 0 || pot_right_bottom_corner[0] < 0)
                {
                    //Debug.Log("Left corner x value below zero: " + pot_left_corner[0]);
                    continue;
                }
                if (Intersects_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (Overlaps_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }

                if (pot_right_bottom_corner[0] <= room.room_corners[3][0])
                {
                    //Debug.Log("right corner too far to the left: " + pot_left_corner[0] + "l");
                    continue;
                }
                if (pot_right_bottom_corner[0] >= room.room_corners[2][0])
                {
                    //Debug.Log("left corner too far to the right: " + pot_right_corner[0]);
                    continue;
                }

                PotPoints.Add(new Vector2(pot_x, pot_z));
            }
            
            // south
            for (int j = (int)room.room_corners[3][0] - (x / 2); j <= (int)room.room_corners[2][0] + (x / 2); j += 10) // points below room
            {
                pot_x = j;
                pot_z = (int)(room.room_center[1] - (z / 2) - (room.room_dimensions[1] / 2));
                Vector2 pot_left_bottom_corner  = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner   = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner  = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);

                if (pot_left_bottom_corner[0] < 0 || pot_right_bottom_corner[0] < 0 || pot_left_bottom_corner[1] < 0)
                {
                    //Debug.Log("value below zero: " + pot_left_corner[0]);
                    continue;
                }
                if (Intersects_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (Overlaps_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (pot_right_upper_corner[0] <= room.room_corners[3][0])
                {
                    //Debug.Log("right corner too far to the left: " + pot_left_corner[0] + "l");
                    continue;
                }
                if (pot_left_upper_corner[0] >= room.room_corners[2][0])
                {
                    //Debug.Log("left corner too far to the right: " + pot_right_corner[0]);
                    continue;
                }
                PotPoints.Add(new Vector2(pot_x, pot_z));
            }

            // west
            for (int j = (int)room.room_corners[0][1] - (z / 2); j <= (int)room.room_corners[3][1] + (z / 2); j += 10) // points to the left of the room
            {
                pot_x = (int)(room.room_center[0] - (x / 2) - (room.room_dimensions[0] / 2));
                pot_z = j;
                Vector2 pot_left_bottom_corner  = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner   = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner  = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                if (pot_left_bottom_corner[0] < 0 || pot_left_bottom_corner[1] < 0)
                {
                    //Debug.Log("value below zero: " + pot_left_corner[0]);
                    continue;
                }
                if (Intersects_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (Overlaps_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (pot_right_upper_corner[1] <= room.room_corners[0][1])
                {
                    //Debug.Log("right corner too far to the left: " + pot_left_corner[0] + "l");
                    continue;
                }
                if (pot_right_bottom_corner[1] >= room.room_corners[3][1])
                {
                    //Debug.Log("left corner too far to the right: " + pot_right_corner[0]);
                    continue;
                }
                PotPoints.Add(new Vector2(pot_x, pot_z));
            }

            // east
            for (int j = (int)room.room_corners[0][1] - (z / 2); j <= (int)room.room_corners[3][1] + (z / 2); j += 10) // points to the left of the room
            {
                pot_x = (int)(room.room_center[0] + (x / 2) + (room.room_dimensions[0] / 2));
                pot_z = j;
                Vector2 pot_left_bottom_corner  = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner   = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner  = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                if (pot_left_bottom_corner[1] < 0)
                {
                    //Debug.Log("value below zero: " + pot_left_corner[0]);
                    continue;
                }
                if (Intersects_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (Overlaps_improved(new Vector2(pot_x, pot_z), new_dimensions, room_list))
                {
                    //Debug.Log("Potential rooms overlaps another room");
                    continue;
                }
                if (pot_left_upper_corner[1] <= room.room_corners[0][1])
                {
                    //Debug.Log("right corner too far to the left: " + pot_left_corner[0] + "l");
                    continue;
                }
                if (pot_left_bottom_corner[1] >= room.room_corners[3][1])
                {
                    //Debug.Log("left corner too far to the right: " + pot_right_corner[0]);
                    continue;
                }
                PotPoints.Add(new Vector2(pot_x, pot_z));
            }
        }
        return PotPoints;
    }

    // function to check if point is inside any of the rooms
    public bool Intersects(Vector2 point, List <Room> room_list)
    {
        Vector2 x_bounds = new Vector2();
        Vector2 z_bounds = new Vector2();
        for (int i = 0; i < room_list.Count; i++)
        {
            Room room = room_list[i];
            x_bounds = new Vector2(room.room_corners[0][0], room.room_corners[1][0]);   // lower x, upper x
            z_bounds = new Vector2(room.room_corners[0][1], room.room_corners[3][1]);   // lower z, upper z
            if (point[0] > x_bounds[0] &&
                point[0] < x_bounds[1] &&
                point[1] > z_bounds[0] &&
                point[1] < z_bounds[1])
            {
                //Debug.Log("Intersects1");
                return true;
            }
        }
        return false;
    }

    // function to check that potential room doesnt intersect any other room
    public bool Intersects_improved(Vector2 pot_center, Vector2 pot_dimensions, List<Room> room_list)
    {
        Vector2 pot_LB = new Vector2(pot_center[0] - pot_dimensions[0] / 2, pot_center[1] - pot_dimensions[1] / 2);
        Vector2 pot_RB = new Vector2(pot_center[0] + pot_dimensions[0] / 2, pot_center[1] - pot_dimensions[1] / 2);
        Vector2 pot_LU = new Vector2(pot_center[0] - pot_dimensions[0] / 2, pot_center[1] + pot_dimensions[1] / 2);
        Vector2 pot_RU = new Vector2(pot_center[0] + pot_dimensions[0] / 2, pot_center[1] + pot_dimensions[1] / 2);

        if (Intersects(pot_LB + new Vector2(1, 1),   room_list)) return true;
        if (Intersects(pot_RB + new Vector2(-1, 1),  room_list)) return true;
        if (Intersects(pot_LU + new Vector2(1, -1),  room_list)) return true;
        if (Intersects(pot_RU + new Vector2(-1, -1), room_list)) return true;

        return false;
    }

    // function to check that potential room doesnt overlap any other room
    public bool Overlaps_improved(Vector2 pot_center, Vector2 pot_dimensions, List<Room> room_list)
    {
        int pot_x_max = (int)pot_center[0] + (int)pot_dimensions[0] / 2;
        int pot_x_min = (int)pot_center[0] - (int)pot_dimensions[0] / 2; // GREEN
        int pot_z_max = (int)pot_center[1] + (int)pot_dimensions[1] / 2; // green
        int pot_z_min = (int)pot_center[1] - (int)pot_dimensions[1] / 2;
        
        int x_max;
        int x_min;  // BLUE
        int z_max;  // blue
        int z_min;

        foreach (Room room in room_list)
        {
            x_max = (int)room.room_corners[1][0];
            x_min = (int)room.room_corners[3][0];
            z_max = (int)room.room_corners[3][1];
            z_min = (int)room.room_corners[1][1];

            if (pot_x_max <= x_min || pot_x_min >= x_max)
            {
                return false;
            }
            if (pot_z_max <= z_min || pot_z_min >= z_max)
            {
                return false;
            }
            //if (Intersects(new Vector2(x_max - 2, z_max - 2), room_list)||
            //    Intersects(new Vector2(x_max - 2, z_min + 2), room_list)||
            //    Intersects(new Vector2(x_min + 2, z_max - 2), room_list)||
            //    Intersects(new Vector2(x_min + 2, z_min + 2), room_list))
            //{
            //    return true;
            //}
            //Debug.DrawLine(new Vector3(x_max - 2, 0, z_max - 2), new Vector3(x_max - 2, 20, z_max - 2), Color.red, 60f);
            //Debug.DrawLine(new Vector3(x_max - 2, 0, z_min + 2), new Vector3(x_max - 2, 20, z_min + 2), Color.red, 60f);
            //Debug.DrawLine(new Vector3(x_min + 2, 0, z_max - 2), new Vector3(x_min + 2, 20, z_max - 2), Color.red, 60f);
            //Debug.DrawLine(new Vector3(x_min + 2, 0, z_min + 2), new Vector3(x_min + 2, 20, z_min + 2), Color.red, 60f);
        }


        return true;
    }

    // Function to rescale number to different scale
    public float Map(float x, float in_min, float in_max, float out_min, float out_max) 
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    // function to generate random int in specified scale from seed
    public int Random_int_in_scale(string seed, int pos, int in_min, int in_max, int out_min, int out_max)
    {
        return (int)Mathf.Round(Map(float.Parse(seed.Substring(pos, 1)), in_min, in_max, out_min, out_max));
    }

    //function to round ints to 10's
    public int Round_to_tens(int num)
    {
        if (num % 10 >= 5)
        {
            while (num % 10 != 0)
            {
                num++;
            }
        }
        else if (num % 10 < 5) {
            while (num % 10 != 0)
            {
                num--;
            }
        }
        return num;
    }
}

// room class
public struct Room
{
    public Vector2 room_center;                          // room center coords x,z
    public Vector2 room_dimensions;                      // room dimensions x,z
    public string room_type;                             // room type, determines floor color & possible furniture 
    public int room_ID;                                  // ID number
    public Vector2[] room_corners;                       // coords of room corners

    // room constructor
    public Room(Vector2 center, Vector2 dimensions, int ID, string type)
    {
        this.room_center = center;
        this.room_dimensions = dimensions;
        this.room_type = type;
        this.room_ID = ID;

        // creating floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3(room_center[0], 0, room_center[1]);
        floor.transform.localScale = new Vector3((int)Mathf.Round(room_dimensions[0]) / 10, 1, (int)Mathf.Round(room_dimensions[1]) / 10); // "100/10" = "global_coords / local_scale_factor "
        switch (this.room_type)
        {
            case "bedroom":
                floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Bedroom_floor_material", typeof(Material)) as Material;
                break;
            case "bathroom":
                floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Bathroom_floor_material", typeof(Material)) as Material;
                break;
            case "kitchen":
                floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Kitchen_floor_material", typeof(Material)) as Material;
                break;
            case "livingroom":
                floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Livingroom_floor_material", typeof(Material)) as Material;
                break;
            case "officeroom":
                floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Officeroom_floor_material", typeof(Material)) as Material;
                break;
        }
        //floor.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Bedroom_floor_material", typeof(Material)) as Material;

        // calculating room corners
        room_corners = new Vector2[4];
        this.room_corners[0] = new Vector2(room_center[0] - room_dimensions[0] / 2, room_center[1] - room_dimensions[1] / 2);// lower  left  -z -x       +z
        this.room_corners[1] = new Vector2(room_center[0] + room_dimensions[0] / 2, room_center[1] - room_dimensions[1] / 2);// lower  right -z +x     -x  +x  
        this.room_corners[2] = new Vector2(room_center[0] + room_dimensions[0] / 2, room_center[1] + room_dimensions[1] / 2);// upper  right +z +x       -z
        this.room_corners[3] = new Vector2(room_center[0] - room_dimensions[0] / 2, room_center[1] + room_dimensions[1] / 2);// upper  left  +z -x

        Build_Wall(room_corners[0], room_corners[1]);
        Build_Wall(room_corners[1], room_corners[2]);
        Build_Wall(room_corners[2], room_corners[3]);
        Build_Wall(room_corners[3], room_corners[0]);

    }
    
    // function that places cubes(walls) between two points
    public static void Build_Wall(Vector2 corner1, Vector2 corner2)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = new Vector3((corner1[0] + corner2[0]) / 2, 5, (corner1[1] + corner2[1]) / 2);
        wall.transform.localScale = new Vector3(Mathf.Abs(corner1[0] - corner2[0]) + 1, 10, Mathf.Abs(corner1[1] - corner2[1]) + 1);
        wall.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Wall_material", typeof(Material)) as Material;
        wall.tag = "Wall";
    }

    //function to create invisible door
    public static void Build_door_tag(Vector2 door_coords)
    { 
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.position = new Vector3(door_coords[0], -5, door_coords[1]);
        door.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        door.tag = "Door";
    }

    public static void Build_wall_w_door(Vector2 corner1, Vector2 corner2, Vector2 door, string orientation)
    {
        switch (orientation)
        {
            // if wall is vertical
            case "vertical":
                // check for other doors on the same wall
                foreach (GameObject prev_door in GameObject.FindGameObjectsWithTag("Door"))
                {
                    if (prev_door.transform.position.x == door[0] &&
                        prev_door.transform.position.z >= corner1[1] &&
                        prev_door.transform.position.z <= corner2[1])
                    {
                        // if other door is found on the same wall, change corner1 or corner2 to prev_door position
                        if (Mathf.Abs(corner1[1] - prev_door.transform.position.z) < Mathf.Abs(corner2[1] - prev_door.transform.position.z))
                        {
                            corner1 = new Vector2(prev_door.transform.position.x, prev_door.transform.position.z + 4);
                        }
                        else
                        {
                            corner2 = new Vector2(prev_door.transform.position.x, prev_door.transform.position.z - 4);
                        }
                    }
                }
                // x - same, z - diff
                foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
                {
                    if (wall.transform.position.x == door[0] &&
                        wall.transform.position.z >= corner1[1] &&
                        wall.transform.position.z <= corner2[1])
                    {
                        if (wall.transform.position.z + wall.transform.localScale.z / 2 > corner2[1])
                        {
                            corner2[1] = wall.transform.position.z + wall.transform.localScale.z / 2;
                        }
                        if (wall.transform.position.z - wall.transform.localScale.z / 2 < corner1[1])
                        {
                            corner1[1] = wall.transform.position.z - wall.transform.localScale.z / 2;

                        }

                        UnityEngine.Object.Destroy(wall);
                    }
                }
                Build_Wall(corner1, new Vector2(door[0], door[1] - 4));
                Build_Wall(corner2, new Vector2(door[0], door[1] + 4));
                Build_door_tag(door);
                break;

            // if wall is horizontal
            case "horizontal":
                // check for other doors on the same wall
                foreach (GameObject prev_door in GameObject.FindGameObjectsWithTag("Door"))
                {
                    if (prev_door.transform.position.z == door[1] &&
                        prev_door.transform.position.x >= corner1[0] &&
                        prev_door.transform.position.x <= corner2[0])
                    {
                        if (Mathf.Abs(corner1[0] - prev_door.transform.position.x) < Mathf.Abs(corner2[0] - prev_door.transform.position.x))
                        {
                            corner1 = new Vector2(prev_door.transform.position.x + 4, prev_door.transform.position.z);
                        }
                        else
                        {
                            corner2 = new Vector2(prev_door.transform.position.x - 4, prev_door.transform.position.z);
                        }
                    }
                }
                // x - diff, z - same
                foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
                {
                    if (wall.transform.position.z == door[1] &&
                        wall.transform.position.x >= corner1[0] &&
                        wall.transform.position.x <= corner2[0])
                    {
                        if (wall.transform.position.x + wall.transform.localScale.x / 2 > corner2[0])
                        {
                            corner2[0] = wall.transform.position.x + wall.transform.localScale.x / 2;
                        }
                        if (wall.transform.position.x - wall.transform.localScale.x / 2 < corner1[0])
                        {
                            corner1[0] = wall.transform.position.x - wall.transform.localScale.x / 2;

                        }
                        UnityEngine.Object.Destroy(wall);
                    }
                }
                Build_Wall(corner1, new Vector2(door[0]-4, door[1]));
                Build_Wall(corner2, new Vector2(door[0]+4, door[1]));
                Build_door_tag(door);
                break;
        }
    }

}