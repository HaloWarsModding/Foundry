using Chef.HW1.Map;
using Chef.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.IO
{
    public static class Xtd
    {
        public static void ReadXtd(Stream stream, TerrainVisual terrain)
        {
            Ecf ecf = new Ecf();
            ecf.Read(stream);

            EcfChunk cheader = null;
            List<EcfChunk> cterrains = new List<EcfChunk>();
            EcfChunk catlas = null;
            EcfChunk cao = null;
            EcfChunk calpha = null;
            EcfChunk ctess = null;
            EcfChunk clighting = null;

            foreach (EcfChunk chunk in ecf.Chunks)
            {
                if (chunk.ID == 0x1111) cheader = chunk;
                if (chunk.ID == 0x2222) cterrains.Add(chunk);
                if (chunk.ID == 0x8888) catlas = chunk;
                if (chunk.ID == 0xCCCC) cao = chunk;
                if (chunk.ID == 0xDDDD) calpha = chunk;
                if (chunk.ID == 0xAAAA) ctess = chunk;
                if (chunk.ID == 0xBBBB) clighting = chunk; //optional? scn15 does not have this.
            }

            if (cheader == null ||
                catlas == null ||
                cao == null ||
                calpha == null ||
                ctess == null) return;

            //int width;
            //ReadHeader(new MemoryStream(cheader.Data), out width);
            //ReadAtlas(new MemoryStream(catlas.Data), terrain);
            ReadAlpha(calpha.Data, terrain);
            //we dont actually care about this -- it is generated on export.
            //ReadAO(new MemoryStream(cao.Data), data);
        }
        
        private static void ReadHeader(Stream stream, out int width)
        {
            using (BinaryReaderEndian r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                r.ReadInt32(); //Version
                width = r.ReadInt32(); //NumXVerts
                r.ReadInt32(); //NumXChunks
                r.ReadSingle(); //TileScale
                for (int i = 0; i < 6; i++) r.ReadSingle(); //AABBWorld
            }
        }
        //Thanks kornman :)
        private const uint kBitMask10 = (1 << 10) - 1;
        private const float kBitMask10Rcp = (float)(1.0 / kBitMask10);
        private static void ReadAtlas(Stream stream, int width, 
            out float[] vx, out float[] vy, out float[] vz,
            out float[] nx, out float[] ny, out float[] nz)
        {
            vx = new float[width * width];
            vy = new float[width * width];
            vz = new float[width * width];
            nx = new float[width * width];
            ny = new float[width * width];
            nz = new float[width * width];

            using (BinaryReaderEndian r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Little))
            {
                StreamableVector4 PosCompMin = new StreamableVector4();
                PosCompMin.Read(stream, Endianness.Big);
                StreamableVector4 PosCompRange = new StreamableVector4();
                PosCompRange.Read(stream, Endianness.Big);

                for (int i = 0; i < width * width; i++)
                {
                    uint v = r.ReadUInt32();
                    uint x = v >> 20 & kBitMask10;
                    uint y = v >> 10 & kBitMask10;
                    uint z = v >> 0 & kBitMask10;

                    vx[i] = x * kBitMask10Rcp * PosCompRange.X - PosCompMin.X;
                    vy[i] = y * kBitMask10Rcp * PosCompRange.Y - PosCompMin.Y;
                    vz[i] = z * kBitMask10Rcp * PosCompRange.Z - PosCompMin.Z;

                    //each vertex is actually an offset from its xz position in the grid.
                    int ovx = i / width;
                    int ovz = i % width;
                    vx[i] += ovx;
                    vz[i] += ovz;
                }
                for (int i = 0; i < width * width; i++)
                {
                    uint v = r.ReadUInt32();
                    uint x = v >> 20 & kBitMask10;
                    uint y = v >> 10 & kBitMask10;
                    uint z = v >> 0 & kBitMask10;

                    nx[i] = x * kBitMask10Rcp * 2 - 1;
                    ny[i] = y * kBitMask10Rcp * 2 - 1;
                    nz[i] = z * kBitMask10Rcp * 2 - 1;
                }
            }
        }
        private static void ReadAlpha(byte[] data, TerrainVisual vis)
        {
            byte[] dxt5 = DXT.Convert_DXT5A_DXT5(data);

            var image_dxt5 = DirectXTexNet.TexHelper.Instance.Initialize2D(DirectXTexNet.DXGI_FORMAT.BC3_UNORM, vis.Width, vis.Width, 1, 1, DirectXTexNet.CP_FLAGS.NONE);
            Marshal.Copy(dxt5, 0, image_dxt5.GetImage(0).Pixels, dxt5.Length);
            var image_a8 = image_dxt5.Decompress(DirectXTexNet.DXGI_FORMAT.A8_UNORM);
            var image_final = image_a8.GetImage(0);

            byte[] a8 = new byte[vis.Width * vis.Width];
            Marshal.Copy(image_final.Pixels, a8, 0, a8.Length);

            for (int h = 0; h < vis.Width; h++)
            {
                for (int w = 0; w < vis.Width; w++)
                {
                    int i = vis.IndexOf(w, h);
                    long t = (h * image_final.Width) + w;
                    vis.Alphas[i] = a8[t] / 255.0f;
                }
            }
        }
    }
}
