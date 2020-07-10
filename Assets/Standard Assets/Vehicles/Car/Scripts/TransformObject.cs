using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;

    public class TransformObject : MonoBehaviour
    {
        private Transform _transform;
        Rigidbody rigidbody;

        
        // Start is called before the first frame update
        void Start()
        {
            //Player_pos = GetComponent<Transform>().position;    //最初のポジション
            _transform = transform;
            rigidbody = this.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            var forward = _transform.forward;
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 1f);
         
            Debug.Log(rigidbody.velocity.magnitude + "m/s");
            Debug.Log($"正面:{forward}");
        }
    }
