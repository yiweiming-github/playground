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
        while (_shouldRun) {
            message = String.valueOf(Math.round(4000.0 + Math.random()*20.0));
            FutureMarketPriceWebSocketServer.Publish(message);
            System.out.println(message);
            try {
                Thread.sleep(250);
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
