# 将MHF早期任务批量转换为MH2Dos任务的工具 #

本工具作者为皓月，axibug.com

关联 皓月PS2 联机服务器，和 皓月MHF 怪物猎人边境服务器。


### 大致描述 ###

通过一系列，数据处理，指针处理。

将怪物猎人边境早期的服务器任务，转换为PS2 怪物猎人2Dos 本地自制任务或2Dos联机服务器可用任务。

本工具的初衷是，还原2Dos的一些官方活动任务


YouTuBe：

<a href="https://www.youtube.com/watch?v=rTiA-OwAVTs" title=""><img width = "672" height = "378"  src="https://res.cloudinary.com/marcomontalbano/image/upload/v1680856711/video_to_markdown/images/youtube--rTiA-OwAVTs-c05b58ac6eb4c4700831b2b3070cd403.jpg" alt="" /></a>

Bilibili:

<a href="https://www.bilibili.com/video/BV1dc411L7eZ/" title=""><img width = "672" height = "378"  src="http://i1.hdslb.com/bfs/archive/2e49f2ff60765db2d2948bff37c9bc2926e6600a.jpg" alt="" /></a>

### 使用前提 ###

PS：本工具先需要将ReFrontier将MHF任务解密；

尽可能只用MHF早期任务，MHF后期任务内容早已和2Dos完全不一样，转换将没有意义。

在工具统计目录下：

创建Input文件夹，放置用于转换的MHF任务文件（放入已解密的MHF任务文件）

（Input 文件夹内仅处理包含.mib 、.bin 的文件）

创建Out文件夹，用于输出文件

（输出文件会在末尾加"_fix"）

### 一些特殊处理 ###

若发现属于仅MHF有道具，如报酬道具，（比如浮岳龙，报酬的神龙苔）将替换为【不可燃烧的废物】。

	包括：报酬道具，支给道具（主线支给，支线1支给，支线2支给），采集点所有道具。

若发现属于仅MHF的鱼，则替换为【刺身鱼】
	
	包括：所有钓鱼点的数据

若任务星级超过2Dos的最大8，则修正为8（好像是8）

若怪物ID、地图ID等超出最大范围，会提示。并不建议使用。

任务ID，我暂时都改为600020(0xEA74)，使其可以走活动任务下载逻辑，否则ID会过小，会读取镜像内本地任务文件。

各种地图传送坐标信息，替换为2Dos同季节同昼夜的坐标数据。


### 目前已知不完善的地方 ###

(已解决)~~1. 任务星级我没理解到MHF的星级，MHF的大部分都是三位数的值，我是直接改成8的（2dos最大值）~~

2. 固体值 2dos最大是0x0A，超过这个的 我都给的Ah，但是这样怪物会比2dos略强，需要更合理

3. 文本虽然塞进去，但是实在腾不出空间了，比原始文本短几个双字节字符。我截取了

(已解决)~~4. 火山等个别地图，区坐标有偏移，但只影响小地图显示（显示坐标是偏的）~~

(已解决)~~5. 支给道具，我还没判断，因为貌似不像报酬道具 有结束符~~

