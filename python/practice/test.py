class Base1(object):
    def __init__(self, name, age):
        self.name = "Base1"
        self.age = 35
        
    def print_name(self):
        print self.name
        
class Base2(object):
    def __init__(self, name):
        self.name = "Base2"
        
    def print_name(self):
        print self.name
        
class Derived(Base2, Base1):
    def __init__(self, name):
        super(Derived, self).__init__(name)
        self.name = "Derived"
        
    # def print_name(self):
    #     print "Derived"        
        
sample_object = Derived("sample_object")
sample_object.print_name()