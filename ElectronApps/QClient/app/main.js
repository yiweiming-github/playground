import { app, BrowserWindow, ipcMain, Menu } from "electron";
import path from "path";
import os from "os";
import shortcut from "electron-localshortcut";
import QMainWindow from "./src/QMainWindow";
import QWindowStatus from "./src/QWindowStatus";
require("electron-debug")();

const appPath = path.resolve(path.join(__dirname, "./"));
const publicFolder = path.join(appPath, "/public");

const isDevMode = !!process.execPath.match(/[\\\/]electron[\\\/]dist[\\\/]Electron\.app[\\\/]/);
if (isDevMode === true) {
	const reload = require("electron-reload"); //eslint-disable-line
	reload(appPath);
}

let mainWindow;

const createWindow = () => {
	mainWindow = new QMainWindow({
		width: 800,
		height: 600,
		autoHideMemuBar: true,
		titleBarStyle: 'hidden'
	});

	mainWindow.loadURL(`file://${appPath}` + '/index.html');

	mainWindow.on("closed", () => {
		mainWindow = null;
	});
	mainWindow.on("ready-to-show", () => {
		mainWindow.show();
	});

	mainWindow.registerMainEvent('main-ticker-change', (event, arg) => {
		mainWindow.publishEvents('publish-ticker-change', arg);
	});

	mainWindow.registerMainEvent('main-open-default-layout', (event, arg) => {
		openDefaultLayout();
	});
};

const openDefaultLayout = () => {

	var status = new QWindowStatus(path.join(appPath, "/src/StatusData.json"));

	if (status.data()) {
		for (var windowConfig in status.data()) {
			var childWindow = mainWindow.createChildWindow(status.data()[windowConfig].name, {
				x: status.data()[windowConfig].x,
				y: status.data()[windowConfig].y,
				width : status.data()[windowConfig].width,
				height : status.data()[windowConfig].height,
				show : false,
				autoHideMemuBar : true,
				titleBarStyle : 'hidden'
			});
			childWindow.loadURL(`file://${publicFolder}/${status.data()[windowConfig].page}`);
			for (var event in status.data()[windowConfig].events) {
				childWindow.registerEventSubscription(status.data()[windowConfig].events[event]);
			}
			
			childWindow.show();
		}
	}
};

app.on("ready", createWindow);

app.on("window-all-closed", () => {
	app.quit();
});

app.on("activate", () => {
	if (mainWindow === null) {
		createWindow();
	}
});
