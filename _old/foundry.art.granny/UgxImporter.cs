using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Art.Granny
{
	public static class UGXImporter
	{
		public static Geometry3D ImportUGXGeometry(string file)
		{
			MeshBuilder builder = new MeshBuilder();

			var chunks = Util.ECF.ReadChunks(file);

			byte[] cached = chunks[0x700][0];
			byte[] vertices = chunks[0x702][0];
			byte[] indices = chunks[0x701][0];

			uint sectionsLen = BitConverter.ToUInt32(cached, 64);
			uint sectionsOffs = BitConverter.ToUInt32(cached, 72);

			for(int sec = 0; sec < sectionsLen; sec++)
			{
				int cur = (int)(sectionsOffs + (sec * 152) );

				int indicesOffs = (int)BitConverter.ToUInt32(cached, cur + 16);
				int triangleCount = (int)BitConverter.ToUInt32(cached, cur + 20);
				int verticesOffsBytes = (int)BitConverter.ToUInt32(cached, cur + 24);
				int verticesLenBytes = (int)BitConverter.ToUInt32(cached, cur + 28);
				int vertexSize = (int)BitConverter.ToUInt32(cached, cur + 32);
				int vertexCount = (int)BitConverter.ToUInt32(cached, cur + 36);

				int packOrderOffset = (int)BitConverter.ToUInt32(cached, cur + 40);

				int currentVertexCount = builder.Positions.Count;
				for (int v = 0; v < vertexCount; v++)
				{
					int curv = verticesOffsBytes + (v * vertexSize);

					float x = util.Misc.ToFloat16(vertices[curv + 0], vertices[curv + 1]);
					float y = util.Misc.ToFloat16(vertices[curv + 2], vertices[curv + 3]);
					float z = util.Misc.ToFloat16(vertices[curv + 4], vertices[curv + 5]);

					var vec = new SharpDX.Vector3(x, y, -z);

					builder.Positions.Add(vec);
				}

				for (int i = 0; i < triangleCount; i++)
				{
					int curi = (indicesOffs * 2) + (i * 6);

					ushort ind0 = BitConverter.ToUInt16(indices, curi);
					ushort ind1 = BitConverter.ToUInt16(indices, curi + 2);
					ushort ind2 = BitConverter.ToUInt16(indices, curi + 4);

					builder.TriangleIndices.Add(currentVertexCount + ind2);
					builder.TriangleIndices.Add(currentVertexCount + ind1);
					builder.TriangleIndices.Add(currentVertexCount + ind0);
				}
			}

			MeshGeometry3D ret = builder.ToMeshGeometry3D();
			ret.Normals = MeshGeometryHelper.CalculateNormals(ret);

			return ret;
		}
	}
}
