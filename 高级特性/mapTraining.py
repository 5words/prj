def normalize(name):
    a = name[1:].lower()
    b = name[0]
    b = b.upper()

    return b+a


L1 = ['adam', 'LISA', 'barT']
L2 = list(map(normalize, L1))
print(L2)