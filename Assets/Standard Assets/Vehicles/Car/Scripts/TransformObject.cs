using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class TransformObject : MonoBehaviour
    {
        private CarController m_CarController;

        private void Awake()//起動時に一回だけ動く
        {
            // get the car controller reference
            m_CarController = GetComponent<CarController>();

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position += new Vector3(0, 0, 0.01f);
        }
    }
}
