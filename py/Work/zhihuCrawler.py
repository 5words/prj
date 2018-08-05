import requests
import time
import re
import base64
from PIL import Image
import matplotlib.pyplot as plt
from http import cookiejar
import json
import hmac
import hashlib


HEADERS = {
    'Connection': 'keep-alive',
    'Host': 'www.zhihu.com',
    'Referer': 'https://www.zhihu.com/',
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
    "username":"",
    "password":"",
    "captcha":"",
    "lang":"en",
    "ref_source":"homepage"
}

class bHu(object):

    def __init__(self):
        self.home_url = Home_url
        self.base_login = Base_login
        self.session = requests.session()
        self.session.cookies = cookiejar.LWPCookieJar(filename="./cookie.txt")
        self.session.headers = HEADERS.copy()
        self.form_data = form_data.copy()
        
        

    def Login(self,username=None,password=None,load_cookies=True):
        if load_cookies and self.load_cookies():
            if self.check_login():
                return True


        headers = self.session.headers.copy()
        self.session.headers.update({
            "X-Xsrftoken":self.get_token()
        })
        username = input("请输入手机号:")
        password = input("请输入密码:")
        if "+86" not in username:
            username = "+86" + username

        timestamp = str(int(time.time()*1000))
        
        self.form_data.update({"username":username,"password":password,"timestamp":timestamp})
        self.form_data.update({
            "captcha":self.get_captcha(headers),
            "signature":self.get_signature(timestamp)
        })


        resp = self.session.post(self.base_login,data = self.form_data,headers=self.session.headers)
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

    def get_token(self):
        
        resp = self.session.get(self.home_url)
        token = re.findall(r'_xsrf=([\w|-]+)', resp.headers.get('Set-Cookie'))[0]
        return token
        
    def load_cookies(self):
        try:
            self.session.cookies.load(ignore_discard=True)
            return True
        except FileNotFoundError:
            return False
 

    def get_captcha(self,headers):
        lang = headers.get("lang","en")
        if lang == "cn":
            api = 'https://www.zhihu.com/api/v3/oauth/captcha?lang=cn'
        else:
            api = 'https://www.zhihu.com/api/v3/oauth/captcha?lang=en'
        resp = self.session.get(api, headers=headers)
        show_captcha = re.search(r'true', resp.text)
        if show_captcha:
            put_resp = self.session.put(api, headers=headers)
            img_base64 = re.findall(
                r'"img_base64":"(.+)"', put_resp.text, re.S)[0].replace(r'\n', '')
            with open('./captcha.jpg', 'wb') as f:
                f.write(base64.b64decode(img_base64))
            img = Image.open('./captcha.jpg')
            if lang == 'cn':
                plt.imshow(img)
                print('点击所有倒立的汉字，按回车提交')
                points = plt.ginput(7)
                capt = json.dumps({'img_size': [200, 44],'input_points': [[i[0]/2, i[1]/2] for i in points]})
            else:
                img.show()
                capt = input('请输入图片里的验证码：')
            # 这里必须先把参数 POST 验证码接口
            self.session.post(api, data={'input_text': capt}, headers=self.session.headers)
            return capt
        return ''

    def get_signature(self,timestamp):
        ha = hmac.new(b'd1b964811afb40118a12068ff74a12f4', digestmod=hashlib.sha1)
        grant_type = self.form_data['grant_type']
        client_id = self.form_data['client_id']
        source = self.form_data['source']
        ha.update(bytes((grant_type + client_id + source + timestamp), 'utf-8'))
        return ha.hexdigest()





a = bHu()
a.Login(username=None,password=None,load_cookies = True)

        
        

