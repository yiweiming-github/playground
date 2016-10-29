const {app, BrowserWindow} = require('electron')
const {ipcMain} = require('electron')
// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let win
let child1
let child2
let currentTicker

function createWindow () {
  currentTicker = 'Empty Ticker'
  // Create the browser window.
  win = new BrowserWindow({
    width: 800, 
    height: 600,
    autoHideMemuBar: true,
    titleBarStyle: 'hidden'
  })

  win.loadURL(`file://${__dirname}/index.html`)
  win.show()  

  // Open the DevTools.
  //win.webContents.openDevTools()

  // Emitted when the window is closed.
  win.on('closed', () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    win = null
  })  
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', () => {
  createWindow()

  ipcMain.on('open-window', (event, arg) => {
    if (arg === 'child1') {
      if (!child1) {
        child1 = new BrowserWindow({
          parent: win,
          width: 400, 
          height: 300,
        })
        child1.loadURL(`file://${__dirname}/child1.html`)
        child1.on('closed', () => {
          child1 = null
        })  
      }
      child1.show()
    }
    else if(arg === 'child2') {
      if (!child2) {
        child2 = new BrowserWindow({
          parent: win,    
          width: 400, 
          height: 300,    
        })
        child2.loadURL(`file://${__dirname}/child2.html`)
        child2.on('closed', () => {
          child2 = null
        })
      }
      child2.show()
    }
  })

  ipcMain.on('change-ticker', (event, arg) => {
    currentTicker = arg
    if (child2) {
      child2.webContents.send('change-ticker', currentTicker)
    }    
  })

  ipcMain.on('query-ticker', (event, arg) => {
    console.log('main got query-ticker:' + currentTicker)
    event.sender.send('change-ticker', currentTicker)
  })
})

// Quit when all windows are closed.
app.on('window-all-closed', () => {
  // On macOS it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  // On macOS it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (win === null) {
    createWindow()
  }
})

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and require them here.


