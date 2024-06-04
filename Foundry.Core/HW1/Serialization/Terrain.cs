using Foundry.HW1.Scenario;
using Foundry.util;
using Foundry.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.HW1.Serialization
{
    public static class Terrain
    {
        /// <summary>
        /// Read a terrain visual from an xtd stream (ecf based).
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>null if there was an error.</returns>
        public static TerrainVisual ReadXtd(Stream stream)
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
                if (chunk.ID == 0xBBBB) clighting = chunk;
            }

            if (cheader == null ||
                catlas == null ||
                cao == null ||
                calpha == null ||
                ctess == null ||
                clighting == null) return null;

            TerrainVisual vis = new TerrainVisual();
            ReadHeader(new MemoryStream(cheader.Data), vis);
            ReadAtlas(new MemoryStream(catlas.Data), vis);
            //we dont actually care about these - they are generated on export.
            //ReadAO(new MemoryStream(cao.Data), data);
            //ReadAlpha(new MemoryStream(calpha.Data), data);
            return vis;
        }
        private static void ReadHeader(Stream stream, TerrainVisual vis)
        {
            using (BinaryReaderEndian r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                /*Version*/ r.ReadInt32();
                /*NumXVerts*/ vis.Width = r.ReadInt32();
                /*NumXVChunks*/ r.ReadInt32();
                /*TileScale*/ r.ReadSingle();
                /*AABBWorld*/ for (int i = 0; i < 6; i++) r.ReadSingle();
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
                    uint x = (v >> 20) & kBitMask10;
                    uint y = (v >> 10) & kBitMask10;
                    uint z = (v >> 0) & kBitMask10;

                    vis.Positions[i].X = (((x * kBitMask10Rcp) * PosCompRange.X) - PosCompMin.X);
                    vis.Positions[i].Y = (((y * kBitMask10Rcp) * PosCompRange.Y) - PosCompMin.Y);
                    vis.Positions[i].Z = (((z * kBitMask10Rcp) * PosCompRange.Z) - PosCompMin.Z);

                    //each vertex is actually an offset from its xz position in the grid.
                    int vx = i % vis.Width;
                    int vz = i / vis.Width;
                    vis.Positions[i].X += vx;
                    vis.Positions[i].Z += vz;
                }
                for (int i = 0; i < vis.Normals.Length; i++)
                {
                    uint v = r.ReadUInt32();
                    uint x = (v >> 20) & kBitMask10;
                    uint y = (v >> 10) & kBitMask10;
                    uint z = (v >> 0) & kBitMask10;

                    vis.Normals[i].X = (((x * kBitMask10Rcp) * 2) - 1);
                    vis.Normals[i].Y = (((y * kBitMask10Rcp) * 2) - 1);
                    vis.Normals[i].Z = (((z * kBitMask10Rcp) * 2) - 1);
                }
            }
        }
        private static void ReadAO(Stream stream, TerrainVisual vis)
        {
        }
        private static void ReadAlpha(Stream stream, TerrainVisual vis)
        {
        }

        /// <summary>
        /// Read terrain sim data from an xsd stream (ecf based).
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>null if there was an error.</returns>
        public static TerrainSim ReadXsd(Stream stream)
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
                flightHeights == null) return null;

            return null;
        }
    }
}
