from django.http import HttpResponse
from django.shortcuts import render_to_response 
from markdown2 import Markdown
from django.views.decorators.csrf import csrf_exempt

@csrf_exempt
def index(request):    
    return render_to_response('index1.html')

@csrf_exempt
def convert(request):
    if request.body.strip() :
        markdownConverter = Markdown()
        print("md: ", request.body)
        html =  markdownConverter.convert(request.body)
        print("html: ", html)
        response = HttpResponse(html)
        response['Access-Control-Allow-Origin'] = '*'
        return response   
    else:
        return HttpResponse("Empty request.")