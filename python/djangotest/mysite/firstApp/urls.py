from django.conf.urls import url
from . import views

app_name = 'firstApp'
urlpatterns = [    
    url(r'^$', views.index, name='index'),
    url(r'^convert', views.convert, name='convert'),
    
    url(r'^q$', views.qindex, name='qindex'),
    url(r'^q/(?P<question_id>[0-9]+)/$', views.detail, name='detail'),
    url(r'^q/(?P<question_id>[0-9]+)/results/$', views.results, name='results'),
    url(r'^q/(?P<question_id>[0-9]+)/vote/$', views.vote, name='vote'),
    url(r'^q/(?P<question_id>[0-9]+)/detail/$', views.detail, name='detail')
]