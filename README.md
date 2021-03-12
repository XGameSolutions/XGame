# XGame

XGame旨在提供一套完整的游戏解决方案，包括前后端、底层驱动、配置数据等。采用`C++`、`C#`、`Lua`等语言开发。

XGame模拟大项目团队管理流程，为了方便项目管理，多部门协作和权限划分等，以提高开发效率，将工程拆分成多个仓库。不同的开发角色可以迁出不同的仓库组合所需的工程。

## 方案组成

| 仓库 | 描述 |
| ------------------------------------------------------------------------------------------------- | -- |
| [XGame-XClientLua](https://github.com/monitor1394/XGame-XClientLua.git)                           | XGame游戏解决方案：客户端Lua代码 |
| [XGame-XConfig](https://github.com/monitor1394/XGame-XConfig.git)                                 | XGame游戏解决方案：配置数据 |
| [XGame-XPublicLua](https://github.com/monitor1394/XGame-XPublicLua.git)                           | XGame游戏解决方案：前后端共用的Lua代码 |
| [XGame-XDriver](https://github.com/monitor1394/XGame-XDriver.git)                                 | XGame游戏解决方案：底层C++驱动代码 |
| [XGame-XServer](https://github.com/monitor1394/XGame-XServer.git)                                 | XGame游戏解决方案：服务端Lua代码 |
| [XGame-XClient-Packages](https://github.com/monitor1394/XGame-XClient-Packages.git)               | XGame游戏解决方案：客户端Unity的Packages目录 |
| [XGame-XClient-ProjectSettings](https://github.com/monitor1394/XGame-XClient-ProjectSettings.git) | XGame游戏解决方案：客户端Unity的ProjectSettings目录 |
| [XGame-XClient-ResAB](https://github.com/monitor1394/XGame-XClient-ResAB.git)                     | XGame游戏解决方案：客户端Unity的ResAB目录 |
| [XGame-XClient-AssetsEditor](https://github.com/monitor1394/XGame-XClient-AssetsEditor.git)       | XGame游戏解决方案：客户端Unity的Assets下的Editor目录 |
| [XGame-XClient-AssetsPlugins](https://github.com/monitor1394/XGame-XClient-AssetsPlugins.git)     | XGame游戏解决方案：客户端Unity的Assets下的Plugins目录 |
| [XGame-XClient-AssetsResPublic](https://github.com/monitor1394/XGame-XClient-AssetsResPublic.git) | XGame游戏解决方案：客户端Unity的Assets下的ResPublic目录 |
| [XGame-XClient-AssetsResEffect](https://github.com/monitor1394/XGame-XClient-AssetsResEffect.git) | XGame游戏解决方案：客户端Unity的Assets下的ResEffect目录 |
| [XGame-XClient-AssetsResModel](https://github.com/monitor1394/XGame-XClient-AssetsResModel.git)   | XGame游戏解决方案：客户端Unity的Assets下的ResModel目录 |
| [XGame-XClient-AssetsResScene](https://github.com/monitor1394/XGame-XClient-AssetsResScene.git)   | XGame游戏解决方案：客户端Unity的Assets下的ResScene目录 |
| [XGame-XClient-AssetsResUI](https://github.com/monitor1394/XGame-XClient-AssetsResUI.git)         | XGame游戏解决方案：客户端Unity的Assets下的ResUI目录 |
| [XGame-XClient-AssetsSrc](https://github.com/monitor1394/XGame-XClient-AssetsSrc.git)             | XGame游戏解决方案：客户端Unity的Assets下的Src目录 |

## 迁出参考

|             仓库               | 程序 | 程序2 | 策划| 测试 | 美术 | 场景 | 模型 | 特效 | UI |
| ----------------------------- | - | - | - | - | - | - | - | - | - |
| XGame-XClientLua              | √ | √ |
| XGame-XConfig                 | √ | √ | √ |
| XGame-XPublicLua              | √ | √ |
| XGame-XDriver                 | √ |   |
| XGame-XServer                 | √ | √ |
| XGame-XClient-Packages        | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-ProjectSettings | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-ResAB           | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsEditor    | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsPlugins   | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsResPublic | √ | √ | √ | √ | √ | √ | √ | √ | √ |
| XGame-XClient-AssetsResEffect | √ |   |   |   | √ |   |   | √ |   |
| XGame-XClient-AssetsResModel  | √ |   |   |   | √ |   | √ |   |   |
| XGame-XClient-AssetsResScene  | √ |   | √ |   | √ | √ |   |   |   |
| XGame-XClient-AssetsResUI     | √ | √ | √ |   | √ |   |   |   | √ |
| XGame-XClient-AssetsSrc       | √ | √ |

## 参考方案

* [打包方案](#打包方案)

## 开发环境

* MacOS 10.15.6
* Unity2019.4.13f1

## 使用教程

1. 将本仓库`clone`或下载到本地，放到一个新目录，如`XGameSolutions`下。
2. 运行`XGame`里面的`project-for-coder-程序.bat`或`project-for-coder-程序.sh`脚本`Clone`工程，如果工程已`Clone`则进行更新。
3. 用`Unity`打开`XUnity`下的`UnityForCoder`工程即可运行。

## 目录结构

``` text
.
├── XGameSolutions
.   ├── XClientLua
    ├── XConfig
    ├── XDriver
    ├── XGame
    ├── XPublicLua
    ├── XServer
    └── XUnity
        ├── UnityForCoder
        |   ├── Assets
        |   |   ├── Editor
        |   |   ├── Plugins
        |   |   ├── ResEffect
        |   |   ├── ResModel
        |   |   ├── ResPublic
        |   |   ├── ResScene
        |   |   ├── ResUI
        |   |   └── Src
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

### 部分AB前缀参考

``` text
lua
cfg
shader
model_weapon_xxx
model_monster_xxx
model_equipment_xxx
model_npc_xxx
model_ride_xxx
model_xxx
model_dep_xxx
model_dep_mat_xxx
model_dep_common_tex_xxx
scene_xxx
scene_dep_xxx
scene_dep_mat_xxx
scene_dep_mat_common_tex_xxx
scene_dep_prefab_xxx
ui_xxx
ui_big_image_xxx
```
