#! /usr/bin/python
# -*- coding: utf8 -*-


import tensorflow as tf
import tensorlayer as tl
import numpy as np
import time
import market

# hyperparameters
shape = (-1, 50, 20, 1)
D = 1000
H = 200
batch_size = 10
learning_rate = 1e-4
gamma = 1.0 # discount factor, we don't use it in TradingGame
decay_rate = 0.99
render = False      # display the game environment
resume = False      # load existing policy network
model_file_name = "model_trading_cnn"
np.set_printoptions(threshold=np.nan)
actionChoices = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]

env = market.MarketEnv()
env.initialize('F:\\temp\\TradingGameData')
observation = env.reset(useCNN = True)
prev_x = None
running_reward = None
reward_sum = 0
episode_number = 0

xs, ys, rs = [], [], []
# observation for training and inference
states_batch_pl = tf.placeholder(tf.float32, shape=[None, 50, 20, 1])

# policy network
network = tl.layers.InputLayer(states_batch_pl, name='input_layer')

network = tl.layers.Conv2dLayer(network,
                                act = tf.nn.relu,
                                shape = [5, 5, 1, 32],
                                strides = [1, 1, 1, 1],
                                padding='SAME',
                                name = 'cnn_layer1')
network = tl.layers.PoolLayer(network,
                              ksize=[1, 2, 2, 1],
                              strides=[1, 2, 2, 1],
                              padding='SAME',
                              pool = tf.nn.max_pool,
                              name ='pool_layer1')
network = tl.layers.Conv2dLayer(network,
                                act = tf.nn.relu,
                                shape = [5, 5, 32, 64],
                                strides=[1, 1, 1, 1],
                                padding='SAME',
                                name ='cnn_layer2')
network = tl.layers.PoolLayer(network,
                              ksize=[1, 2, 2, 1],
                              strides=[1, 2, 2, 1],
                              padding='SAME',
                              pool = tf.nn.max_pool,
                              name ='pool_layer2')
network = tl.layers.FlattenLayer(network, name ='flatten_layer')
# network = tl.layers.DropoutLayer(network, keep = 0.5, name = 'dropout_layer1')
# network = tl.layers.DenseLayer(network, n_units = 500, act = tf.nn.relu, name='relu1')
# network = tl.layers.DropoutLayer(network, keep = 0.5, name = 'dropout_layer2')

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

    start_time = time.time()
    game_number = 0
    while True:
        cur_x = observation

        #use change to decide action???
        #x = cur_x - prev_x if prev_x is not None else np.zeros(0)
        #x = x.reshape(shape)
        x = cur_x

        prev_x = cur_x
        #print(x)

        prob = sess.run(
            sampling_prob,
            feed_dict={states_batch_pl: x}
        )
        
        action = np.random.choice(actionChoices, p=prob.flatten())

        observation, reward, done = env.nextTradingCycle(action, useCNN = True)
        reward_sum += reward
        xs.append(x)            # all observations in a episode
        ys.append(action - 1)   # all fake labels in a episode (action begins from 1, so minus 1)
        rs.append(reward)       # all rewards in a episode
        
        #print ('%d action %s got reward %f' % (game_number, action, reward))
        game_number += 1
        if done:
            episode_number += 1
            game_number = 0

            if episode_number % batch_size == 0:
                print('batch over...... updating parameters......')
                epx = np.vstack(xs)
                epy = np.asarray(ys)
                epr = np.asarray(rs)
                disR = tl.rein.discount_episode_rewards(epr, gamma)
                disR -= np.mean(disR)
                disR /= np.std(disR)

                xs, ys, rs = [], [], []

                sess.run(
                    train_op,
                    feed_dict={
                        states_batch_pl: epx,
                        actions_batch_pl: epy,
                        discount_rewards_batch_pl: disR
                    }
                )

            if episode_number % (batch_size * 10) == 0:
                print('************************************    saving model %s    ************************************' % (model_file_name))
                tl.files.save_npz(network.all_params, name=model_file_name+'.npz')

            pv = env.getPV()
            benchmark = env.getBenchmark()
            #running_reward = reward_sum if running_reward is None else running_reward * 0.99 + reward_sum * 0.01
            running_reward = pv - 1.0 if running_reward is None else running_reward * 0.99 + (pv - 1.0) * 0.01
            print('resetting env. episode %d reward total was %f. running mean: %f. PV: %f. Benchmark: %f. win: %f' % (episode_number, reward_sum, running_reward, pv, benchmark, pv/benchmark - 1.0))
            reward_sum = 0
            observation = env.reset(useCNN = True) # reset env
            prev_x = None

        
        # if reward != 0:
        #     print(('episode %d: game %d took %.5fs, reward: %f' %
        #                 (episode_number, game_number,
        #                 time.time()-start_time, reward)),
        #                 ('' if reward == -1 else ' !!!!!!!!'))
        #     start_time = time.time()
        #     game_number += 1
