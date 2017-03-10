# Kbar image classfication using tensorflow

import tensorflow as tf
import numpy as np
import time

def load_kbar_data(grid_file, predict_file, random_shuffle=False):
    grid_data = np.loadtxt(grid_file)
    predict_data = np.loadtxt(predict_file)

    total_count = len(grid_data)
    train_count = int(total_count * 0.71)
    val_count = int((total_count - train_count) * 0.5)
    test_count = total_count - train_count - val_count

    x_train = grid_data[:train_count]
    y_train = predict_data[:train_count]

    x_val = grid_data[train_count : train_count + val_count]
    y_val = predict_data[train_count : train_count + val_count]

    x_test = grid_data[train_count + val_count:]
    y_test = predict_data[train_count + val_count:]

    return x_train, y_train, x_val, y_val, x_test, y_test

def getBatchWithShuffle(inputs=None, targets=None, batch_size=None, epoch=0, nextDataStart=0):
    start = nextDataStart
    inputCount = len(inputs)
    # shuffle
    if epoch == 0 and start == 0:
        perm0 = np.arange(inputCount)
        np.random.shuffle(perm0)
        currentInputs = inputs[perm0]
        currentTargets = targets[perm0]
    
    if start + batch_size > inputCount:        
        restInputCount = inputCount - start
        restInputs = inputs[start:inputCount]
        restTargets = targets[start:inputCount]
        # shuffle
        perm = np.arange(inputCount)
        np.random.shuffle(perm)
        currentInputs = inputs[perm]
        currentTargets = targets[perm]
        # start next epoch
        start = 0
        nextDataStart = batch_size - restInputCount
        end = nextDataStart
        inputNewPart = inputs[start:end]
        targetNewPart = targets[start:end]
        return np.concatenate((restInputs, inputNewPart), axis=0) , np.concatenate((restTargets, targetNewPart), axis=0), -1
    else:
        nextDataStart += batch_size
        end = nextDataStart
        return inputs[start:end], targets[start:end], nextDataStart


# prepare data
X_train, y_train, X_val, y_val, X_test, y_test = load_kbar_data('F:\\temp\\grid.txt', 'F:\\temp\\predict.txt')

# define placeholder
x = tf.placeholder(tf.float32, shape=[None, 1000], name='x')
y_ = tf.placeholder(tf.int64, shape=[None, ], name='y_')

#define NN
#dropoutLayer1
with tf.name_scope('dropout1'):
    dropoutLayer1 = tf.nn.dropout(x, keep_prob=0.8, name='drop1')

#denseLayer1
with tf.name_scope('dense1'):
    n_in = int(dropoutLayer1.get_shape()[-1])
    n_units = 800
    weights = tf.Variable(tf.random_uniform([n_in,n_units], -1.0, 1.0), name='weights')
    bias = tf.Variable(tf.zeros(shape=[n_units]), name='bias')
    denseLayer1 = tf.nn.relu(tf.matmul(dropoutLayer1, weights) + bias)

#dropoutLayer2
with tf.name_scope('drop2'):
    dropoutLayer2 = tf.nn.dropout(denseLayer1, keep_prob=0.5, name='drop2')

#denseLayer2
with tf.name_scope('dense2'):
    n_in = int(dropoutLayer2.get_shape()[-1])
    n_units = 10
    weights = tf.Variable(tf.random_uniform([n_in,n_units], -1.0, 1.0), name='weights')
    bias = tf.Variable(tf.zeros(shape=[n_units]), name='bias')
    denseLayer2 = tf.identity(tf.matmul(dropoutLayer2, weights) + bias)

y = denseLayer2

# define loss
loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(logits=y, labels=y_))

# define evaluation
correct_prediction = tf.equal(tf.argmax(y, 1), y_)
evaluation = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))

#define optimizer and train
optimizer = tf.train.AdamOptimizer(learning_rate=0.001, beta1=0.9, beta2=0.999, epsilon=1e-08, use_locking=False)
train_op = optimizer.minimize(loss)

summary = tf.summary.merge_all()
saver = tf.train.Saver()

with tf.Session() as sess:
    summary_writer = tf.summary.FileWriter('F:\\temp\\TFoutputs', sess.graph)

    # initialize
    init = tf.global_variables_initializer()
    sess.run(init)

    # train
    batch_size = 1000
    epochCount = 100
    
    print('start training...')
    for epoch in range(epochCount):
        totalLoss, totalAccuracy, nextDataStart, step = 0, 0, 0, 0
        start_time = time.time()
        while nextDataStart >= 0:             
            # get batch        
            X_train_a, y_train_a, nextDataStart = getBatchWithShuffle(X_train, y_train, batch_size, epoch, nextDataStart)
            # one train
            lossValue = sess.run([train_op, loss], feed_dict = {x: X_train_a, y_: y_train_a})
            totalLoss += lossValue            
            step += 1
        duration = time.time() - start_time
        print('loss = %f, time spent: %.3f' % (totalLoss / step, duration))
        # summary_str = sess.run(summary, feed_dict={x: X_train, y_: y_train})
        # summary_writer.add_summary(summary_str, step)
        # summary_writer.flush()
        
        # get validation batch
        valLoss, valAcc, step, nextValDataStart = 0, 0, 0, 0
        while nextValDataStart >= 0:
            X_val_a, y_val_a, nextValDataStart = getBatchWithShuffle(X_val, y_val, batch_size, epoch, nextValDataStart)
            # one validation
            accuracy = sess.run(evaluation, feed_dict={x: X_val_a, y_: y_val_a})
            valAcc += accuracy
            valLoss += loss
            step += 1
        
        valAcc = valAcc / step
        print("   val acc: %f" % (valAcc))

    # # test
    # X_test_a, y_test_a = getBatchWithShuffle(X_test, y_test, batch_size)        
    # testLoss, testAcc = sess.run(acc, feed_dict={x: X_test_a, y_: y_test_a})
    # print("   test loss: %f" % (testLoss))
    # print("   test acc: %f" % (testAcc))
