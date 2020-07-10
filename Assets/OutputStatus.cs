using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputStatus : MonoBehaviour
{
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rigidbody.velocity.magnitude + "m/s");
    }
}
