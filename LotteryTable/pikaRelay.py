#!/usr/bin/python
import time
import pika
from collections import deque 
import threading

connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()
inQueueName = 'raw.market'
outQueueName = 'snapshot.market'

cache = deque()
def messageCallback(ch, method, properties, body):
    print("get raw message %s" % body)
    if len(cache) > 20:
        cache.popleft()
    cache.append(body)

def publishSnapshot():
    while True:
        if len(cache) > 0:
            print("publishing snapshot")
            msg = '[' + ','.join(list(cache)) + ']'
            channel.basic_publish(exchange='',
                                  routing_key=outQueueName,
                                  body=msg)
        time.sleep(0.5)

publishThread = threading.Thread(target=publishSnapshot)
publishThread.setDaemon(True)
publishThread.start()


channel.queue_declare(queue=inQueueName)
channel.basic_consume(messageCallback, queue=inQueueName, no_ack=True)

channel.queue_declare(queue=outQueueName)

print('wating for message...')
channel.start_consuming()

print('ending...')
connection.close()