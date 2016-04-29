import tornado.httpserver
import tornado.web

class IndexHandler(tornado.web.RequestHandler):
    def get(self):
        greeting = self.get_argument('greeting', 'hello')
        self.write(greeting + ', tornado!')
        
class ContentHandler(tornado.web.RequestHandler):
    def get(self):
        items = ['item1', 'item2', 'item3']
        self.render('templates/content.html', title='This is content page', items = items)
        
class CallbackStyleHandler(tornado.web.RequestHandler):
    @tornado.web.asynchronous
    def get(self):
        http = tornado.httpclient.AsyncHTTPClient()
        http.fetch("http://xxxx", callback = on_response)
        
    def on_response(self, response):
        if response.error:
            raise tornado.web.HTTPError(500)
        json = tornado.escape.join_decode(response.body)
        self.write(json)
        self.finish()
        
        
class CoroutineStyleHandler(tornado.web.RequestHandler):
    @tornado.gen.coroutine
    def get(self):
        http = tornado.httpclient.AsyncHTTPClient();
        response = yield http.fetch("http:/xxxx")
        json = tornado.escape.join_decode(response.body)
        self.write(json)