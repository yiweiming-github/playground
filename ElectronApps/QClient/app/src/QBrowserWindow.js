import {BrowserWindow} from "electron";

//BrowseWindow in Electron is a wrapper of native C++ class,
//so Electron team decided not to make it inheritable.
//Please see the issue here: https://github.com/electron/electron/issues/23.
//So, we have to wrap BrowseWindow instead of extend it.
export default class QBrowserWindow {
    constructor(name, setting, parent) {
        this.electronWindow = new BrowserWindow(setting);
        this.name = name;
        this.parentWindow = parent;

        this.electronWindow.on ('closed', () => {
            if (this.parentWindow) {
                this.parentWindow.closeChildWindow(this.name);
            }
        });
    }

    //Extended functions
    registerEventSubscription(topic) {
        if (this.parentWindow) {
            this.parentWindow.registerEventSubscription(topic, this.name);            
        }
    }

    removeEventSubscription(topic) {
        if (this.parentWindow) {
            this.parentWindow.removeEventSubscription(topic, this.name);
        }
    }

    sendEventToMain(topic, content) {
        ipcRenderer.send(topic, content);
    }

    //To wrap BrowseWindow, have to expose its members
    webContents() {
        return this.electronWindow.webContents;
    }

    //To wrap BrowseWindow, have to implement all of its methods
    on(name, callback) {
        this.electronWindow.on(name, callback);
    }
    
    destroy() {
        this.electronWindow.destroy();
    }

    close() {
        this.electronWindow.close();
    }

    focus() {
        this.electronWindow.focus();
    }

    blur() {
        this.electronWindow.blur();
    }

    isFocused() {
        return this.electronWindow.isFocused();
    }

    isDestroyed() {
        return this.electronWindow.isDestroyed();
    }

    show() {
        this.electronWindow.show();
    }

    showInactive() {
        this.electronWindow.showInactive();
    }

    hide() {
        this.electronWindow.hide();
    }

    isVisible() {
        return this.electronWindow.isVisible();
    }

    isModal() {
        return this.electronWindow.isModal();
    }

    maximize() {
        this.electronWindow.maximize();
    }

    unmaximize() {
        this.electronWindow.unmaximize();
    }

    minimize() {
        this.electronWindow.minimize();
    }

    restore() {
        this.electronWindow.restore();
    }

    isMinimized() {
        return this.electronWindow.isMinimized();
    }

    setFullScreen(flag) {
        this.electronWindow.setFullScreen(flag);
    }

    isFullScreen() {
        return this.electronWindow.isFullScreen();
    }

    getBounds() {
        return this.electronWindow.getBounds();
    }

    getContentBounds() {
        return this.electronWindow.getContentBounds();
    }

    getSize() {
        return this.electronWindow.getSize();
    }

    getContentSize() {
        return this.electronWindow.getContentSize();
    }

    setMinimumSize(width, height) {
        this.electronWindow.setMinimumSize(width, height);
    }

    getMinimumSize() {
        return this.electronWindow.getMinimumSize();
    }

    setMaximumSize(width, height) {
        this.electronWindow.setMaximumSize(width, height);
    }

    getMaxmumSize() {
        return this.electronWindow.getMaxmumSize();
    }

    setResizable(resizable) {
        this.electronWindow.setResizable(resizable);
    }

    isResizable() {
        return this.electronWindow.isResizable();
    }

    setMovable(movable) {
        this.electronWindow.setMovable(movable);
    }

    isMovable() {
        return this.electronWindow.isMovable();
    }


    //////////////////////////////////

    loadURL(url) {
        this.electronWindow.loadURL(url);
    }

    setMenu(menu) {
        this.electronWindow.setMenu(menu);
    }
}