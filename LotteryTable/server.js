/**
 * This file provided by Facebook is for non-commercial testing and evaluation
 * purposes only. Facebook reserves all rights not expressly granted.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * FACEBOOK BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

var fs = require('fs');
var path = require('path');
var express = require('express');
var bodyParser = require('body-parser');

var app = express();

var COMMENTS_FILE = path.join(__dirname, 'comments.json');
var ANALYSIS_FILE = path.join(__dirname, 'analysis.json');

app.set('port', (process.env.PORT || 3000));

app.use('/', express.static(path.join(__dirname, 'public')));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: true}));

// Additional middleware which will set headers that we need on each request.
app.use(function(req, res, next) {
    // Set permissive CORS header - this allows this server to be used only as
    // an API server in conjunction with something like webpack-dev-server.
    res.setHeader('Access-Control-Allow-Origin', '*');

    // Disable caching so we'll always get the latest comments.
    res.setHeader('Cache-Control', 'no-cache');
    next();
});

app.get('/analysis', function(req, res){
  res.sendFile(__dirname + "/public/analysis.html")
});
app.get('/stream', function(req, res){
  res.sendFile(__dirname + "/public/stream.html")
});
app.get('/test', function(req, res){
  res.sendFile(__dirname + "/public/reactbootstraptest.html")
});
// app.get('/server_stream', function(req, res){  
// });


app.get('/api/analysis', function(req, res){
  fs.readFile(ANALYSIS_FILE, function(err, data) {
    if (err) {
      console.error(err);
      process.exit(1);
    }
    var jsonStr = JSON.parse(data);
    //console.log(jsonStr);
    res.json(jsonStr);
  })
});

app.get('/api/comments', function(req, res) {
  fs.readFile(COMMENTS_FILE, function(err, data) {
    if (err) {
      console.error(err);
      process.exit(1);
    }
    res.json(JSON.parse(data));
  });
});

app.post('/api/comments', function(req, res) {
  fs.readFile(COMMENTS_FILE, function(err, data) {
    if (err) {
      console.error(err);
      process.exit(1);
    }
    var comments = JSON.parse(data);
    // NOTE: In a real implementation, we would likely rely on a database or
    // some other approach (e.g. UUIDs) to ensure a globally unique id. We'll
    // treat Date.now() as unique-enough for our purposes.
    var newComment = {
      id: Date.now(),
      author: req.body.author,
      text: req.body.text,
    };
    comments.push(newComment);
    fs.writeFile(COMMENTS_FILE, JSON.stringify(comments, null, 4), function(err) {
      if (err) {
        console.error(err);
        process.exit(1);
      }
      res.json(comments);
    });
  });
});

var http = require('http').createServer(app);
var io = require('socket.io').listen(http);
io.on('connection', function(socket)
{
    console.log('a user connected');    
});

var amqp = require('amqplib/callback_api');
amqp.connect('amqp://localhost', function(err, conn)
{
    conn.createChannel(function(err, ch)
    {
        var q = 'snapshot.market';

        ch.assertQueue(q, {durable: false});
        console.log("waiting for message in %s", q);
        ch.consume(q, function(msg)
        {
            if( msg !== null)
            {                
                io.emit('market_quote', JSON.parse(msg.content));
                console.log(" received: %s", msg.content.toString());
                ch.ack(msg);
            }
        });
    });
});


// app.listen(app.get('port'), function() {
//   console.log('Server started: http://localhost:' + app.get('port') + '/');
// });

http.listen(app.get('port'), function() {
  console.log('Server started: http://localhost:' + app.get('port') + '/');
});
