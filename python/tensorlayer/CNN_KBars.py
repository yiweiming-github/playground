import time
import numpy as np
import tensorflow as tf

"""
using pure tensorflow to implement a CNN network
"""

def load_kbar_data(grid_file, predict_file):
    """
    load kbar data from grid file and predict file
    """
    grid_data = np.loadtxt(grid_file)
    predict_data = np.loadtxt(predict_file)

    total_count = len(grid_data)
    train_count = int(total_count * 0.71)
    val_count = int((total_count - train_count) * 0.5)

    x_train = grid_data[:train_count]
    y_train = predict_data[:train_count]

    x_val = grid_data[train_count : train_count + val_count]
    y_val = predict_data[train_count : train_count + val_count]

    x_test = grid_data[train_count + val_count:]
    y_test = predict_data[train_count + val_count:]

    return x_train, y_train, x_val, y_val, x_test, y_test

def get_batch_with_shuffle(inputs=None,
                           targets=None,
                           batch_size=None,
                           epoch=0,
                           next_data_start=0):
    """
    get next batch data with shuffle
    """
    start = next_data_start
    input_count = len(inputs)
    # shuffle
    if epoch == 0 and start == 0:
        perm0 = np.arange(input_count)
        np.random.shuffle(perm0)

    if start + batch_size > input_count:
        rest_input_count = input_count - start
        rest_inputs = inputs[start:input_count]
        rest_targets = targets[start:input_count]
        # shuffle
        perm = np.arange(input_count)
        np.random.shuffle(perm)

        # start next epoch
        start = 0
        next_data_start = batch_size - rest_input_count
        end = next_data_start
        input_new_part = inputs[start:end]
        target_new_part = targets[start:end]
        return np.concatenate((rest_inputs, input_new_part), axis=0), \
        np.concatenate((rest_targets, target_new_part), axis=0), \
        -1
    else:
        next_data_start += batch_size
        end = next_data_start
        return inputs[start:end], targets[start:end], next_data_start

