import sys
name = sys.argv[1]
unit = sys.argv[2]
goods_url = sys.argv[3]
xpath0 = sys.argv[4]
import requests
from bs4 import BeautifulSoup
my_header = {'User-Agent':'Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0'}
res = requests.get(goods_url,headers=my_header)

if res.status_code != 200:
    print('网址打开错误,错误码为%s' %res.status_code)
    print('现在退出')
    sys.exit()
'''直接通过xpath来获取'''
from lxml import etree
tree=etree.HTML(res.text)
nodes = tree.xpath(xpath0)
price = nodes[0].text
if price== None:
    print('开始换用PhantomJS.exe来获取,速度可能较慢...')
    from selenium import webdriver
    browser = webdriver.PhantomJS(executable_path = './phantomjs')
    browser.get(goods_url)
    res = browser.find_element_by_xpath(xpath0)
    price=res.text
    browser.quit()
    if price== None:
        print('没找到')
    else:
        print("价格为:%s" %price)
else:
    print("价格为:%s" %price)

import time
date = time.strftime('%Y-%m-%d')

File = './商品价格/%s.csv' %name
File_dir = './商品价格/'
'''判断文件是否存在,不存在就新建;存在,就检查是否有最新日期'''
import os
if os.path.exists(File):
        f = open(File,'r')
        temp = f.readlines()[-1]
        newest_price = temp.split(',')[1]
        f.close()
        if newest_price != price: # 无最新价格,需添加
            f = open(File,'a')
            print('%s,%s,%s' %(date,price,unit),file=f)
            f.close()
            print('已写入,今日价格为%s%s' %(price,unit))
        else:
            print('最新价格已存在,无需写入.今日价格为%s%s' %(price,unit))
else:
    if not os.path.exists(File_dir):
        os.makedirs(File_dir)
    f = open(File,'w') # 当文件夹不存在时,它就无法执行
    print('日期,价格,单位',file=f)
    print('%s,%s,%s' %(date,price,unit),file=f)
    f.close()
    print('已写入')
