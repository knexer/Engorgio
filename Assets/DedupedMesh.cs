using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    /** A mesh that dedupes all vertices such that each point in space corresponds to exactly one vertex. */
    public class DedupedMesh
    {
        public int[] triangles;
        public Vector3[] vertices;

        public DedupedMesh(Mesh mesh) {
            DedupeMesh(mesh, out vertices, out triangles);
        }

        private void DedupeMesh(Mesh mesh, out Vector3[] vertices, out int[] triangles) {
            // Maps each vertex index to the first index that had that position
            int[] vertexToOriginalIndex = new int[mesh.vertices.Length];

            vertices = DedupeVertices (vertexToOriginalIndex, mesh.vertices);
            triangles = DedupeTriangles (vertexToOriginalIndex, mesh.triangles);

        }

        private Vector3[] DedupeVertices(int[] vertexToOriginalIndex, Vector3[] meshVertices) {
            // Maps a position in space to the first vertex index that had that position
            Dictionary<Vector3, int> vertexToOriginal = new Dictionary<Vector3, int> ();
            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < meshVertices.Length; i++) {
                Vector3 vertex = meshVertices [i];
                if (vertexToOriginal.ContainsKey (vertex)) {
                    int original = vertexToOriginal [vertex];
                    vertexToOriginalIndex [i] = original;
                }
                else {
                    vertices.Add(vertex);
                    vertexToOriginal [vertex] = vertices.Count - 1;
                    vertexToOriginalIndex [i] = vertices.Count - 1;
                }
            }
            return vertices.ToArray ();
        }

        private int[] DedupeTriangles(int[] vertexToOriginalIndex, int[] meshTriangles) {
            int[] deduped = new int[meshTriangles.Length];
            for (int i = 0; i < meshTriangles.Length; i++) {
                deduped [i] = vertexToOriginalIndex [meshTriangles [i]];
            }
            return deduped;
        }
    }
}

