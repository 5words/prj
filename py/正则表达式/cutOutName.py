import re

def name_of_email(addr):
    n = re.match(r"^<([\w\s]+)>\s.+@\w+\.\w+$",addr)
    m = re.match(r"(.+)@\w+\.\w+$",addr)

    if n :
        return n.group(1)
    elif m:
        return m.group(1)
    
# 测试:
assert name_of_email('<Tom Paris> tom@voyager.org') == 'Tom Paris'
assert name_of_email('tom@voyager.org') == 'tom'
print('ok')