#!/usr/bin/python
import time
import pika

queueName = 'raw.market'
connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()
channel.queue_declare(queue=queueName)

for i in range(0, 5000):
    msg = '{"date": ' + str(i) + ',"homeTeam": "team1","awayTeam": "team2", "win": 1.12, "draw": 3.1,"lose": 5.13,"predict": 3,"result": 3 }'
    channel.basic_publish(exchange='',
                          routing_key=queueName,
                          body=msg)
    print('message sent %s' % i)
    time.sleep(0.01)
    
connection.close()