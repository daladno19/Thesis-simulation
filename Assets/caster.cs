using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caster : MonoBehaviour
{
    public GameObject body;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity += body.transform.forward * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity += body.transform.right * -1 * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity += body.transform.forward * -1 * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity += body.transform.right * Time.deltaTime * 5;
        }

        RaycastHit[] hit_arr = Physics.BoxCastAll(body.transform.position, new Vector3(2, 1, 2), body.transform.up * -1);

        string msg = "";
        foreach (RaycastHit hit in hit_arr)
        {
            if(hit.transform.tag != "Tile")
                msg += hit.transform.tag + " || ";
        }
        Debug.Log(msg);
    }
}
