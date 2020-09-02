using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtransform : MonoBehaviour
{
    Rigidbody rigidbody;    //car(自動運転車)のrigidbodyを格納する変数?
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0, 0, 40 / 3.6f);
    }

    // Update is called once per frame
    void Update()
    { 
        
        //rigidbody.velocity = new Vector3(0, 0, 40 / 3.6f);
    }
}
