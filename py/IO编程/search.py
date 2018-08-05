import os

def findFile(path="C:\\ ",nameStr=""):
    for x in os.listdir(path):             #遍历path目录
        pathX = os.path.join(path, x)      #pathX为通过join组合的目录+文件名
        if os.path.isdir(pathX):           #判断pathX是否为目录
            print(pathX)                   #输出目录名
            findFile(pathX,nameStr)        #再次执行函数以循环多层目录
        elif nameStr in os.path.splitext(x)[0]: #关键字与切割后的文件名对比，[0]位文件名，[1]位文件类型
            a = os.path.splitext(x)[0]
            b = os.path.splitext(x)[1]
            print('file in...'+path+"\\"+a+b)

findFile('D:\\asd','taobao')