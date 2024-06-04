using Foundry.HW1.Serialization;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D11.Device;

namespace Foundry.UI.WinForms
{
    /// <summary>
    /// Takes care of a lot of boilerplate code for setting up a winforms d3d11 swapchain and rtv.
    /// Also statically holds the global device (singleton? i hardly knew her).
    /// </summary>
    public class D3DViewport : UserControl
    {
        static D3DViewport()
        {
            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.Debug);
        }
        /// <summary>
        /// The device.
        /// </summary>
        public static Device Device { get; set; }

        public SwapChain1 SwapChain { get; private set; }
        private Texture2D TargetTex { get; set; }
        public RenderTargetView Target { get; private set; }
        private Texture2D DepthTex { get; set; }
        public DepthStencilView Depth { get;private set; }
        public D3DViewport()
        {
            var desc = new SwapChainDescription1()
            {
                BufferCount = 2,
                Flags = SwapChainFlags.None,
                Usage = Usage.RenderTargetOutput,
                AlphaMode = AlphaMode.Ignore,
                Format = Format.R8G8B8A8_UNorm,
                Width = Width,
                Height = Height,
                SwapEffect = SwapEffect.FlipDiscard,
                SampleDescription = new SampleDescription() { Count = 1, Quality = 0 },
                
            };
            SwapChain = new SwapChain1(new Factory2(), Device, Handle, ref desc);

            TargetTex = SwapChain.GetBackBuffer<Texture2D>(0);
            Target = new RenderTargetView(Device, TargetTex);

            var depthDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Width = 1,
                Height = 1,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription() { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            };
            DepthTex = new Texture2D(Device, depthDesc);
            Depth = new DepthStencilView(Device, DepthTex, new DepthStencilViewDescription()
            {
                Flags = DepthStencilViewFlags.None,
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = Format.D24_UNorm_S8_UInt
            });
        }

        public void ResizeBackBuffer(int width, int height)
        {
            Target.Dispose();
            TargetTex.Dispose();

            Depth.Dispose();
            DepthTex.Dispose();

            SwapChain.ResizeBuffers(0, width, height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);

            var depthDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                Format = Format.R32_Typeless,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Width = Width,
                Height = Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription() { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            };
            DepthTex = new Texture2D(Device, depthDesc);
            Depth = new DepthStencilView(Device, DepthTex, new DepthStencilViewDescription()
            {
                Flags = DepthStencilViewFlags.None,
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = Format.D32_Float,
            });

            TargetTex = SwapChain.GetBackBuffer<Texture2D>(0);
            Target = new RenderTargetView(Device, TargetTex);
        }
        public void Clear(Color color)
        {
            Device.ImmediateContext.ClearRenderTargetView(Target, new SharpDX.Mathematics.Interop.RawColor4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
            Device.ImmediateContext.ClearDepthStencilView(Depth, DepthStencilClearFlags.Depth, 1, 0);
        }
        public void Present()
        {
            SwapChain.Present(0, PresentFlags.None);
        }

    }
}
