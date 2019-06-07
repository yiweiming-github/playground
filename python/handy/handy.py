import sys
import markdown
import codecs
from optparse import OptionParser

# A tool of http test
def tool_http_usage():
    print(
'''
python handy.py http -u {url} -x [POST|GET] [-d|-f] [data or json file name]")
samples:
python handy.py http -u http://localhost:8081/test -x POST -d '{"name": "John"}'
python handy.py http -u http://localhost:8081/test -x POST -f input.json
''')

def tool_http(args):
    tool_http_usage()

def tool_ping_usage():
    print(
'''
python handy.py ping {host}:[port]")
samples:
python handy.py ping 127.0.0.1
python handy.py ping 127.0.0.1:3396
''')

def tool_ping(args):
    tool_ping_usage()

def previewmd_usage():
    print(
'''
python handy.py previewmd -f {markdown file} -o {output file}
samples:
python handy.py previewmd -f test.md -o preview.html
''')

def tool_previewmd(args):
    parser = OptionParser()
    parser.add_option("-f", "--file", dest="filename", help="input markdown file name")
    parser.add_option("-o", "--output", dest="output_filename", help="output html file name")
    (options, args) = parser.parse_args(args)

    if not options.filename or not options.output_filename:
        previewmd_usage()        
    else:
        input_file = codecs.open(options.filename, mode="r", encoding="utf-8")
        text = input_file.read()
        html = markdown.markdown(text)
        output_file = codecs.open(options.output_filename, mode="w", encoding="utf-8",errors="xmlcharrefreplace")
        output_file.write(html)


function_dict = \
    {'http': tool_http, \
    'ping': tool_ping, \
    'previewmd': tool_previewmd}

def usage():
    print(
'''
handy tool usage
=================
python handy.py {command} [options]
commands:
http - send http request and see result
ping - send ping and telnet request to check connectivity
previewmd - generate preview from Markdown file
''')

def main(args):
    if args[0] in function_dict:
        function_dict[args[0]](args[1:])
    else:
        usage()


if __name__ == '__main__':
    main(sys.argv[1:])
