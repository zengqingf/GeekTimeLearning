# -*- coding: utf-8 -*-

class A:
    def prt(self):
        print "A"

class B:
    def prt(self):
        print "B"

class C(A):
    def prt(self):
        print "C"

class D(A):
    pass

class E:
    def prt(self):
        print "E"

class F:
    pass

def test(arg):
    arg.prt()

if __name__ == "__main__":
    a = A()
    b = B()
    c = C()
    d = D()
    e = E()
    f = F()

    test(a)
    test(b)
    test(c)
    test(d)
    test(e)
    test(f)