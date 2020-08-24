using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransformObject : MonoBehaviour
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
    public float CoLine = 1.4f; //Coの閾値
    public float CmLine = 1.4f; //Cmの閾値
    [System.NonSerialized] public float BaseSpeed_ms;   //carの基本速度(m/s) unity上ではm/sで速度を与える必要がある
    [System.NonSerialized] public float MaxSpeed_ms;    //carの最高速度(m/s)
    //[System.NonSerialized] public float SlowSpeed_ms;   //vppが遅いときの閾値(m/s)
    [System.NonSerialized] public float Cm, Cm1, Cm2;
    [System.NonSerialized] public float Co, Co1, Co2;
    float time = 0.02f;
    
    float prevelocityvpp;   //作業用変数 vppの前の速度(急ブレーキ時の加速度を求めるため)
    float prerotationvpp;   //作業用変数　vppの前の角度
    float vppacceleration;
    float PreCarPositionX, PreCarPositionZ;  //前回のエージェントの座標
    float PreVppPositionX, PreVppPositionZ;  //前回の人の座標
    float CarPositionX, CarPositionZ;  //今のエージェントの座標
    float VppPositionX, VppPositionZ;  //今の人の座標

    Vector3 pos;
    Vector3 target;

    float VppSpeed, VppSpeed_ms;
    float CarSpeed, CarSpeed_ms, PreCarSpeed_ms;
    public int DrivingMode = 0;
    int pass_N = 0; //追い越しの状態遷移
    int pass_N2 = 0;    //蛇行の状態遷移

    int countG;
    int countreachtime, countsafetime;
    int countsuddenbraking, countsafetime2;
    int countD;

    int personality;
    int f;

    bool CarisFront; //0なら自動運転車が後ろ、1なら前
    bool CarisRight; //0ならCarが左、1なら右
    // Start is called before the first frame update
    void Start()
    {
        BaseSpeed_ms = BaseSpeed_kmh / 3.6f;    //km/hをm/sに変換
        MaxSpeed_ms = MaxSpeed_kmh / 3.6f;      //km/hをm/sに変換
        //SlowSpeed_ms = SlowSpeed_kmh / 3.6f;
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
        
        f = 0;
        cartransform.Translate(0, 0, (BaseSpeed_ms + (f * time)) * time);
    }
}
