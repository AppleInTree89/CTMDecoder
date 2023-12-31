using System;

namespace _3mxImport
{
	public class Algorithm
	{
	    public static float[] revertVertices(int[] intVertices, int[] gridIndices, CTMGrid grid, float vertexPrecision)
	    {
	        int ve = CTMMesh.CTM_POSITION_ELEMENT_COUNT;
	        int vc = intVertices.Length / ve;
	
	        int prevGridIndex = 0x7fffffff;
	        int prevDeltaX = 0;
	        float[] vertices = new float[vc * ve];
	        for (int i = 0; i < vc; ++i) {
	            // Get grid box origin
	            int gridIdx = gridIndices[i];
	            float[] gridOrigin = gridIdxToPoint(grid, gridIdx);
	
	            // Restore original point
	            int deltaX = intVertices[i * ve];
	            if (gridIdx == prevGridIndex) {
	                deltaX += prevDeltaX;
	            }
	            vertices[i * ve] = vertexPrecision * deltaX + gridOrigin[0];
	            vertices[i * ve + 1] = vertexPrecision * intVertices[i * ve + 1] + gridOrigin[1];
	            vertices[i * ve + 2] = vertexPrecision * intVertices[i * ve + 2] + gridOrigin[2];
	
	            prevGridIndex = gridIdx;
	            prevDeltaX = deltaX;
	        }
	        return vertices;
	    }
	    public static float[] gridIdxToPoint(CTMGrid grid, int idx)
	    {
	        int[] gridIdx = new int[3];
	
	        int ydiv = grid.getDivision()[0];
	        int zdiv = ydiv * grid.getDivision()[1];
	
	        gridIdx[2] = idx / zdiv;
	        idx -= gridIdx[2] * zdiv;
	        gridIdx[1] = idx / ydiv;
	        idx -= gridIdx[1] * ydiv;
	        gridIdx[0] = idx;
	
	        float[] size = grid.getSize();
	        float[] point = new float[3];
	        for (int i = 0; i < 3; ++i) {
	            point[i] = gridIdx[i] * size[i] + grid.getMin()[i];
	        }
	        return point;
	    }
	    public static float[] GetSmoothNormals(float[] vertices, int[] indices)
	    {
	        int vc = vertices.Length / CTMMesh.CTM_POSITION_ELEMENT_COUNT;
	        int tc = indices.Length / 3;
	        float[] smoothNormals = new float[vc * CTMMesh.CTM_NORMAL_ELEMENT_COUNT];
	        for (int i = 0; i < tc; ++i) {
				int[] tri = new int[3];
				Array.Copy(indices, i*3, tri, 0, 3);
	            float[] v1 = new float[3];
	            float[] v2 = new float[3];
	            for (int j = 0; j < 3; ++j) {
	                v1[j] = vertices[tri[1] * 3 + j] - vertices[tri[0] * 3 + j];
	                v2[j] = vertices[tri[2] * 3 + j] - vertices[tri[0] * 3 + j];
	            }
	            float[] n = new float[3];
	            n[0] = v1[1] * v2[2] - v1[2] * v2[1];
	            n[1] = v1[2] * v2[0] - v1[0] * v2[2];
	            n[2] = v1[0] * v2[1] - v1[1] * v2[0];
	            float len = (float) Math.Sqrt(n[0] * n[0] + n[1] * n[1] + n[2] * n[2]);
	            if (len > 1e-10f) {
	                len = 1.0f / len;
	            } else {
	                len = 1.0f;
	            }
	            for (int j = 0; j < 3; ++j) {
	                n[j] *= len;
	            }
	            for (int k = 0; k < 3; ++k) {
	                for (int j = 0; j < 3; ++j) {
	                    smoothNormals[tri[k] * 3 + j] += n[j];
	                }
	            }
	        }
	        for (int i = 0; i < vc; ++i) {
	            float len = (float) Math.Sqrt(smoothNormals[i * 3] * smoothNormals[i * 3]
	                    + smoothNormals[i * 3 + 1] * smoothNormals[i * 3 + 1]
	                    + smoothNormals[i * 3 + 2] * smoothNormals[i * 3 + 2]);
	            if (len > 1e-10f) {
	                len = 1.0f / len;
	            } else {
	                len = 1.0f;
	            }
	            for (int j = 0; j < 3; ++j) {
	                smoothNormals[i * 3 + j] *= len;
	            }
	        }
	
	        return smoothNormals;
	    }
	    public static float[] NormalCoordSys(float[] normals, int offset)
	    {
	
	        float[] m = new float[9];
	        m[6] = normals[offset];
	        m[7] = normals[offset + 1];
	        m[8] = normals[offset + 2];
	        m[0] = -normals[offset + 1];
	        m[1] = normals[offset] - normals[offset + 2];
	        m[2] = normals[offset + 1];
	        float len = (float) Math.Sqrt(2.0 * m[0] * m[0] + m[1] * m[1]);
	        if (len > 1.0e-20f) {
	            len = 1.0f / len;
	            m[0] *= len;
	            m[1] *= len;
	            m[2] *= len;
	        }
	        m[3 + 0] = m[6 + 1] * m[2] - m[6 + 2] * m[1];
	        m[3 + 1] = m[6 + 2] * m[0] - m[6 + 0] * m[2];
	        m[3 + 2] = m[6 + 0] * m[1] - m[6 + 1] * m[0];
	
	        return m;
	    }
	}
}

