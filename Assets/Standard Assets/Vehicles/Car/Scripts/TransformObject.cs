using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;


public class TransformObject : MonoBehaviour
{
    private Transform _transform;
    Rigidbody rigidbody;    //car(自動運転車)のrigidbodyを格納する変数?
    Rigidbody rigidbodyvpp; //vpp(人の車)のrigidbodyを格納する変数?
    Transform cartransform; //carのtransformを格納する変数?
    Transform vpptransform; //vppのtransformを格納する変数?
    Quaternion quaternionvpp;
    public GameObject vpp;
    public float Cm;
    public float d = 20; //接近距離の閾値
    public float BaseSpeed_kms = 40;    //carの基本速度(km)
    public float MaxSpeed_kms = 60;      //carの最高速度(km)
    [System.NonSerialized] public float BaseSpeed_ms;   //carの基本速度(m/s) unity上ではm/sで速度を与える必要がある
    [System.NonSerialized] public float MaxSpeed_ms;    //carの最高速度(m/s)
    [System.NonSerialized] public float v;              //実際にcarに与える速度
    float prevelocityvpp;   //作業用変数 vppの前の速度(急ブレーキ時の加速度を求めるため)
    float prerotationvpp;   //作業用変数　vppの前の角度
    float rotation;         
    float CountRotationErr;   //蛇行した時間
    [System.NonSerialized] public float t;    //距離が閾値未満になった継続時間
    bool CarisFront; //0なら自動運転車が後ろ、1なら前

    // Start is called before the first frame update
    void Start()
        {
        BaseSpeed_ms = BaseSpeed_kms / 3.6f;    //km/sをm/sに変換
        MaxSpeed_ms = MaxSpeed_kms / 3.6f;      //km/sをm/sに変換
        v = BaseSpeed_ms;                       //carの速度の初期値
        cartransform = this.transform;
        vpptransform = vpp.transform;
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbodyvpp = vpp.GetComponent<Rigidbody>();
        prerotationvpp = quaternionvpp.eulerAngles.y;
        prevelocityvpp = rigidbodyvpp.velocity.magnitude;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
        //var forward = _transform.forward;
        //rigidbodyvpp.velocity.magnitude vppの速度
        //rigidbody.velocity = transform.forward * 1.0f; //carの向いている方向に速度1m/sを与える。こんな感じで速度を出させる

        Vector3 carpos = transform.position;    //carの位置
        Vector3 vpppos = vpp.transform.position;    //vppの位置
        float dis = Vector3.Distance(carpos, vpppos);   //車間距離

        quaternionvpp = vpp.transform.rotation;
        //Debug.Log("車間距離:" + dis);
        if (dis <= d)    //車間距離が閾値より短くなった時
        {
            t = t + 0.02f;   //継続時間が0.02加算される(0.02秒ごとに呼び出されるから)
            Cm = t * (d - dis) / d;
        }
        else
        {
            Cm = Cm - 0.00001f;
        }
        Cm = Mathf.Clamp(Cm, 0, 100);   //Mathf.Clamp(x, min, max) …xの値の範囲の指定ができる
        if (Cm > 5)
        {
            v = v + 0.02f / 3.6f;
            v = Mathf.Clamp(v, 0, MaxSpeed_ms);
            rigidbody.velocity = transform.forward * v;
        }
        else
        {
            v = v - 0.02f / 3.6f;
            v = Mathf.Clamp(v, BaseSpeed_ms, MaxSpeed_ms);
            rigidbody.velocity = transform.forward * v;
        }

        //myTransform = this.transform;
         Debug.Log("carの速度" + rigidbody.velocity.magnitude*3.6 + "km/h");    //速さの出力
        Debug.Log("Cmの値" + Cm);
        //Debug.Log("vppの前の速度" + prevelocityvpp);
        //Debug.Log("vppの今の速度" + rigidbodyvpp.velocity.magnitude);
        //Debug.Log("加速度" + (prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02);
        if((prevelocityvpp - rigidbodyvpp.velocity.magnitude)/0.02 >= 9.00)
        {
            //Debug.Log("加速度" + (prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02);
            Debug.Log("急ブレーキ検知");
        }

        CheckPosition();
        /*if(CarisFront == true)
        {
            Debug.Log("car:前");
        }
        else
        {
            Debug.Log("car:後ろ");
        }*/
        
        if(Math.Abs(quaternionvpp.eulerAngles.y) > 10)
        {
            CountRotationErr = CountRotationErr + 0.02f;
            //Debug.Log("CountRotationErr:" + CountRotationErr);
        }
        if(CountRotationErr > 10)
        {
            Debug.Log("蛇行検知");
        } 
        prevelocityvpp = rigidbodyvpp.velocity.magnitude;   //前のvppの速度の更新
        //Debug.Log(quaternionvpp.eulerAngles.y);
            //Debug.Log("carのz座標:" + transform.position.z);
        //Debug.Log("vppのz座標:" + vpp.transform.position.z);
        //Debug.Log("vppの速度" + rigidbodyvpp.velocity.magnitude + "m/s");
            //Debug.Log($"正面:{forward}");
        }

    void CheckPosition()
    {
        if (cartransform.position.z - vpptransform.position.z >= 0)
        {
            CarisFront = true;
        }
        else
        {
            CarisFront = false;
        }
    }
}
