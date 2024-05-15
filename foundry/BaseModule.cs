using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
	public abstract class BaseModule
	{
        /// <summary>
        /// Reference of the running FoundryInstance that this module is loaded into.
        /// </summary>
        public FoundryInstance Instance { get; private set; }



        public void Init(FoundryInstance i)
        {
            Instance = i;
            OnInit();
        }
        public void PostInit()
        {
            OnPostInit();
        }
        public void WorkspaceOpened()
        {
            OnWorkspaceOpened();
        }
        public void WorkspaceClosed()
        {
            OnWorkspaceClosed();
        }
        public void UpdateModule()
        {
            UpdateModuleArgs args = new UpdateModuleArgs()
            {
            };
            ModuleUpdated?.Invoke(this, args);
        }
        public class UpdateModuleArgs
        {
        }
        public event EventHandler<UpdateModuleArgs> ModuleUpdated;

        /// <summary>
        /// Occurs when the module is loaded.
        /// Go nuts.
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// Occurs after all modules have been loaded, only if this module is valid.
        /// Go nuts.
        /// </summary>
        protected virtual void OnPostInit() { }
        protected virtual void OnWorkspaceOpened() { }
        protected virtual void OnWorkspaceClosed() { }
	}
}
