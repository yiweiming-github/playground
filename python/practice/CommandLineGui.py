import os
import time
import sys
import ctypes

STD_INPUT_HANDLE = -10  
STD_OUTPUT_HANDLE= -11  
STD_ERROR_HANDLE = -12  
  
FOREGROUND_BLACK = 0x0  
FOREGROUND_BLUE = 0x01 # text color contains blue.  
FOREGROUND_GREEN= 0x02 # text color contains green.  
FOREGROUND_RED = 0x04 # text color contains red.  
FOREGROUND_INTENSITY = 0x08 # text color is intensified.  
  
BACKGROUND_BLUE = 0x10 # background color contains blue.  
BACKGROUND_GREEN= 0x20 # background color contains green.  
BACKGROUND_RED = 0x40 # background color contains red.  
BACKGROUND_INTENSITY = 0x80 # background color is intensified.

class OutputHelper(object):  
    ''''' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winprog/winprog/windows_api_reference.asp 
    for information on Windows APIs.'''  
    std_out_handle = ctypes.windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)  
      
    def set_cmd_color(self, color, handle=std_out_handle):  
        """(color) -> bit 
        Example: set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY) 
        """  
        bool = ctypes.windll.kernel32.SetConsoleTextAttribute(handle, color)  
        return bool  
      
    def reset_color(self):  
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE)  
      
    def print_red_text(self, print_text):  
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY)  
        print print_text  
        self.reset_color()  
          
    def print_green_text(self, print_text):  
        self.set_cmd_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY)  
        print print_text  
        self.reset_color()  
      
    def print_blue_text(self, print_text):   
        self.set_cmd_color(FOREGROUND_BLUE | FOREGROUND_INTENSITY)  
        print print_text  
        self.reset_color()  
            
    def print_red_text_with_blue_bg(self, print_text):  
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY| BACKGROUND_BLUE | BACKGROUND_INTENSITY)  
        print print_text  
        self.reset_color() 
        
    def display(self, s):
        os.write(1, s)
        sys.stdout.flush()

class ProgressBar(object):
    def __init__(self, max):
        self.max = max
        self.current = 0
        self.output_helper = OutputHelper()
        
    def progress(self, step):
        if step > 0:
            self.current += step
            if self.finished():
                self.finishBar()
            else:
                percent = float(self.current) / float(self.max)                
                self.output_helper.display("\r[%s] |%s" % (format(percent, ".0%"), "="*int(percent*50)))                
    
    def finished(self):
        return self.current >= self.max
    
    def finishBar(self):
        self.output_helper.display("\r[%s] |%s|" % (format(1, ".0%"), "="*50))        
        self.output_helper.print_green_text("Finished.")

class Menu(object):
    def __init__(self):
        self.items = {}
        self.output_helper = OutputHelper()
        
    def load_from_json(self, str):
        self.items = {}
    
    def add_item(self, key, menu_item):
        self.items[key] = menu_item
        
    def show(self):
        menu_str = ""
        for (key, item) in self.items.items():
            menu_str += "\r[%s] %s" % (key, item.text)        
        self.output_helper.display(menu_str)
            
class MenuItem(object):
    def __init__(self, text):
        self.text = text
