using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRender
{
    public static class SigConst
    {
        internal const string StaticVfxCreateSig = "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08";//静态特效Sig
        private const string StaticVfxRunSig = "E8 ?? ?? ?? ?? 8B 4B 7C 85 C9";//特效运行Sig
        private const string StaticVfxRemoveSig = "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9";//静态特效移除Sig
        private const string ActorVfxCreateSig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";//角色特效创建Sig
        public const string ActorVfxRemoveSig = "0F 11 48 10 48 8D 05";//角色特效移除Sig
        public const string RescourseLoadSyncSig = "E8 ?? ?? ?? ?? 48 8B ?? ?? ?? 84 ?? 40 ?? ?? F6 B9";//资源加载(同步Sig)
        public const string SetOmenColorSig = "48 ?? ?? ?? ?? ?? ?? 48 ?? ?? 74 ?? 48 ?? ?? ?? f3 ?? ?? ?? ?? ?? f3 0f 11 89";//设置Omen颜色Sig
        public const string SetOmenMatrixSig = "e8 ?? ?? ?? ?? 48 ?? ?? e8 ?? ?? ?? ?? 85 ?? 74 ?? 84";//设置Omen矩阵Sig
        public const string CreateVfxSig = "e8 ?? ?? ?? ?? 48 ?? ?? 48 ?? ?? 74 ?? b2 ?? 48 ?? ?? e8 ?? ?? ?? ?? b2 ?? 48 ?? ?? e8 ?? ?? ?? ?? f6 05";//创建特效Sig
        public const string CreateOmenSig = "E8 ?? ?? ?? ?? 8D 4F ?? 48 63 D1 48 89 84 D3 ?? ?? ?? ??";//创建OmenSig
        public const string RemoveOmenSig = "e8 ?? ?? ?? ?? 4c ?? ?? 48 ?? ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? e8 ?? ?? ?? ?? eb";//删除OmenSig
        public const string SetVfxP1Sig = "e8 ?? ?? ?? ?? b2 ?? 48 ?? ?? e8 ?? ?? ?? ?? f6 05";//不知道设置什么Sig1
        public const string SetVfxP2Sig = "e8 ?? ?? ?? ?? f6 05 ?? ?? ?? ?? ?? 74 ?? 80 3d ?? ?? ?? ?? ?? 73";//不知道什么Sig2
        public const string InitVfxParam = "e8 ?? ?? ?? ?? f3 ?? ?? ?? ?? ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 48 ?? ?? ?? 48 ?? ?? ?? ?? c7 44 24";// 初始化特效参数Sig
    }
}
