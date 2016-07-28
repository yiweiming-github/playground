import zmq
import time

context = zmq.Context()
sender = context.socket(zmq.PUSH)
sender.bind("tcp://*:5556")
#socket.bind("inproc://xxx")
s = "this is a message from servers."
i = 1

while True:
    msg = "%s %d" % (s, i)
    sender.send_string(msg)
    print(msg)
    time.sleep(0.2)
    i = i + 1
    