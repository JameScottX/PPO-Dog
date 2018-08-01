import random
import sys
from PyQt5.QtCore import QByteArray, QDataStream, QIODevice, QSettings
from PyQt5.QtWidgets import *
from PyQt5.QtNetwork import *

from Server import Server
from PPO import PPO


import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
import gym


Method = ['train','run'][1]


class UICenter(QDialog,Server):

        def __init__(self, parent = None):

            super(UICenter,self).__init__(parent)     

            self.setWindowTitle("Meachine Learning in Unity3D")
            VB_Main = QVBoxLayout(self)
            HB_Set = QHBoxLayout()
            HB_Mode = QHBoxLayout()
            VB_Main.addLayout(HB_Set)
            VB_Main.addLayout(HB_Mode)

            self.textContent = QTextBrowser(self)
            VB_Main.addWidget(self.textContent)

            pbClear = QPushButton()
            pbClear.setText("Clear")
            VB_Main.addWidget(pbClear)
            pbClear.released.connect(self.clearText)
            
            self.rbTrain = QRadioButton()
            self.rbTrain.setText("Train")
            HB_Mode.addWidget(self.rbTrain)
            

            self.rbRun = QRadioButton()
            self.rbRun.setText("Run")
            HB_Mode.addWidget(self.rbRun)
           
          
            self.pbSave = QPushButton()
            self.pbSave.setText("Save")
            HB_Mode.addWidget(self.pbSave)
            self.pbSave.released.connect(self.saveTrain)

            lab_le_Address = QLabel()
            lab_le_Address.setText("Address:")
            HB_Set.addWidget(lab_le_Address)

            self.le_Address = QLineEdit()
            HB_Set.addWidget(self.le_Address)

            lab_le_Address = QLabel()
            lab_le_Address.setText("Port:")
            HB_Set.addWidget(lab_le_Address)

            self.le_Port =  QLineEdit()
            HB_Set.addWidget(self.le_Port)

            self.pb_Create = QPushButton()
            self.pb_Create.setText("Create Server")
            HB_Set.addWidget(self.pb_Create)

            self.pb_Create.released.connect(self.CreateTCP)   # button build TCP Server 

            self.ep_max = 1500
            self.ep_length = 100
            self.is_gameover = True
            self.batch_count = 0
            self.ep_count =0
            self.sum_reward =0
         

            self.buffer_s,self.buffer_a,self.buffer_r=[],[],[]

            self.ppo = PPO(ep_max = self.ep_max,ep_length = self.ep_length ,
                      batch = 32,gmma = 0.9,obs_dim = 10,acts_dim = 8,acts_bound =1.3)


            if Method =='train':

                self.Receive_Msg.connect(self.Train_Process)    #TCP Signal emits states message
                self.ppo.ppo_initializer(judge = 'train')
                self.rbTrain.setChecked(True)
                
                self.writer = tf.summary.FileWriter('logs/',self.ppo.sess.graph)  

            elif Method == 'run':
                self.Receive_Msg.connect(self.Run_Process)       #TCP Signal emits states message
                self.ppo.ppo_initializer(judge='run')
                self.rbRun.setChecked(True)


        def saveTrain(self):

           save_path = self.ppo.save_net()
           self.textContent.append("Your save path is :"+ save_path)
           self.ppo.save_net()

        def CreateTCP(self):         #create  and close TCP by button

            if self.pb_Create.text() == "Create Server":

                self.pb_Create.setText("Close Server")
                self.buildServer()
                self.le_Address.setText(self.ipAddress)
                self.le_Port.setText(str(self.ipPort))
                self.textContent.append("Create TCP Server "+self.ipAddress+" Port: "+str(self.ipPort))
                
                

            elif self.pb_Create.text() == "Close Server":
                 self.pb_Create.setText("Create Server")
                 
                 self.serverShutDown()
                 self.textContent.append("Has Been shut down TCP Server at "+self.ipAddress+" _ "+str(self.ipPort))
                 
        def clearText(self):          #UI Clear Info_TextBrowser

            self.textContent.clear()

        def Msg_send(self,str_send):

            self.sendData(str_send)


        def Train_Process(self,str_get):  # here get environment params from TCP Server
            
            self.env_buffer  = str_get.split('_')  #c#的split和 python 有差异                        
            
            if len(self.env_buffer)==11:
                
                s = []
                a = []  

                self.batch_count +=1

                s.append( float(self.env_buffer[1])) #X轴的角度
                s.append( float(self.env_buffer[2])) #Z轴的角度

                s.append( float(self.env_buffer[3]))
                s.append( float(self.env_buffer[4]))
                s.append( float(self.env_buffer[5]))
                s.append( float(self.env_buffer[6]))
                s.append( float(self.env_buffer[7]))
                s.append( float(self.env_buffer[8]))
                s.append( float(self.env_buffer[9]))
                s.append( float(self.env_buffer[10]))

                s = np.hstack(s)

                r = float(self.env_buffer[0])
                
                a = self.ppo.choose_action(s)

                msg_action = str(float(a[0])) +'_'+str(float(a[1]))+'_'+str(float(a[2]))+'_'+str(float(a[3]))+'_'+str(float(a[4])) +'_'+str(float(a[5]))+'_'+str(float(a[6]))+'_'+str(float(a[7]))    

                self.Msg_send(msg_action)

                self.buffer_s.append(s)
                self.buffer_a.append(a)
                self.buffer_r.append((r+2)/2)

                self.sum_reward += r

                if self.batch_count % self.ppo.batch == 0:
         
                    self.ep_count += 1
                    self.batch_count = 0

                    v_s= self.ppo.get_value(s)

                    discounted_r = []

                    for r in self.buffer_r[::-1]:
                        v_s = r+ self.ppo.gmma * v_s
                        discounted_r.append(v_s)

                    discounted_r.reverse()

                    bs,ba,br = np.vstack(self.buffer_s),np.vstack(self.buffer_a),np.vstack(discounted_r) 
                    self.buffer_s,self.buffer_a,self.buffer_r=[],[],[]

                    self.ppo.update( bs,ba,br,self.ep_count)
                    
                    
                    print('Ep: %i'% self.ep_count + 'Reward : %i'%self.sum_reward )

                    self.sum_reward =0

                    if self.ep_count % self.ep_length == 0:

                        self.Msg_send('GameOver_')
                       
                    
            else:

                if self.env_buffer[0] == 'GameOver':                   
                    self.buffer_s,self.buffer_a,self.buffer_r=[],[],[]
                    self.batch_count = 0
                    self.sum_reward = 0
                    print('GameOver')
                    return

            
        def Run_Process(self,str_get):

            self.env_buffer  = str_get.split('_')

            if len(self.env_buffer)==11:

                s = []
                a = []  

                s.append( float(self.env_buffer[1])) #X轴的角度
                s.append( float(self.env_buffer[2])) #Z轴的角度

                s.append( float(self.env_buffer[3]))
                s.append( float(self.env_buffer[4]))
                s.append( float(self.env_buffer[5]))
                s.append( float(self.env_buffer[6]))
                s.append( float(self.env_buffer[7]))
                s.append( float(self.env_buffer[8]))
                s.append( float(self.env_buffer[9]))
                s.append( float(self.env_buffer[10]))

                s = np.hstack(s)          
                
                a = self.ppo.choose_action(s)

                msg_action = str(float(a[0])) +'_'+str(float(a[1]))+'_'+str(float(a[2]))+'_'+str(float(a[3]))+'_'+str(float(a[4])) +'_'+str(float(a[5]))+'_'+str(float(a[6]))+'_'+str(float(a[7]))    

                self.Msg_send(msg_action)




