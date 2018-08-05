L1 = ['Hello', 'World', 18, 'Apple', None]
'''
def low(L):
    LA=[]
    for n in L:
        if isinstance(n,str)==False:
            pass
        else:
            LA.append(n)
    return(LA)
l1 = low(L1)
L2 = [s.lower() for s in l1]
'''

L2 = [s.lower() for s in L1 if isinstance(s, str)]
print(L2)
if L2 == ['hello', 'world', 'apple']:
    print('测试通过!')
else:
    print('测试失败!')

