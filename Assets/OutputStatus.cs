using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class OutputStatus : MonoBehaviour
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
        DrivingMode2 = to2.DrivingMode;
        dis = to.dis;
    }

    // データ出力
    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7, string txt8, string txt9, string txt10, string txt11, string txt12, string txt13)
    {
        sw = new StreamWriter(@"SaveData.csv", true, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13 };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        // StreamWriterを閉じる
        sw.Close();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 180, 500, 100), "BlueCarSpeed" + BlueCarSpeed.ToString());
        GUI.Label(new Rect(0, 200, 500, 100), "BlueCarCm" + pCm.ToString());
        GUI.Label(new Rect(0, 220, 500, 100), "BlueCarCo" + pCo.ToString());
        GUI.Label(new Rect(0, 240, 500, 100), "BlueCarCo" + CarisFront.ToString());
        GUI.Label(new Rect(0, 160, 500, 100), "BlueDrivingMode" + DrivingMode.ToString());
        GUI.Label(new Rect(150, 180, 500, 100), "GreenCarSpeed" + GreenCarSpeed.ToString());
        GUI.Label(new Rect(150, 200, 500, 100), "GreenCarCm" + qCm.ToString());
        GUI.Label(new Rect(150, 220, 500, 100), "GreenCarCo" + qCo.ToString());
        GUI.Label(new Rect(150, 240, 500, 100), "GreenCarCo" + CarisFront2.ToString());
        GUI.Label(new Rect(150, 160, 500, 100), "GreenDrivingMode" + DrivingMode2.ToString());
        GUI.Label(new Rect(150, 140, 500, 100), "GreenQB" + QB_s.ToString());

        //GUI.Label(new Rect(150, 240, 500, 100), "f" + f.ToString());
        //GUI.Label(new Rect(0, 240, 500, 100), "Add1" + Add1.ToString());
        //GUI.Label(new Rect(0, 260, 500, 100), "Add4" + Add4.ToString());
        //GUI.Label(new Rect(0, 180, 500, 100), "BlueCarSpeed" + CarSpeed.ToString());
        //GUI.Label(new Rect(0, 180, 500, 100), "BlueCarSpeed" + CarSpeed.ToString());
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
        GreenCarSpeed = to2.CarSpeed;
        GreenCarPositionX = to2.CarPositionX;
        GreenCarPositionZ = to2.CarPositionZ;
        CarisFront = to.CarisFront;
        DrivingMode = to.DrivingMode;
        DrivingMode2 = to2.DrivingMode;
        QB_s = to2.QB;
        dis = to.dis;
       
    }
}
