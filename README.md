# Unity_HotUpdate_AAHybirdCLR 
### 基于Addressable+HybirdCLR的热更框架 
# 配置说明
#### 注：源工程虽然已经配置好所有，但是为了及防止不可预料的疏漏报错，还是提供了从头配置说明，建议只取所需，按照说明从头配置一遍！如此，即利于学习，也防止踩坑。
### **基础热更环境配置**
1. PackageManager中导入AA,github中导入HybirdCLR
2. 初始化设置HybirdCLR----上方工具栏，点击**HybirdCLR/install**安装，然后点击**HybirdCLR/Generate/All**(此操作建议在每次在出工程包前点击)，生成必要文件AddressableAssetsData和HybridCLRGenerate文件夹,然后在Unity-Preferences-Addressables设置中取消勾选 Debug Build Layout，**不然打包aa会报错**（虽然不重要，但是看着糟心）
3. 配置AA的资源包默认输出路径 ，在AddressableAssetSettings中的Profile中配置custom路径，BuildPath和LoadPath为"**[UnityEngine.Application.persistentDataPath]/[BuildTarget]**"，这两个路径是程序打包后资源包的输出路径和程序运行时读取资源包的路径（persistentDataPath是unity唯一的跨平台可读写路径）
4. 配置AA的catlog.json(资源包索引文件)和他的hash文件生成位置，在AddressableAssetSettings中找到
Catlog中的BuildRemoteCatolog,**在勾选后显出出来的Build&LoadPaths下拉列表选择Local**（这样catlog.json将会在程序打包后，生成到Profile配置的路径中，并且打包的程序运行时也会从这个路径中读取catlog.json。）
5. 配置好热更资源后，进行打AA包测试，如果成功，**将会在persistentDataPath看到资源包文件夹HotUpdateData**，里面有对应平台的热更资源。
6. 删除HotUpdateData文件夹，打包工程可执行文件后，**如果再次生成HotUpdateData文件夹**，说明AA插件配置完毕。
7. 新建**脚本文件夹Assets/ScriptsHot**(所有需要热更的脚本都要放这)，并在此**在目录下右键 Create/Assembly Definition**，创建一个名为**HotUpdate**的程序集模块
8. 打开菜单栏目中 **HybridCLR/Settings**， 在Hot Update Assemblies配置项中添加HotUpdate程序集(Assets/HotScripts/HotUpdate.asmdef)，设置程序集对所需程序集的引用，勾选Use GUIDs，并在下方添加，Unity.ResourceManager和Unity.Addressables两个程序集的GUID，**这样热更代码就可以引用Unity的资源管理和AA的资源管理了**,以后如果有其他引用也要在此添加，不然代码会报错。
9. **Scripting Backend 切换为 IL2CPP,Api Compatability Level 切换为 .Net 4.x(Unity 2019-2020) 或 .Net Framework（Unity 2021+）**
10. **导入Assets/Editor/HotUpdateTool.cs文件**，此文件编写了快捷出包脚本,删除persistentDataPath路径中的HotUpdateData文件夹，然后在上方菜单栏测试一键更新资源功能(资源操作-更新所有)，**如果HotUpdateData文件夹重新生成，且无报错，就证明热更框架配置成功**
11. **创建热更启动场景Assets/Scenes/Start.unity，创建空物体挂上Assets/Scripts/AboutHotUpdate/HotUpdateStarter.cs脚本**，脚本组件上暴露了一个场景路径参数，需要**填入您工程的初始热场景路径**，案例中为"Assets/Scenes/HotSceneDemo.unity",您可以**在此场景中加载您的热更资源，挂载热更代码**。
12.将工程打包可执行文件并运行（如果打包出错，可以再确认执行下步骤2），运行后将**跳转HotSceneDemo场景，并确定热更代码是否执行正常，热更资源是否加载正常**。
13.如果以上都正常，恭喜你，未来**如果只是更改了热更资源，只需要点击菜单栏中的资源操作-更新所有，再运行之前打包的可执行文件，就可以查验热更效果**。
14. 此热更只适用于本机程序测试热更效果，因为生成的热更资源只在您电脑上有，**如果别人拿到你的可执行文件，代码是索引不到persistentDataPath下的热更资源的**，要让别人拿到程序，程序能自动热更下载匹配所需热更资源，请接着看“**网络热更环境配置**”。

### **网络热更环境配置**
1. PackageManager中导入AA,github中导入HybirdCLR