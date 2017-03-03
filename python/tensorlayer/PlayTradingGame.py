#! /usr/bin/python
# -*- coding: utf8 -*-


import tensorflow as tf
import tensorlayer as tl
import numpy as np
import time
import market
import matplotlib.pyplot as plt
import matplotlib.finance as mpf

def plotKBars(chart):
    print(chart)
    dataList = []    
    for i in range(0, 20):
        (last, high, low, open, close) = (0, 0, 0, 0, 0)        
        for j in range(0, 50):
            if chart[j][i] == 1:
                if last == 0:
                    high = 50 - j
                    close = 50 - j                
                elif last == 2:
                    close = 50 - j
                    open = 50 - j
                else:
                    open = 50 - j
                    low = 50 - j
            elif chart[j][i] == 2:
                if last == 0:
                    high = 50 - j
                else:
                    low = 50 - j
            elif chart[j][i] == -1:
                if last == 0:
                    high = 50 - j
                    open = 50 - j
                    low = 50 - j
                elif last == -2:
                    open = 50 - j
                    close = 50 - j                
                else:
                    low = 50 - j
                    close = 50 - j
            elif chart[j][i] == -2:
                if last == 0:
                    high = 50 - j
                else:
                    low = 50 - j
            last = chart[j][i]
        bar = (i+1, open, high, low, close)
        dataList.append(bar)

    print(dataList)
    
    fig, ax = plt.subplots()
    fig.subplots_adjust(bottom=0.2)    
    #ax.xaxis_date()
    ax.autoscale_view()    
    plt.xticks(rotation=45)
    plt.yticks()
    plt.title("K bars")
    plt.xlabel("seq")
    plt.ylabel("price point")
    mpf.candlestick_ohlc(ax,dataList,width=0.5,colorup='r',colordown='green')
    plt.grid()
    plt.show()


# hyperparameters
D = 1000
H = 200
batch_size = 10
learning_rate = 1e-4
gamma = 1.0 # discount factor, we don't use it in TradingGame
decay_rate = 0.99
resume = True      # load existing policy network
model_file_name = "model_trading.bak"
np.set_printoptions(threshold=np.nan)
actionChoices = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]

env = market.MarketEnv()
env.initialize('F:\\temp\\TradingGameTestData')
observation = env.reset()
startChart = observation
prev_x = None
running_reward = None
reward_sum = 0
episode_number = 0

xs, ys, rs = [], [], []
# observation for training and inference
states_batch_pl = tf.placeholder(tf.float32, shape=[None, D])
# policy network
network = tl.layers.InputLayer(states_batch_pl, name='input_layer')
network = tl.layers.DenseLayer(network, n_units=H,
                                        act = tf.nn.relu, name='relu1')
network = tl.layers.DenseLayer(network, n_units=len(actionChoices),
                            act = tf.identity, name='output_layer')
probs = network.outputs
sampling_prob = tf.nn.softmax(probs)

actions_batch_pl = tf.placeholder(tf.int32, shape=[None])
discount_rewards_batch_pl = tf.placeholder(tf.float32, shape=[None])
loss = tl.rein.cross_entropy_reward_loss(probs, actions_batch_pl,
                                                    discount_rewards_batch_pl)
train_op = tf.train.RMSPropOptimizer(learning_rate, decay_rate).minimize(loss)

with tf.Session() as sess:
    # init = tf.initialize_all_variables()
    # sess.run(init)
    tl.layers.initialize_global_variables(sess)
    if resume:
        load_params = tl.files.load_npz(name=model_file_name+'.npz')
        tl.files.assign_params(sess, load_params, network)
    network.print_params()
    network.print_layers()

    done = False
    while not done:
        prob = tl.utils.predict(sess, network, observation.reshape(1, D), states_batch_pl, sampling_prob)
        #print(prob)
        action = np.random.choice(actionChoices, p=prob[0])
        observation, reward, done = env.nextTradingCycle(action)
        print('action: %d, pv: %f' % (action, env.getPV()))
    
    print ('Result - PV: %f, Benchmark: %f, Win: %f' % (env.getPV(), env.getBenchmark(), env.getPV()/env.getBenchmark() - 1.0))
    plotKBars(startChart.reshape(50,20))