def main():
    """
    main function
    """
    # prepare data
    x_train, y_train, x_val, y_val, x_test, y_test = load_kbar_data(
        'F:\\temp\\grid.txt',
        'F:\\temp\\predict.txt')

    shape = (-1, 50, 20, 1)
    x_train = x_train.reshape(shape)
    x_val = x_val.reshape(shape)
    x_test = x_test.reshape(shape)

    x_train = np.asarray(x_train, dtype=np.float)
    y_train = np.asarray(y_train, dtype=np.int64)
    x_val = np.asarray(x_val, dtype=np.float)
    y_val = np.asarray(y_val, dtype=np.int64)
    x_test = np.asarray(x_test, dtype=np.float)
    y_test = np.asarray(y_test, dtype=np.int64)

    print('x_train.shape', x_train.shape)
    print('y_train.shape', y_train.shape)
    print('x_val.shape', x_val.shape)
    print('y_val.shape', y_val.shape)
    print('x_test.shape', x_test.shape)
    print('y_test.shape', y_test.shape)
    print('X %s   y %s' % (x_test.dtype, y_test.dtype))

    sess = tf.InteractiveSession()

    batch_size = 1000
    #define placeholder
    x = tf.placeholder(tf.float32, shape=[None, 50, 20, 1], name='x')
    y_ = tf.placeholder(tf.int64, shape=[None, ], name='y_')

    #define CNN
    with tf.name_scope('cnn_layer1'):
        weights = tf.get_variable(name='W_conv2d1', shape=[5, 5, 1, 32])
        conv2d_layer1 = tf.nn.relu(tf.nn.conv2d(x,
                                                weights,
                                                strides=[1, 1, 1, 1],
                                                padding='SAME',
                                                name='cnn_layer1'))

    with tf.name_scope('pool_layer1'):
        pool_layer1 = tf.nn.max_pool(conv2d_layer1,
                                     ksize=[1, 2, 2, 1],
                                     strides=[1, 2, 2, 1],
                                     padding='SAME',
                                     name='pool_layer1')

    with tf.name_scope('cnn_layer2'):
        weights = tf.get_variable(name='W_conv2d2', shape=[5, 5, 32, 64])
        conv2d_layer2 = tf.nn.relu(tf.nn.conv2d(pool_layer1,
                                                weights,
                                                strides=[1, 1, 1, 1],
                                                padding='SAME',
                                                name='cnn_layer2'))

    with tf.name_scope('pool_layer2'):
        pool_layer2 = tf.nn.max_pool(conv2d_layer2,
                                     ksize=[1, 2, 2, 1],
                                     strides=[1, 2, 2, 1],
                                     padding='SAME',
                                     name='pool_layer2')

    with tf.name_scope('flattern_layer'):
        dim = 1
        for d in pool_layer2.get_shape()[1:].as_list():
            dim *= d
        flatten_layer = tf.reshape(pool_layer2, shape=[-1, dim], name='flatten_layer')

    with tf.name_scope('dropout_layer1'):
        dropout_layer1 = tf.nn.dropout(flatten_layer, keep_prob=0.5, name='dropout_layer1')

    with tf.name_scope('dense_layer1'):
        n_in = int(dropout_layer1.get_shape()[-1])
        n_units = 500
        weights = tf.Variable(tf.random_uniform([n_in, n_units], -1.0, 1.0), name='weights')
        bias = tf.Variable(tf.zeros(shape=[n_units]), name='bias')
        dense_layer1 = tf.nn.relu(tf.matmul(dropout_layer1, weights) + bias)

    with tf.name_scope('dropout_layer2'):
        dropout_layer2 = tf.nn.dropout(dense_layer1, keep_prob=0.5, name='dropout_layer2')

    with tf.name_scope('output_layer'):
        n_in = int(dropout_layer2.get_shape()[-1])
        n_units = 10
        weights = tf.Variable(tf.random_uniform([n_in, n_units], -1.0, 1.0), name='weights')
        bias = tf.Variable(tf.zeros(shape=[n_units]), name='bias')
        dense_layer2 = tf.identity(tf.matmul(dropout_layer2, weights) + bias)

    y = dense_layer2

    #define loss: cross_entropy
    loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(logits=y, labels=y_))

    #define Evaluation
    correct_prediction = tf.equal(tf.argmax(y, 1), y_)
    evaluation = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))

    #define optimizer and train
    optimizer = tf.train.AdamOptimizer(learning_rate=0.0001,
                                       beta1=0.9,
                                       beta2=0.999,
                                       epsilon=1e-08,
                                       use_locking=False)
    train_op = optimizer.minimize(loss)

    n_epoch = 100
    with tf.Session() as sess:
        # initialize
        init = tf.global_variables_initializer()
        sess.run(init)

        print('start training...')
        for epoch in range(n_epoch):
            total_loss, next_data_start, step = 0, 0, 0
            start_time = time.time()
            while next_data_start >= 0:
                x_train_a, y_train_a, next_data_start = get_batch_with_shuffle(x_train,
                                                                               y_train,
                                                                               batch_size,
                                                                               epoch,
                                                                               next_data_start)
                _, loss_value = sess.run([train_op, loss], feed_dict={x: x_train_a, y_: y_train_a})
                total_loss += loss_value
                step += 1

            duration = time.time() - start_time
            print('loss = %f, time spent: %.3f' % (total_loss / step, duration))

            # get validation batch
            val_loss, val_acc, step, next_val_data_start = 0, 0, 0, 0
            while next_val_data_start >= 0:
                x_val_a, y_val_a, next_val_data_start = get_batch_with_shuffle(x_val,
                                                                               y_val,
                                                                               batch_size,
                                                                               epoch,
                                                                               next_val_data_start)
                # one validation
                accuracy = sess.run(evaluation, feed_dict={x: x_val_a, y_: y_val_a})
                val_acc += accuracy
                val_loss += loss
                step += 1

            val_acc = val_acc / step
            print("   val acc: %f" % (val_acc))

if __name__ == '__main__':
    main()
