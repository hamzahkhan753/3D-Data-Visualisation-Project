using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLabelTiltControl : MonoBehaviour
{
    public Transform CameraRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(CameraRef.position.x, transform.position.y, CameraRef.position.z);
        transform.LookAt(targetPos);
    }
}
