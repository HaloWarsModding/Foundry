using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Chef.HW1.Unit;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Chef.Win.Render
{
    public class ModelSectionMesh : IDisposable
    {
        public Buffer Vertices { get; set; }
        public Buffer Indices { get; set; }
        public int NumIndices { get; set; }

        public void Dispose()
        {
            Vertices.Dispose();
            Indices.Dispose();
            NumIndices = 0;
        }
    }

    public static class ModelRenderer
    {
        static ModelRenderer()
        {
            string vs = @"
cbuffer CameraBuffer : register(b0)
{
    float4x4 v;
    float4x4 p;
};

cbuffer InstanceBuffer : register(b1)
{
    float4x4 ms[64];
};

struct VSInput
{
    uint instance : SV_InstanceID;
    float3 pos : POSITION;
    float3 normal : NORMAL;
    float2 uv0 : UV0;
    float2 uv1 : UV1;
    float2 uv2 : UV2;
};

struct VSOutput
{
    float4 position : SV_Position;
    float light : LIGHT;
    float2 uv0 : UV0;
    float2 uv1 : UV1;
    float2 uv2 : UV2;
};

VSOutput Main(VSInput input)
{
    VSOutput output = (VSOutput)0;

    float4x4 m = ms[input.instance];
	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);

    float3 normal = normalize(mul(input.normal, m));
	output.light = max(dot(normal, float3(0,1,0)), 0.0);

    output.position = mul(float4(input.pos, 1), mvp);

    output.uv0 = input.uv0;
    output.uv1 = input.uv1;
    output.uv2 = input.uv2;

    return output;
}";
            var vsC = SharpDX.D3DCompiler.ShaderBytecode.Compile(vs, "Main", "vs_5_0");
            ModelVS = new VertexShader(D3DViewport.Device, vsC);
            ModelVS.DebugName = "ModelVS";
            ModelIL = new InputLayout(D3DViewport.Device, vsC, [
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "POSITION", AlignedByteOffset = 0, Slot = 0 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "NORMAL", AlignedByteOffset = 12, Slot = 0 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32_Float, SemanticName = "UV", AlignedByteOffset = 24, Slot = 0, SemanticIndex = 0 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32_Float, SemanticName = "UV", AlignedByteOffset = 32, Slot = 0, SemanticIndex = 1 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32_Float, SemanticName = "UV", AlignedByteOffset = 40, Slot = 0, SemanticIndex = 2},
            ]);

            string ps = @"
Texture2D diffuse : register (t0);
SamplerState samplerState
{
	Filter = ANISOTROPIC;
	MaxAnisotropy = 4;

	AddressU = WRAP;
	AddressV = WRAP;
};

struct PSInput
{
    float4 position: SV_Position;
    float light : LIGHT;
    float2 uv0 : UV0;
    float2 uv1 : UV1;
    float2 uv2 : UV2;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

PSOutput Main(PSInput input)
{
    PSOutput output = (PSOutput)0;

    float3 sample = diffuse.Sample(samplerState, input.uv0);
    output.color = float4(sample * (input.light + .2), 1);

    return output;
}";
            var psC = SharpDX.D3DCompiler.ShaderBytecode.Compile(ps, "Main", "ps_5_0");
            ModelPS = new PixelShader(D3DViewport.Device, psC);
            ModelPS.DebugName = "ModelPS";

            CameraBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * 16 * 2, //2 4x4 matrices.
                Usage = ResourceUsage.Default
            });
            InstanceBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * 16 * 64, //64 4x4 matrices.
                Usage = ResourceUsage.Default
            });
        }

        public static int InstanceBucketSize { get; } = 64;
        public static void DrawMesh(RenderTargetView renderTarget, DepthStencilView depthStencil, ModelSectionMesh mesh, Camera camera, Matrix4x4[] transforms, ShaderResourceView diffuse)
        {
            //target
            D3DViewport.Device.ImmediateContext.OutputMerger.SetRenderTargets(depthStencil, renderTarget);
            D3DViewport.Device.ImmediateContext.Rasterizer.SetViewport(0, 0, camera.Width, camera.Height, 0, 1);
            RasterizerStateDescription s = new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = true,
            };
            D3DViewport.Device.ImmediateContext.Rasterizer.State = new RasterizerState(D3DViewport.Device, s);
            D3DViewport.Device.ImmediateContext.OutputMerger.DepthStencilState = new DepthStencilState(D3DViewport.Device, new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.All
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
            D3DViewport.Device.ImmediateContext.InputAssembler.InputLayout = ModelIL;
            D3DViewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            D3DViewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, [mesh.Vertices], [ModelVertex.cVertexSize], [0]); //position data buffer
            D3DViewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(mesh.Indices, SharpDX.DXGI.Format.R16_UInt, 0);

            //camera buffer
            D3DViewport.Device.ImmediateContext.UpdateSubresource([
                Matrix4x4.Transpose(camera.ViewMatrix),
                Matrix4x4.Transpose(camera.ProjectionMatrix)],
                CameraBuffer);

            //vs
            D3DViewport.Device.ImmediateContext.VertexShader.Set(ModelVS);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(0, CameraBuffer);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(1, InstanceBuffer);

            //ps
            D3DViewport.Device.ImmediateContext.PixelShader.Set(ModelPS);
            if (diffuse != null)
            {
                D3DViewport.Device.ImmediateContext.PixelShader.SetShaderResource(0, diffuse);
            }

            //draw
            int bucketCount = transforms.Length / InstanceBucketSize;
            if (transforms.Length % InstanceBucketSize != 0) bucketCount++;
            Matrix4x4[][] buckets = new Matrix4x4[bucketCount][];
            for (int i = 0; i < bucketCount; i++)
            {
                int first = i * InstanceBucketSize;
                int count = InstanceBucketSize;
                if (first + InstanceBucketSize > transforms.Length)
                {
                    count = transforms.Length % InstanceBucketSize;
                }
                buckets[i] = new Matrix4x4[InstanceBucketSize];

                for (int j = 0; j < count; j++)
                {
                    buckets[i][j] = Matrix4x4.Transpose(transforms[i * InstanceBucketSize + j]);
                }
                D3DViewport.Device.ImmediateContext.UpdateSubresource(buckets[i], InstanceBuffer);
                D3DViewport.Device.ImmediateContext.DrawIndexedInstanced(mesh.NumIndices, count, 0, 0, 0);
            }
        }
        public static ModelSectionMesh UploadSection(ModelSection section)
        {
            Buffer vbuffer = Buffer.Create(D3DViewport.Device, section.Vertices, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = ModelVertex.cVertexSize * section.Vertices.Length,
                StructureByteStride = ModelVertex.cVertexSize,
                Usage = ResourceUsage.Dynamic
            });
            Buffer ibuffer = Buffer.Create(D3DViewport.Device, section.Indices, new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 2 * section.Indices.Length,
                StructureByteStride = 2,
                Usage = ResourceUsage.Dynamic
            });

            return new ModelSectionMesh()
            {
                Vertices = vbuffer,
                Indices = ibuffer,
                NumIndices = section.Indices.Length
            };
        }

        private static InputLayout ModelIL { get; set; }
        private static VertexShader ModelVS { get; set; }
        private static PixelShader ModelPS { get; set; }
        private static Buffer CameraBuffer { get; set; }
        private static Buffer InstanceBuffer { get; set; }
    }
}
