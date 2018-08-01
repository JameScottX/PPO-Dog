using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Leg : MonoBehaviour {

    GameObject RB_Thigh;
    GameObject RB_Calf;

    private float velocity_ = 250;
    private float force_ = 300;

    void objectInit()
    {

        RB_Thigh = GameObject.Find("RB_Thigh");
        RB_Calf = GameObject.Find("RB_Calf");

        HingeJoint hinge_RBT = RB_Thigh.GetComponent<HingeJoint>();

        JointLimits limits = hinge_RBT.limits;
        JointMotor motor = hinge_RBT.motor;

        limits.min = -50;
        limits.max = -5;
        hinge_RBT.useLimits = true;
        hinge_RBT.limits = limits;

        motor.targetVelocity = -velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_RBT.useMotor = true;
        hinge_RBT.motor = motor;


        HingeJoint hinge_RBC = RB_Calf.GetComponent<HingeJoint>();
        limits = hinge_RBC.limits;
        motor = hinge_RBC.motor;

        limits.min = 20;
        limits.max = 100;
        hinge_RBC.useLimits = true;
        hinge_RBC.limits = limits;

        motor.targetVelocity = velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_RBC.useMotor = true;
        hinge_RBC.motor = motor;

    }

    void Start()
    {

        objectInit();
    }


    public void Thigh_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_RBT = RB_Thigh.GetComponent<HingeJoint>();

        float angle_ = hinge_RBT.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 10 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_RBT.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_RBT.motor = motor;

    }

    public void Calf_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_RBC = RB_Calf.GetComponent<HingeJoint>();

        float angle_ = hinge_RBC.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 15 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_RBC.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_RBC.motor = motor;

    }

    public float Thigh_GetAngle()
    {

        HingeJoint hinge_RBT = RB_Thigh.GetComponent<HingeJoint>();

        float angle_rad = hinge_RBT.angle * Mathf.Deg2Rad;

        return angle_rad;

    }

    public float Calf_GetAngle()
    {

        HingeJoint hinge_RBC = RB_Calf.GetComponent<HingeJoint>();

        float angle_rad = hinge_RBC.angle * Mathf.Deg2Rad;

        return angle_rad;

    }
}
