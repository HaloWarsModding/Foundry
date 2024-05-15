using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Util
{
	public static class ECF
	{
		public static uint CalcAdler32(byte[] barr, int offs, int len)
		{
			const int mod = 65521;
			uint a = 1, b = 0;
			for (int i = offs; i < len + offs; i++)
			{
				byte c = barr[i];
				a = (a + c) % mod;
				b = (b + a) % mod;
			}
			return b << 16 | a;
		}

		public static Dictionary<long, List<byte[]>> ReadChunks(string file)
		{
			Dictionary<long, List<byte[]>> chunkDatas = new Dictionary<long, List<byte[]>>();

			using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				//ecf header.
				const int ecfHeaderSize = 32;
				byte[] ecfHeader = new byte[ecfHeaderSize];
				fs.Read(ecfHeader, 0, ecfHeaderSize);

				//number of chunks.
				ushort numChunks = BitConverter.ToUInt16(ecfHeader, 16);
				numChunks = BinaryPrimitives.ReverseEndianness(numChunks);

				const int ecfChunkHeaderSize = 24;
				byte[] ecfChunkHeaders = new byte[numChunks * ecfChunkHeaderSize];
				fs.Read(ecfChunkHeaders, 0, numChunks * ecfChunkHeaderSize);

				//get chunk data and store by id.
				for (int i = 0; i < numChunks; i++)
				{
					int cur = i * ecfChunkHeaderSize;

					long id = BitConverter.ToInt64(ecfChunkHeaders, cur);
					int offset = BitConverter.ToInt32(ecfChunkHeaders, cur + 8);
					int size = BitConverter.ToInt32(ecfChunkHeaders, cur + 12);

					id = BinaryPrimitives.ReverseEndianness(id);
					offset = BinaryPrimitives.ReverseEndianness(offset);
					size = BinaryPrimitives.ReverseEndianness(size);

					byte[] chunkData = new byte[size];
					fs.Seek(offset, SeekOrigin.Begin);
					fs.Read(chunkData, 0, size);

					if (!chunkDatas.ContainsKey(id))
						chunkDatas.Add(id, new List<byte[]>());
					chunkDatas[id].Add(chunkData);
				}
			}

			return chunkDatas;
		}
	}
}
