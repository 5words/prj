import re


def is_valid_email(test):
    if re.match(r"(\w+|\w+\.\w+)@(\w+\.\w+$)",test):   #.在正则表达式中表示任意字符，故需要转义
        return True
    else:
        return False


# 测试:
assert is_valid_email('someone@gmail.com')
assert is_valid_email('bill.gates@microsoft.com')
assert not is_valid_email('bob#example.com')
assert not is_valid_email('mr-bob@example.com')
print('ok')
