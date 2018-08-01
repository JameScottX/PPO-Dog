import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt
import gym


ACTOR_UPDATE_STEPS = 10
CRITIC_UPDATE_STEPS =10
ACTOR_LEARNING_RATE = 0.00009
CRITIC_LEARNING_RARTE = 0.00018



METHOD = [
    dict(name='kl_pen', kl_target=0.01, lam=0.5),   # KL penalty
    dict(name='clip', epsilon=0.2),                 # Clipped surrogate objective, find this is better
][1]     

class PPO(object):

    def __init__(self,ep_max,ep_length,batch,gmma,obs_dim,acts_dim,acts_bound):

        
        self.ep_max = ep_max
        self.ep_length = ep_length
        self.batch = batch
        self.gmma = gmma
        self.obs_dim = obs_dim
        self.acts_dim = acts_dim
        self.acts_bound = acts_bound

        
        self.obs  =tf.placeholder(tf.float32,[None,self.obs_dim],name = 'obs')


        #critic
        self.critic_init(hid1_mult = 20)   
        #actor
        pi,pi_params  =self.build_net(train_able = True,name = 'pi',hid1_mult = 20 )
        oldpi,oldpi_params = self.build_net(train_able = False,name = 'oldpi',hid1_mult = 20)
        
        with tf.variable_scope('sample_action'):
            self.sample_op = tf.squeeze(pi.sample(1),axis =0)

        with tf.variable_scope('upadte_oldpi'):
            self.update_oldpi_pi = [oldp.assign(p) for p ,oldp in zip(pi_params,oldpi_params)]
        
        self.action = tf.placeholder(tf.float32,[None,self.acts_dim],'action')
        self.action_advantage = tf.placeholder(tf.float32,[None,1],'advantage')


        with tf.variable_scope('loss'):

            with tf.variable_scope('surrogate'):

                ratio = pi.prob(self.action)/oldpi.prob(self.action)
                pg_losses   = ratio * self.action_advantage

                if METHOD['name'] == 'k1_pen':
                    pass
                else:

                    pg_losses2 = tf.clip_by_value(ratio, 1.-METHOD['epsilon'], 1.+ METHOD['epsilon'])*self.action_advantage

                    self.action_loss = - tf.reduce_mean(tf.minimum(pg_losses,pg_losses2))
                                                        
                    tf.summary.scalar('loss',self.action_loss)  #显示loss变化

        with tf.variable_scope('action_train'):

            self.action_train_op = tf.train.AdamOptimizer(ACTOR_LEARNING_RATE).minimize(self.action_loss)
        

        self.saver = tf.train.Saver()
        self.saver_path = 'net_params'
        self.saver_name = 'object_ppo'

        self.sess = tf.Session()

        self.merged = tf.summary.merge_all()


    def critic_init(self,hid1_mult):

         with tf.variable_scope( 'critic'):

             hid1_size = self.obs_dim * hid1_mult
             #hid2_size = int(np.sqrt(hid1_size * 1))

             out = tf.layers.dense(self.obs,hid1_size,tf.nn.relu,name = 'h1');             
             #out = tf.layers.dense(out,hid2_size,tf.nn.relu,name = 'h2');   

             self.v_ = tf.layers.dense(out,1)

             self.discount_reward  =  tf.placeholder(tf.float32,[None,1],'discount_reward')

             self.advantage = self.discount_reward - self.v_
             self.cross  = tf.reduce_mean(tf.square(self.advantage))
             self.critic_train_op = tf.train.AdamOptimizer(CRITIC_LEARNING_RARTE).minimize(self.cross)

             reward_sum = tf.reduce_sum(self.discount_reward)
             tf.summary.scalar('critic_loss', self.cross)  #显示loss变化
             tf.summary.scalar('discount_reward',reward_sum)#显示折扣的奖励



    def build_net(self,train_able,name,hid1_mult):

        with tf.variable_scope(name):

            hid1_size = self.obs_dim * hid1_mult
            hid3_size = self.acts_dim * 10
            #hid2_size = int(np.sqrt(hid1_size * hid3_size))

            out = tf.layers.dense(self.obs,hid1_size,tf.nn.relu6,trainable = train_able,name = 'h1')

            #out = tf.layers.dense(out,hid2_size,tf.nn.relu,trainable = train_able,name = 'h2')

            out = tf.layers.dense(out,hid3_size,tf.nn.relu,trainable = train_able,name = 'h3')

            mu = self.acts_bound * tf.layers.dense(out,self.acts_dim ,tf.nn.tanh,trainable = train_able)

            sigmu = tf.layers.dense(out,self.acts_dim ,tf.nn.softplus,trainable = train_able)

            norm_dist = tf.distributions.Normal(loc = mu,scale =sigmu)

        params = tf.get_collection(tf.GraphKeys.GLOBAL_VARIABLES,scope = name)


        return norm_dist, params



    def choose_action(self,s):

        s = s[np.newaxis,:]
        a = self.sess.run(self.sample_op,{self.obs:s})[0]
       
        return np.clip(a,-self.acts_bound ,self.acts_bound )


    def update(self,s,a,r,count):

        self.sess.run(self.update_oldpi_pi)
        advantage = self.sess.run(self.advantage,{self.obs:s,self.discount_reward:r})

        if METHOD['name'] == 'kl_pen':

            pass

        else:

            [self.sess.run(self.action_train_op,{self.obs:s,self.action:a,self.action_advantage:advantage})for _ in range(ACTOR_UPDATE_STEPS)]
            [self.sess.run(self.critic_train_op,{self.obs:s,self.discount_reward:r})for _ in range(CRITIC_UPDATE_STEPS)]          

            result = self.sess.run(self.merged,{self.obs:s,self.action:a,self.action_advantage:advantage,self.discount_reward:r})
            self.writer.add_summary(result,count)

    def get_value(self,s):

        if s.ndim<2: s=s[np.newaxis,:]

        return self.sess.run(self.v_,{self.obs:s})[0,0]



    def ppo_initializer(self,judge ='train'):

        if judge == 'train':

            self.sess.run(tf.global_variables_initializer())

        elif judge == 'run':

            self.saver.restore(self.sess,tf.train.latest_checkpoint(self.saver_path))


    def save_net(self):

        save_path = self.saver.save(self.sess,(self.saver_path + '/'+self.saver_name) ,write_meta_graph=False)

        return save_path












