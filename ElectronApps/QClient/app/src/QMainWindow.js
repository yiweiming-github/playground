import QBrowserWindow from './QBrowserWindow';
const {ipcMain} = require('electron');

export default class QMainWindow extends QBrowserWindow {
    constructor(setting) {
        super(setting);
        this.childWindows = [];
        this.eventRegistration = [];
    }

    createChildWindow(name, setting) {
        if (this.childWindows[name]) {
            this.closeChildWindow(name);
        }
        var win = new QBrowserWindow(name, setting, this);
        this.childWindows[name] = win;
        this.removeAllEventSubscriptionForWindow(name);        
        return win;
    }

    closeChildWindow(name) {
        if (this.childWindows[name]) {
            if (!this.childWindows[name].isDestroyed()) {
                this.childWindows[name].close();
            }            
            this.childWindows.splice(name, 1);
        }
    }

    registerMainEvent(topic, handler) {
        ipcMain.on(topic, handler);        
    }

    registerEventSubscription(topic, name) {
        if (this.childWindows[name]) {
            if (!this.eventRegistration[topic]) {
                this.eventRegistration[topic] = [];
            }            
            this.eventRegistration[topic].push(name);            
        }
    }

    removeEventSubscription(topic, name) {
        if (this.eventRegistration[topic] &&
            this.eventRegistration[topic][name]) {
            this.eventRegistration[topic].remove(name);
        }
    }

    removeAllEventSubscriptionForWindow(name) {
        for (var topic in this.eventRegistration) {
            this.removeEventSubscription(topic, name);
        }
    }

    publishEvents(topic, content) {
        if (this.eventRegistration[topic]) {            
            for (var name in this.eventRegistration[topic]) {                      
                this.childWindows[this.eventRegistration[topic][name]].webContents().send(topic, content);
            }
        }
    }

    saveChildWindowConfig(windowConfig) {
        for (var i in windowConfig) {
             if (this.childWindows[windowConfig[i].name]) {
                var pos = this.childWindows[windowConfig[i].name].getPosition();
                var size = this.childWindows[windowConfig[i].name].getSize();
                windowConfig[i].x = pos[0];
                windowConfig[i].y = pos[1];
                windowConfig[i].width = size[0];
                windowConfig[i].height = size[1];
            }
        }
        
        return windowConfig;
    }
}