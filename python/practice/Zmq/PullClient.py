import sys
import zmq
import time

pusherAddresses = ['tcp://localhost:5556', 'tcp://localhost:5557']

#  Socket to talk to server
context = zmq.Context()
poller = zmq.Poller()

receivers = []
for address in pusherAddresses:
    receiver = context.socket(zmq.PULL)
    receiver.connect(address)
    poller.register(receiver, zmq.POLLIN)
    receivers.append(receiver)

print("Collecting updates from weather server")

while True:
    try:
        socks = dict(poller.poll())
    except KeyboardInterrupt:
        break
    
    for receiver in receivers:
        if receiver in socks:
            string = receiver.recv_string()
            print(string)
    