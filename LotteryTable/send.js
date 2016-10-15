#!/usr/bin/env node

var amqp = require('amqplib/callback_api');

amqp.connect('amqp://localhost', function(err, conn)
{
    conn.createChannel(function(err, ch)
    {
        var q = 'hello';
        ch.assertQueue(q, {durable: false});
        //ch.sendToQueue(q, new Buffer('Hello World!'));
        //ch.sendToQueue(q, new Buffer(JSON.parse('{"date": 20160901,"homeTeam": "team1","awayTeam": "team2", "win": 1.12, "draw": 3.1,"lose": 5.13,"predict": 3,"result": 3 }')));
        for(var i = 0; i < 100; ++i)
        {
            ch.sendToQueue(q, new Buffer('{"date": ' + i + ',"homeTeam": "team1","awayTeam": "team2", "win": 1.12, "draw": 3.1,"lose": 5.13,"predict": 3,"result": 3 }'));
            console.log("Sent one message ");
            //sleep(1000);
        }        
    });

    setTimeout(function()
    {
        conn.close();
        process.exit(0);
    }, 500);
});

function sleep(sleepTime) {
    for(var start = +new Date; +new Date - start <= sleepTime; ) { }
}