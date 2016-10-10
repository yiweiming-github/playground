#!/usr/bin/python
import datetime
import time
import pika

queueName = 'raw.market'
connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()
channel.queue_declare(queue=queueName)

today = datetime.datetime.now().strftime('%Y-%m-%d')

for i in range(0, 5000):
    msg = '{"date": "' + today + '", "seq": ' + str(i) + ',"time": "' + str(datetime.datetime.now()) + '","code": "sample code", "ask": 100, "bid": 99,"mid": 99.5}'
    channel.basic_publish(exchange='',
                          routing_key=queueName,
                          body=msg)
    print('message sent %s' % msg)
    time.sleep(0.005)
    
connection.close()