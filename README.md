# Unity_HotUpdate_AAHybirdCLR 
### 基于Addressable+HybirdCLR的热更框架 
## 配置说明
1. PackageManager中导入AA,github中导入HybirdCLR
2. 初始化设置HybirdCLR----上方工具栏，点击HybirdCLR/install安装，然后点击HybirdCLR/Generate/All(保持出工程包前点击习惯)，生成必要文件
AddressableAssetsData和HybridCLRGenerate文件夹,然后在Unity-Preferences-Addressables设置中取消勾选 Debug Build Layout，不然打包aa会报错（虽然不重要，但是看着糟心）
3. 配置AA的资源包默认输出路径 ，在AddressableAssetSettings中的Profile中配置custom路径，BuildPath和LoadPath为
"[UnityEngine.Application.persistentDataPath]/[BuildTarget]"，这两个路径是程
序打包后资源包的输出路径和程序运行时读取资源包的路径（persistentDataPath是unity唯一的跨平台可读写路径）
4. 配置AA的catlog.json(资源包索引文件)和他的hash文件生成位置，在AddressableAssetSettings中找到
Catlog中的BuildRemoteCatolog,在勾选后显出出来的Build&LoadPaths下拉列表选择Local（这样catlog.json将会在程序打包后，
生成到Profile配置的路径中，并且打包的程序运行时也会从这个路径中读取catlog.json。）
5. 配置好热更资源后，进行打AA包测试，如果成功，将会在persistentDataPath看到资源包文件夹HotUpdateData，里面有对应平台的热更资源。
6. 删除HotUpdateData文件夹，打包工程可执行文件后，如果重复生成HotUpdateData文件夹等文件，说明AA插件配置完毕。
7. 新建脚本文件夹Assets/Scripts，在目录下 右键 Create/Assembly Definition，创建一个名为HotUpdate的程序集模块
8. 打开菜单 HybridCLR/Settings， 在Hot Update Assemblies配置项中添加HotUpdate程序集
9. Scripting Backend 切换为 IL2CPP,Api Compatability Level 切换为 .Net 4.x(Unity 2019-2020) 或 .Net Framework（Unity 2021+）
9. 导入Assets/Editor/HotUpdateTool.cs文件，此文件编写了快捷出包脚本
10. 创建热更启动场景Assets/Scenes/Start.unity，创建空物体挂上HotUpdateStarter脚本。  