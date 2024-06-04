using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Foundry.HW1.Scenario;
using Foundry.Util;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Foundry.UI.WinForms
{
    public class TerrainVisualMesh
    {
        public Buffer Positions { get; set; }
        public Buffer Normals { get; set; }
        public Buffer Indices { get; set; }
        public int NumXVerts { get; set; }
        public int NumIndices { get; set; }
    }

    public static class TerrainRenderer
    {
        static TerrainRenderer()
        {
            string vs = @"
cbuffer CameraBuffer : register(b0)
{
    float4x4 m;
    float4x4 v;
    float4x4 p;
};

cbuffer TerrainBuffer : register(b1)
{
    float3 hitpos;
    int numXVerts;
};

//Texture2D<float3> vdata;
//SamplerState samplerState
//{
//	Filter = ANISOTROPIC;
//	MaxAnisotropy = 4;

//	AddressU = WRAP;
//	AddressV = WRAP;
//};

struct VSInput
{
    float3 pos : POSITION;
    float3 normal : NORMAL;
};

struct VSOutput
{
    float4 position: SV_Position;
    float hit : HITPOS;
    float3 normal: NORMAL;
};

VSOutput Main(VSInput input)
{
    VSOutput output = (VSOutput)0;

	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);
    output.position = mul(float4(input.pos, 1), mvp);
    
    output.hit = min(distance(input.pos, hitpos), 100) / 100;

	output.normal = normalize(mul(input.normal, m));
    return output;
}";
            var vsC = SharpDX.D3DCompiler.ShaderBytecode.Compile(vs, "Main", "vs_5_0");
            TerrainVS = new VertexShader(D3DViewport.Device, vsC);
            TerrainIL = new InputLayout(D3DViewport.Device, vsC, [
                new InputElement() {Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "POSITION", AlignedByteOffset = 0, Slot = 0 },
                new InputElement() { Format = SharpDX.DXGI.Format.R32G32B32_Float, SemanticName = "NORMAL", AlignedByteOffset = 0, Slot = 1 }
            ]);

            string ps = @"
struct PSInput
{
    float4 position: SV_Position;
    float hit : HITPOS;
    float3 normal: NORMAL;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

PSOutput Main(PSInput input)
{
    PSOutput output = (PSOutput)0;

	float3 lightDir = normalize(float3(0,10,0) - input.position.xyz);
	float diff = max(dot(input.normal, float3(0,1,0)), 0.0);

    output.color = float4(float3(1, 1, 1) * diff * input.hit, 1.0);

    return output;
}";
            var psC = SharpDX.D3DCompiler.ShaderBytecode.Compile(ps, "Main", "ps_5_0");
            TerrainPS = new PixelShader(D3DViewport.Device, psC);

            CameraBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * 16 * 3, //3 4x4 matrices.
                Usage = ResourceUsage.Default
            });

            TerrainBuffer = new Buffer(D3DViewport.Device, new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 16,
                Usage = ResourceUsage.Default
            });
        }

        public static void DrawVisualMesh(RenderTargetView renderTarget, DepthStencilView depthStencil, TerrainVisualMesh mesh, Camera camera, Vector3 TEMP_hitpos)
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

            //ia
            D3DViewport.Device.ImmediateContext.InputAssembler.InputLayout = TerrainIL;
            D3DViewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            D3DViewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, [mesh.Positions], [12], [0]); //position data buffer
            D3DViewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(1, [mesh.Normals], [12], [0]); //normal data buffer
            D3DViewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(mesh.Indices, SharpDX.DXGI.Format.R32_UInt, 0);

            //camera buffer
            D3DViewport.Device.ImmediateContext.UpdateSubresource([Matrix4x4.Identity, camera.ViewMatrix, camera.ProjectionMatrix]/*m, v, p*/, CameraBuffer);

            //terrain buffer
            D3DViewport.Device.ImmediateContext.UpdateSubresource([TEMP_hitpos], TerrainBuffer);
            //D3DViewport.Device.ImmediateContext.UpdateSubresource([mesh.NumXVerts], TerrainBuffer, 0, 12);

            //vs
            D3DViewport.Device.ImmediateContext.VertexShader.Set(TerrainVS);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(0, CameraBuffer);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(1, TerrainBuffer);

            //ps
            D3DViewport.Device.ImmediateContext.PixelShader.Set(TerrainPS);

            //draw
            D3DViewport.Device.ImmediateContext.DrawIndexed(mesh.NumIndices, 0, 0);
        }
        public static TerrainVisualMesh UploadVisualMesh(TerrainVisual vis)
        {
            Buffer vertBuffer = Buffer.Create(D3DViewport.Device, vis.Positions, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 12 * vis.Positions.Length,
                StructureByteStride = 12,
                Usage = ResourceUsage.Dynamic
            });
            Buffer normalBuffer = Buffer.Create(D3DViewport.Device, vis.Normals, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 12 * vis.Normals.Length,
                StructureByteStride = 12,
                Usage = ResourceUsage.Dynamic
            });

            //indices
            List<uint> indices = new List<uint>();
            for (uint i = 0; i < vis.Width - 1; i++)
            {
                for (uint j = 0; j < vis.Width - 1; j++)
                {
                    uint row0 = i * (uint)vis.Width;
                    uint row1 = (i + 1) * (uint)vis.Width;

                    indices.Add(row0 + j);
                    indices.Add(row0 + j + 1);
                    indices.Add(row1 + j);
                    indices.Add(row1 + j + 1);
                    indices.Add(row1 + j);
                    indices.Add(row0 + j + 1);
                }
            }
            Buffer indBuffer = Buffer.Create(D3DViewport.Device, indices.ToArray(), new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * indices.Count,
                Usage = ResourceUsage.Dynamic
            });
            return new TerrainVisualMesh()
            {
                NumXVerts = vis.Width,
                NumIndices = indices.Count,
                Indices = indBuffer,
                Positions = vertBuffer,
                Normals = normalBuffer
            };
        }

        private static InputLayout TerrainIL { get; set; }
        private static VertexShader TerrainVS { get; set; }
        private static PixelShader TerrainPS { get; set; }
        private static Buffer CameraBuffer { get; set; }
        private static Buffer TerrainBuffer { get; set; }
    }
}
