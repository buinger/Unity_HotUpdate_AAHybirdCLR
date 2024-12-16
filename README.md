# Unity_HotUpdate_AAHybirdCLR 
### 基于Addressable+HybirdCLR的热更框架 


# 一.开发规范（使用此热更框架需要遵循的）
### 1.热更代码文件一律要放在Assets/ScriptsHot路径下
### 2.热更资源一律要在Inspector中勾选Addressable选项
### 3.热更预制件一定要放在Assets/Prefabs路径下

## 4.对象池使用说明（可选，统一动态加载释放，优化性能）
#### 0. 重点（每次加载新场景切记执行一次释放对象池函数）：
```
        //它能够清理池中所有null对象。
        GameObjectPoolTool.Release();
        //当然如果您不想跨场景复用池中对象，也可这样调用，这样会清空释放所有池对象。
        GameObjectPoolTool.Release(true);
```
#### 1. 加载资源函数：
```
        //第一个bool变量，设定生成的物体是否可显示，设为false则生成后自动隐藏

        //同步加载，返回GameObject
        GameObjectPoolTool.GetFromPoolForce(true, "Addressable预制件路径");
        //异步加载，返回GameObject
        GameObjectPoolTool.GetFromPoolForceAsync(true, "Addressable预制件路径");
        //(不常用)同步加载,返回GameObject,如果池中没有，会返回空
        GameObjectPoolTool.GetFromPool(true, "Addressable预制件路径");
```
#### 2. 回收资源函数：
```
        //此函数参数只能回收对象池生成的GameObject，放其他GameObject,不符合这个对象池的设计理念，会报错。
        //回收的资源将会存在对象池中，下次生成相同Addressable资源，会优先从对象池中取出
        GameObjectPoolTool.PutInPool(对象池生成的GameObject);
```
#### 3. 不想回收资源：
```
        //您也可以直接销毁,不会影响对象池的逻辑
        Destroy(对象池生成的GameObject);
```
#### 4. 附加（可选）：
点击unity的上方工具栏中---资源操作/一键刷新热更预制件索引脚本，便能够将所有热更预制件挂上一个路径索引脚本（PrefabInfo）。通过这个脚本，可以实现在程序运行时，**通过预制件快速拿到它的Addressable地址（Addressable原生是不支持的）。如果有新的热更预制件，切记要点击一次**。这个功能会在**每次更新热更资源的时候执行一次，保证脚本路径和Addressable设定的路径一致**。




