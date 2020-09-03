using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class OutputStatus : MonoBehaviour
{
    public int mode;   //mode == 0…モデル同士　mode == 1…モデルと人
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
    float con3;


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
        CarisFront2 = to2.CarisFront;
        DrivingMode = to.DrivingMode;
        DrivingMode2 = to2.DrivingMode;
        dis = to.dis;
        con3 = to.con3;
    }

    private void OnGUI()
    {
        if (mode == 0)
        {
            
            GUI.Label(new Rect(0, 160, 500, 100), "BlueCarSpeed " + BlueCarSpeed.ToString());
            GUI.Label(new Rect(0, 180, 500, 100), "BlueCarPositionX " + BlueCarPositionX.ToString());
            GUI.Label(new Rect(0, 200, 500, 100), "BlueCarPositionZ " + BlueCarPositionZ.ToString());
            GUI.Label(new Rect(0, 220, 500, 100), "BlueCarCm " + pCm.ToString());
            GUI.Label(new Rect(0, 240, 500, 100), "BlueCarCo " + pCo.ToString());
            GUI.Label(new Rect(0, 260, 500, 100), "BlueDrivingMode " + DrivingMode.ToString());
            GUI.Label(new Rect(0, 280, 500, 100), "CarisFront " + CarisFront.ToString());
            


            GUI.Label(new Rect(180, 260, 500, 100), "GreenDrivingMode " + DrivingMode2.ToString());
            GUI.Label(new Rect(180, 160, 500, 100), "GreenCarSpeed " + GreenCarSpeed.ToString());
            GUI.Label(new Rect(180, 220, 500, 100), "GreenCarCm " + qCm.ToString());
            GUI.Label(new Rect(180, 240, 500, 100), "GreenCarCo " + qCo.ToString());
            GUI.Label(new Rect(180, 280, 500, 100), "CarisFront " + CarisFront2.ToString());
            GUI.Label(new Rect(180, 180, 500, 100), "GreenCarPositionX " + GreenCarPositionX.ToString());
            GUI.Label(new Rect(180, 200, 500, 100), "GreenCarPositionZ " + GreenCarPositionZ.ToString());

            GUI.Label(new Rect(90, 300, 500, 100), "distance " + dis.ToString());
        }
        else if(mode == 1)
        {
            GUI.Label(new Rect(0, 160, 500, 100), "BlueCarSpeed " + BlueCarSpeed.ToString());
            GUI.Label(new Rect(0, 180, 500, 100), "BlueCarPositionX " + BlueCarPositionX.ToString());
            GUI.Label(new Rect(0, 200, 500, 100), "BlueCarPositionZ " + BlueCarPositionZ.ToString());
            GUI.Label(new Rect(0, 220, 500, 100), "BlueCarCm " + pCm.ToString());
            GUI.Label(new Rect(0, 240, 500, 100), "BlueCarCo " + pCo.ToString());
            GUI.Label(new Rect(0, 260, 500, 100), "BlueDrivingMode " + DrivingMode.ToString());
            GUI.Label(new Rect(0, 280, 500, 100), "CarisFront " + CarisFront.ToString());


            GUI.Label(new Rect(180, 160, 500, 100), "RedCarSpeed " + GreenCarSpeed.ToString());
            GUI.Label(new Rect(180, 180, 500, 100), "GreenCarPositionX " + GreenCarPositionX.ToString());
            GUI.Label(new Rect(180, 200, 500, 100), "GreenCarPositionZ " + GreenCarPositionZ.ToString());

            GUI.Label(new Rect(90, 300, 500, 100), "distance " + dis.ToString());
            GUI.Label(new Rect(90, 320, 500, 100), "con3 " + con3.ToString());
        }



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
        GreenCarSpeed = to.VppSpeed;
        GreenCarPositionX = to.VppPositionX;
        GreenCarPositionZ = to.VppPositionZ;
        CarisFront = to.CarisFront;
        CarisFront2 = to2.CarisFront;
        DrivingMode = to.DrivingMode;
        DrivingMode2 = to2.DrivingMode;
        dis = to.dis;
        con3 = to.con3;
    }
}
