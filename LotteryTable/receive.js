#!/usr/bin/env node

var amqp = require('amqplib/callback_api');

amqp.connect('amqp://localhost', function(err, conn)
{
    conn.createChannel(function(err, ch)
    {
        var q = 'hello';

        ch.assertQueue(q, {durable: false});
        console.log("waiting for message in %s", q);
        ch.consume(q, function(msg)
        {
            console.log(" received: %s", msg.content.toString());
        }, {noAcke: true});
    });
});