using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class Dog : MonoBehaviour {

    TCPSocket client;

    GameObject target_LF_Leg;
    GameObject target_LB_Leg;
    GameObject target_RF_Leg;
    GameObject target_RB_Leg;
    GameObject target_Body;

    LF_Leg lf_leg;
    RF_Leg rf_leg;
    LB_Leg lb_leg;
    RB_Leg rb_leg;
    Body body;

    private short msgDelayCount = 0;
    private string InfoSend ;

    void Start () {

        client = new TCPSocket();   //建立TCP通讯
        client.Connect2TcpServer();

        target_LF_Leg = GameObject.Find("LF_Leg");
        target_RF_Leg = GameObject.Find("RF_Leg");
        target_LB_Leg = GameObject.Find("LB_Leg");
        target_RB_Leg = GameObject.Find("RB_Leg");
        target_Body = GameObject.Find("Body");

        lf_leg = target_LF_Leg.GetComponent<LF_Leg>();
        rf_leg = target_RF_Leg.GetComponent<RF_Leg>();
        lb_leg = target_LB_Leg.GetComponent<LB_Leg>();
        rb_leg = target_RB_Leg.GetComponent<RB_Leg>();
        body = target_Body.GetComponent<Body>();

    }
    
    void FixedUpdate()
    {
        msgDelayCount++;
        

        if (msgDelayCount% 17 ==0 ) {
            
            msgDelayCount = 0;
            
            if (body.GameReset() != body.RIGHT)
            {  //这里游戏重置

                InfoSend = "GameOver_";
                client.SendMessage(InfoSend);
                Debug.Log("***********GameOver***********");
                GameReset(); //环境重置

            }
            else {

                InfoSend = body.GetXSpeedState().ToString() + "_" +   //X轴的速度奖励
                           body.GetXRotationState().ToString() + "_" +     //X轴的角度
                           body.GetZRotationState().ToString() + "_" +  //Z轴的角度
                           lf_leg.Thigh_GetAngle().ToString() + "_" +   //左前腿的大腿角度
                           lf_leg.Calf_GetAngle().ToString() + "_" +    //左前腿的小腿角度
                           rf_leg.Thigh_GetAngle().ToString() + "_" +    //左后腿的大腿角度
                           rf_leg.Calf_GetAngle().ToString() + "_" +  //左后腿的小腿角度
                           lb_leg.Thigh_GetAngle().ToString() + "_" +   //右前腿的大腿角度
                           lb_leg.Calf_GetAngle().ToString() + "_" +    //右前腿的小腿角度
                           rb_leg.Thigh_GetAngle().ToString() + "_" +    //右后腿的大腿角度
                           rb_leg.Calf_GetAngle().ToString() ;     //右后腿的小腿角度

                client.SendMessage(InfoSend);
                Debug.Log(InfoSend);
            }

        }

        while (client.MsgReceived) {

            client.MsgReceived = false;

            string[] msg_splited = client.strMsg.Split('_');  //分离行为信息

            if (msg_splited.Length == 8)
            {

                lf_leg.Thigh_RunAngle(float.Parse(msg_splited[0]));
                lf_leg.Calf_RunAngle(float.Parse(msg_splited[1]));
                rf_leg.Thigh_RunAngle(float.Parse(msg_splited[2]));
                rf_leg.Calf_RunAngle(float.Parse(msg_splited[3]));
                lb_leg.Thigh_RunAngle(float.Parse(msg_splited[4]));
                lb_leg.Calf_RunAngle(float.Parse(msg_splited[5]));
                rb_leg.Thigh_RunAngle(float.Parse(msg_splited[6]));
                rb_leg.Calf_RunAngle(float.Parse(msg_splited[7]));
            }
            else {

                GameReset();  //训练环境重置
            }          
        }
    }

    void GameReset(){

        client.TCPSocketQuit(); //这里需要先退出后在重置
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit()
    {

        client.TCPSocketQuit();
    }

}
