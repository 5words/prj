import requests
from http import cookiejar
import time
import re

headers = {
    "x-xsrftoken":"4cb85f99-ae62-4c29-9b17-c2bda70e8085",
    "authorization":"oauth c3cef7c66a1843f8b3a9e6a1e3160e20",
    'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36'
}
Home_url = "https://www.zhihu.com/signup"
Base_login = "https://www.zhihu.com/api/v3/oauth/sign_in"
form_data = {
    "client_id":"c3cef7c66a1843f8b3a9e6a1e3160e20",
    "grant_type":"password",
    "timestamp":"",
    "source":"com.zhihu.web",
    "signature":"69db867d5b477d8c9e25cdbf27a268e4f44f593e",
    "username":"",
    "password":"",
    "captcha":"",
    "lang":"cn",
    "ref_source":"homepage"
}

class bHu(object):

    def __init__(self):
        self.home_url = Home_url
        self.base_login = Base_login
        self.session = requests.session()
        self.session.headers = headers.copy()
        self.form_data = form_data.copy()
        
        self.session = cookiejar.LWPCookieJar(filename="./cookie.txt")

    def Login(self,username=None,password=None,load_cookies=True):
        username = input("请输入手机号:")
        password = input("请输入密码:")
        if "+86" not in username:
            username = "+86" + username

        timestamp = str(int(time.time()*1000))
        
        self.form_data.update({"username":username,"password":password,"timestamp":timestamp})

        resp = requests.post(self.base_login,data = self.form_data,headers=headers)
        if 'error' in resp.text:
            print(re.findall(r'"message":"(.+?)"', resp.text)[0])
        elif self.check_login():
            return True
        print('登录失败')
        return False


    def check_login(self):
        resp = self.session.get(self.home_url, allow_redirects=False)
        if resp.status_code == 302:
            self.session.cookies.save()
            print('登录成功')
            return True
        return False




a = bHu()
a.Login(username=None,password=None,load_cookies = True)

        
        

