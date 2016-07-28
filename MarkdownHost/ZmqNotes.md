#ZMQ Notes

### Some random points

+ `socket` in ZMQ is not a real socket.
+ ZMQ message has its own frame, which is not compatible with other protocol, such as HTTP.
+ ZMQ uses a background thread to do async I/O. `send()` only puts message in a queue, and the background thread will send it out.
  - each context has one I/O thread, and it can be set to more.
+ ZMQ supports four __core__ patterns
  - Request-Reply
  - Pub-Sub
  - Pipeline (Push/Pull)
  - Exclusve pair. It's for two threads in one process.
+ for multipart message, all parts or none part will be delivered. 


### Proxy and dynamic discovery
XPUB/XSUB

### Shared queue
+ it uses ROUTER/DEALER to implement a broker between multiple clients and servers, and the broker uses POLLIN.
+ ZMQ provides a shortcut to implement this (python code): `zmq.proxy(frontend, backend)`

Here is a sample C# code:

```java
public static int TimestampToInt(string timestamp)
{
    int result = 0;
    char[] del = new[] { ':' };
    int index = timestamp.IndexOf(" ");

    if (!string.IsNullOrEmpty(timestamp))
    {
        string[] parts = timestamp.Substring(index + 1).Split(del);
        if (parts != null && parts.Length == 3)
        {
             result = Int32.Parse(parts[0])*100 + Int32.Parse(parts[1]);
        }
    }

    return result;
}
```