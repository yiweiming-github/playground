package com.example.demo3;

import org.springframework.beans.factory.DisposableBean;
import org.springframework.stereotype.Component;

@Component
public class MarketDataThread implements DisposableBean, Runnable {
    private Thread _thread;
    private volatile boolean _shouldRun = true;

    MarketDataThread() {
        this._thread = new Thread(this);
        this._thread.start();
    }

    @Override
    public void run() {
        String message = "";
        StringBuilder strBuilder = new StringBuilder();
        while (_shouldRun) {
            long price = Math.round(4000.0 + Math.random()*20.0);
            int trend = Math.random() > 0.5 ? 1 : -1;
            strBuilder.delete(0, strBuilder.length());
            strBuilder.append("[{\"ticker\": \"RB1901\", \"price\": ");
            strBuilder.append(price);
            strBuilder.append(", \"trend\" :");
            strBuilder.append(trend);
            strBuilder.append("}, {\"ticker\": \"RB1905\", \"price\": ");
            strBuilder.append(Math.round(price + Math.random()*5.0));
            strBuilder.append(", \"trend\" :");
            strBuilder.append(-trend);
            strBuilder.append("}]");
            message = strBuilder.toString();
            FutureMarketPriceWebSocketServer.Publish(message);
            System.out.println(message);
            try {
                Thread.sleep(500);
            } catch (InterruptedException ex) {
                continue;
            }
        }
    }

    @Override
    public void destroy() {
        _shouldRun = false;
    }
}
