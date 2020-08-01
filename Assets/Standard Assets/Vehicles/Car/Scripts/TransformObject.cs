using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.ComponentModel;


//carの全長4.8mくらい
//vppの座標から後ろまで2.7mくらい、前まで2.5mくらい
//車間距離　= unityで設定した距離-7.3
//0.02秒で(40/3.6)*0.02m　移動すれば時速40kmと同じになるはず
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
    float con1, con2, con3, con4, con5, con6;
    float prevelocityvpp;   //作業用変数 vppの前の速度(急ブレーキ時の加速度を求めるため)
    float prerotationvpp;   //作業用変数　vppの前の角度
    float vppacceleration;
    float rotation;         
    float CountRotationErr;   //蛇行した時間
    float Add1, Add2, Add3;
    float Sub1, Sub2, Sub3;
    float Add4, Add5, Add6;
    float Sub4, Sub5, Sub6;
    float PreCarPositionX;  //前のエージェントのx座標
    float PreCarPositionZ;  //前のエージェントのz座標(進行方向)
    float PreVppPositionX;  //前の人のx座標
    float PreVppPositionZ;  //前の人のz座標(進行方向)
    float G_sum;
    float G_Cm = 0.5f;      //ずれの閾値Cm
    float G_Co = 1.0f;      //ずれの閾値Co
    float Gap_CarX;  
    float Gap_VppX;
    float Gap;  //人の車とエージェントの車の0.02秒間のx座標のずれ
    double VppSpeed;
    double CarSpeed;

    int countG;
    int countreachtime,countsafetime;
    int countsuddenbraking, countsafetime2 ;

    
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
        vppacceleration = (prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02f;
        //rigidbody.velocity = transform.forward * BaseSpeed_ms;
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
        CalucSpeedAndAcceleration();
        quaternionvpp = vpp.transform.rotation;
        Debug.Log("車間距離:" + dis);

        if(CarisFront == true)  //エージェントの車が前の時
        {
            if(dis <= d)        //車間距離が閾値より短い
            {
                t1 = t1 + 0.02f;   //継続時間が0.02加算される(0.02秒ごとに呼び出されるから)          
                t2 = Mathf.Clamp(t2 - 0.02f, 0, 1000); 
            } else if(dis > d)  //車間距離が閾値より長い
            {
                t2 = t2 + 0.02f;   //継続時間が0.02加算される(0.02秒ごとに呼び出されるから
                //Sub1 = (float)Math.Sqrt((Math.Abs(d - dis)*a2 / d) * t2*b2);
                t1 = Mathf.Clamp(t1 - 0.02f, 0, 1000);
            }
            if(dis <= d)
            {
                countreachtime += 1;
                con2 = Mathf.Clamp(con2 - 0.02f, 0 ,1000);
                countsafetime = 0;
            }
            else if(dis > d)
            {
                if(countreachtime < 50 && countreachtime != 0)
                {
                    countreachtime = 0;
                } else if(countreachtime >= 50)
                {
                    con1 = con1 + 1;
                    countreachtime = 0;
                } else if(countreachtime == 0)
                {
                    countsafetime += 1;
                    if(countsafetime == 1500)
                    {
                        con1 = Mathf.Clamp(con1 - 1, 0, 1000);
                        countsafetime = 0;
                    }
                }
                con2 = con2 + 0.02f;
            }
            Add1 = (float)Math.Sqrt((Math.Abs(d - dis) * a1 / d) * t1 * b1);
            Sub1 = t2 * b2;

            Add4 = (float)Math.Sqrt(con1);
            Sub4 = (float)Math.Sqrt(con2);
        }
        else if(CarisFront == false)    //エージェントの車が後ろの時
        {
            if(rigidbodyvpp.velocity.magnitude < BaseSpeed_ms)
            {
                if (dis < 30f)
                {
                    t3 = t3 + 0.02f;
                    t4 = Mathf.Clamp(t4 - 0.02f, 0, 1000);
                    //rigidbody.velocity = transform.forward * rigidbodyvpp.velocity.magnitude;   //ぶつからないように、速度を下げる
                    /*if(rigidbody.velocity.magnitude > rigidbodyvpp.velocity.magnitude)
                    {
                        rigidbody.AddForce(-transform.forward * 10f, ForceMode.Acceleration);
                    }*/
                    
                }
                else
                {
                    //rigidbody.velocity = transform.forward * BaseSpeed_ms;
                    /*if(rigidbody.velocity.magnitude < BaseSpeed_ms )
                    {
                        rigidbody.AddForce(transform.forward * 10f, ForceMode.Acceleration);
                    }*/
                    
                }
            } else if(rigidbodyvpp.velocity.magnitude >= BaseSpeed_ms)
            {
                t4 = t4 + 0.02f;             
                t3 = t3 - Mathf.Clamp(t3 - 0.02f, 0, 1000);
                //rigidbody.velocity = transform.forward * BaseSpeed_ms;
            }

            if (countG < 50)
            {
                G_sum += Gap;
                countG += 1;
            }
            else if (countG == 50)
            {
                if (G_Co > G_sum && G_sum >= G_Cm)
                {
                    t5 = t5 + 1;                   
                    t6 = Mathf.Clamp(t6 - 1.00f, 0, 1000);
                    //Debug.Log("蛇行検知");
                }
                else if(G_sum < G_Cm)
                {
                    t6 = t6 + 1;
                    t5 = Mathf.Clamp(t5 - 1.00f, 0, 1000);

                    con6 = con6 + 1;
                    con5 = Mathf.Clamp(con5 - 1.00f, 0, 1000);
                }
                else if(G_sum >= G_Co)
                {
                    con5 = con5 + 1;
                    
                    con6 = Mathf.Clamp(con6 - 1.00f, 0, 1000);
                    //Debug.Log("蛇行検知");
                }
                else if(G_sum < G_Co)
                {
                    con6 = con6 + 1;
                    con5 = Mathf.Clamp(con5 - 1.00f, 0, 1000);
                }

                countG = 0;
                G_sum = 0;
            }
            if (vppacceleration >= 9.0f && dis < d)
            {
                countsuddenbraking += 1;
                con4 = Mathf.Clamp(con4 - 0.02f, 0, 1000);
                countsafetime2 = 0;
            } else if(vppacceleration < 9.0f)
            {
                if (countsuddenbraking < 50 && countsuddenbraking != 0)
                {
                    countsuddenbraking = 0;
                }
                else if (countsuddenbraking >= 50)
                {
                    con3 = con3 + 1;
                    countsuddenbraking = 0;
                }
                else if (countsuddenbraking == 0)
                {
                    countsafetime2 += 1;
                    if (countsafetime2 == 1500)
                    {
                        con3 = Mathf.Clamp(con3 - 1, 0, 1000);
                        countsafetime2 = 0;
                    }
                }
                con4 = con4 + 0.02f;
            }
            Add2 = (float)Math.Sqrt((Math.Abs(BaseSpeed_ms - rigidbodyvpp.velocity.magnitude) * a3 / BaseSpeed_ms) * t3 * b3);
            Sub2 = (float)Math.Sqrt((Math.Abs(BaseSpeed_ms - rigidbodyvpp.velocity.magnitude) * a4 / BaseSpeed_ms) * t4 * b4);
            Add3 = (float)Math.Sqrt((Math.Abs(G_Cm - G_sum) * a5 / G_Cm) * t5 * b5);
            Sub3 = (float)Math.Sqrt((Math.Abs(G_Cm - G_sum) * a6 / G_Cm) * t6 * b6);

            Add5 = (float)Math.Sqrt(con3 * (float)Math.Abs(vppacceleration - 9.0f) / 9.0f);
            Sub5 = (float)Math.Sqrt(con4);
            Add6 = (float)Math.Sqrt((Math.Abs(G_Co - G_sum) / G_Co) * con5);
            Sub6 = (float)Math.Sqrt((Math.Abs(G_Co - G_sum) / G_Co) * con6);
        }
        //Cm1 = Add1 + Add2 / ((Add1 + Add2) - (Sub1 + Sub2));
        Add1 = Mathf.Clamp(Add1, 0, 50f);
        Add2 = Mathf.Clamp(Add2, 0, 50f);
        Add3 = Mathf.Clamp(Add3, 0, 50f);
        Sub1 = Mathf.Clamp(Sub1, 0, Add1);
        Sub2 = Mathf.Clamp(Sub2, 0, Add2);
        Sub3 = Mathf.Clamp(Sub3, 0, Add3);

        Add4 = Mathf.Clamp(Add4, 0, 50f);
        Add5 = Mathf.Clamp(Add5, 0, 50f);
        Add6 = Mathf.Clamp(Add6, 0, 50f);
        Sub4 = Mathf.Clamp(Sub4, 0, Add4);
        Sub6 = Mathf.Clamp(Sub5, 0, Add5);
        Sub6 = Mathf.Clamp(Sub6, 0, Add6);

        Cm1 = (float)Math.Exp(Add1 + Add2) / ((float)Math.Exp(Add1 + Add2) + (float)Math.Exp(Sub1 + Sub2));
        Cm2 = (float)Math.Exp(Add3) / ((float)Math.Exp(Add3) + (float)Math.Exp(Sub3));

        Cm = Cm1;
        Co1 = (float)Math.Exp(Add4 + Add5) / ((float)Math.Exp(Add4 + Add5) + (float)Math.Exp(Sub4 + Sub5));
        Co2 = (float)Math.Exp(Add6) / ((float)Math.Exp(Add6) + (float)Math.Exp(Sub6));

        cartransform.Translate(0, 0, (40 / 3.6f) * 0.02f);
        //Debug.Log(Math.Abs(PreCarPositionZ - cartransform.position.z) * 3.6 / 0.02);
        //Debug.Log(VppSpeed);
        //Debug.Log("carの速度" + rigidbody.velocity.magnitude*3.6 + "km/h");    //速さの出力
        //Debug.Log("Cmの値" + Cm1);
        Debug.Log("Co1の値" + Co1);
        Debug.Log("Co2の値" + Co2);
        //Debug.Log("vppの前の速度" + prevelocityvpp);
        //Debug.Log("vppの今の速度" + rigidbodyvpp.velocity.magnitude * 3.6);
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

    void CalucSpeedAndAcceleration()
    {
        VppSpeed = Math.Abs(PreVppPositionZ - vpptransform.position.z) * 3.6f / 0.02f;
        CarSpeed = Math.Abs(PreCarPositionZ - cartransform.position.z) * 3.6 / 0.02f;
        vppacceleration = (prevelocityvpp - rigidbodyvpp.velocity.magnitude) / 0.02f;
    }
}
