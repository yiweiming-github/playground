import { app, BrowserWindow, ipcMain, Menu } from "electron";
import path from "path";
import os from "os";
import fs from "fs";
import shortcut from "electron-localshortcut";
import QMainWindow from "./src/QMainWindow";
import QWindowStatus from "./src/QWindowStatus";
import { menuTemplate } from "./src/MenuTemplate";
require("electron-debug")();

const appPath = path.resolve(path.join(__dirname, "./"));
const publicFolder = path.join(appPath, "/public");

const isDevMode = !!process.execPath.match(/[\\\/]electron[\\\/]dist[\\\/]Electron\.app[\\\/]/);
if (isDevMode === true) {
	const reload = require("electron-reload"); //eslint-disable-line
	reload(appPath);
}

const windowConfigFile = path.join(appPath, "/src/StatusData.json");
let mainWindow;
let windowConfig = (new QWindowStatus(windowConfigFile)).data();

const initMenus = (template) => {
	Menu.setApplicationMenu(Menu.buildFromTemplate(template));
};

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

	mainWindow.registerMainEvent('main-close-all-child-windows', (event, arg) => {
		if (windowConfig) {
			for (var index in windowConfig) {
				mainWindow.closeChildWindow(windowConfig[index].name);
			}
		}
	});

	mainWindow.registerMainEvent('main-save-current-layout', (event, arg) => {
		windowConfig = mainWindow.saveChildWindowConfig(windowConfig);
		fs.writeFile(windowConfigFile, JSON.stringify(windowConfig));
	});

	initMenus(menuTemplate);
	//mainWindow.setMenu(null);
};

const openDefaultLayout = () => {
	if (windowConfig) {
		for (var index in windowConfig) {
			if (windowConfig[index].defaultOpen === true) {
				var childWindow = mainWindow.createChildWindow(windowConfig[index].name, {
					x: windowConfig[index].x,
					y: windowConfig[index].y,
					width: windowConfig[index].width,
					height: windowConfig[index].height,
					show: false,
					autoHideMemuBar: true,
					titleBarStyle: 'hidden'
				});
				childWindow.loadURL(`file://${publicFolder}/${windowConfig[index].page}`);
				for (var i in windowConfig[index].events) {
					childWindow.registerEventSubscription(windowConfig[index].events[i]);
				}

				childWindow.show();
			}
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
