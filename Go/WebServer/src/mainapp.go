package main

import (
    "net/http"
    "github.com/emicklei/go-restful"
)

type User struct {
    Name string
    Age int
}

func SampleService(request *restful.Request, response *restful.Response) {
    user := new(User)
    user.Name = "John"
    user.Age = 35
    response.WriteHeaderAndEntity(http.StatusCreated, user)
}

func main() {
    http.Handle("/", http.FileServer(http.Dir("./static/")))

    wsContainer := restful.NewContainer()
    ws := new(restful.WebService)
    ws.Path("/services/sample_service").Doc("Sample Service").Consumes(restful.MIME_JSON).Produces(restful.MIME_JSON) // you can specify this per route as well
    ws.Route(ws.GET("").To(SampleService).Doc("Sample Service").Operation("SampleService"))//.Reads(User{})) // from the request
    wsContainer.Add(ws)

    http.ListenAndServe(":8999", wsContainer)
}