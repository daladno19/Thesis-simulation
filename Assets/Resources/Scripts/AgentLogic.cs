using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentLogic : MonoBehaviour
{
    public GameObject Body;
    public GameObject Left_wheel;
    public GameObject Right_wheel;
    public int id = -1;
    public Material[] body_material_arr;
    public Material[] trail_material_arr;

    private Color ray_color;
    private Material body_material;
    private Material trail_material;

    /*// Start is called before the first frame update
    void Start()
    {
        this.body_material = body_material_arr[id - 1];
        Body.GetComponent<MeshRenderer>().material = body_material;

        this.trail_material = trail_material_arr[id - 1];

        Color[] colors = {Color.blue, Color.green, Color.magenta, Color.red, Color.yellow};
        this.ray_color = colors[id - 1];
    }*/

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        { 
            Left_wheel.GetComponent<Rigidbody>().velocity  += Body.transform.right * 3;
            Right_wheel.GetComponent<Rigidbody>().velocity += Body.transform.right * 3;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Left_wheel.GetComponent<Rigidbody>().velocity  += Body.transform.right * -1.5f;
            Right_wheel.GetComponent<Rigidbody>().velocity += Body.transform.right * 1.5f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Left_wheel.GetComponent<Rigidbody>().velocity  += Body.transform.right * -3;
            Right_wheel.GetComponent<Rigidbody>().velocity += Body.transform.right * -3;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Left_wheel.GetComponent<Rigidbody>().velocity  += Body.transform.right * 1.5f;
            Right_wheel.GetComponent<Rigidbody>().velocity += Body.transform.right * -1.5f;
        }

        RaycastHit hit = new RaycastHit();

        for (float width = -1.25f; width <= 1.25f; width += 0.5f)
        {
            if (Physics.Raycast(Body.transform.position + Body.transform.right + Body.transform.forward * width, Vector3.down, out hit, 1f) && hit.transform.tag == "Tile")
            {
                Tile tile = hit.transform.GetComponent<Tile>() as Tile;
                if (!tile.covered)
                {
                    tile.covered = true;
                    tile.coverer = this.id;
                    hit.transform.GetComponent<MeshRenderer>().material = this.trail_material;
                }

                if (tile.covered && tile.coverer != this.id)
                {
                    tile.overlaped = true;
                    hit.transform.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/Agent_Materials/cyan_trail");
                }
            }
        }

        for (float rotation = 0f; rotation <= 360f; rotation += 45f)
        {
            Debug.DrawRay(Body.transform.position + new Vector3(0,1,0), Quaternion.AngleAxis(rotation, Body.transform.up) * Body.transform.right * 7, ray_color);
        }

        RaycastHit sensor_hit = new RaycastHit();
        float[] distance_arr = new float[8];
        for (float rotation = 0f; rotation <= 360f; rotation += 45f)
        {
            if (Physics.Raycast(Body.transform.position + new Vector3(0, 1, 0), Quaternion.AngleAxis(rotation, Body.transform.up) * Body.transform.right, out sensor_hit, 7f)
                && (sensor_hit.transform.tag == "Wall" || sensor_hit.transform.tag == "Obstacle"))
            {
                //Debug.Log(sensor_hit.distance);
                //distance_arr[(int)Mathf.Round(rotation / 45f)-1] = sensor_hit.distance;
            }
            //else
            //    distance_arr[(int)Mathf.Round(rotation / 45f)-1] = 7f;
        }

        //Debug.Log(id + " || " + distance_arr);
    }
}
