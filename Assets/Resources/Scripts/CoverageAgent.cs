using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CoverageAgent : Agent
{
    public static GameObject Self;
    public GameObject body;
    public int ID;
    private float speed = 5f;
    private float rotationSpeed = 5f;
    public override void Initialize()
    {
        InitEnvironment();
        InitAgent();
    }
    public override void OnEpisodeBegin()
    {
        DeleteByTag("Wall");
        DeleteByTag("Door");
        DeleteByTag("Obstacle");
        DeleteByTag("Tile");
        DeleteByTag("Floor");

        InitEnvironment();
        InitAgent();
    }
    public override void CollectObservations(VectorSensor sensor)
    {

        // Add observations from the 8 rays
        // You can add other observations such as relative position in the environment, cleaned area, etc.

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions might include moving forward, turning left, turning right, etc.
        // Assuming you are using a discrete action space where:
        // 0: move forward, 1: turn left, 2: turn right

        int action = actions.DiscreteActions[0];

        switch (action)
        {
            case 0:
                // Move forward
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                break;
            case 1:
                // Turn left
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                break;
            case 2:
                // Turn right
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                break;
        }

        // Cast a ray downwards to detect if the agent is over a tile

        RaycastHit hit = new RaycastHit();

        for (float width = -1.25f; width <= 1.25f; width += 0.5f)
        {
            if (Physics.Raycast(Self.transform.position + Self.transform.right + Self.transform.forward * width, Vector3.down, out hit, 1f) && hit.transform.tag == "Tile")
            {
                Tile tile = hit.transform.GetComponent<Tile>() as Tile;
                if (!tile.covered)
                {
                    tile.covered = true;
                    tile.coverer = this.ID;
                    AddReward(0.05f);
                }

                if (tile.covered && tile.coverer != this.ID)
                {
                    tile.overlaped = true;
                    AddReward(-0.005f);
                }
            }
        }

        // Penalize the agent for taking too much time
        AddReward(-0.001f);
    }

    public void HandleFailure()
    {
        // Conditions for when the agent fails
        // End the episode and provide a negative reward
        EndEpisode();
    }

    public static void InitEnvironment()
    {
        Environment environment = new Environment(0);
        foreach (Room room in environment.room_list)
        {
            Furniture_grid grid = new Furniture_grid(room);
        }
        Coverage_grid coverage = new Coverage_grid(environment);
    }

    public static void DeleteByTag(string tag)
    {
        GameObject[] to_delete = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject piece in to_delete)
        { 
            GameObject.Destroy(piece);
        }
    }

    public static void InitAgent()
    {
        System.Random rnd = new System.Random();
        int max_x = 1000;
        int max_z = 1000;
        Vector3 pot_pos = new Vector3(rnd.Next(0, max_x), 35, rnd.Next(0, max_z));

        if (viablePos(pot_pos))
        {
            Self.transform.position = new Vector3(pot_pos.x, 1, pot_pos.z);    
        }
    }

    public static bool viablePos(Vector3 pos)
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
