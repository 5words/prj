import re
from datetime import datetime, timezone, timedelta

def to_timestamp(dt_str,tz_str):
    time = datetime.strptime(dt_str,"%Y-%m-%d %H:%M:%S")
    a = re.match(r"UTC(.+)\:\d+",tz_str).group(1)
    b = int(a)
    time=time.replace(tzinfo= timezone(timedelta(hours=b)))
    time=time.timestamp()
    return time

'''
def to_timestamp(dt_str, tz_str):
    tz_str = re.match('UTC(.\d+).', tz_str)
    return datetime.strptime(dt_str, '%Y-%m-%d %H:%M:%S').replace(tzinfo=timezone(timedelta(hours=int(tz_str.group(1))))).timestamp()
'''

# 测试:


t2 = to_timestamp('2015-5-31 16:10:30', 'UTC-09:00')
assert t2 == 1433121030.0, t2

t1 = to_timestamp('2015-6-1 08:10:30', 'UTC+7:00')
assert t1 == 1433121030.0, t1

print('ok')