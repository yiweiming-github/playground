import KbarUtils as kbutil
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
model_file_name = "model_trading_cnn.bak"
np.set_printoptions(threshold=np.nan)
actionChoices = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]

env = market.MarketEnv()
env.initialize('F:\\temp\\TradingGameTestData')
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
    tl.layers.initialize_global_variables(sess)
    load_params = tl.files.load_npz(name=model_file_name+'.npz')
    tl.files.assign_params(sess, load_params, network)
    network.print_params()
    network.print_layers()

    prices = env.getPrices()
    benchmarkPrice = prices[0][2]
    done = False
    pvList = []
    pvList.append(benchmarkPrice) #to align with bar count in K-bar chart
    pvList.append(benchmarkPrice) 
    while not done:
        prob = tl.utils.predict(sess, network, observation.reshape(shape), states_batch_pl, sampling_prob)
        #print(prob)
        action = np.random.choice(actionChoices, p=prob[0])
        observation, reward, done = env.nextTradingCycle(action)
        # latestChart = observation.reshape(50, 20)
        # latestBar = np.array([latestChart[0:, 19]]).T
        # startChart = np.concatenate((startChart, latestBar), axis = 1)
        pvList.append(env.getPV() * benchmarkPrice)
        print('action: %d, pv: %f' % (action, env.getPV()))
    
    print ('CNN Result - PV: %f, Benchmark: %f, Win: %f' % (env.getPV(), env.getBenchmark(), env.getPV()/env.getBenchmark() - 1.0))
    #plotKBarsFromChart(startChart, 50, 20)        
    kbutil.plotKBarsFromPrices(prices, pvList)