using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Chef.HW1.Map;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Chef.Win.Render
{
    public class TerrainVisualMesh : IDisposable
    {
        public Buffer Coords { get; set; }
        public Buffer Indices { get; set; }
        public ShaderResourceView Positions { get; set; }
        public ShaderResourceView Normals { get; set; }
        public ShaderResourceView Alphas { get; set; }
        public ShaderResourceView Splatting { get; set; }
        public int NumXVerts { get; set; }
        public int NumIndices { get; set; }

        public void Dispose()
        {
            Positions.Dispose();
            Normals.Dispose();
            Indices.Dispose();
            NumXVerts = 0;
            NumIndices = 0;
        }
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
    uint numXVerts;
    uint numActiveTextures;
};

Texture2D<float3> positions : register (t0);
Texture2D<float3> normals : register (t1);
Texture2D<float> alphas : register (t2);
SamplerState samplerVert
{
	Filter = LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct VSInput
{
    uint index : SV_VertexID;
};

struct VSOutput
{
    float4 position: SV_Position;
    float3 normal: NORMAL;
    float alpha : ALPHA;
    float2 worldUV : UV0;
    float2 chunkUV : UV1;
};

VSOutput Main(VSInput input)
{
    VSOutput output = (VSOutput)0;

	float4x4 mv = mul(m, v);
	float4x4 mvp = mul(mv, p);

    //vertex coords//
    int vx = input.index / numXVerts;
    int vy = input.index % numXVerts;

    int3 uvVert = int3(vx, vy, 0);

    float3 position = positions.Load(uvVert);
    float3 normal = normals.Load(uvVert);
    float alpha = alphas.Load(uvVert);

    //chunk start coords//
    int cx = vx / 64;
    int cy = vy / 64;
    //chunk vertex coords//
    int cvx = vx % 64;
    int cvy = vy % 64;

    float2 uvTex = float2(cvx / 64.0, cvy / 64.0);

    output.position = mul(float4(position, 1), mvp);
	output.normal = normalize(mul(normal, m));
    output.alpha = alpha;
    output.chunkUV = uvTex;
    output.worldUV = float2(uvVert.x / (float)numXVerts, uvVert.y / (float)numXVerts);

    return output;
}";
            var vsC = SharpDX.D3DCompiler.ShaderBytecode.Compile(vs, "Main", "vs_5_0");
            TerrainVS = new VertexShader(D3DViewport.Device, vsC);
            TerrainVS.DebugName = "TerrainVS";
            //TerrainIL = new InputLayout(D3DViewport.Device, vsC, []);

            string ps = @"
cbuffer TerrainBuffer : register(b1)
{
    uint numXVerts;
    uint numActiveTextures;
};

struct PSInput
{
    float4 position: SV_Position;
    float3 normal: NORMAL;
    float alpha : ALPHA;
    float2 worldUV : UV0;
    float2 chunkUV : UV1;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

Texture2DArray diffuses : register (t0);
Texture2DArray splatting : register (t1);
SamplerState samplerUV
{
	Filter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
	AddressW = WRAP;
};

float3 GetSplatColor(PSInput input)
{
    float3 final = float3(0, 0, 0);
    for (int i = 0; i < numActiveTextures; i++)
    {
        float3 diffuse = diffuses.Sample(samplerUV, float3(input.chunkUV, i)).xyz;
        float alpha = splatting.Sample(samplerUV, float3(input.worldUV, i));

        final = lerp(final, diffuse, alpha);
    }
    return final;
}

PSOutput Main(PSInput input)
{
    PSOutput output = (PSOutput)0;

	float diff = max(dot(input.normal, float3(0,1,0)), 0.0);

    output.color = float4(GetSplatColor(input) * diff, input.alpha);

    return output;
}";
            var psC = SharpDX.D3DCompiler.ShaderBytecode.Compile(ps, "Main", "ps_5_0");
            TerrainPS = new PixelShader(D3DViewport.Device, psC);
            TerrainPS.DebugName = "TerrainPS";

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

        public static void DrawVisualMesh(RenderTargetView renderTarget, DepthStencilView depthStencil, TerrainVisualMesh mesh, Camera camera, ShaderResourceView diffuseArray)
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
            D3DViewport.Device.ImmediateContext.InputAssembler.InputLayout = null;
            D3DViewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            D3DViewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(mesh.Indices, SharpDX.DXGI.Format.R32_UInt, 0);

            //camera buffer
            D3DViewport.Device.ImmediateContext.UpdateSubresource([
                Matrix4x4.Transpose(Matrix4x4.Identity),
                Matrix4x4.Transpose(camera.ViewMatrix),
                Matrix4x4.Transpose(camera.ProjectionMatrix)
                ],
                CameraBuffer);

            //terrain buffer
            var diffuseResource = diffuseArray.ResourceAs<Texture2D>();
            D3DViewport.Device.ImmediateContext.UpdateSubresource([(uint)mesh.NumXVerts, (uint)diffuseResource.Description.ArraySize], TerrainBuffer);
            diffuseResource.Dispose();

            //vs
            D3DViewport.Device.ImmediateContext.VertexShader.Set(TerrainVS);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(0, CameraBuffer);
            D3DViewport.Device.ImmediateContext.VertexShader.SetConstantBuffer(1, TerrainBuffer);
            D3DViewport.Device.ImmediateContext.VertexShader.SetShaderResource(0, mesh.Positions);
            D3DViewport.Device.ImmediateContext.VertexShader.SetShaderResource(1, mesh.Normals);
            D3DViewport.Device.ImmediateContext.VertexShader.SetShaderResource(2, mesh.Alphas);

            //ps
            D3DViewport.Device.ImmediateContext.PixelShader.Set(TerrainPS);
            D3DViewport.Device.ImmediateContext.PixelShader.SetConstantBuffer(1, TerrainBuffer);
            D3DViewport.Device.ImmediateContext.PixelShader.SetShaderResource(0, diffuseArray);
            D3DViewport.Device.ImmediateContext.PixelShader.SetShaderResource(1, mesh.Splatting);

            //draw
            D3DViewport.Device.ImmediateContext.DrawIndexed(mesh.NumIndices, 0, 0);
        }
        public static TerrainVisualMesh UploadVisualMesh(TerrainVisual vis)
        {
            Texture2D positionsTex = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R32G32B32_Float,
                Width = vis.Width,
                Height = vis.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });
            positionsTex.DebugName = "Positions";
            D3DViewport.Device.ImmediateContext.UpdateSubresource(vis.Positions, positionsTex, 0, 12 * vis.Width);

            Texture2D normalsTex = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R32G32B32_Float,
                Width = vis.Width,
                Height = vis.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });
            normalsTex.DebugName = "Normals";
            D3DViewport.Device.ImmediateContext.UpdateSubresource(vis.Normals, normalsTex, 0, 12 * vis.Width);

            Texture2D alphasTex = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R32_Float,
                Width = vis.Width,
                Height = vis.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });
            alphasTex.DebugName = "Alphas";
            D3DViewport.Device.ImmediateContext.UpdateSubresource(vis.Alphas, alphasTex, 0, 4 * vis.Width);

            Texture2D splatTex = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R32_Float,
                Width = vis.Width,
                Height = vis.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                ArraySize = TerrainVisual.cMaxTextureLayers,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
            });
            splatTex.DebugName = "Splatting";
            for (int i = 0; i < TerrainVisual.cMaxTextureLayers; i++)
            {
                long wpitch, spitch;
                DirectXTexNet.TexHelper.Instance.ComputePitch((DirectXTexNet.DXGI_FORMAT)splatTex.Description.Format, vis.Width, vis.Width, out wpitch, out spitch, DirectXTexNet.CP_FLAGS.NONE);

                D3DViewport.Device1.ImmediateContext.UpdateSubresource(vis.TextureAlphas[i], splatTex, i, (int)wpitch, (int)spitch);
            }

            //indices
            List<uint> indices = new List<uint>();
            for (uint j = 0; j < vis.Width - 1; j++)
            {
                for (uint i = 0; i < vis.Width - 1; i++)
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
                Positions = new ShaderResourceView(D3DViewport.Device, positionsTex),
                Normals = new ShaderResourceView(D3DViewport.Device, normalsTex),
                Alphas = new ShaderResourceView(D3DViewport.Device, alphasTex),
                Splatting = new ShaderResourceView(D3DViewport.Device, splatTex),
            };
        }
        public static void UpdateSplattedTexture(TerrainVisual terrain)
        {
            Texture2D albedo = new Texture2D(D3DViewport.Device, new Texture2DDescription()
            {
                CpuAccessFlags = CpuAccessFlags.Write,
                BindFlags = BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.R32_Float,
                Width = terrain.Width,
                Height = terrain.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Dynamic,
            });


            for(int i = 0; i < TerrainVisual.cMaxTextureLayers; i++)
            {
                //Texture2D splat = new Texture2D(D3DViewport.Device, )
            }
        }

        private static InputLayout TerrainIL { get; set; }
        private static VertexShader TerrainVS { get; set; }
        private static PixelShader TerrainPS { get; set; }
        private static Buffer CameraBuffer { get; set; }
        private static Buffer TerrainBuffer { get; set; }
    }
}
