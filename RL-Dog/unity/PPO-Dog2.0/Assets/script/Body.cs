using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    GameObject head;

    private float x_speed = 0;
    private float x_last = 0;

    private float x_speed_last = 0;
    private float x_Acceleration = 0;

    public char RIGHT = 'R';
    public char ROTATION_X_ERROR = 'X';
    public char ROTATION_Z_ERROR = 'Z';
    private float ROTATION_X_LIMIT = 45;
    private float ROTATION_Z_LIMIT = 60;

    public double XSpeedReward = 0;
    public double XACCReward = 0;

    void Start () {

        head = GameObject.Find("Head");

    }

    private void FixedUpdate()
    {

        float x_now = transform.position.x;
        x_speed = x_now - x_last;
        x_last = x_now;

        x_Acceleration = x_speed - x_speed_last;
        x_speed_last = x_speed;

        
        GameReset();
    }


    public float GetXSpeedState() { //X方向得速度

        return x_speed*50;

    }

    public float GetXACCState() {  //X方向上得加速度

        return x_Acceleration *50;
    }

    public float GetYRotationState() { //Y轴得角度

        float trans = 180 - Mathf.Abs(transform.localEulerAngles.y - 180);   //0~180
        float angle_rad = trans * Mathf.Deg2Rad ;    
        
        return angle_rad;
    }

    public float GetXRotationState() {

        float XRotation = transform.rotation.x;

            return XRotation;
    }

    public float GetZRotationState() {

        float ZRotation = transform.rotation.z;

        return ZRotation;
    }

    public char GameReset() {

        float trans_x = 180 - Mathf.Abs(transform.localEulerAngles.x - 180);   //0~180
        float trans_z = 180 - Mathf.Abs(transform.localEulerAngles.z - 180);   //0~180

        if (trans_x > ROTATION_X_LIMIT)
        {

            return ROTATION_X_ERROR;
        }
        if (trans_z > ROTATION_Z_LIMIT)
        {

            return ROTATION_Z_ERROR;
        }
       
        return RIGHT;
    }


}
