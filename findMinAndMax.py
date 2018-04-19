def findMinAndMax(nums):
    if nums == []:
        return(None,None)
    else:
        ma = nums[0]
        mi = nums[0]
        for n in nums:
            if n> ma:
                ma = n
            if n< mi:
                mi = n 
        return(mi,ma)
# 测试
if findMinAndMax([]) != (None, None):
    print('测试失败!')
elif findMinAndMax([7]) != (7, 7):
    print('测试失败!')
elif findMinAndMax([7, 1]) != (1, 7):
    print('测试失败!')
elif findMinAndMax([7, 1, 3, 9, 5]) != (1, 9):
    print('测试失败!')
else:
    print('测试成功!')
    print('aaaaaa')