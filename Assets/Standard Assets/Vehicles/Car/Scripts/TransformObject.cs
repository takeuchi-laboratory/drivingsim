using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

//carの全長4.8mくらい
//vppの座標から後ろまで2.7mくらい、前まで2.5mくらい
public class TransformObject : MonoBehaviour
{
    private Transform _transform;
    Rigidbody rigidbody;    //car(自動運転車)のrigidbodyを格納する変数?
    Rigidbody rigidbodyvpp; //vpp(人の車)のrigidbodyを格納する変数?
    Transform cartransform; //carのtransformを格納する変数?
    Transform vpptransform; //vppのtransformを格納する変数?
    Quaternion quaternionvpp;
    public GameObject vpp;

    public float d = 30; //接近距離の閾値
    public float BaseSpeed_kmh = 40;    //carの基本速度(km/h)
    public float MaxSpeed_kmh = 60;      //carの最高速度(km/h)
    //public float SlowSpeed_kmh = 40;    //vppが遅いときの閾値(km/h)
    public float a1 = 50f, b1 = 5f;
    public float a2 = 1, b2 = 1;
    public float a3 = 50, b3 = 5; //パラメータ
    public float a4 = 20, b4 = 1;    //パラメータ
    public float a5 = 20, b5 = 1;    //パラメータ
    public float a6 = 20, b6 = 1;
    [System.NonSerialized] public float BaseSpeed_ms;   //carの基本速度(m/s) unity上ではm/sで速度を与える必要がある
    [System.NonSerialized] public float MaxSpeed_ms;    //carの最高速度(m/s)
    //[System.NonSerialized] public float SlowSpeed_ms;   //vppが遅いときの閾値(m/s)
    [System.NonSerialized] public float v;              //実際にcarに与える速度
    [System.NonSerialized] public float Cm, Cm1, Cm2;
    [System.NonSerialized] public float Co, Co1, Co2;
    float t1, t2, t3, t4, t5, t6;    //距離が閾値未満になった継続時間
    float prevelocityvpp;   //作業用変数 vppの前の速度(急ブレーキ時の加速度を求めるため)
    float prerotationvpp;   //作業用変数　vppの前の角度
    float rotation;         
    float CountRotationErr;   //蛇行した時間
    float Add1, Add2, Add3;
    float Sub1, Sub2, Sub3;
    float PreCarPositionX;  //前のエージェントのx座標
    float PreCarPositionZ;  //前のエージェントのz座標(進行方向)
    float PreVppPositionX;  //前の人のx座標
    float PreVppPositionZ;  //前の人のz座標(進行方向)
    float G_sum;
    float G_s = 0.5f;      //ずれの閾値
    float Gap_CarX;  
    float Gap_VppX;
    float Gap;  //人の車とエージェントの車の0.02秒間のx座標のずれ

    int countG;
    int countsuddenbraking, CountSuddenBraking;

    
    bool CarisFront; //0なら自動運転車が後ろ、1なら前

    // Start is called before the first frame update
    void Start()
        {
        BaseSpeed_ms = BaseSpeed_kmh / 3.6f;    //km/hをm/sに変換
        MaxSpeed_ms = MaxSpeed_kmh / 3.6f;      //km/hをm/sに変換
        //SlowSpeed_ms = SlowSpeed_kmh / 3.6f;
        v = BaseSpeed_ms;                       //carの速度の初期値
        cartransform = this.transform;
        vpptransform = vpp.transform;
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbodyvpp = vpp.GetComponent<Rigidbody>();
        prerotationvpp = quaternionvpp.eulerAngles.y;
        prevelocityvpp = rigidbodyvpp.velocity.magnitude;
        PreCarPositionX = cartransform.position.x;
        PreCarPositionZ = cartransform.position.z;
        PreVppPositionX = vpptransform.position.x;
        PreVppPositionZ = vpptransform.position.z;  
        rigidbody.velocity = transform.forward * BaseSpeed_ms;
        //rigidbody.AddForce(transform.forward * 4/3.6f, ForceMode.Acceleration);
    }

