# Unity_HotUpdate_AAHybirdCLR 
### 基于Addressable+HybirdCLR的热更框架 


# 一.开发规范（使用此热更框架需要遵循的）
### 1.热更代码文件一律要放在Assets/ScriptsHot路径下
### 2.热更资源一律要在Inspector中勾选Addressable选项


# 二.配置说明
#### 注：源工程虽已经配置好所有，但为了及防止不可预料的疏漏报错，建议按照说明从头配置一遍！如此即利于学习，也防止踩坑。
### **基础热更环境配置**
1. PackageManager中导入AA,导入项目根目录文件"com.code-philosophy.hybridclr/package.json"（或者用git的url：https://github.com/focus-creative-games/hybridclr_unity.git） 导入HybirdCLR
2. 先做初始化，上方工具栏，点击**HybirdCLR/install**安装，然后点击**HybirdCLR/Generate/All**(此操作建议在每次在出工程包前点击)，然后**进入Addressable面板，如果您是重头配置，需要点击，Create Addressable Settings**,确保文件夹AddressableAssetsData和HybridCLRGenerate存在于Assets根目录,然后在Unity-Preferences-Addressables设置中取消勾选 Debug Build Layout，**不然打包aa会报错**（虽然不重要，但是看着糟心）
3. 接着配置AA的资源包的默认读写路径 ，**把AddressableAssetSettings文件的Profile设为Default**，同时进入Default编辑窗口，找到里面的Local设定，**直接设定为Built-In就行了**，这样就使用aa内部的全平台适配路径了。**（PS：下面的BuildPath“[UnityEngine.AddressableAssets.Addressables.BuildPath]/[BuildTarget]”和LoadPath“{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]”是程序打包后资源包的输出路径和程序运行时读取资源包的路径）**
4. 配置AA的catlog.json(资源包索引文件)和他的hash文件生成位置，在AddressableAssetSettings文件中找到ContentUpdate,**把Build&LoadPaths下拉列表选择Local**，确定下方路径预览末尾是BuildTarget文件夹名。（这样catlog.json将会在热更资源打包后，生成到Profile配置的路径中，并且打包的程序运行时也会从这个路径中读取catlog.json。）
5. 配置好热更资源后，打包热更资源，成功后，点击上方工具栏-资源操作-打开当前平台热更资源文件夹，**将会打开文件夹BuildTarget**，里面是当前平台的热更资源。
6. 删除BuildTarget文件夹，打包工程可执行文件后，**如果再次生成BuildTarget文件夹**，说明AA插件配置完毕。
7. 新建**脚本文件夹Assets/ScriptsHot**(所有需要热更的脚本都要放这)，并在此**在目录下右键 Create/Assembly Definition**，创建一个名为**HotUpdate**的程序集模块
8. 打开菜单栏目中 **HybridCLR/Settings**， 在Hot Update Assemblies配置项中添加HotUpdate程序集(Assets/HotScripts/HotUpdate.asmdef)，设置程序集文件对所需程序集的引用，勾选Use GUIDs，并在下方添加，Unity.ResourceManager和Unity.Addressables两个程序集的GUID，**这样热更代码就可以引用Unity的资源管理和AA的资源管理了**,以后如果有其他引用也要在此添加，不然代码会报错。
9. 在buildSetting中，需要配置打包设置 **Scripting Backend 切换为 IL2CPP,Api Compatability Level 切换为 .Net 4.x(Unity 2019-2020) 或 .Net Framework（Unity 2021+）**
10. 点击上方工具栏-资源操作-打开当前平台热更资源文件夹，删除打开的文件夹BuildTarget中所有东西，然后点击上方工具栏-资源操作-更新所有，**如果BuildTarget中有文件重新生成，且无报错，就证明热更框架配置成功**
11. **创建热更启动场景Assets/Scenes/Start.unity，创建空物体挂上Assets/Scripts/AboutHotUpdate/HotUpdateStarter.cs脚本**，脚本组件上暴露了mainScenePath参数，需要**填入您工程的初始热场景路径**，同时切记**取消勾选ifCheckUpdate变量，当前配置暂时用不到**。案例中热更场景为"Assets/Scenes/HotSceneDemo.unity",您可以**在此场景中加载您的热更资源，挂载热更代码**。
12. 将工程打包可执行文件并运行（如果打包出错，大概率是vs没有安装“**使用c++的游戏开发**”模块，同时再确认执行下步骤2），运行后将**跳转HotSceneDemo场景，并确定热更代码是否执行正常，热更资源是否加载正常**。
13. 如果以上都正常，恭喜你，未来**如果只是更改了热更资源，只需要点击菜单栏中的资源操作-更新所有，再运行之前打包的可执行文件，就可以查验热更效果**。
14. 此热更只适用于本机程序测试热更效果，要让别人拿到程序，程序能自动热更下载匹配所需热更资源，请接着看“**网络热更环境配置**”。

### **网络热更环境配置**
1. 紧接基础热更环境配置，先编译工程目录下的HotUpdateServer\HotUpdateServer.go，关于go语言如何配置环境和编译，可以自行百度，编译成功后会生成一个HotUpdateServer.exe文件（假设你的服务器是windows系统），**这个文件是热更服务器程序**。
2. 将HotUpdateServer.exe和工程目录下的HotUpdateServer\config.json，放在您的服务器上的文件夹里，运行HotUpdateServer.exe（工程中默认在本机直接运行测试，**正式部署的话，请运行在外网服务器**）。如果是正式环境，修改Assets/Editor/HotUpdateTool.cs文件中的 **"http://127.0.0.1:637"**，ip为外网服务器的ip，端口号不变;
3. 在您的启动场景Assets/Scenes/Start.unity中，找到您之前挂的HotUpdateStarter组件，**勾选ifCheckUpdate变量，并在 urlHead 变量中填入http请求url头："http://127.0.0.1:637"(正式部署请把ip改成您服务器ip)  这样程序启动时会自动检查热更资源**，最后一个变量是loadingText，是UGUI的文本组件，需要拖拽一个，**这是用来显示更新进度的组件**。
4. 接下来进行测试就好，在unity工具栏点击**资源操作-更新所有**，热更资源更新后，再点击**资源操作-干净上传**，这样热更资源就会上传到服务器，**上传的数据，在您服务程序运行目录的HotUpdateData文件夹中**。
5. 打包运行您的unity工程，因为**目前服务端的热更数据和您本机数据是一样的不会触发更新**。
6. 修改HotSceneDemo中的模型位置，颜色，和代码，并再次在unity工具栏点击**资源操作-更新所有**，热更资源更新后，再点击**资源操作-干净上传**，这时服务端的资源便会更新。
7. 再次运行之前打包的unity工程，**此时程序会发现服务端的热更数据和本地不一样，触发更新**。如果运行结果是最新的，那么网络热更环境配置成功。