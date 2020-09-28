using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Outputcsv : MonoBehaviour
{
    StreamWriter sw;
    float timeleft;
    GameObject car;
    public GameObject car2;
    TransformObject to, to2;
    float pCo, pCm;
    float qCo, qCm;
    float BlueCarSpeed;
    float BlueCarPositionX;
    float BlueCarPositionZ;
    float GreenCarSpeed;
    float GreenCarPositionX;
    float GreenCarPositionZ;
    bool CarisFront, CarisFront2;
    int DrivingMode, DrivingMode2;
    int QB_s;
    float dis;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("Car");
        to = car.GetComponent<TransformObject>();
        to2 = car2.GetComponent<TransformObject>();
        pCo = to.Co;
        pCm = to.Cm;
        qCo = to2.Co;
        qCm = to2.Cm;
        BlueCarSpeed = to.CarSpeed;
        BlueCarPositionX = to.CarPositionX;
        BlueCarPositionZ = to.CarPositionZ;
        GreenCarSpeed = to.VppSpeed;
        GreenCarPositionX = to.VppPositionX;
        GreenCarPositionZ = to.VppPositionZ;
        CarisFront = to.CarisFront;
        DrivingMode = to.DrivingMode;
        dis = to.dis;

        // ファイル書き出し
        // 現在のフォルダにsaveData.csvを出力する(決まった場所に出力したい場合は絶対パスを指定してください)
        // 引数説明：第1引数→ファイル出力先, 第2引数→ファイルに追記(true)or上書き(false), 第3引数→エンコード
        sw = new StreamWriter(@"test.csv", false, Encoding.GetEncoding("Shift_JIS"));
        // ヘッダー出力
        //string[] s1 = { "time", "BlueCarCo", "BlueCarCm", "GreenCarCo", "GreenCarCm", "BlueCarSpeed", "BlueCarPositionX", "BlueCarPositionZ", "GreenCarSpeed", "GreenCarPositionX", "GreenCarPositionZ", "distance", "CoButton", "CmButton" };
        string[] s1 = { "time", "YellowCarCo", "YellowCarCm", "GreenCarCo", "GreenCarCm", "YellowCarSpeed", "YellowCarPositionX", "YellowCarPositionZ", "GreenCarSpeed", "GreenCarPositionX", "GreenCarPositionZ", "distance", "CoButton", "CmButton" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        // StreamWriterを閉じる
        sw.Close();
    }

    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8, string txt9, string txt10, string txt11, string txt12, string txt13, string txt14)
    {
        sw = new StreamWriter(@"test.csv", true, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14 };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        // StreamWriterを閉じる
        sw.Close();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pCo = to.Co;
        pCm = to.Cm;
        qCo = to2.Co;
        qCm = to2.Cm;
        BlueCarSpeed = to.CarSpeed;
        BlueCarPositionX = to.CarPositionX;
        BlueCarPositionZ = to.CarPositionZ;
        GreenCarSpeed = to.VppSpeed;
        GreenCarPositionX = to.VppPositionX;
        GreenCarPositionZ = to.VppPositionZ;
        CarisFront = to.CarisFront;
        DrivingMode = to.DrivingMode;
        dis = to.dis;

        timeleft -= Time.deltaTime;
        if (timeleft <= 0.0)
        {
            timeleft = 1.0f;
            //csv書き出し
            SaveData(time.ToString(), pCo.ToString(), pCm.ToString(), qCo.ToString(), qCm.ToString(), BlueCarSpeed.ToString(), BlueCarPositionX.ToString(), BlueCarPositionZ.ToString(), GreenCarSpeed.ToString(), GreenCarPositionX.ToString(), GreenCarPositionZ.ToString(), dis.ToString(), Input.GetButton("CoButton").ToString(), Input.GetButton("CmButton").ToString());
            time += 1;
        }
    }
}
