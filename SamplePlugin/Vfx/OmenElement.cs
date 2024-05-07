using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Animation;
using OpenTK.Mathematics;
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
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

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
        /// <summary>
        /// Oemn缩放
        /// </summary>
        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Omen位置
        /// </summary>
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        /// <summary>
        /// Omen初始颜色
        /// </summary>
        public Vector4 Color { get; set; } = new Vector4(1f, 1f, 1f, 1f);
        /// <summary>
        /// Omen目标颜色
        /// </summary>
        public Vector4 TargetColor { get; set; } = new Vector4(1f, 1f, 1f, 1f);
        /// <summary>
        /// Omen旋转面向
        /// </summary>
        public float Facing { get; set; } = 0;
        /// <summary>
        /// OmenKey (CRC32)计算路径
        /// </summary>
        public uint OmenKey { get; set; } = 0;
        /// <summary>
        /// Omen 状态 暂时无用
        /// </summary>
        public AVFX_STATUS Status { get; set; } = AVFX_STATUS.AVFX_NONE;
        /// <summary>
        /// Omen 所属对象(绑定)
        /// </summary>
        public GameObject? Owner { get; set; }
        /// <summary>
        /// Vfx句柄
        /// </summary>
        internal nint? VfxHandle { get; set; }
        /// <summary>
        /// Omen 旋转角度
        /// </summary>
        public float Rotation { get; set; } = 0;
        /// <summary>
        /// Omen销毁时间
        /// </summary>
        public long DestoryAt { get; set; } = 3000;
        /// <summary>
        /// Omen偏移坐标
        /// </summary>
        public Vector3 Offset { get; set; } = new Vector3(0, 0, 0);
        /// <summary>
        /// 初始化Omen
        /// </summary>
        /// <param name="path">OmenPath</param>
        /// <param name="scale">缩放</param>
        /// <param name="Owner">绑定GameObejct</param>
        /// <param name="color">Omen颜色</param>
        public OmenElement(string path, System.Numerics.Vector3 scale, GameObject Owner, Vector4 color):
            this(path,scale,Owner.Position,color,Owner.Rotation){ 
            this.Owner = Owner;
        }
        
        private long runningTime = 0;
        private long startTime = 0;
        private Vector4 CurrentColor { get; set; }

        /// <summary>
        /// 初始化Omen 无绑定
        /// </summary>
        /// <param name="path">Omen路径</param>
        /// <param name="scale">缩放</param>
        /// <param name="position">坐标</param>
        /// <param name="color">颜色</param>
        /// <param name="facing">面向</param>
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
            lock (VfxManager.drawOmenElementList)
            {
                VfxManager.drawOmenElementList.Add(this);
            }
            startTime = Environment.TickCount64;
            Service.Framework.Update += Framework_Update;
        }

        private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
        {
            runningTime = Environment.TickCount64 - startTime;
            UpdatePosition();
            UpdateColor();
        }


        private void UpdatePosition()
        {
            if (Owner)
            {
                var baseRotation = Owner.Rotation;
                baseRotation += Rotation;
                var rotatedOffset = RotateVector(Offset, baseRotation);
                var finalPosition = Owner.Position - rotatedOffset;

                Matrix4 translateMatrix = Matrix4.CreateTranslation(finalPosition.X, finalPosition.Y, finalPosition.Z);
                Matrix4 rotateMatrix = Matrix4.CreateFromAxisAngle(new OpenTK.Mathematics.Vector3(0, 1, 0), baseRotation);
                Matrix4 scaleMatrix = Matrix4.CreateScale(Scale.X, Scale.Y, Scale.Z);
                Matrix4 finalMatrix = scaleMatrix * rotateMatrix * translateMatrix;

                IntPtr matrixPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Matrix4)));
                Marshal.StructureToPtr(finalMatrix, matrixPtr, false);

                VfxManager.SetOmenMatrix?.Invoke((nint)this.VfxHandle, matrixPtr);
            }
        }

        private Vector3 RotateVector(Vector3 vector, float rotation)
        {
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float newX = vector.X * cos + vector.Z * sin; // 注意这里的正负号变化
            float newZ = -vector.X * sin + vector.Z * cos; // 注意这里的正负号变化

            return new Vector3(newX, vector.Y, newZ);
        }
        private void UpdateColor()
        {
            // 计算剩余时间
            long remainingTime = DestoryAt - runningTime;

            // 如果剩余时间小于 1000 毫秒，执行颜色插值
            if (remainingTime < 100)
            {
                // 计算插值比例
                float interpolation = (float)remainingTime / 100f;

                interpolation = 1 - interpolation;
                // 初始颜色
                Vector4 initialColor = CurrentColor;

                // 目标颜色，将 alpha 通道设置为 0
                Vector4 targetColor = new Vector4(initialColor.X, initialColor.Y, initialColor.Z, 0);
                Vector4 interpolatedColor = Vector4.Lerp(initialColor, targetColor, interpolation);

                // 更新颜色
                VfxManager.SetOmenColor?.Invoke((nint)this.VfxHandle, interpolatedColor.X, interpolatedColor.Y, interpolatedColor.Z, interpolatedColor.W);
            }
            // 如果剩余时间大于等于 1000 毫秒，执行另一种颜色插值
            else
            {
                // 计算插值比例
                float interpolation = 1f - (float)remainingTime / (DestoryAt - 100f);

                // 初始颜色
                Vector4 initialColor = Color;

                // 目标颜色
                Vector4 targetColor = TargetColor;
                // 插值颜色
                Vector4 interpolatedColor = Vector4.Lerp(initialColor, targetColor, interpolation);
                CurrentColor = interpolatedColor;
                // 更新颜色 
                VfxManager.SetOmenColor?.Invoke((nint)this.VfxHandle, interpolatedColor.X, interpolatedColor.Y, interpolatedColor.Z, interpolatedColor.W);
            }

            if (remainingTime < 0)
            {
                this.Dispose();
            }
        }
        /// <summary>
        /// 销毁Omen
        /// </summary>
        public void Dispose()
        {
            Service.Framework.Update -= Framework_Update;
            VfxManager.RemoveOmenHook.Original.Invoke((nint)this.VfxHandle, 1);
            lock (VfxManager.drawOmenElementList)
            {
                VfxManager.drawOmenElementList.Remove(this);
            }
        }
    }
}
