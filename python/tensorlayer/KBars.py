import tensorflow as tf
import tensorlayer as tl
import numpy as np

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
    

sess = tf.InteractiveSession()

# prepare data
X_train, y_train, X_val, y_val, X_test, y_test = load_kbar_data('F:\\temp\\grid.txt', 'F:\\temp\\predict.txt')

# define placeholder
x = tf.placeholder(tf.float32, shape=[None, 1000], name='x')
y_ = tf.placeholder(tf.int64, shape=[None, ], name='y_')

# define the network
network = tl.layers.InputLayer(x, name='input_layer')
network = tl.layers.DropoutLayer(network, keep=0.8, name='drop1')
# network = tl.layers.DenseLayer(network, n_units=500,
#                                 act = tf.nn.relu, name='relu1')
# network = tl.layers.DropoutLayer(network, keep=0.5, name='drop2')
network = tl.layers.DenseLayer(network, n_units=800,
                                act = tf.nn.relu, name='relu2')
network = tl.layers.DropoutLayer(network, keep=0.5, name='drop3')
# the softmax is implemented internally in tl.cost.cross_entropy(y, y_) to
# speed up computation, so we use identity here.
# see tf.nn.sparse_softmax_cross_entropy_with_logits()
network = tl.layers.DenseLayer(network, n_units=10,
                                act = tf.identity,
                                name='output_layer')
# define cost function and metric.
y = network.outputs
cost = tl.cost.cross_entropy(y, y_)
correct_prediction = tf.equal(tf.argmax(y, 1), y_)
acc = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))
y_op = tf.argmax(tf.nn.softmax(y), 1)

# define the optimizer
train_params = network.all_params
train_op = tf.train.AdamOptimizer(learning_rate=0.001, beta1=0.9, beta2=0.999,
                            epsilon=1e-08, use_locking=False).minimize(cost, var_list=train_params)

# initialize all variables in the session
tl.layers.initialize_global_variables(sess)

# print network information
network.print_params()
network.print_layers()

# train the network
tl.utils.fit(sess, network, train_op, cost, X_train, y_train, x, y_,
            acc=acc, batch_size=1000, n_epoch=100, print_freq=1,
            X_val=X_val, y_val=y_val, eval_train=False)

# evaluation
tl.utils.test(sess, network, acc, X_test, y_test, x, y_, batch_size=None, cost=cost)

# save the network to .npz file
tl.files.save_npz(network.all_params , name='model.npz')
sess.close()