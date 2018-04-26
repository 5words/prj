import time
import functools

def metric(fn):
    def wapper(*args,**kw):
        t_start = time.time()
        r = fn(*args,**kw) #这一步调用原函数
        t_end = time.time()
        print("call %s in %f s" , fn.__name__,(t_end-t_start))
        return r  #wapper的返回值就是原函数的返回值
    return wapper

# 测试
@metric
def fast(x, y):
    time.sleep(0.0012)
    return x + y

@metric
def slow(x, y, z):
    time.sleep(0.1234)
    return x * y * z

f = fast(11, 22)
s = slow(11, 22, 33)
if f != 33:
    print('测试失败!')
elif s != 7986:
    print('测试失败!')