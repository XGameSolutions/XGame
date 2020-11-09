# XGame

XGame旨在提供一套完整的游戏解决方案，包括前后端、底层驱动、配置数据等。采用`C++`、`C#`、`Lua`等语言开发。

XGame由以下多个仓库组成。拆分成多仓库有许多好处，更多的是为了方便项目管理和权限划分，以及提高开发效率。不同的开发角色可以迁出不同的仓库组合所要的开发方案。

## 方案组成

| 仓库 | 描述 |
| ------------------------------------------------------------------------------------------------- | -- |
| [XGame-XClientLua](https://github.com/monitor1394/XGame-XClientLua.git)                           | XGame游戏解决方案：客户端Lua代码 |
| [XGame-XConfig](https://github.com/monitor1394/XGame-XConfig.git)                                 | XGame游戏解决方案：配置数据 |
| [XGame-XPublicLua](https://github.com/monitor1394/XGame-XPublicLua.git)                           | XGame游戏解决方案：前后端共用的Lua代码 |
| [XGame-XDriver](https://github.com/monitor1394/XGame-XDriver.git)                                 | XGame游戏解决方案：底层C++驱动代码 |
| [XGame-XServer](https://github.com/monitor1394/XGame-XServer.git)                                 | XGame游戏解决方案：服务端Lua代码 |
| [XGame-XClient-Packages](https://github.com/monitor1394/XGame-XClient-Packages.git)               | XGame游戏解决方案：客户端Unity的Packages目录 |
| [XGame-XClient-ProjectSettings](https://github.com/monitor1394/XGame-XClient-ProjectSettings.git) | XGame游戏解决方案：客户端Unity的Packages目录 |
| [XGame-XClient-ResAB](https://github.com/monitor1394/XGame-XClient-ResAB.git)                     | XGame游戏解决方案：客户端Unity的ResAB目录 |
| [XGame-XClient-AssetsEditor](https://github.com/monitor1394/XGame-XClient-AssetsEditor.git)       | XGame游戏解决方案：客户端Unity的Assets目录下的Editor |
| [XGame-XClient-AssetsPlugins](https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git)     | XGame游戏解决方案：客户端Unity的Assets目录下的Plugins |
| [XGame-XClient-AssetsResPublic](https://github.com/monitor1394/XGame-XClient-AssetsResPublic.git) | XGame游戏解决方案：客户端Unity的Assets目录下的ResPublic |
| [XGame-XClient-AssetsResModel](https://github.com/monitor1394/XGame-XClient-AssetsResModel.git)   | XGame游戏解决方案：客户端Unity的Assets目录下的ResModel |
| [XGame-XClient-AssetsResScene](https://github.com/monitor1394/XGame-XClient-AssetsResScene.git)   | XGame游戏解决方案：客户端Unity的Assets目录下的ResScene |
| [XGame-XClient-AssetsResUI](https://github.com/monitor1394/XGame-XClient-AssetsResUI.git)         | XGame游戏解决方案：客户端Unity的Assets目录下的ResUI |
| [XGame-XClient-AssetsSrc](https://github.com/monitor1394/XGame-XClient-AssetsSrc.git)             | XGame游戏解决方案：客户端Unity的Assets目录下的Src |

## 迁出参考

|             仓库               | 程序 | 程序2 | 策划| 测试 | 美术 | 场景 | 特效 | UI |
| ----------------------------- | - | - | - | - | - | - | - | - |
| XGame-XClientLua              | √ | √ |
| XGame-XConfig                 | √ | √ | √ |
| XGame-XPublicLua              | √ | √ |
| XGame-XDriver                 | √ |   |
| XGame-XServer                 | √ | √ |
| XGame-XClient-Packages        | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-ProjectSettings | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-ResAB           | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsEditor    | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsPlugins   | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsResPublic | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsResModel  | √ |   |   |   | √ |   | √ |   |
| XGame-XClient-AssetsResScene  | √ |   | √ |   | √ | √ |   |   |
| XGame-XClient-AssetsResUI     | √ | √ | √ |   |   |   |   | √ |
| XGame-XClient-AssetsSrc       | √ | √ |

## 主要功能

* `C++`和`Lua`交互方案 （已完成）
* 基于`RecastNavigation`的导航网格寻路方案（已完成）
* 基于`Libuv`的网络通信框架方案（已完成）
* `RPC`通信协议方案（已完成）
* `Lua`-`C++`定时器方案（已完成）
* `Unity`调用`C++` `DLL`方案（已完成）
* `SFM`影子跟随移动方案（待开发）
* 状态同步方案（待开发）
* `LuaBT`行为树`AI`方案（开发中）
* 数据库存储和读写方案（待开发）
* `Unity`下`XLua`编程方案（待开发）
* 热更`C#`代码方案（待开发）
* `AssetsBundle`打包方案（开发中）
* 资源加载和管理方案（待开发）
* 日志输出和收集方案（待开发）

## 开发环境

* MacOS 10.15.6
* Unity2019.4.13f1

## 使用

1. 将本仓库`clone`或下载到本地，放到一个新目录，如`XGameSolutions`下
2. 运行`XGame`里面的`xgame-clone-or-pull-programmer-程序.sh`脚本迁出工程
3. 用`Unity`打开`XUnity`下的工程即可运行
