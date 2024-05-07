using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRender.Vfx
{
    public static class VfxHelper
    {
        public static byte[] MakeFan(byte[] avfxData,float radian)
        {
            float ring_fan_value = (float)((1 - Math.Cos(radian / 2)) / 2);
            byte[] ring_fan_bytes = BitConverter.GetBytes(ring_fan_value);

            // 计算 scroll1 的值
            float scroll1_value = 0.45333326f - 3.18309884f * radian;
            byte[] scroll1_bytes = BitConverter.GetBytes(scroll1_value);

            // 计算 scroll2 的值
            float scroll2_value = 5.40770276f + 14.22240645f * radian;
            byte[] scroll2_bytes = BitConverter.GetBytes(scroll2_value);

            // 创建一个字节数组来存储 _data 的副本
            byte[] _data = avfxData.ToArray();

            // 将计算得到的值更新到 _data 中的特定范围
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x17bc, ring_fan_bytes.Length);
            Buffer.BlockCopy(scroll1_bytes, 0, _data, 0x1a90, scroll1_bytes.Length);
            Buffer.BlockCopy(scroll1_bytes, 0, _data, 0x1c74, scroll1_bytes.Length);
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x2574, ring_fan_bytes.Length);
            Buffer.BlockCopy(scroll2_bytes, 0, _data, 0x2848, scroll2_bytes.Length);
            Buffer.BlockCopy(scroll2_bytes, 0, _data, 0x2a2c, scroll2_bytes.Length);
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x332c, ring_fan_bytes.Length);
            return _data;
        }

        public static void RegisterFanVfx(float radian,string path)
        {
            byte[] newFan = MakeFan(Properties.Resources.tmp_fan, radian);
            VfxManager.ResourceAdd(path, newFan);
        }

        public static void RegisterDountVfx(string path, float ignore_percent, float? fan_rad = null)
        {
            byte[] newDount = MakeDonut(Properties.Resources.tmp_donut,ignore_percent,fan_rad);
            VfxManager.ResourceAdd(path, newDount);
        }

        public static void RegisterCircleVfx(string path)
        {
            byte[] newCircle = Properties.Resources.tmp_circle;
            VfxManager.ResourceAdd(path, newCircle);
        }

        private static byte[] MakeDonut(byte[] temp, float ignore_percent, float? fan_rad = null)
        {
            float ring_fan_value = fan_rad is not null ? (float)((1 - Math.Cos(fan_rad.Value / 2)) / 2) : 1;
            byte[] ring_fan_bytes = BitConverter.GetBytes(ring_fan_value);

            float _x = 0.5f * (1 - ignore_percent) / (1 + ignore_percent);
            byte[] x_bytes = BitConverter.GetBytes(_x);

            float revised_value = 1 / (0.5f + _x);
            byte[] revised_bytes = BitConverter.GetBytes(revised_value);

            byte[] _data = new byte[temp.Length];
            temp.CopyTo(_data, 0);

            Buffer.BlockCopy(revised_bytes, 0, _data, 0x0184, revised_bytes.Length);
            Buffer.BlockCopy(revised_bytes, 0, _data, 0x019c, revised_bytes.Length);
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x179c, ring_fan_bytes.Length);
            Buffer.BlockCopy(x_bytes, 0, _data, 0x17c8, x_bytes.Length);
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x2244, ring_fan_bytes.Length);
            Buffer.BlockCopy(x_bytes, 0, _data, 0x2270, x_bytes.Length);
            Buffer.BlockCopy(ring_fan_bytes, 0, _data, 0x2cec, ring_fan_bytes.Length);
            Buffer.BlockCopy(x_bytes, 0, _data, 0x2d18, x_bytes.Length);

            return _data;
        }
    }
}
