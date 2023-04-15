using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public float x;
    public float z;
    public bool covered;
    public bool overlaped;

    public Tile(float x, float z)
    {
        this.x = x;
        this.z = z;

        this.covered = false;
        this.overlaped = false;
    }
}
