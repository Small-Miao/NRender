# NRender

一个调用游戏内函数绘制AVFX的库。

使用方法:

```C#
 NRenderMain.Init(pluginInterface, "NDraw");//初始化渲染库
```

绘制Omen

```c#
OmenElement vfx = new OmenElement(string path, System.Numerics.Vector3 scale, GameObject Owner, Vector4 color):
或
OmenElement vfx = new OmenElement(string path,Vector3 scale,Vector3 position,Vector4 color,float facing) 
```

facing 为 弧度

### 自定义扇形/原型/月环

```c#
   VfxHelper.RegisterFanVfx(弧度, "vfx/omen/eff/nd/customFan.avfx");//后面为AVFX注册路径 (注册可覆盖)
   VfxHelper.RegisterCircleVfx("vfx/omen/eff/nd/customCircle.avfx");
   VfxHelper.RegisterDountVfx("vfx/omen/eff/nd/customDount.avfx",内径百分比);比如 0.5f 大小为 12 内部为 6
```

### 点名等特效

```c#
ActorVfx(string path, GameObject caster, GameObject target)
ActorVfx(string path, nint caster, nint target, bool? shouldRemove = null) shouldRemove 可以忽略 遗留参数 懒得改
```

