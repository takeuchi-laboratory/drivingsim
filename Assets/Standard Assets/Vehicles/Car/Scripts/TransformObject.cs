using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;


public class TransformObject : MonoBehaviour
{
    private Transform _transform;
    Rigidbody rigidbody;
    Rigidbody rigidbodyvpp;
    public GameObject vpp;

        // Start is called before the first frame update
        void Start()
        {
        
        _transform = transform;
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbodyvpp = vpp.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
        //var forward = _transform.forward;
        //rigidbodyvpp.velocity.magnitude vppの速度
        //rigidbody.velocity = new Vector3(0, 0, 1f); //Carの正面方向(z方向)に速度1m/sを与える
        Vector3 carpos = transform.position;
        Vector3 vpppos = vpp.transform.position;
        float dis = Vector3.Distance(carpos, vpppos);
        Debug.Log("車間距離:" + dis);
         //rigidbody.velocity = new Vector3(0, 0, rigidbodyvpp.velocity.magnitude);
        if(dis <= 20)
        {
            rigidbody.velocity = new Vector3(0, 0, rigidbodyvpp.velocity.magnitude*1.2f);
        }
        else
        {
            rigidbody.velocity = new Vector3(0, 0, 5f);
        }
            //myTransform = this.transform;
            Debug.Log("carの速度" + rigidbody.velocity.magnitude + "m/s");    //速さの出力
            //Debug.Log("carのz座標:" + transform.position.z);
        //Debug.Log("vppのz座標:" + vpp.transform.position.z);
        //Debug.Log("vppの速度" + rigidbodyvpp.velocity.magnitude + "m/s");
            //Debug.Log($"正面:{forward}");
        }
}
