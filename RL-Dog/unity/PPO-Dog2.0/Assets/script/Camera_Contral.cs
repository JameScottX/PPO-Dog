using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Contral : MonoBehaviour {

    GameObject target_Body;
    Body body;

    public Vector3 offsetFormTarge = new Vector3(3, 0, -5);

    Vector3 destination = Vector3.zero;
  

    void Start () {

        target_Body = GameObject.Find("Body");
        body = target_Body.GetComponent<Body>();

    }

    private void LateUpdate()
    {

        transform.position = body.transform.position + offsetFormTarge;
        transform.rotation = Quaternion.Euler(0,-40,0);
    }


}
