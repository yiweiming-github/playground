import tensorflow as tf
import tensorlayer as tl
import numpy as np
import time

def load_kbar_data(grid_file, predict_file, train_count, val_count, test_count):
    grid_data = np.loadtxt(grid_file)
    predict_data = np.loadtxt(predict_file)

    x_train = grid_data[:train_count]
    y_train = predict_data[:train_count]

    x_val = grid_data[train_count : train_count + val_count]
    y_val = predict_data[train_count : train_count + val_count]

    x_test = grid_data[train_count + val_count:]
    y_test = predict_data[train_count + val_count:]

    return x_train, y_train, x_val, y_val, x_test, y_test

totalCount = 69620
trainCount = int(totalCount * 0.71)
valCount = int((totalCount - trainCount) * 0.5)
testCount = totalCount - trainCount - valCount

# prepare data
X_train, y_train, X_val, y_val, X_test, y_test = load_kbar_data('F:\\temp\\grid.txt', 'F:\\temp\\predict.txt', trainCount, valCount, testCount)

shape = (-1, 50, 20, 1)
X_train = X_train.reshape(shape)
X_val = X_val.reshape(shape)
X_test = X_test.reshape(shape)

X_train = np.asarray(X_train, dtype=np.float32)
y_train = np.asarray(y_train, dtype=np.int64)
X_val = np.asarray(X_val, dtype=np.float32)
y_val = np.asarray(y_val, dtype=np.int64)
X_test = np.asarray(X_test, dtype=np.float32)
y_test = np.asarray(y_test, dtype=np.int64)

print('X_train.shape', X_train.shape)
print('y_train.shape', y_train.shape)
print('X_val.shape', X_val.shape)
print('y_val.shape', y_val.shape)
print('X_test.shape', X_test.shape)
print('y_test.shape', y_test.shape)
print('X %s   y %s' % (X_test.dtype, y_test.dtype))

sess = tf.InteractiveSession()

batch_size = 200
x = tf.placeholder(tf.float32, shape=[None, 50, 20, 1], name='x')
y_ = tf.placeholder(tf.int64, shape=[None, ], name='y_')

network = tl.layers.InputLayer(x, name ='input_layer')
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
network = tl.layers.DropoutLayer(network, keep = 0.5, name = 'dropout_layer1')
network = tl.layers.DenseLayer(network, n_units = 500, act = tf.nn.relu, name='relu1')
network = tl.layers.DropoutLayer(network, keep = 0.5, name = 'dropout_layer2')
network = tl.layers.DenseLayer(network, n_units = 10, act = tf.identity, name='output_layer')

y = network.outputs
cost = tl.cost.cross_entropy(y, y_, 'cost')

correct_prediction = tf.equal(tf.argmax(y, 1), y_)
acc = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))

# train
n_epoch = 100
learning_rate = 0.0001
print_freq = 1

train_params = network.all_params
train_op = tf.train.AdamOptimizer(learning_rate, beta1=0.9, beta2=0.999,
        epsilon=1e-08, use_locking=False).minimize(cost, var_list=train_params)

tl.layers.initialize_global_variables(sess)
network.print_params()
network.print_layers()

print('   learning_rate: %f' % learning_rate)
print('   batch_size: %d' % batch_size)

for epoch in range(n_epoch):
    start_time = time.time()
    for X_train_a, y_train_a in tl.iterate.minibatches(
                                    X_train, y_train, batch_size, shuffle=True):
        feed_dict = {x: X_train_a, y_: y_train_a}
        feed_dict.update( network.all_drop )        # enable noise layers
        sess.run(train_op, feed_dict=feed_dict)

    if epoch + 1 == 1 or (epoch + 1) % print_freq == 0:
        print("Epoch %d of %d took %fs" % (epoch + 1, n_epoch, time.time() - start_time))
        train_loss, train_acc, n_batch = 0, 0, 0
        for X_train_a, y_train_a in tl.iterate.minibatches(
                                    X_train, y_train, batch_size, shuffle=True):
            dp_dict = tl.utils.dict_to_one( network.all_drop )    # disable noise layers
            feed_dict = {x: X_train_a, y_: y_train_a}
            feed_dict.update(dp_dict)
            err, ac = sess.run([cost, acc], feed_dict=feed_dict)
            train_loss += err; train_acc += ac; n_batch += 1
        print("   train loss: %f" % (train_loss/ n_batch))
        print("   train acc: %f" % (train_acc/ n_batch))
        val_loss, val_acc, n_batch = 0, 0, 0
        for X_val_a, y_val_a in tl.iterate.minibatches(
                                        X_val, y_val, batch_size, shuffle=True):
            dp_dict = tl.utils.dict_to_one( network.all_drop )    # disable noise layers
            feed_dict = {x: X_val_a, y_: y_val_a}
            feed_dict.update(dp_dict)
            err, ac = sess.run([cost, acc], feed_dict=feed_dict)
            val_loss += err; val_acc += ac; n_batch += 1
        print("   val loss: %f" % (val_loss/ n_batch))
        print("   val acc: %f" % (val_acc/ n_batch))
        # try:
        #     tl.visualize.CNN2d(network.all_params[0].eval(),
        #                         second=10, saveable=False,
        #                         name='cnn1_'+str(epoch+1), fig_idx=2012)
        # except:
        #     raise Exception("# You should change visualize.CNN(), if you want to save the feature images for different dataset")

print('Evaluation')
test_loss, test_acc, n_batch = 0, 0, 0
for X_test_a, y_test_a in tl.iterate.minibatches(
                                X_test, y_test, batch_size, shuffle=True):
    dp_dict = tl.utils.dict_to_one( network.all_drop )    # disable noise layers
    feed_dict = {x: X_test_a, y_: y_test_a}
    feed_dict.update(dp_dict)
    err, ac = sess.run([cost, acc], feed_dict=feed_dict)
    test_loss += err; test_acc += ac; n_batch += 1
print("   test loss: %f" % (test_loss/n_batch))
print("   test acc: %f" % (test_acc/n_batch))