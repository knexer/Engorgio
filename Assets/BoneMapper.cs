using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Connect a spawned blob-mesh to a rendered mesh via bones and shit
public class BoneMapper : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

	// Use this for initialization
	void Start () {
		// Assume that all the child GOs are blob-mesh surface nodes
        // TODO in practice this won't be the case... but it may be irrelevant!
        // Create a bone for each one
	    var bones = new Transform[transform.childCount];
	    var bindPoses = new Matrix4x4[transform.childCount];
	    for (int i = 0; i < transform.childCount; i++)
	    {
	        bones[i] = transform.GetChild(i);
	        bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
	    }

	    skinnedMeshRenderer.bones = bones;
	    mesh.bindposes = bindPoses;
	    skinnedMeshRenderer.sharedMesh = mesh;
	    skinnedMeshRenderer.updateWhenOffscreen = true; // because the bones are ragdoll and not animation driven

	    // Populate bone weights for each vertex in the rendered mesh based on the three closest bones, weighted by proximity
        var boneWeights = new BoneWeight[mesh.vertexCount];
	    for (int i = 0; i < mesh.vertexCount; i++)
	    {
	        Vector3 vertex = mesh.vertices[i];
            // Find the closest three bones.
            // In case of perf issues, replace orderby with a lazy quicksort, and if they persist use a spatial data structure.
            // Should be fine though - it's only n^2 log n.
	        var closestBones = bones.Select((bone, index) => new {bone, index}).OrderBy(bone => Vector3.SqrMagnitude(vertex - bone.bone.position)).Take(3).ToArray();

            // make up a weight for each bone.  Maybe it's even a useful weight!
	        float[] weights = closestBones.Select(bone => 1 / Vector3.Distance(bone.bone.transform.position, vertex)).ToArray();
            float totalWeight = weights.Sum();
	        float[] normalizedWeights = weights.Select(weight => weight / totalWeight).ToArray();

	        boneWeights[i] = new BoneWeight
	        {
	            boneIndex0 = closestBones[0].index,
	            weight0 = normalizedWeights[0],
	            boneIndex1 = closestBones[1].index,
	            weight1 = normalizedWeights[1],
	            boneIndex2 = closestBones[2].index,
	            weight2 = normalizedWeights[2]
	        };
	    }

	    mesh.boneWeights = boneWeights;
	}

    // Update is called once per frame
    void LateUpdate () {
        Debug.Log("Blob-mesh 0: " + transform.GetChild(0).position);
        Debug.Log("Bone 0 is at: " + skinnedMeshRenderer.bones[0].position);
        Debug.Log("Bone weight 0 is: " + skinnedMeshRenderer.sharedMesh.boneWeights[0]);
        var bones = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            bones[i] = transform.GetChild(i);
        }

        //skinnedMeshRenderer.bones = bones;
    }
}
