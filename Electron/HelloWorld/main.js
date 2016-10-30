const {app, BrowserWindow} = require('electron')
const {ipcMain} = require('electron')

let win
let childWindows
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
    if (!childWindows) {
      childWindows = {}
    }
      if (!childWindows[arg]) {
        childWindows[arg] = new BrowserWindow({
          parent: win,
          width: 400, 
          height: 300,
        })
        childWindows[arg].loadURL(`file://${__dirname}/` + arg + '.html')
        childWindows[arg].on('closed', () => {
          childWindows[arg] = null
        })  
      }
      childWindows[arg].show()
  })

  ipcMain.on('change-ticker', (event, arg) => {
    currentTicker = arg
    if (childWindows) {
      for (var index in childWindows) {
        if (childWindows[index]) {
          childWindows[index].webContents.send('change-ticker', currentTicker)
        }
      }
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


