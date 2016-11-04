import { app, BrowserWindow, ipcMain, Menu } from "electron";
import path from "path";
import os from "os";
import shortcut from "electron-localshortcut";
import windowState from "./src/windowState";
import AppConfig from "./src/appConfig";
import ConfigStorage from "./src/configStorage";
import QMainWindow from "./src/QMainWindow";
require( "electron-debug" )();

const appPath = path.resolve( path.join( __dirname, "./" ) );
const publicFolder = path.join( appPath, "/public" );

const isDevMode = !!process.execPath.match( /[\\\/]electron[\\\/]dist[\\\/]Electron\.app[\\\/]/ );
if ( isDevMode === true ) {
	const reload = require( "electron-reload" ); //eslint-disable-line
	reload( appPath );
}

const store = new ConfigStorage( null, true, app.getPath( "userData" ) );
const appConfig = new AppConfig( store );

let mainWindow;

const createWindow = () => {
	mainWindow = new QMainWindow({
		width: 800, 
    	height: 600,
    	autoHideMemuBar: true,
    	titleBarStyle: 'hidden'
	});	

	mainWindow.loadURL( `file://${ appPath }` + '/index.html' );	

	mainWindow.on( "closed", () => {
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
	var childWindow1 = mainWindow.createChildWindow('child1', {
		width: 400, 
    	height: 300,
    	autoHideMemuBar: true,
    	titleBarStyle: 'hidden'
	});

	childWindow1.loadURL(`file://${publicFolder}` + '/child1.html');	
	childWindow1.show();

	var childWindow2 = mainWindow.createChildWindow('child2', {
		width: 400, 
    	height: 300,
    	autoHideMemuBar: true,
    	titleBarStyle: 'hidden'
	});

	childWindow2.loadURL(`file://${publicFolder}` + '/child2.html');
	childWindow2.registerEventSubscription('publish-ticker-change');
	childWindow2.show();
};

app.on( "ready", createWindow );

app.on( "window-all-closed", () => {
	app.quit();
} );

app.on( "activate", () => {
	if ( mainWindow === null ) {
		createWindow();
	}
} );

ipcMain.on( "get-app-state", ( event, arg ) => {
	event.returnValue = appConfig.config;
} );

ipcMain.on( "set-app-state", ( event, arg ) => {
	appConfig.config = arg;
	event.returnValue = true;
} );
