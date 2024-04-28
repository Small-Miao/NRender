using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.UI.Info.InfoProxyCircle;

namespace NRender.Vfx
{

    public enum AVFX_STATUS
    {
        AVFX_NONE = -1,
        AVFX_WANT_KILL = 0,
        AVFX_CONTINUE = 1,
        AVFX_WANT_UPDATE = 2
    }

    public class OmenElement
    {
        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Vector4 Color { get; set; } = new Vector4(1f, 1f, 1f, 1f);
        public float Facing { get; set; } = 0;
        public uint OmenKey { get; set; } = 0;
        public AVFX_STATUS Status { get; set; } = AVFX_STATUS.AVFX_NONE;
        public GameObject? Owner { get; set; }
        internal nint? VfxHandle { get; set; }
        public OmenElement(string path, Vector3 scale, GameObject Owner, Vector4 color):
            this(path,scale,Owner.Position,color,Owner.Rotation){ 
            this.Owner = Owner;
        }
        public unsafe OmenElement(string path,Vector3 scale,Vector3 position,Vector4 color,float facing) {
            var crc32 = new Crc32();
            var bytes = Encoding.UTF8.GetBytes(path);
            crc32.Append(bytes);
            OmenKey = BitConverter.ToUInt32(crc32.GetCurrentHash());
            Scale = scale;
            Position = position;
            Color = color;
            Facing = facing;

            IntPtr paramPtr = Marshal.AllocHGlobal(0x1a0);
            var paramPoint = VfxManager.InitVfxParam(paramPtr);
            
            this.VfxHandle = VfxManager.CreateVfx?.Invoke(Marshal.StringToHGlobalAnsi(path), paramPoint,2,0,Position.X,Position.Y,Position.Z,Scale.X,Scale.Y,Scale.Z,facing,1,-1);
            if (VfxHandle != 0)
            {
                VfxManager.SetVfxP1?.Invoke((nint)VfxHandle, 1.ToString());
                VfxManager.SetVfxP2?.Invoke((nint)VfxHandle, 1.ToString());
            }

            VfxManager.SetOmenColor?.Invoke((nint)this.VfxHandle, Color.X,Color.Y,Color.Z,Color.W);
            VfxManager.drawOmenElementList.Add(this);
        }

        public void Dispose()
        {
            VfxManager.RemoveOmenHook.Original.Invoke((nint)this.VfxHandle, 1);
            VfxManager.drawOmenElementList.Remove(this);
        }
    }
}
