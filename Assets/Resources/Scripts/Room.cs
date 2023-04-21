using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        floor.AddComponent<BoxCollider>();
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
        floor.tag = "Floor";

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
        wall.transform.position = new Vector3((corner1[0] + corner2[0]) / 2, 11, (corner1[1] + corner2[1]) / 2);
        wall.transform.localScale = new Vector3(Mathf.Abs(corner1[0] - corner2[0]) + 1, 22, Mathf.Abs(corner1[1] - corner2[1]) + 1);
        wall.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Wall_material", typeof(Material)) as Material;
        wall.tag = "Wall";
        GameObject.Destroy(wall.GetComponent<BoxCollider>());
        wall.AddComponent<BoxCollider>();
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
                            corner2[1] = Environment.Round_to_ten((int)(wall.transform.position.z + wall.transform.localScale.z / 2 - 1));
                        }
                        if (wall.transform.position.z - wall.transform.localScale.z / 2 < corner1[1])
                        {
                            corner1[1] = Environment.Round_to_ten((int)(wall.transform.position.z - wall.transform.localScale.z / 2 + 1));

                        }
                        //GameObject.Destroy(wall.GetComponent<BoxCollider>());
                        GameObject.Destroy(wall);
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
                            corner2[0] = Environment.Round_to_ten((int)(wall.transform.position.x + wall.transform.localScale.x / 2));
                        }
                        if (wall.transform.position.x - wall.transform.localScale.x / 2 < corner1[0])
                        {
                            corner1[0] = Environment.Round_to_ten((int)(wall.transform.position.x - wall.transform.localScale.x / 2));

                        }
                        UnityEngine.Object.Destroy(wall);
                    }
                }
                Build_Wall(corner1, new Vector2(door[0] - 4, door[1]));
                Build_Wall(corner2, new Vector2(door[0] + 4, door[1]));
                Build_door_tag(door);
                break;
        }
    }

    public static void Build_wall__w_door_V2(Vector2 door_coords, string orientation)
    {

    }
}