# 二.配置说明
#### 注：源工程虽已经配置好所有，但为了及防止不可预料的疏漏报错，建议按照说明从头配置一遍！如此即利于学习，也防止踩坑。
### **基础热更环境配置**
1. PackageManager中导入AA,github中导入HybirdCLR
2. 先做初始化，上方工具栏，点击**HybirdCLR/install**安装，然后点击**HybirdCLR/Generate/All**(此操作建议在每次在出工程包前点击)，然后**进入Addressable面板，如果您是重头配置，需要点击，Create Addressable Settings**,确保文件夹AddressableAssetsData和HybridCLRGenerate存在于Assets根目录,然后在Unity-Preferences-Addressables设置中取消勾选 Debug Build Layout，**不然打包aa会报错**（虽然不重要，但是看着糟心）
3. 接着配置AA的资源包的默认读写路径 ，在AddressableAssetSettings中的Profile中配置custom路径，BuildPath和LoadPath填写为"**[UnityEngine.Application.persistentDataPath]/[BuildTarget]**"，这两个路径是程序打包后资源包的输出路径和程序运行时读取资源包的路径（persistentDataPath是unity唯一的跨平台可读写路径）
4. 配置AA的catlog.json(资源包索引文件)和他的hash文件生成位置，在AddressableAssetSettings文件中找到
Catlog中的BuildRemoteCatolog,**在勾选后显出出来的Build&LoadPaths下拉列表选择Local**（这样catlog.json将会在热更资源打包后，生成到Profile配置的路径中，并且打包的程序运行时也会从这个路径中读取catlog.json。）
5. 配置好热更资源后，打包热更资源，如果成功，**将会在persistentDataPath看到资源包文件夹HotUpdateData**，里面有对应平台的热更资源。
6. 删除HotUpdateData文件夹，打包工程可执行文件后，**如果再次生成HotUpdateData文件夹**，说明AA插件配置完毕。
7. 新建**脚本文件夹Assets/ScriptsHot**(所有需要热更的脚本都要放这)，并在此**在目录下右键 Create/Assembly Definition**，创建一个名为**HotUpdate**的程序集模块
8. 打开菜单栏目中 **HybridCLR/Settings**， 在Hot Update Assemblies配置项中添加HotUpdate程序集(Assets/HotScripts/HotUpdate.asmdef)，设置程序集文件对所需程序集的引用，勾选Use GUIDs，并在下方添加，Unity.ResourceManager和Unity.Addressables两个程序集的GUID，**这样热更代码就可以引用Unity的资源管理和AA的资源管理了**,以后如果有其他引用也要在此添加，不然代码会报错。
9.在buildSetting中，需要配置打包设置 **Scripting Backend 切换为 IL2CPP,Api Compatability Level 切换为 .Net 4.x(Unity 2019-2020) 或 .Net Framework（Unity 2021+）**
10. **导入Assets/Editor/HotUpdateTool.cs文件**，此文件编写了快捷出包脚本,删除persistentDataPath路径中的HotUpdateData文件夹，然后在上方菜单栏测试一键更新资源功能(资源操作-更新所有)，**如果HotUpdateData文件夹重新生成，且无报错，就证明热更框架配置成功**
11. **创建热更启动场景Assets/Scenes/Start.unity，创建空物体挂上Assets/Scripts/AboutHotUpdate/HotUpdateStarter.cs脚本**，脚本组件上暴露了mainScenePath参数，需要**填入您工程的初始热场景路径**，同时切记**取消勾选ifCheckUpdate变量，当前配置暂时用不到**。案例中热更场景为"Assets/Scenes/HotSceneDemo.unity",您可以**在此场景中加载您的热更资源，挂载热更代码**。
12.将工程打包可执行文件并运行（如果打包出错，可以再确认执行下步骤2），运行后将**跳转HotSceneDemo场景，并确定热更代码是否执行正常，热更资源是否加载正常**。
13.如果以上都正常，恭喜你，未来**如果只是更改了热更资源，只需要点击菜单栏中的资源操作-更新所有，再运行之前打包的可执行文件，就可以查验热更效果**。
14. 此热更只适用于本机程序测试热更效果，因为生成的热更资源只在您电脑上有，**如果别人拿到你的可执行文件，代码是索引不到persistentDataPath下的热更资源的**，要让别人拿到程序，程序能自动热更下载匹配所需热更资源，请接着看“**网络热更环境配置**”。

### **网络热更环境配置**
1. 紧接基础热更环境配置，先编译工程目录下的HotUpdateServer\HotUpdateServer.go，关于go语言如何配置环境和编译，可以自行百度，编译成功后会生成一个HotUpdateServer.exe文件（假设你的服务器是windows系统），**这个文件是热更服务器程序**。
2. 将HotUpdateServer.exe和工程目录下的HotUpdateServer\config.json，放在您的服务器上的文件夹里，运行HotUpdateServer.exe（工程中默认在本机直接运行测试，**正式部署的话，请运行在外网服务器**）。如果是正式环境，修改Assets/Editor/HotUpdateTool.cs文件中的 **"http://127.0.0.1:637/"**，ip为外网服务器的ip，端口号不变;
3. 在您的启动场景Assets/Scenes/Start.unity中，找到您之前挂的HotUpdateStarter组件，**勾选ifCheckUpdate变量，并在 urlHead 变量中填入http请求url头："http://127.0.0.1:637"(正式部署请把ip改成您服务器ip)  这样程序启动时会自动检查热更资源**，最后一个变量是loadingText，是UGUI的文本组件，需要拖拽一个，**这是用来显示更新进度的组件**。
4. 接下来进行测试就好，在unity工具栏点击**资源操作-更新所有**，热更资源更新后，再点击**资源操作-上传所有到服务器**，这样热更资源就会上传到服务器，**上传的数据，在您服务程序运行目录的HotUpdateData文件夹中**。
5. 打包运行您的unity工程，因为**目前服务端的热更数据和您本机数据是一样的不会触发更新**。
6. 修改HotSceneDemo中的模型位置，颜色，和代码，并再次在unity工具栏点击**资源操作-更新所有**，热更资源更新后，再点击**资源操作-上传所有到服务器**，这时服务端的资源便会更新。
7. 再次运行之前打包的unity工程，**此时程序会发现服务端的热更数据和本地不一样，触发更新**。如果运行结果是最新的，那么网络热更环境配置成功。