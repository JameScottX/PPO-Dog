using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RF_Leg : MonoBehaviour {

    GameObject RF_Thigh;
    GameObject RF_Calf;

    private float velocity_ = 250;
    private float force_ = 300;

    void objectInit()
    {

        RF_Thigh = GameObject.Find("RF_Thigh");
        RF_Calf = GameObject.Find("RF_Calf");

        HingeJoint hinge_RFT = RF_Thigh.GetComponent<HingeJoint>();

        JointLimits limits = hinge_RFT.limits;
        JointMotor motor = hinge_RFT.motor;

        //DOG2.0 - 1

        limits.min = -50;
        limits.max = -5;
        hinge_RFT.useLimits = true;
        hinge_RFT.limits = limits;

        motor.targetVelocity = -velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_RFT.useMotor = true;
        hinge_RFT.motor = motor;


        HingeJoint hinge_RFC = RF_Calf.GetComponent<HingeJoint>();
        limits = hinge_RFC.limits;
        motor = hinge_RFC.motor;

        limits.min = 20;
        limits.max = 100;
        hinge_RFC.useLimits = true;
        hinge_RFC.limits = limits;

        motor.targetVelocity = velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_RFC.useMotor = true;
        hinge_RFC.motor = motor;

    }

    void Start()
    {

        objectInit();
    }


    public void Thigh_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_RFT = RF_Thigh.GetComponent<HingeJoint>();

        float angle_ = hinge_RFT.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 10 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_RFT.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_RFT.motor = motor;

    }

    public void Calf_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_RFC = RF_Calf.GetComponent<HingeJoint>();

        float angle_ = hinge_RFC.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 15 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_RFC.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_RFC.motor = motor;

    }

    public float Thigh_GetAngle()
    {

        HingeJoint hinge_RFT = RF_Thigh.GetComponent<HingeJoint>();

        float angle_rad = hinge_RFT.angle * Mathf.Deg2Rad;

        return angle_rad;

    }

    public float Calf_GetAngle()
    {

        HingeJoint hinge_RFC = RF_Calf.GetComponent<HingeJoint>();

        float angle_rad = hinge_RFC.angle * Mathf.Deg2Rad;

        return angle_rad;

    }
}
