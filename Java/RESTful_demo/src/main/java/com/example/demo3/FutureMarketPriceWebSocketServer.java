package com.example.demo3;

import org.springframework.stereotype.Component;

import javax.websocket.OnClose;
import javax.websocket.OnOpen;
import javax.websocket.Session;
import javax.websocket.server.PathParam;
import javax.websocket.server.ServerEndpoint;
import java.io.IOException;
import java.util.concurrent.CopyOnWriteArraySet;

@ServerEndpoint("/websocket/{ticker}")
@Component
public class FutureMarketPriceWebSocketServer {
    private static int _clientCount = 0;
    private static CopyOnWriteArraySet<FutureMarketPriceWebSocketServer> _webSocketServers = new CopyOnWriteArraySet<FutureMarketPriceWebSocketServer>();
    private Session _session;
    private String _ticker="";

    @OnOpen
    public void OnOpen(Session session, @PathParam("ticker") String ticker) {
        this._session = session;
        _webSocketServers.add(this);
        AddOnlineClientCount();
        this._ticker = ticker;
        try {
            SendMessage("Connected.");
        } catch (IOException ex) {

        }
    }

    @OnClose
    public void OnClose() {
        _webSocketServers.remove(this);
        SubOnlineClientCount();
    }

    public void SendMessage(String message) throws IOException {
        this._session.getBasicRemote().sendText(message);
    }

    public static void Publish(String message) {
        for (FutureMarketPriceWebSocketServer socket : _webSocketServers) {
            try {
                socket.SendMessage(message);
            } catch (IOException ex) {
                continue;
            }
        }
    }

    public static synchronized void AddOnlineClientCount() {
        FutureMarketPriceWebSocketServer._clientCount++;
    }

    public static synchronized void SubOnlineClientCount() {
        FutureMarketPriceWebSocketServer._clientCount--;
    }
}
