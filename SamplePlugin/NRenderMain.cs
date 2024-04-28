using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using NRender.Vfx;
using System.Collections.Generic;

namespace NRender;

public static class NRenderMain
{
    internal static string _name = "NRender";
    private static bool _inited = false;
    public static void Init(DalamudPluginInterface pluginInterface,string name)
    {
        pluginInterface.Create<Service>();
        Service.pluginLog.Info("NRender Init");
        Service.Framework.Update += Framework_Update;
        VfxManager.Init();
        _inited = true;
    }

    public static void Dispose()
    {
        if (!_inited) return;
        _inited = false;
        VfxManager.Dispose();
        Service.Framework.Update -= Framework_Update;
    }
    private static void Framework_Update(IFramework framework)
    {
        
    }
}