if __name__ == '__main__':
  
    import sys 

    app = QApplication(sys.argv)
    mainWindow = UICenter()
    mainWindow.show()

    #env = gym.make('CartPole-v0').unwrapped
    #ppo = PPO(ep_max = 1000,batch = 32,gmma = 0.9)
    #r_sum = []

    #for ep in range(ppo.ep_max):
    #    s = env.reset()
    #    buffer_s, buffer_a, buffer_r = [], [], []
    #    ep_r=0

    #    for t in range(ppo.ep_length):
    #        env.render()
    #        a = ppo.choose_action(s)
    #        s_,r,done,_ = env.step(a)
    #        buffer_s.append(s)
    #        buffer_a.append(a)
    #        buffer_r.append((r+8)/8)

    #        s= s_
    #        ep_r += r

    #        if t % ppo.batch ==0 or t== ppo.ep_length-1:

    #            v_s_ = ppo.get_value(s_)

    #            discounted_r = []

    #            for r in buffer_r[::-1]:

    #                v_s_ = r+ ppo.gmma * v_s_
    #                discounted_r.append(v_s_)
    #            discounted_r.reverse()
                
    #            bs,ba,br = np.vstack(buffer_s),np.vstack(buffer_a),np.vstack(discounted_r) 
    #            buffer_s,buffer_a,buffer_r=[],[],[]
    #            ppo.update( bs,ba,br)

    #    r_sum.append(ep_r)
    #    print(
    #        'EP : %i'%ep,
    #        '** ep_r : %i'%ep_r,
    #        )
    #plt.plot(np.arange(len(r_sum)), r_sum)
    #plt.show()

    sys.exit(mainWindow.exec_())





