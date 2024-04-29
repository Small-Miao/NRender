using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
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
        /// <summary>
        /// 绘制Omen存储列表
        /// </summary>
        public static List<OmenElement> drawOmenElementList = new List<OmenElement>();

        /// <summary>
        /// Omen创建委托
        /// </summary>
        /// <param name="omenId"></param>
        /// <param name="pos"></param>
        /// <param name="chara"></param>
        /// <param name="speed"></param>
        /// <param name="facing"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="isEnemy"></param>
        /// <param name="priority"></param>
        /// <returns>VFX句柄</returns>
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

        public unsafe delegate ResourceHandle* GetResourceSyncDelegate(nint pFileManager, uint* pCategoryId, uint* pResourceType, uint* pResourceHash, Byte* pPath, nint pUnknown);
        public static GetResourceSyncDelegate? GetResourceSync;
        public static Hook<GetResourceSyncDelegate> GetResourceSyncHook;

        public unsafe delegate nint VfxResoucesLoadDelegate(nint* pVfx, nint* pFile, uint fileSize, ResourceHandle* pHandle);
        public static VfxResoucesLoadDelegate? VfxResoucesLoad;

        public unsafe delegate nint VfxResSettupCompleteDelegate(ResourceHandle* pHandle);
        public static VfxResSettupCompleteDelegate? VfxResSettupComplete;
        public static nint ResouceManagerAddress = 0x0;

        public static void Init()
        {
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

            var initVfxParamFunctionAddress = Service.SigSCanner.ScanText(SigConst.InitVfxParamSig);
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

            unsafe{
                var getResourceSyncFunctionAddress = Service.SigSCanner.ScanText(SigConst.GetResourceSyncSig);
                GetResourceSync = Marshal.GetDelegateForFunctionPointer<GetResourceSyncDelegate>(getResourceSyncFunctionAddress);
                GetResourceSyncHook = Service.Hook.HookFromAddress<GetResourceSyncDelegate>(getResourceSyncFunctionAddress, GetResourceSyncHandle);

                var vfxResoucesLoadSyncFucntionAddress = Service.SigSCanner.ScanText(SigConst.RescourseLoadSyncSig);
                VfxResoucesLoad = Marshal.GetDelegateForFunctionPointer<VfxResoucesLoadDelegate>(vfxResoucesLoadSyncFucntionAddress);

                var vfxResSettupCompleteFunctionAddress = Service.SigSCanner.ScanText(SigConst.VfxResSettupCompleteSig);
                VfxResSettupComplete = Marshal.GetDelegateForFunctionPointer<VfxResSettupCompleteDelegate>(vfxResSettupCompleteFunctionAddress);
            }
        }

        private static unsafe ResourceHandle* GetResourceSyncHandle(nint pFileManager, uint* pCategoryId, uint* pResourceType, uint* pResourceHash, byte* pPath, nint pUnknown)
        {
            ResouceManagerAddress = pFileManager;
            return GetResourceSyncHook.Original.Invoke(pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown);
        }

        public unsafe static void ResourceAdd(string path, byte[] data)
        {
            uint cateID = 0x08;
            uint type = 1635149432; 
            var crc32 = new System.IO.Hashing.Crc32();
            var bytes = Encoding.UTF8.GetBytes(path);
            crc32.Append(bytes);
            IntPtr ptr = Marshal.StringToHGlobalAnsi(path);
            uint omenID = BitConverter.ToUInt32(crc32.GetCurrentHash());
            var result = GetResourceSync.Invoke(ResouceManagerAddress, &cateID, &type, &omenID, (byte*)ptr, IntPtr.Zero);
            if (result != null)
            {
                Marshal.WriteByte((nint)result + 0xA8, 2);
                Marshal.WriteByte((nint)result + 0xA9, 7);

                var temp = IntPtr.Add((nint)result, 0xc0).ToPointer();
                Byte[] fileData = data;
                IntPtr fileptr = Marshal.AllocHGlobal(fileData.Length);
                Marshal.Copy(fileData, 0, fileptr, fileData.Length);
                var r = VfxManager.VfxResoucesLoad?.Invoke((nint*)Marshal.ReadIntPtr((nint)temp), (nint*)fileptr, Convert.ToUInt32(fileData.Length), result);
                VfxManager.VfxResSettupComplete?.Invoke(result);
            }
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
