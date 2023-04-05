# XGame

XGame旨在提供一套完整的大型游戏开发解决方案，包括前后端、底层驱动、配置数据等。采用`C++`、`C#`、`Lua`、`Python`等语言开发。

XGame模拟大项目团队管理流程，为了方便项目管理、多部门协作、权限划分、避免版本管理错乱等，将工程拆分成多个仓库。不同的开发角色可以迁出不同的仓库组合所需的最小工程，提高开发效率，减少冗余错误，降低维护成本。

XGame对人数百人以上，资源量30G以上的团队具有一定的参考意义。小团队小游戏的话可以考虑更简洁的方案。

本方案重点放在整体框架设计和资源管理上，有些功能只是简单实现，特别是底层驱动和服务器部分，没有具体参考意义。

## 方案组成

| 仓库 | 描述 |
| ------------------------------------------------------------------------------------------------- | -- |
| [XGame-XClientLua](https://github.com/monitor1394/XGame-XClientLua.git)                           | XGame游戏解决方案：客户端Lua代码 |
| [XGame-XConfig](https://github.com/monitor1394/XGame-XConfig.git)                                 | XGame游戏解决方案：配置数据 |
| [XGame-XCommon](https://github.com/monitor1394/XGame-XCommon.git)                                 | XGame游戏解决方案：前后端共用的Lua代码 |
| [XGame-XDriver](https://github.com/monitor1394/XGame-XDriver.git)                                 | XGame游戏解决方案：底层C++驱动代码，前后端共用 |
| [XGame-XServer](https://github.com/monitor1394/XGame-XServer.git)                                 | XGame游戏解决方案：服务端Lua代码 |
| [XGame-XClient-Packages](https://github.com/monitor1394/XGame-XClient-Packages.git)               | XGame游戏解决方案：Unity客户端的Packages目录 |
| [XGame-XClient-ProjectSettings](https://github.com/monitor1394/XGame-XClient-ProjectSettings.git) | XGame游戏解决方案：Unity客户端的ProjectSettings目录 |
| [XGame-XClient-iOS](https://github.com/monitor1394/XGame-XClient-iOS.git)                         | XGame游戏解决方案：Unity客户端的ResAB下的iOS目录 |
| [XGame-XClient-Editors](https://github.com/monitor1394/XGame-XClient-Editors.git)                 | XGame游戏解决方案：Unity客户端的Assets下的Editors目录，用于放工具类或不用打包的代码 |
| [XGame-XClient-Plugins](https://github.com/monitor1394/XGame-XClient-Plugins.git)                 | XGame游戏解决方案：Unity客户端的Assets下的Plugins目录，用于放第三方插件代码 |
| [XGame-XClient-ResData](https://github.com/monitor1394/XGame-XClient-ResData.git)                 | XGame游戏解决方案：Unity客户端的Assets下的ResData目录，用于放程序和策划资源数据 |
| [XGame-XClient-ResPublic](https://github.com/monitor1394/XGame-XClient-ResPublic.git)             | XGame游戏解决方案：Unity客户端的Assets下的ResPublic目录，公共资源 |
| [XGame-XClient-ResEffect](https://github.com/monitor1394/XGame-XClient-ResEffect.git)             | XGame游戏解决方案：Unity客户端的Assets下的ResEffect目录，特效资源 |
| [XGame-XClient-ResModel](https://github.com/monitor1394/XGame-XClient-ResModel.git)               | XGame游戏解决方案：Unity客户端的Assets下的ResModel目录，角色模型资源 |
| [XGame-XClient-ResScene](https://github.com/monitor1394/XGame-XClient-ResScene.git)               | XGame游戏解决方案：Unity客户端的Assets下的ResScene目录，场景资源 |
| [XGame-XClient-ResSceneModel](https://github.com/monitor1394/XGame-XClient-ResSceneModel.git)     | XGame游戏解决方案：Unity客户端的Assets下的ResSceneModel目录，场景模型资源 |
| [XGame-XClient-ResUI](https://github.com/monitor1394/XGame-XClient-ResUI.git)                     | XGame游戏解决方案：Unity客户端的Assets下的ResUI目录，UI资源 |
| [XGame-XClient-Runtime](https://github.com/monitor1394/XGame-XClient-Runtime.git)                 | XGame游戏解决方案：Unity客户端的Assets下的Runtime目录，运行时或打包用的代码 |

## 迁出参考

√√ 表示需要迁出仓库，可以提交仓库
√x 表示需要迁出仓库，不能提交仓库

|             仓库               | 打包 | 程序1 | 程序2 | 策划 | QA1 | QA2 | 美术 | 角色 | 场编 | 场模 | 特效 | UI |
| ----------------------------- | -  | -  | -  | -  | -  | -  | -  | -  | -  | -  | -  | -  |
| XGame-XClientLua              | √√ | √√ | √√ | √x | √x |
| XGame-XConfig                 | √√ | √x | √x | √√ | √x |
| XGame-XCommon                 | √√ | √√ | √√ | √x | √x |
| XGame-XDriver                 |    | √√ |
| XGame-XServer                 |    | √√ | √√ | √x | √x |
| XGame-XClient-Packages        | √√ | √√ | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-ProjectSettings | √√ | √√ | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-ResAB           | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-Editors         |    | √√ | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-Plugins         | √√ | √√ | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-Runtime         | √√ | √√ | √√ | √x | √x | √x | √x | √x | √x | √x | √x | √x |
| XGame-XClient-ResData         | √√ | √√ |    | √√ | √x |    | √x | √x | √x | √x | √x | √x |
| XGame-XClient-ResEffect       | √√ | √√ |    | √√ | √x |    | √√ |    |    |    | √√ |    |
| XGame-XClient-ResModel        | √√ | √√ |    | √√ | √x |    | √√ | √√ |    |    |    |    |
| XGame-XClient-ResScene        | √√ | √√ |    | √√ | √x |    | √√ |    | √√ | √x |    |    |
| XGame-XClient-ResSceneModel   | √√ | √√ |    | √√ | √x |    | √√ |    | √x | √√ |    |    |
| XGame-XClient-ResUI           | √√ | √√ | √√ | √x | √x |    | √√ |    |    |    |    | √√ |
| XGame-XClient-ResPublic       | √√ | √√ | √√ | √√ | √x |    | √√ | √√ | √√ | √√ | √√ | √√ |

## 参考方案

* [打包方案](#打包方案)
* 首包分包方案
* 多语言包方案
* 多工程多版本的资源同步方案
* 自动化首包资源抽取方案

## 主要功能

| 功能 | 完成情况 |
| ----------------------------------- | ----- |
| `C++`和`Lua`交互方案                  | 已完成 |
| `XLua`                              | 已完成 |
| 基于`RecastNavigation`的导航网格寻路   | 已完成 |
| 基于`Libuv`的网络通信                 | 已完成 |
| 自动化打包                           | 开发中 |
| AB浏览和管理工具                      | 开发中 |
| Editor-Phone远程调试工具              | 开发中 |
| Shader和变体管理工具                  | 开发中 |
| 资源加载和管理                        | 待开发 |
| Patch和热更                          | 待开发 |
| `LuaBT`行为树`AI`方案                | 待开发 |
| 数据库存储和读写方案                   | 待开发 |
| 日志输出和收集                        | 待开发 |
| `SFM`影子跟随移动                     | 待开发 |
| 战斗同步                             | 待开发 |

## 开发环境

* MacOS 10.15.6
* Unity2019.4.39f1

## 使用教程

1. 将本仓库`clone`或下载到本地，放到一个新目录，如`XGameSolutions`下。
2. 运行`XGame`里面的`project_pull_clone_developer.bat`或`project_pull_clone_developer.sh`脚本`Clone`工程，如果工程已`Clone`则进行更新。
3. 用`Unity`打开`XUnity`下的`UnityForCoder`工程即可运行。

## 目录结构

``` text
.
├── XGameSolutions
.   ├── XClientLua
    ├── XConfig
    ├── XDriver
    ├── XGame
    ├── XCommon
    ├── XServer
    └── XUnity
        ├── UnityForCoder
        |   ├── Assets
        |   |   ├── Editors
        |   |   ├── Plugins
        |   |   ├── ResEffect
        |   |   ├── ResModel
        |   |   ├── ResScene
        |   |   ├── ResSceneModel
        |   |   ├── ResUI
        |   |   ├── ResPublic
        |   |   └── Runtime
        |   ├── Packages
        |   ├── ProjectSettings
        |   └── ResAB
        ├── UnityForArt

```

## 打包方案

### 规则

* 需要手动设置ABName的资源：要用的.unity场景文件，要用的模型prefab，要用的特效prefab。其他的资源不要手动设置ABName，脚本会自动分析依赖设置。
* AB加前缀，方便分类查看，如 model_prefabname, scene_scenename, scene_dep_texturename。
* 自动分析的依赖AB加明显标志，方便每次重新分析设置时清理，如：_dep_。
* AB的名字只能由小写字母、数字和下划线组成，不能包含其他特殊字符，统一将 `[^a-zA-Z0-9]`替换为`_`。
* 清理依赖AB时，有时候Unity里看不到ABName了但meta文件里还存在，可以通过读取meta文件清除ABName。
* 所有Lua代码都打到一个同一个AB里。
* 所有配置表都打到同一个AB里。
* 【UI】UI按目录自动设置AB，同一个系统的UI可都打到同一个AB里。
* 【UI】大图单独打包，一张大图一个AB。
* 【UI】贴图和预设分离打包，减少热更量和避免循环依赖。
* 【Shader】Shader单独打包，方便热更，避免打包冗余。
* 【Shader】Shader变体手动维护。
* 【Shader】代码要用的Shader统一放到同一个AB，自定义Shader加载，不能直接用Shader.Find。
* 【Shader】非代码用的shader不要手动设ABName，通过依赖分析自动设置，避免把不再使用的Shader打进包里。
* 【模型】只需给要用的prefab手动设置ABName，其他资源的ABName由依赖分析自动设置。
* 【模型】模型中用到的材质球和它用到的贴图单独打包。
* 【模型】如果贴图被多个材质球共用，贴图抽离出来单独打包。如果贴图很小（如小于50KB），可统一打到一个公共的模型贴图包里。
* 【场景】只需给要用的.unity场景手动设置ABName，其他资源的ABName由依赖分析自动设置。
* 【场景】场景中用到的材质球和它用到的贴图单独打包。
* 【场景】如果贴图被多个材质球共用，贴图抽离出来单独打包。如果贴图很小（如小于50KB），可统一打到一个公共的场景贴图包里。
* 【场景】场景中用到的prefab和它引用到的资源单独打包（shader除外，已设ABName的资源除外）。

### 依赖设置ABName流程

1. 清空meta文件里的依赖ABName。
2. 获取所有ABName的依赖资源。
3. 清空依赖资源的ABName。
4. 有些目录强设ABName。
5. 模型的材质球单独设置ABName。
6. 场景的材质球单独设置ABName。
7. 场景的Prefab单独设置ABName。
8. 其他依赖资源设置ABname。
9. Shader引用的资源打包到对应的Shader里。
