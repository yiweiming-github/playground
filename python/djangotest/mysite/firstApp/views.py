from django.http import HttpResponse
from django.shortcuts import render_to_response 
from markdown2 import Markdown
from django.views.decorators.csrf import csrf_exempt
from .models import Question
from django.template import loader
from django.http import Http404
from django.shortcuts import render, get_object_or_404

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
        
def qindex(request):
    latest_question_list = Question.objects.order_by('-pub_date')[:5]
    return render(request, 'qindex.html', {'latest_question_list': latest_question_list})

def detail(request, question_id):
    question = get_object_or_404(Question, pk=question_id)
    return render(request, 'detail.html', {'question': question})
    
def results(request, question_id):
    return HttpResponse("You're looking at the results of question %s." % question_id)
    
def vote(request, question_id):
    return HttpResponse("You're voting on question %s." % question_id)