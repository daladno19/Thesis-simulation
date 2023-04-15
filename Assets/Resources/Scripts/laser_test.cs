using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser_test : MonoBehaviour
{
    public GameObject laser;
    private int speed = 30;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        laser.transform.position += Movement * speed * Time.deltaTime;


        if (viablePos(laser.transform.position))
        {
            Debug.Log("viable");
        }
        else
            Debug.Log("Obstacle");
    }

    public bool viablePos(Vector3 pos)
    {
        RaycastHit hit = new RaycastHit();

        if ((Physics.Raycast(pos + new Vector3(0, 0,-2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(0, 0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(0, 0,+2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0,-2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(2, 0,+2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2,0,-2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2,0, 0), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor") &&
            (Physics.Raycast(pos + new Vector3(-2,0,+2), Vector3.down, out hit, 40f) && hit.transform.tag == "Floor"))
        { 
            return true;
        }

        return false;
    }
}
