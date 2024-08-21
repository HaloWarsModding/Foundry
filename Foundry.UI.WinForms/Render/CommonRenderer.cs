using Chef.HW1.Map;
using Chef.HW1.Unit;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Chef.Win.Render
{
    public static class CommonRenderer
    {
        static CommonRenderer()
        {
            string vs = @"
cbuffer CameraBuffer : register(b0)
{
    float4x4 m;
    float4x4 v;
    float4x4 p;
};

struct VSInput
{
    float3 position : POSITION;
    float3 color : COLOR;
};

struct VSOutput
{
    float4 position: SV_Position;
    float3 color : COLOR;
};

VSOutput Main(VSInput input)
{
    VSOutput output = (VSOutput)0;

	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);
    output.position = mul(float4(input.position, 1), mvp);

    output.color = input.color;

    return output;
}";
            var vsC = SharpDX.D3DCompiler.ShaderBytecode.Compile(vs, "Main", "vs_5_0");
            LineVS = new VertexShader(D3DViewport.Device, vsC);
            LineVS.DebugName = "LineVS";
            //TerrainIL = new InputLayout(D3DViewport.Device, vsC, []);

            string ps = @"
struct PSInput
{
    float4 position: SV_Position;
    float3 color : COLOR;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

PSOutput Main(PSInput input)
{
    PSOutput output = (PSOutput)0;
    
    output.color = float4(input.color, 1);

    return output;
}";
            var psC = SharpDX.D3DCompiler.ShaderBytecode.Compile(ps, "Main", "ps_5_0");
            LinePS = new PixelShader(D3DViewport.Device, psC);
            LinePS.DebugName = "LinePS";

            CameraBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * 16 * 3, //3 4x4 matrices.
                Usage = ResourceUsage.Default
            });

            LineIL = new InputLayout(D3DViewport.Device, vsC, [
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "POSITION", AlignedByteOffset = 0, Slot = 0 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "COLOR", AlignedByteOffset = 12, Slot = 0 },
            ]);

            LineBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 48, //12 floats
                Usage = ResourceUsage.Default
            });
        }

        public static Texture2D UploadTexture(DirectXTexNet.Image image)
        {
            Texture2D texture = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = (SharpDX.DXGI.Format)image.Format,
                Width = image.Width,
                Height = image.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            });
            D3DViewport.Device.ImmediateContext.UpdateSubresource(new SharpDX.DataBox(image.Pixels, (int)image.RowPitch, (int)image.SlicePitch), texture, 0);
            return texture;
        }
        public static Texture2D UploadTerrainTextures(DirectXTexNet.Image[] images, bool stretchX, bool stretchY)
        {
            if (images.Length == 0) return null;

            SharpDX.DXGI.Format format = (SharpDX.DXGI.Format)images[0].Format;
            int widthMax = 0;
            int heightMax = 0;

            foreach (var image in images)
            {
                if ((SharpDX.DXGI.Format)image.Format != format)
                    return null;

                widthMax = Math.Max(widthMax, image.Width);
                heightMax = Math.Max(heightMax, image.Height);
            }

            Texture2D texture = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                ArraySize = images.Length,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = format,
                Width = widthMax,
                Height = heightMax,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            });

            var atlas = DirectXTexNet.TexHelper.Instance.InitializeTemporary(images, new DirectXTexNet.TexMetadata(widthMax, 512, 1, images.Length, 1, 0, 0, DirectXTexNet.DXGI_FORMAT.R8G8B8A8_UNORM, DirectXTexNet.TEX_DIMENSION.TEXTURE2D));
            //int i = 0;
            //var atlas = DirectXTexNet.TexHelper.Instance.Initialize2D(DirectXTexNet.DXGI_FORMAT.R8G8B8A8_UNORM, 1024, 1024, images.Length, 1, DirectXTexNet.CP_FLAGS.NONE);
            //foreach (var image in images)
            //{
            //    DirectXTexNet.TexHelper.Instance.CopyRectangle(image, 0, 0, image.Width, image.Height, atlas.GetImage(i), DirectXTexNet.TEX_FILTER_FLAGS.DEFAULT, 0, 0);


            //    D3DViewport.Device.ImmediateContext.UpdateSubresource(
            //        new SharpDX.DataBox(atlas.GetImage(i).Pixels, (int)atlas.GetImage(i).RowPitch, (int)atlas.GetImage(i).SlicePitch),
            //        texture,
            //        i);

            //    i++;
            //}

            for (int i = 0; i < images.Length; i++)
            {
                D3DViewport.Device.ImmediateContext.UpdateSubresource(
                    new SharpDX.DataBox(
                        atlas.GetImage(i).Pixels,
                        (int)atlas.GetImage(i).RowPitch,
                        (int)atlas.GetImage(i).SlicePitch),
                    texture,
                    i);
            }

            return texture;
        }
        public static void DrawLine(RenderTargetView renderTarget, DepthStencilView depthStencil, Camera camera, Vector3 start, Vector3 end, Color color, Matrix4x4 transform)
        {
            //upload vertex buffer
            Vector3 colorVec = new Vector3(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            D3DViewport.Device.ImmediateContext.UpdateSubresource([start, colorVec, end, colorVec], LineBuffer);

            //target
            D3DViewport.Device.ImmediateContext.OutputMerger.SetRenderTargets(depthStencil, renderTarget);
            D3DViewport.Device.ImmediateContext.Rasterizer.SetViewport(0, 0, camera.Width, camera.Height, 0, 1);
            RasterizerStateDescription s = new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true,
            };
            D3DViewport.Device.ImmediateContext.Rasterizer.State = new RasterizerState(D3DViewport.Device, s);
            D3DViewport.Device.ImmediateContext.OutputMerger.DepthStencilState = new DepthStencilState(D3DViewport.Device, new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.All,
            });

            //blending
            var blendDesc = new BlendStateDescription()
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false,
            };
            blendDesc.RenderTarget[0].IsBlendEnabled = true;
            blendDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            blendDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            D3DViewport.Device.ImmediateContext.OutputMerger.BlendState = new BlendState(D3DViewport.Device, blendDesc);

            //ia
            D3DViewport.Device.ImmediateContext.InputAssembler.InputLayout = LineIL;
            D3DViewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;
            D3DViewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, [LineBuffer], [24], [0]);

            //camera buffer
            D3DViewport.Device.ImmediateContext.UpdateSubresource([
                Matrix4x4.Transpose(transform),
                Matrix4x4.Transpose(camera.ViewMatrix),
                Matrix4x4.Transpose(camera.ProjectionMatrix)
                ],
                CameraBuffer);

            //vs
            D3DViewport.Device.ImmediateContext.VertexShader.Set(LineVS);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(0, CameraBuffer);

            //ps
            D3DViewport.Device.ImmediateContext.PixelShader.Set(LinePS);

            D3DViewport.Device.ImmediateContext.Draw(2, 0);
        }
        public static void DrawCube(RenderTargetView renderTarget, DepthStencilView depthStencil, Camera camera, Vector3 min, Vector3 max, Color color, Matrix4x4 transform)
        {
            DrawLine(renderTarget, depthStencil, camera, min, new Vector3(max.X, min.Y, min.Z), color, transform);
            DrawLine(renderTarget, depthStencil, camera, min, new Vector3(min.X, max.Y, min.Z), color, transform);
            DrawLine(renderTarget, depthStencil, camera, min, new Vector3(min.X, min.Y, max.Z), color, transform);
            
            DrawLine(renderTarget, depthStencil, camera, new Vector3(min.X, max.Y, min.Z), new Vector3(min.X, max.Y, max.Z), color, transform);
            DrawLine(renderTarget, depthStencil, camera, new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color, transform);

            DrawLine(renderTarget, depthStencil, camera, new Vector3(min.X, max.Y, max.Z), max, color, transform);
            DrawLine(renderTarget, depthStencil, camera, new Vector3(max.X, min.Y, max.Z), max, color, transform);
            DrawLine(renderTarget, depthStencil, camera, new Vector3(max.X, max.Y, min.Z), max, color, transform);

            DrawLine(renderTarget, depthStencil, camera, new Vector3(max.X, min.Y, max.Z), new Vector3(min.X, min.Y, max.Z), color, transform);
            DrawLine(renderTarget, depthStencil, camera, new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, min.Y, min.Z), color, transform);

            DrawLine(renderTarget, depthStencil, camera, new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color, transform);
            DrawLine(renderTarget, depthStencil, camera, new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color, transform);

        }
        public static void DrawCircle(RenderTargetView renderTarget, DepthStencilView depthStencil, Camera camera, Color color, Matrix4x4 transform)
        {
            const int segments = 32;
            Vector3[] vertices = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                Vector3 cur = new Vector3();

                float theta = 2.0f * (float)Math.PI * i / segments;

                cur.X = (float)Math.Cos(theta) * 1.0f;
                cur.Y = (float)Math.Sin(theta) * 1.0f;

                vertices[i] = cur;
            }

            for(int i = 0; i < segments - 1; i++)
            {
                DrawLine(renderTarget, depthStencil, camera, vertices[i], vertices[i + 1], color, transform);
            }
            DrawLine(renderTarget, depthStencil, camera, vertices[segments - 1], vertices[0], color, transform);
        }

        private static InputLayout LineIL { get; set; }
        private static VertexShader LineVS { get; set; }
        private static PixelShader LinePS { get; set; }
        private static Buffer LineBuffer { get; set; }
        private static Buffer CameraBuffer { get; set; }
    }
}
