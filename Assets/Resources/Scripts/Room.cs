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
        floor.layer = 9;

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
        wall.layer = 9;
    }

    //function to create invisible door
    public static void Build_door_tag(Vector2 door_coords)
    {
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.position = new Vector3(door_coords[0], -5, door_coords[1]);
        door.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        door.tag = "Door";
        door.layer = 9;
    }

    public static void Door(Vector2 door_pos)
    {
        //Debug.DrawLine(new Vector3(door_pos.x, 30, door_pos.y), new Vector3(door_pos.x, 0, door_pos.y), Color.red, 60f);

        int door_x = (int)door_pos.x;
        int door_z = (int)door_pos.y;

        RaycastHit[] walls_hit = Physics.BoxCastAll(new Vector3(door_x, 30, door_z), new Vector3(3,1,3), Vector3.down);

        foreach (RaycastHit hit in walls_hit)
        {
            if(hit.transform.tag == "Wall")
            {
                //Debug.DrawLine(new Vector3(door_x, 30, door_z), hit.transform.position, Color.blue, 60f);
                int scale_x = (int)hit.transform.localScale.x;
                int scale_z = (int)hit.transform.localScale.z;

                int pos_x = (int)hit.transform.position.x;
                int pos_z = (int)hit.transform.position.z;

                Vector2 corner1 = new Vector2();
                Vector2 corner2 = new Vector2();
                Vector2 door_bound1 = new Vector2();
                Vector2 door_bound2 = new Vector2();

                if (scale_x == 1) // vertical
                {
                    corner1 = new Vector2(pos_x, pos_z + scale_z / 2);
                    corner2 = new Vector2(pos_x, pos_z - scale_z / 2);

                    door_bound1 = new Vector2(pos_x, door_z + 5);
                    door_bound2 = new Vector2(pos_x, door_z - 5);
                }
                else // horizontal
                {
                    corner1 = new Vector2(pos_x + scale_x / 2, pos_z);
                    corner2 = new Vector2(pos_x - scale_x / 2, pos_z);
                    door_bound1 = new Vector2(door_x + 5, pos_z);
                    door_bound2 = new Vector2(door_x - 5, pos_z);
                }

                //Debug.DrawLine(new Vector3(corner1[0], 30, corner1[1]), new Vector3(corner1[0], 0, corner1[1]), Color.green, 60f);
                //Debug.DrawLine(new Vector3(corner2[0], 30, corner2[1]), new Vector3(corner2[0], 0, corner2[1]), Color.green, 60f);
                //Debug.DrawLine(new Vector3(door_bound1[0], 30, door_bound1[1]), new Vector3(door_bound1[0], 0, door_bound1[1]), Color.yellow, 60f);
                //Debug.DrawLine(new Vector3(door_bound2[0], 30, door_bound2[1]), new Vector3(door_bound2[0], 0, door_bound2[1]), Color.yellow, 60f);
                //Debug.DrawLine(new Vector3(corner1[0], 30, corner1[1]),new Vector3(door_bound1[0], 30, door_bound1[1]), Color.magenta, 60f);
                //Debug.DrawLine(new Vector3(corner2[0], 30, corner2[1]), new Vector3(door_bound2[0], 30, door_bound2[1]), Color.magenta, 60f);

                Build_Wall(corner1, door_bound1);
                Build_Wall(corner2, door_bound2);

                Build_door_tag(door_pos);

                GameObject.Destroy(hit.transform.gameObject);
            }
            
        }


    }
}
