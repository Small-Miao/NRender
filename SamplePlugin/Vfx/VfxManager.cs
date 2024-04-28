using Dalamud.Hooking;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NRender.Vfx
{
    public static class VfxManager
    {
        public static List<OmenElement> drawOmenElementList = new List<OmenElement>();

        public delegate nint CreateOmenDelegate(uint omenId, nint pos, long chara, float speed, float facing, float scaleX, float scaleY, float scaleZ, int isEnemy, int priority);
        public static CreateOmenDelegate? CreateOmen;
        public static Hook<CreateOmenDelegate> CreateOmenHook;
        public delegate nint CreateVfxDelegate(nint resPath,nint param,int attr,int priority,float posX,float posY,float posZ,float scaleX,float scaleY,float scaleZ,float facing,float speed,int offscreen);
        public static CreateVfxDelegate CreateVfx;
        public static Hook<CreateVfxDelegate> CreateVfxHook;

        public delegate nint StaticVfxCreateDelegate(string path, string pool);
        public static StaticVfxCreateDelegate? StaticVfxCreate;
        public static Hook<StaticVfxCreateDelegate> StaticVfxHook;

        public delegate nint InitVfxParamDelegate(nint array);
        public static InitVfxParamDelegate InitVfxParam;

        public delegate nint SetVfxP1Delegate(nint a1,string a2);
        public static SetVfxP1Delegate SetVfxP1;

        public delegate nint SetVfxP2Delegate(nint a1, string a2);
        public static SetVfxP2Delegate SetVfxP2;

        public delegate nint SetOmenColorDelegate(nint handle, float r, float g, float b, float a);
        public static SetOmenColorDelegate SetOmenColor;

        public delegate nint RemoveOmenDelegate(nint handle,int uk1);
        public static RemoveOmenDelegate RemoveOmen;
        public static Hook<RemoveOmenDelegate> RemoveOmenHook;

        public delegate nint SetOmenMatrixDelegate(nint handle, nint pos);
        public static SetOmenMatrixDelegate SetOmenMatrix;
        public static Hook<SetOmenMatrixDelegate> SetOmenMatrixHook;


        public static void Init()
        {
            Service.pluginLog.Info("Init");
            var createOmenFunctionAddress = Service.SigSCanner.ScanText(SigConst.CreateOmenSig);
            CreateOmen = Marshal.GetDelegateForFunctionPointer<CreateOmenDelegate>(createOmenFunctionAddress);
            CreateOmenHook = Service.Hook.HookFromAddress<CreateOmenDelegate>(createOmenFunctionAddress,createOmenFunctionHandle);
            CreateOmenHook.Enable();

            var createVfxFunctionAddress = Service.SigSCanner.ScanText(SigConst.CreateVfxSig);
            CreateVfx = Marshal.GetDelegateForFunctionPointer<CreateVfxDelegate>(createVfxFunctionAddress);
            CreateVfxHook = Service.Hook.HookFromAddress<CreateVfxDelegate>(createVfxFunctionAddress,createVfxFunctionHandle);
            CreateVfxHook.Enable();

            var staticVfxCreateFunctionAddress = Service.SigSCanner.ScanText(SigConst.StaticVfxCreateSig);
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<StaticVfxCreateDelegate>(staticVfxCreateFunctionAddress);
            StaticVfxHook = Service.Hook.HookFromAddress<StaticVfxCreateDelegate>(staticVfxCreateFunctionAddress,StaticVfxFunctionHandle);
            StaticVfxHook.Enable();

            var initVfxParamFunctionAddress = Service.SigSCanner.ScanText(SigConst.InitVfxParam);
            InitVfxParam = Marshal.GetDelegateForFunctionPointer<InitVfxParamDelegate>(initVfxParamFunctionAddress);

            var setVfxP1FunctionAddress = Service.SigSCanner.ScanText(SigConst.SetVfxP1Sig);
            SetVfxP1 = Marshal.GetDelegateForFunctionPointer<SetVfxP1Delegate>(setVfxP1FunctionAddress);

            var setVfxP2FunctionAddress = Service.SigSCanner.ScanText(SigConst.SetVfxP2Sig);
            SetVfxP2 = Marshal.GetDelegateForFunctionPointer<SetVfxP2Delegate>(setVfxP2FunctionAddress);

            var setOmenColorFunctionAddress = Service.SigSCanner.ScanText (SigConst.SetOmenColorSig);
            SetOmenColor = Marshal.GetDelegateForFunctionPointer<SetOmenColorDelegate>(setOmenColorFunctionAddress);

            var removeOmenFunctionAddress = Service.SigSCanner.ScanText(SigConst.RemoveOmenSig);
            RemoveOmen = Marshal.GetDelegateForFunctionPointer<RemoveOmenDelegate>(removeOmenFunctionAddress);
            RemoveOmenHook = Service.Hook.HookFromAddress<RemoveOmenDelegate>(removeOmenFunctionAddress, RemoveOmenHandle);
            RemoveOmenHook.Enable();

            var setOmenMatrixFunctionAddress = Service.SigSCanner.ScanText(SigConst.SetOmenMatrixSig);
            SetOmenMatrix = Marshal.GetDelegateForFunctionPointer<SetOmenMatrixDelegate> (setOmenMatrixFunctionAddress);
            SetOmenMatrixHook = Service.Hook.HookFromAddress<SetOmenMatrixDelegate>(setOmenMatrixFunctionAddress, SetOmenMatrixHandle);
        }

        private static nint SetOmenMatrixHandle(nint handle, nint pos)
        {

            var a = Marshal.PtrToStructure<Matrix4>(pos);
            Service.pluginLog.Info(a.ToString());
            return SetOmenMatrixHook.Original.Invoke(handle, pos);
        }

        private static nint RemoveOmenHandle(nint handle, int uk1)
        {
            return RemoveOmenHook.Original.Invoke(handle, uk1);
        }

        private static nint StaticVfxFunctionHandle(string path, string pool)
        {
            //PluginLog.Log($"Creating Static VFX: {path} pool:{pool}");
            var vfx = StaticVfxHook.Original(path, pool);

            return vfx;
        }

        private static nint createVfxFunctionHandle(nint resPath, nint param, int attr, int priority, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, float facing, float speed, int offscreen)
        {
            var a = CreateVfxHook.Original.Invoke(resPath, param, attr, priority, posX, posY, posZ, scaleX, scaleY, scaleZ, facing, speed, offscreen);
            Service.pluginLog.Info($"VfxHandle:{a} ResPath:{Marshal.PtrToStringAnsi(resPath)} param:{param} attr:{attr} Pos:[{posX} {posY} {posZ}] Scale:[{scaleX} {scaleY} {scaleZ}] facing:{facing} speed:{speed}");
            return a;
        }

        private static nint createOmenFunctionHandle(uint omenId, nint pos, long chara, float speed, float facing, float scaleX, float scaleY, float scaleZ, int isEnemy, int priority)
        {
            var a = CreateOmenHook.Original.Invoke(omenId, pos, chara, speed, facing, scaleX, scaleY, scaleZ, isEnemy, priority);
            Service.pluginLog.Info($"OmenHandle:{a}");
            return a;
        }

        public static void Dispose()
        {
            foreach (var item in drawOmenElementList)
            {
                RemoveOmen?.Invoke((nint)item.VfxHandle, 1);
            }

            CreateOmenHook.Dispose();
            CreateVfxHook.Dispose();
            StaticVfxHook.Dispose();
        }
    }
}
