using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JSONTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSONRead.Projectile test = JSONRead.ProjLookUp(1);
        Debug.Log(test.name);
    }
}
