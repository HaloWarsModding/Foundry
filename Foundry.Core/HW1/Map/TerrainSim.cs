using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

namespace Chef.HW1.Map
{
    public enum TerrainSimObstructionType : byte
    {
        None = 0,

        Land,
        All = 3,
        Air,

        Flood,
        Scarab,

        LandAndFlood,
        LandAndScarab,
        FloodAndScarab
    }
    public struct TerrainSimTile
    {
        public float Height { get; set; }
        public TerrainSimObstructionType Obstruction { get; set; }
        /// This is unused by the game.
        //public bool Buildable { get; set; }
        public byte TileType { get; set; }
    }
    public class TerrainSim
    {
        /// <summary>
        /// The width of the terrain sim. Vanilla maps use 1/4th of the visual width.
        /// </summary>
        public int SimWidth
        {
            get
            {
                return (int)Math.Sqrt(Tiles.Length);
            }
            set
            {
                TerrainSimTile[] tiles = new TerrainSimTile[value * value];
                if (Tiles != null)
                {
                    int tcopy = Math.Min(Tiles.Length, tiles.Length);
                    Array.Copy(Tiles, tiles, tcopy);
                }
                Tiles = tiles;
            }
        }
        /// <summary>
        /// The width of the camera plane. Vanilla maps use 1/64th of the visual width.
        /// </summary>
        public int CameraWidth
        {
            get
            {
                return (int)Math.Sqrt(CamHeights.Length);
            }
            set
            {
                float[] heights = new float[value * value];
                if (CamHeights != null)
                {
                    int tcopy = Math.Min(CamHeights.Length, heights.Length);
                    Array.Copy(CamHeights, heights, tcopy);
                }
                CamHeights = heights;
            }
        }
        /// <summary>
        /// The width of the air unit plane. Vanilla maps use 1/64th of the visual width.
        /// </summary>
        public int FlightWidth
        {
            get
            {
                return (int)Math.Sqrt(FlightHeights.Length);
            }
            set
            {
                float[] heights = new float[value * value];
                if (FlightHeights != null)
                {
                    int tcopy = Math.Min(FlightHeights.Length, heights.Length);
                    Array.Copy(FlightHeights, heights, tcopy);
                }
                FlightHeights = heights;
            }
        }

        public TerrainSimTile[] Tiles { get; private set; }
        public float[] CamHeights { get; private set; }
        public float[] FlightHeights { get; private set; }
    }
}