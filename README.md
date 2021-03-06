# price_collect

### 1.介绍

界面是这样的，只要输入物品的链接和它的xpath，点击`开始获取` 即可。采集信息保存在`price_collect\bin\x64\Debug\商品价格`目录中的csv文件中， 文件名为 `商品名`

![1515499071282](a/1515499071282.gif)

xpath是什么？它是用来定位数据的位置的,**不懂没关系! 能找到它就行了** 

使用火狐浏览器，打开一个商品链接：https://item.jd.com/2330112.html

![sp180109_202158](a/sp180109_202158.gif)

安装插件`Distil Web monitor` ,点击它的图标,然后点`选择页面的某些部分` ,然后用鼠标点击`价格所在的位置` 

![sp180109_202358](a/sp180109_202358.gif)

![sp180109_202552](a/sp180109_202552.gif)

**本程序的界面针对价格获取量身定做，是受股票的启发，实际上里可以用来获取任何数据，GDP、销售量、评分；只需要将界面上的文字修改一下，即可**

![sp180110_010902](a/sp180110_010902.gif) 

### 2.编译、运行

用visual studio打开工程文件	`price_collect.sln`   ,编译运行.

也可以通过下面链接下载执行程序,百度云：https://pan.baidu.com/s/1bq9W8rl 密码：ee25

**(1)**本工程的vs版本是2015,低版本出现错误时,用记事本打开`price_collect.sln` 

> ```
> 修改参数：
> Microsoft Visual Studio Solution File, Format Version 12.00
> # Visual Studio 2012
> 改为：
> Microsoft Visual Studio Solution File, Format Version 11.00
> # Visual Studio 2010
> 保存退出。
> ```

如果还是不行，请参考:https://zhidao.baidu.com/question/1386103078558771340.html  。

**(2)**本工程使用x64的Debug和Release都不会有问题,用于其他解决方案时,会出现`系统找不到文件` ,将`bin\x64\Debug\` 目录里的文件都复制过去就好了

比如,32位电脑X86的Debug解决方案,就将这些文件复制到`bin\x86\Debug\` 目录下面

### 3.文件介绍

`/Debug/价格采集.exe` 、`/Release/价格采集.exe`python写的爬虫

`价格采集.py` ：`价格采集.exe`的 源代码，编译方法见[编译py文件并复制到对应位置.bat](https://github.com/qqblack/price_collect/blob/master/%E7%BC%96%E8%AF%91py%E6%96%87%E4%BB%B6%E5%B9%B6%E5%A4%8D%E5%88%B6%E5%88%B0%E5%AF%B9%E5%BA%94%E4%BD%8D%E7%BD%AE.bat) 



### 说明

**爬虫功能全部由python完成，C#只是用来做界面。本工程爬虫动态加载上来的数据，有些问题，是C#调用的问题；建议这时候用命令行来调用** 

用C#调用它，爬取普通数据没问题，通过它调用`phantomjs.exe` 就会失败，错误是“句柄无效”

![1515307779763](a/1515307779763.png)

`价格采集.exe` 没有问题，**用windows命令行去调用它时，不会出错，可以爬取任何数据** 

希望知情者提出修改意见

**如果你想要一个可以直接运行的工程**，见我的另外一个工程（利用C#完成`phantomjs.exe`的调用），https://github.com/qqblack/price_collect

### 将来的版本

1. 添加，一键添加`定时任务` ,定期爬取数据
2. 添加，一键生成`走势图`,掌握行情变化，预测未来趋势
3. 有了数据库之后，提供给用户搜索，让商品价格就像股票价格一样清晰

### 欢迎提出意见

- 1.欢迎上传改进代码、更多商品xpath信息
- 2.反应评论、留言
- 2.我的联系方式:1137146278@qq.com , 或关注`坛子分享` 微信公众号,这个我回复得更及时一点