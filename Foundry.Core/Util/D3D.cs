//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SharpDX.Direct3D11;

//namespace Foundry.Util
//{
//    /// <summary>
//    /// High-ish level d3d render context.
//    /// </summary>
//    public class D3DContext
//    {
//        public D3DContext()
//        {
//            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.None);
//            NextId = 0;
//        }

//        public ulong CreateMesh()
//        {
//            //Resource.
//        }

//        private static Device Device { get; set; }
//        public ulong NextId { get; set; }
//    }

//    public class Mesh
//    {

//        public Mesh(Device context)
//        {

//        }

//        public Resource VB { get; set; }
//        public Resource IB { get; set; }
//    }
//}
