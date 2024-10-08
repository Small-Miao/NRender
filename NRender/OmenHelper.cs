using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRender
{
    public static class OmenHelper
    {
        /// <summary>
        /// Make name to lock on path.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LockOn(this string str) => $"vfx/lockon/eff/{str}.avfx";

        /// <summary>
        /// Un lock on the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnLockOn(this string str) => str.Length > 20 ? str[15..^5] : string.Empty;

        /// <summary>
        /// Make name to omen path.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Omen(this string str) => $"vfx/omen/eff/{str}.avfx";

        /// <summary>
        /// Un omen the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnOmen(this string str) => str.Length > 18 ? str[13..^5] : string.Empty;

        /// <summary>
        /// channeling the str.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Channeling(this string str) => $"vfx/channeling/eff/{str}.avfx";

        /// <summary>
        /// Un channeling the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnChanneling(this string str) => str.Length > 18 ? str[19..^5] : string.Empty;
    }
}
