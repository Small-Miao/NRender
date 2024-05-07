using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRender.Vfx
{
    public class ActorVfx
    {
        public readonly nint _handle;
        public bool isDispose = false;

        public DateTime DeadTime = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public ActorVfx(string path, GameObject caster, GameObject target)
            : this(path, caster.Address, target.Address) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <param name="shouldRemove">Should this vfx be removed manually?</param>
        /// <param name="path"></param>
        public ActorVfx(string path, nint caster, nint target, bool? shouldRemove = null)
        {
            _handle = VfxManager.ActorVfxCreate?.Invoke(path, caster, target, -1, (char)0, 0, (char)0) ?? nint.Zero;

            lock (VfxManager.drawActorVfxList){ 
                VfxManager.drawActorVfxList.Add(this);
            }
            Service.Framework.Update += Framework_Update;
        }

        private void Framework_Update(IFramework framework)
        {
            if (DeadTime != DateTime.MinValue && DeadTime < DateTime.Now)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (!isDispose)
            {
                VfxManager.ActorVfxRemove?.Invoke(_handle, (char)1);
                isDispose =  true;
            }
            lock(VfxManager.drawActorVfxList)
            {
                VfxManager.drawActorVfxList.Remove(this);
            }
            Service.Framework.Update -= Framework_Update;
        }
    }
}
