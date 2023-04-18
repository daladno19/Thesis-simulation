using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment
{
    public int number_of_rooms;
    public List<Room> room_list;

    // environment constructor
    public Environment(int seed)
    {
        // init seeded random
        System.Random rnd = new System.Random(seed);

        // getting random number of rooms
        this.number_of_rooms = rnd.Next(2, 5);

        // defined types of rooms and their sizes
        string[] room_types = { "bedroom", "bathroom", "kitchen", "livingroom", "officeroom" };
        Vector2[] room_type_dimensions = { new Vector2(30, 70), new Vector2(20, 40), new Vector2(30, 60), new Vector2(40, 80), new Vector2(30, 50) };

        // setting up a list to store all rooms
        this.room_list = new List<Room>();

        // creating first room
        int first_room_type = rnd.Next(0,5);

        Vector2 new_dimensions = new Vector2(Round_to_ten(rnd.Next((int)room_type_dimensions[first_room_type][0],(int)room_type_dimensions[first_room_type][1])),
                                             Round_to_ten(rnd.Next((int)room_type_dimensions[first_room_type][0], (int)room_type_dimensions[first_room_type][1])));

        Vector2 new_center = new Vector2(new_dimensions[0] / 2, new_dimensions[1] / 2);
        this.room_list.Add(new Room(new_center, new_dimensions, 1, room_types[first_room_type]));

        // generate other rooms
        for (int i = 1; i < number_of_rooms; i++)
        {
            // generate random room dimensions and type
            int room_type = rnd.Next(0,5);
            new_dimensions = new Vector2(Round_to_ten(rnd.Next((int)room_type_dimensions[room_type][0], (int)room_type_dimensions[room_type][1])),
                                         Round_to_ten(rnd.Next((int)room_type_dimensions[room_type][0], (int)room_type_dimensions[room_type][1])));

            // find a list of all potential room placement center points
            List<Vector2> pot_points = Find_Pot_Points(room_list, new_dimensions);

            // get random point to build room from
            new_center = pot_points[rnd.Next(0, pot_points.Count)];

            // place a room on that point
            this.room_list.Add(new Room(new_center, new_dimensions, i + 1, room_types[room_type]));

            // get new room
            Room new_room = room_list[i];

            //find prev room, to which new one got connected
            Room prev_room = Find_connected_room(new_room, room_list);
            //Debug.DrawLine(new Vector3(new_room.room_center[0], 5, new_room.room_center[1]), new Vector3(prev_room.room_center[0], 5, prev_room.room_center[1]), Color.red, 60f);

            //make a door between new_room and prev_room
            Cut_door(new_room, prev_room);

        }
    }

    public static int Round_to_ten(int number)
    {
        if (number % 10 >= 5)
            return number + (10 - number % 10);
        else
            return number - (number % 10);
    }

    public List<Vector2> Find_Pot_Points(List<Room> room_list, Vector2 new_dimensions)
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
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
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
                Vector2 pot_left_bottom_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);

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
                Vector2 pot_left_bottom_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
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
                Vector2 pot_left_bottom_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_right_bottom_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z - new_dimensions[1] / 2);
                Vector2 pot_left_upper_corner = new Vector2(pot_x - new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
                Vector2 pot_right_upper_corner = new Vector2(pot_x + new_dimensions[0] / 2, pot_z + new_dimensions[1] / 2);
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

    public bool Intersects(Vector2 point, List<Room> room_list)
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

    public bool Intersects_improved(Vector2 pot_center, Vector2 pot_dimensions, List<Room> room_list)
    {
        Vector2 pot_LB = new Vector2(pot_center[0] - pot_dimensions[0] / 2, pot_center[1] - pot_dimensions[1] / 2);
        Vector2 pot_RB = new Vector2(pot_center[0] + pot_dimensions[0] / 2, pot_center[1] - pot_dimensions[1] / 2);
        Vector2 pot_LU = new Vector2(pot_center[0] - pot_dimensions[0] / 2, pot_center[1] + pot_dimensions[1] / 2);
        Vector2 pot_RU = new Vector2(pot_center[0] + pot_dimensions[0] / 2, pot_center[1] + pot_dimensions[1] / 2);

        if (Intersects(pot_LB + new Vector2(1, 1), room_list)) return true;
        if (Intersects(pot_RB + new Vector2(-1, 1), room_list)) return true;
        if (Intersects(pot_LU + new Vector2(1, -1), room_list)) return true;
        if (Intersects(pot_RU + new Vector2(-1, -1), room_list)) return true;
        if (Intersects(pot_center, room_list)) return true;

        return false;
    }

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
        }


        return true;
    }

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
            int[] z_points = { (int)room1.room_corners[0][1], (int)room1.room_corners[3][1], (int)room2.room_corners[0][1], (int)room2.room_corners[3][1] };
            Array.Sort(z_points);
            Vector2 door = new Vector2(x_bounds, (z_points[1] + z_points[2]) / 2);
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
            int[] x_points = { (int)room1.room_corners[0][0], (int)room1.room_corners[1][0], (int)room2.room_corners[0][0], (int)room2.room_corners[1][0] };
            Array.Sort(x_points);
            Vector2 door = new Vector2((x_points[1] + x_points[2]) / 2, z_bounds);
            Room.Build_wall_w_door(new Vector2(x_points[0], z_bounds), new Vector2(x_points[3], z_bounds), door, "horizontal");
        }
    }

    public void Cut_door_V2(Room room1, Room room2)
    { 
        
    }
}
