using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public GameObject Body;
    public GameObject Left_wheel;
    public GameObject Right_wheel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * 0f,    Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * 0.5f,  Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * 1.0f,  Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * 1.5f,  Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * -0.5f, Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * -1.0f, Vector3.down, Color.blue);
        Debug.DrawRay(Body.transform.position + Body.transform.right + Body.transform.forward * -1.5f, Vector3.down, Color.blue);

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

        for (float width = -1.5f; width <= 1.5f; width += 0.5f)
        {
            if (Physics.Raycast(Body.transform.position + Body.transform.right + Body.transform.forward * width, Vector3.down, out hit, 1f))
            {
                Tile tile = hit.transform.GetComponent<Tile>();
                if(!tile.covered)
                {
                    tile.covered = true;
                    Debug.Log("Covered");
                }
                    
            }
        }
    }
}