        // Update is called once per frame
        void FixedUpdate()
        {
        //var forward = _transform.forward;
        //rigidbodyvpp.velocity.magnitude vppの速度
        //rigidbody.velocity = transform.forward * 1.0f; //carの向いている方向に速度1m/sを与える。こんな感じで速度を出させる
        float carpos = cartransform.position.z;    //carの位置
        float vpppos = vpptransform.position.z;    //vppの位置
        float dis = (float)Math.Abs(carpos - vpppos);   //車間距離の絶対値
        CheckPosition();
        CalucGap();
        quaternionvpp = vpp.transform.rotation;
        //Debug.Log("車間距離:" + dis);

        if(CarisFront == true)  //エージェントの車が前の時
        {
            if(dis <= d)        //車間距離が閾値より短い
            {
                t1 = t1 + 0.02f;   //継続時間が0.02加算される(0.02秒ごとに呼び出されるから)          
                Add1 = (float)Math.Sqrt((Math.Abs(d - dis) * a1 / d) * t1 * b1);
                t2 = Mathf.Clamp(t2 - 0.02f, 0, 1000); 
            } else if(dis > d)  //車間距離が閾値より長い
            {
                t2 = t2 + 0.02f;   //継続時間が0.02加算される(0.02秒ごとに呼び出されるから)
                Sub1 = t2 * b2;
                //Sub1 = (float)Math.Sqrt((Math.Abs(d - dis)*a2 / d) * t2*b2);
                t1 = Mathf.Clamp(t1 - 0.02f, 0, 1000);
            }
        }
        else if(CarisFront == false)    //エージェントの車が後ろの時
        {
            if(rigidbodyvpp.velocity.magnitude < BaseSpeed_ms)
            {
                //t3 = t3 + 0.02f;
                //Add2 = (float)Math.Sqrt((Math.Abs(BaseSpeed_ms - rigidbodyvpp.velocity.magnitude) * a3 / BaseSpeed_ms) * t3 * b3);
                //t4 = Mathf.Clamp(t4 - 0.02f, 0, 1000);
                if (dis < 30f)
                {
                    t3 = t3 + 0.02f;
                    Add2 = (float)Math.Sqrt((Math.Abs(BaseSpeed_ms - rigidbodyvpp.velocity.magnitude) * a3 / BaseSpeed_ms) * t3 * b3);
                    t4 = Mathf.Clamp(t4 - 0.02f, 0, 1000);
                    rigidbody.velocity = transform.forward * rigidbodyvpp.velocity.magnitude;   //ぶつからないように、速度を下げる
                    /*if(rigidbody.velocity.magnitude > rigidbodyvpp.velocity.magnitude)
                    {
                        rigidbody.AddForce(-transform.forward * 10f, ForceMode.Acceleration);
                    }*/
                    
                }
                else
                {
                    rigidbody.velocity = transform.forward * BaseSpeed_ms;
                    /*if(rigidbody.velocity.magnitude < BaseSpeed_ms )
                    {
                        rigidbody.AddForce(transform.forward * 10f, ForceMode.Acceleration);
                    }*/
                    
                }
            } else if(rigidbodyvpp.velocity.magnitude >= BaseSpeed_ms)
            {
                t4 = t4 + 0.02f;
                Sub2 = (float)Math.Sqrt((Math.Abs(BaseSpeed_ms - rigidbodyvpp.velocity.magnitude) * a4 / BaseSpeed_ms) * t4 * b4);
                t3 = t3 - Mathf.Clamp(t3 - 0.02f, 0, 1000);
                rigidbody.velocity = transform.forward * BaseSpeed_ms;
            }

            if (countG < 50)
            {
                G_sum += Gap;
                countG += 1;
            }
            else if (countG == 50)
            {
                if (G_sum >= G_s)
                {
                    t5 = t5 + 1;
                    Add3 = (float)Math.Sqrt((Math.Abs(G_s - G_sum) * a5 / G_s) * t5 * b5);
                    t6 = Mathf.Clamp(t6 - 1.00f, 0, 1000);
                    Debug.Log("蛇行検知");
                }
                else
                {
                    t6 = t6 + 1;
                    Sub3 = (float)Math.Sqrt((Math.Abs(G_s - G_sum) * a6 / G_s) * t6 * b6);
                    t5 = Mathf.Clamp(t5 - 1.00f, 0, 1000);
                }
                countG = 0;
                G_sum = 0;
            }
            if ((prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02 >= 9.00)
            {
                Debug.Log("急ブレーキ検知");
            }
        }
        //Cm1 = t1;
        //Cm1 = Add1 + Add2 / ((Add1 + Add2) - (Sub1 + Sub2));
        Add1 = Mathf.Clamp(Add1, 0, 50f);
        Add2 = Mathf.Clamp(Add2, 0, 50f);
        Sub1 = Mathf.Clamp(Sub1, 0, Add1);
        Sub2 = Mathf.Clamp(Sub2, 0, Add2);
        
        Cm1 = (float)Math.Exp(Add1 + Add2) / ((float)Math.Exp(Add1 + Add2) + (float)Math.Exp(Sub1 + Sub2));

        Cm = Cm1;
        if (Cm > 0.6 && CarisFront == true)    //Cmが閾値より高いかつ、エージェント前、人が後ろのとき
        {
            /*if(rigidbody.velocity.magnitude < MaxSpeed_ms)
            {
                rigidbody.AddForce(transform.forward * 20f, ForceMode.Acceleration);
            }*/
            v = v + 0.02f / 3.6f;
            v = Mathf.Clamp(v, 0, MaxSpeed_ms);
            rigidbody.velocity = transform.forward * v;
        }
        else if(Cm > 0.6 && CarisFront == false)   //Cmが閾値より高いかつ、エージェント後ろ、人が前のとき
        {
            /*v = v - 0.02f / 3.6f;
            v = Mathf.Clamp(v, rigidbodyvpp.velocity.magnitude, BaseSpeed_ms);
            rigidbody.velocity = transform.forward * v;*/
        } else if(Cm <= 0.6 && CarisFront == true)
        {
            /*if (rigidbody.velocity.magnitude > BaseSpeed_ms)
            {
                rigidbody.AddForce(-transform.forward * 20f, ForceMode.Acceleration);
            }
            else if (rigidbody.velocity.magnitude < BaseSpeed_ms)
            {
                rigidbody.AddForce(transform.forward * 40f, ForceMode.Acceleration);
            }*/
            v = v - 0.02f / 3.6f;
            v = Mathf.Clamp(v, BaseSpeed_ms, MaxSpeed_ms);
            rigidbody.velocity = transform.forward * v;
        } else if(Cm <= 0.6 && CarisFront == false)
        {
            /*if (rigidbody.velocity.magnitude < MaxSpeed_ms)
            {
                rigidbody.AddForce(transform.forward * 20f, ForceMode.Acceleration);
            }*/
            //rigidbody.velocity = transform.forward * BaseSpeed_ms;
        }

        Debug.Log("carの速度" + rigidbody.velocity.magnitude*3.6 + "km/h");    //速さの出力
        Debug.Log("Cmの値" + Cm1);
        //Debug.Log("vppの前の速度" + prevelocityvpp);
        //Debug.Log("vppの今の速度" + rigidbodyvpp.velocity.magnitude);
        //Debug.Log("加速度" + (prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02);
        //Debug.Log("carのz座標:" + transform.position.z);
        //Debug.Log("vppのz座標:" + vpp.transform.position.z);
        //Debug.Log("vppの速度" + rigidbodyvpp.velocity.magnitude + "m/s");
        //Debug.Log($"正面:{forward}");
        prevelocityvpp = rigidbodyvpp.velocity.magnitude;   //前のvppの速度の更新
        PreCarPositionX = cartransform.position.x;
        PreCarPositionZ = cartransform.position.z;
        PreVppPositionX = vpptransform.position.x;
        PreVppPositionZ = vpptransform.position.z;
    }

    void CheckPosition()
    {
        if (cartransform.position.z - vpptransform.position.z >= 0)
        {
            CarisFront = true;
        }
        else if(cartransform.position.z - vpptransform.position.z < 0)
        {
            CarisFront = false;
        }
    }

    void CalucGap()
    {
        Gap_CarX = (float)Math.Abs(cartransform.position.x - PreCarPositionX);
        Gap_VppX = (float)Math.Abs(vpptransform.position.x - PreVppPositionX);
        Gap = (float)Math.Abs(Gap_CarX - Gap_VppX);
        /*PreCarPositionX
        PreCarPositionZ
        PreVppPositionX
        PreVppPositionZ
        cartransform.position.x
        cartransform.position.z
        vpptransform.position.x
        vpptransform.position.z*/
    }
}
