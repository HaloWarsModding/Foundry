using Chef.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chef.HW1.Map
{
    public static class TerrainIO
    {
        /// <summary>
        /// Read a terrain visual from an xtd stream (ecf based).
        /// </summary>
        /// <param name="stream"></param>
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

            ReadHeader(new MemoryStream(cheader.Data), terrain);
            ReadAtlas(new MemoryStream(catlas.Data), terrain);
            ReadAlpha(calpha.Data, terrain);
            //we dont actually care about this -- it is generated on export.
            //ReadAO(new MemoryStream(cao.Data), data);
        }
        private static void ReadHeader(Stream stream, TerrainVisual vis)
        {
            using (BinaryReaderEndian r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                r.ReadInt32(); //Version
                vis.Width = r.ReadInt32(); //NumXVerts
                r.ReadInt32(); //NumXChunks
                r.ReadSingle(); //TileScale
                for (int i = 0; i < 6; i++) r.ReadSingle(); //AABBWorld
            }
        }
        //Thanks kornman :)
        private const uint kBitMask10 = (1 << 10) - 1;
        private const float kBitMask10Rcp = (float)(1.0 / kBitMask10);
        private static void ReadAtlas(Stream stream, TerrainVisual vis)
        {
            using (BinaryReaderEndian r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Little))
            {
                StreamableVector4 PosCompMin = new StreamableVector4();
                PosCompMin.Read(stream, Endianness.Big);
                StreamableVector4 PosCompRange = new StreamableVector4();
                PosCompRange.Read(stream, Endianness.Big);

                for (int i = 0; i < vis.Positions.Length; i++)
                {
                    uint v = r.ReadUInt32();
                    uint x = v >> 20 & kBitMask10;
                    uint y = v >> 10 & kBitMask10;
                    uint z = v >> 0 & kBitMask10;

                    vis.Positions[i].X = x * kBitMask10Rcp * PosCompRange.X - PosCompMin.X;
                    vis.Positions[i].Y = y * kBitMask10Rcp * PosCompRange.Y - PosCompMin.Y;
                    vis.Positions[i].Z = z * kBitMask10Rcp * PosCompRange.Z - PosCompMin.Z;

                    //each vertex is actually an offset from its xz position in the grid.
                    int vx = i / vis.Width;
                    int vz = i % vis.Width;
                    vis.Positions[i].X += vx;
                    vis.Positions[i].Z += vz;
                }
                for (int i = 0; i < vis.Normals.Length; i++)
                {
                    uint v = r.ReadUInt32();
                    uint x = v >> 20 & kBitMask10;
                    uint y = v >> 10 & kBitMask10;
                    uint z = v >> 0 & kBitMask10;

                    vis.Normals[i].X = x * kBitMask10Rcp * 2 - 1;
                    vis.Normals[i].Y = y * kBitMask10Rcp * 2 - 1;
                    vis.Normals[i].Z = z * kBitMask10Rcp * 2 - 1;
                }
            }
        }
        private static void ReadAO(Stream stream, TerrainVisual vis)
        {
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


        /// <summary>
        /// Read terrain sim data from an xsd stream (ecf based) into a terrain structure.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void ReadXsd(Stream stream, TerrainVisual terrain)
        {
            Ecf ecf = new Ecf();
            ecf.Read(stream);

            EcfChunk header = null;
            EcfChunk simHeights = null;
            EcfChunk obstructions = null;
            EcfChunk buildable = null;
            EcfChunk tileTypes = null;
            EcfChunk camHeights = null;
            EcfChunk flightHeights = null;

            foreach (EcfChunk chunk in ecf.Chunks)
            {
                if (chunk.ID == 0x1111) header = chunk;
                if (chunk.ID == 0x2222) simHeights = chunk;
                if (chunk.ID == 0x4444) obstructions = chunk;
                if (chunk.ID == 0xCCCC) buildable = chunk;
                if (chunk.ID == 0x8888) tileTypes = chunk;
                if (chunk.ID == 0xAAAA) camHeights = chunk;
                if (chunk.ID == 0xBBBB) flightHeights = chunk;
            }

            if (header == null ||
                simHeights == null ||
                obstructions == null ||
                buildable == null ||
                tileTypes == null ||
                camHeights == null ||
                flightHeights == null) return;// null;

            //return null;
        }


        /// <summary>
        /// Read terrain texture data from an xtt stream (ecf based) into a terrain structure.
        /// </summary>
        /// <returns></returns>
        public static void ReadXtt(Stream stream, TerrainVisual terrain)
        {
            Ecf ecf = new Ecf();
            ecf.Read(stream);

            EcfChunk header = null;
            List<EcfChunk> chunks = new List<EcfChunk>();
            EcfChunk albedo = null;

            foreach (var chunk in ecf.Chunks)
            {
                if (chunk.ID == 0x1111) header = chunk;
                if (chunk.ID == 0x2222) chunks.Add(chunk);
                if (chunk.ID == 0x6666) albedo = chunk;
            }

            if (header == null ||
                albedo == null) return; //null;

            ReadXttHeader(new MemoryStream(header.Data), terrain);
            foreach (var chunk in chunks)
                ReadSplatChunk(new MemoryStream(chunk.Data), terrain);
        }
        private static void ReadXttHeader(Stream stream, TerrainVisual terrain)
        {
            using (BinaryReader reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                reader.ReadInt32(); //version
                int textures = reader.ReadInt32();
                int decals = reader.ReadInt32();
                reader.ReadInt32(); //active decal instances

                for (int i = 0; i < TerrainVisual.cMaxTextureLayers; i++)
                {
                    string texture = "";
                    if (i < textures)
                    {
                        for (int c = 0; c < 256; c++)
                        {
                            char ch = reader.ReadChar();
                            if (ch != '\0')
                                texture += ch;
                        }
                        int uScale = reader.ReadInt32();
                        int vScale = reader.ReadInt32();
                        reader.ReadInt32(); //blend op
                    }
                    terrain.Textures[i] = texture;
                }
            }
        }

        public static void Deswizzle(ushort[,] pixels)
        {
            int w = pixels.GetLength(0);
            int h = pixels.GetLength(1);
            ushort[,] ret = new ushort[w, h];

            int blockX = 4;
            int blockY = 8;
            int blockC = 4;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; h++)
                {

                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static void ReadSplatChunk(Stream stream, TerrainVisual terrain)
        {
            using (BinaryReaderEndian reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                int gridX = reader.ReadInt32();
                int gridZ = reader.ReadInt32();
                reader.ReadInt32(); //spec pass
                reader.ReadInt32(); //self pass
                reader.ReadInt32(); //env pass
                reader.ReadInt32(); //alpha pass
                int fullyOpaque = reader.ReadInt32();
                int numSplatLayers = reader.ReadInt32();
                reader.ReadInt32(); //numDecalLayers

                if (numSplatLayers > 1)
                {
                    //if there are more than 1 splat layer, round to 4.
                    if (numSplatLayers % 4 != 0)
                        numSplatLayers += 4 - (numSplatLayers % 4);

                    int[] layerindices = new int[numSplatLayers];
                    for (int i = 0; i < numSplatLayers; i++)
                    {
                        layerindices[i] = reader.ReadInt32();
                    }

                    //READ:
                    //splats are 4bpp alpha values, in 4 layers per chunk.
                    //Thus 8 layers will consist of 2 blocks each with four 4-bit components per vertex.
                    //Basically 4bpp ARGB textures per four layers.
                    //ALSO:
                    //There is a gross morton-like encoding, see "_resources/morton.txt" for a hand-made map of serially read
                    //8x4 pixel chunks to x,y coord locations.
                    List<int> touched = new List<int>();
                    for (int s = 0; s < numSplatLayers; s += 4)
                    {
                        int layer0 = layerindices[s + 0];
                        int layer1 = layerindices[s + 1];
                        int layer2 = layerindices[s + 2];
                        int layer3 = layerindices[s + 3];

                        int[] offsA = new int[4] { 0, 1, 2, 3 };
                        int[] offsB = new int[4] { 2, 3, 0, 1 };
                        for (int h = 0; h < 128; h++)
                        {
                            int[] curOffs = (h / 4 % 4) > 1 ? offsB : offsA;

                            int curCol = ((h / 16 % 2) * 4) + curOffs[h % 4];
                            int curRow = (h / 4 % 4) + (h / 32 * 4);

                            for (int r = 0; r < 4; r++)
                            {
                                for (int c = 0; c < 8; c++)
                                {
                                    int curPX = curCol * 8 + c;
                                    int curPY = curRow * 4 + r;

                                    int curChunkX = gridX * 64;
                                    int curChunkY = gridZ * 64;

                                    ushort pixel = reader.ReadUInt16();
                                    int pa = (pixel >> 12) & 0xF;
                                    int pr = (pixel >> 8) & 0xF;
                                    int pg = (pixel >> 4) & 0xF;
                                    int pb = (pixel >> 0) & 0xF;

                                    int vIndex = terrain.IndexOf(curChunkX + curPX, curChunkY + curPY);

                                    //TODO: the order might need adjusting.
                                    if (!touched.Contains(layer0)) terrain.TextureAlphas[layer0][vIndex] = pr / 15.0f; //4bpp == [0-15].
                                    if (!touched.Contains(layer1)) terrain.TextureAlphas[layer1][vIndex] = pg / 15.0f; //4bpp == [0-15].
                                    if (!touched.Contains(layer2)) terrain.TextureAlphas[layer2][vIndex] = pb / 15.0f; //4bpp == [0-15].
                                    if (!touched.Contains(layer3)) terrain.TextureAlphas[layer3][vIndex] = pa / 15.0f; //4bpp == [0-15].
                                }
                            }
                        }

                        touched.Add(layer0);
                        touched.Add(layer1);
                        touched.Add(layer2);
                        touched.Add(layer3);
                    }
                }
            }
        }
    }
}