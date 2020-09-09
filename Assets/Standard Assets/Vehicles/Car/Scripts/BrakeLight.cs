using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class BrakeLight : MonoBehaviour
    {
        public CarController car; // reference to the car controller, must be dragged in inspector

        private Renderer m_Renderer;
        float f;
        GameObject carA;
        TransformObject to, to2;


        private void Start()
        {
            m_Renderer = GetComponent<Renderer>();
            carA = GameObject.Find("Car");
            to = car.GetComponent<TransformObject>();
        }


        private void Update()
        {
            f = to.fbreak;
            // enable the Renderer when the car is braking, disable it otherwise.
            //m_Renderer.enabled = car.BrakeInput > 0f;
            if(f <= -5.0f)
            {
                m_Renderer.enabled = true;
            }
            else
            {
                m_Renderer.enabled = false;
            }
            
        }
    }
}
