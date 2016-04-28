import tornado.httpserver
import tornado.ioloop
import tornado.options
import tornado.web
from tornado.options import define, options

from Handlers import *

define('port', default=8000, help='run on the given port', type=int)

global_handlers=[
    (r'/', IndexHandler),
    (r'/content/', ContentHandler),
]
        
if __name__ == '__main__':
    tornado.options.parse_command_line()
    app = tornado.web.Application(global_handlers)
    http_server = tornado.httpserver.HTTPServer(app)
    http_server.listen(options.port)
    tornado.ioloop.IOLoop.instance().start()