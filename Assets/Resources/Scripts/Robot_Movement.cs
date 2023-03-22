using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_Movement : MonoBehaviour
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
        this.Left_wheel.GetComponent<Rigidbody>().velocity = this.Body.transform.forward;
        this.Right_wheel.GetComponent<Rigidbody>().velocity = this.Body.transform.forward;

    }
}
