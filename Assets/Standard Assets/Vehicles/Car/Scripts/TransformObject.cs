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
    public float Cm;
    public float d = 20; //距離の閾値
    public float t;    //距離が閾値未満になった時間の総数
    public float v = 40f/3.6f;     //速度

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
        float dis = Vector3.Distance(carpos, vpppos);   //車間距離
        
        Debug.Log("車間距離:" + dis);
         //rigidbody.velocity = new Vector3(0, 0, rigidbodyvpp.velocity.magnitude);
        if(dis <= d)
        {
            t = t + 0.02f;   //0.02秒ごとに呼び出されるから
            Cm = t * (d - dis) / d;
            Cm = Mathf.Clamp(Cm, 0, 100);
        }
        else
        {
            Cm = Cm - 0.00001f;
            Cm = Mathf.Clamp(Cm, 0, 100);
           
        }
        if(Cm > 5)
        {
            v = v + 0.02f / 3.6f;
            v = Mathf.Clamp(v, 0, 60 / 3.6f);
            rigidbody.velocity = new Vector3(0, 0, v);
        }
        else
        {
            v = v - 0.02f / 3.6f;
            v = Mathf.Clamp(v, 40f / 3.6f, 60 / 3.6f);
            rigidbody.velocity = new Vector3(0, 0, v);
        }
            //myTransform = this.transform;
            Debug.Log("carの速度" + rigidbody.velocity.magnitude*3.6 + "km/h");    //速さの出力
        Debug.Log("Cmの値" + Cm);
            //Debug.Log("carのz座標:" + transform.position.z);
        //Debug.Log("vppのz座標:" + vpp.transform.position.z);
        //Debug.Log("vppの速度" + rigidbodyvpp.velocity.magnitude + "m/s");
            //Debug.Log($"正面:{forward}");
        }
}
