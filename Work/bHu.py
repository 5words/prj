import requests


headers = {
    "x-xsrftoken":"4cb85f99-ae62-4c29-9b17-c2bda70e8085",
    "authorization":"oauth c3cef7c66a1843f8b3a9e6a1e3160e20",
    'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36'
}
home_url = "https://www.zhihu.com/signup"
base_login = "https://www.zhihu.com/api/v3/oauth/sign_in"
form_data = {
    "client_id":"c3cef7c66a1843f8b3a9e6a1e3160e20",
    "grant_type":"password",
    "timestamp":"",
    "source":"com.zhihu.web",
    "signature":"69db867d5b477d8c9e25cdbf27a268e4f44f593e",
    "username":"+8615902110521",
    "password":"365871221",
    "captcha":"",
    "lang":"cn",
    "ref_source":"homepage"
}
