using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Leg : MonoBehaviour {

    GameObject LB_Thigh;
    GameObject LB_Calf;

    private float velocity_ = 250;
    private float force_ = 300;

    void objectInit()
    {

        LB_Thigh = GameObject.Find("LB_Thigh");
        LB_Calf = GameObject.Find("LB_Calf");

        HingeJoint hinge_LBT = LB_Thigh.GetComponent<HingeJoint>();

        JointLimits limits = hinge_LBT.limits;
        JointMotor motor = hinge_LBT.motor;

        limits.min = -50;
        limits.max = -5;
        hinge_LBT.useLimits = true;
        hinge_LBT.limits = limits;

        motor.targetVelocity = -velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_LBT.useMotor = true;
        hinge_LBT.motor = motor;


        HingeJoint hinge_LBC = LB_Calf.GetComponent<HingeJoint>();
        limits = hinge_LBC.limits;
        motor = hinge_LBC.motor;

        limits.min = 20;
        limits.max = 100;
        hinge_LBC.useLimits = true;
        hinge_LBC.limits = limits;

        motor.targetVelocity = velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_LBC.useMotor = true;
        hinge_LBC.motor = motor;

    }

    void Start()
    {

        objectInit();
    }


    public void Thigh_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_LBT = LB_Thigh.GetComponent<HingeJoint>();

        float angle_ = hinge_LBT.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 10 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_LBT.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_LBT.motor = motor;

    }

    public void Calf_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_LBC = LB_Calf.GetComponent<HingeJoint>();

        float angle_ = hinge_LBC.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 15 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_LBC.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_LBC.motor = motor;

    }

    public float Thigh_GetAngle()
    {

        HingeJoint hinge_LBT = LB_Thigh.GetComponent<HingeJoint>();

        float angle_rad = hinge_LBT.angle * Mathf.Deg2Rad;

        return angle_rad;

    }

    public float Calf_GetAngle()
    {

        HingeJoint hinge_LBC = LB_Calf.GetComponent<HingeJoint>();

        float angle_rad = hinge_LBC.angle * Mathf.Deg2Rad;

        return angle_rad;

    }
}
