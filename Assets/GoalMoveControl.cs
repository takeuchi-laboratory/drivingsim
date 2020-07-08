using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMoveControl : MonoBehaviour
{
    private Vector3 targetpos;
    // Start is called before the first frame update
    void Start()
    {
        targetpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Sin(Time.time) * 1.0f + targetpos.x, targetpos.y, targetpos.z);
    }
}
