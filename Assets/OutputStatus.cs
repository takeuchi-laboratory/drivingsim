using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputStatus : MonoBehaviour
{

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
        GreenCarSpeed = to2.CarSpeed;
        GreenCarPositionX = to2.CarPositionX;
        GreenCarPositionZ = to2.CarPositionZ;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 180, 500, 100), "BlueCarSpeed" + BlueCarSpeed.ToString());
        GUI.Label(new Rect(0, 200, 500, 100), "BlueCarCm" + pCm.ToString());
        GUI.Label(new Rect(0, 220, 500, 100), "BlueCarCo" + pCo.ToString());
        GUI.Label(new Rect(150, 180, 500, 100), "GreenCarSpeed" + GreenCarSpeed.ToString());
        GUI.Label(new Rect(150, 200, 500, 100), "GreenCarCm" + qCm.ToString());
        GUI.Label(new Rect(150, 220, 500, 100), "GreenCarCo" + qCo.ToString());
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
    }
}
