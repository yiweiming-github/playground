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