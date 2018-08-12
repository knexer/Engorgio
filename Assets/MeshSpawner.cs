using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSpawner : MonoBehaviour
{
    [SerializeField] private Rigidbody MeshNode;
    [SerializeField] private float springStrength = 10f;
    [SerializeField] private float dampenStrength = 1f;
    [SerializeField] private float distance = 1f;
    [SerializeField] private int nodeCount = 3;
    [SerializeField] private float growthFactor = 1.05f;
    [SerializeField] private Mesh sphereMesh;

	// Use this for initialization
	void Awake ()
	{
        MakeCube();
	}

    private void MakeSphere()
    {
        // We have this convenient sphereMesh that is already mostly sphereish and should be good enough

        // Make the outer skin:
        // TODO Spawn a node at each vertex of the sphere
        // Connect it to each of its neighbors (that have already been spawned).  Now you have the outer skin.

        // Make the inner skin:
        // For each triangle in the mesh, create another node at the middle of the face
        // (but inset towards the center of the sphere by approximately the distance between adjacent nodes)
        // Connect it to the inner skin nodes for each neighboring face
        // Connect it to the nodes for the vertices on the face this was spawned from

        // Add the center node and connect it to all the inner skin nodes.
        // (We may need a different internal structure to make non-convex blobbing possible but this should be OK for now)
        // (These connections probably will need different spring configurations)
    }

    private void MakeCube()
    {
        var nodes = new Rigidbody[nodeCount, nodeCount, nodeCount];
        for (int x = 0; x < nodeCount; x++)
        {
            for (int y = 0; y < nodeCount; y++)
            {
                for (int z = 0; z < nodeCount; z++)
                {
                    Rigidbody node = Instantiate(MeshNode, gameObject.transform);
                    node.transform.position += new Vector3(x, y, z) * distance;
                    nodes[x, y, z] = node;
                    if (x > 0) AddSpringJoint(node, nodes[x - 1, y, z]);
                    if (y > 0) AddSpringJoint(node, nodes[x, y - 1, z]);
                    if (z > 0) AddSpringJoint(node, nodes[x, y, z - 1]);
                }
            }
        }
    }

    private void AddSpringJoint(Rigidbody node, Rigidbody otherNode)
    {
        var joint = node.gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = otherNode;
        joint.spring = springStrength;
        joint.damper = dampenStrength;
        joint.autoConfigureConnectedAnchor = false;
    }

    private void MakeTriangle()
    {
        var nodes = new Rigidbody[4];
        var relativePositons = new[]
            {Vector3.zero, Vector3.left * distance, Vector3.forward * distance, Vector3.up * distance};
        for (int i = 0; i < 4; i++)
        {
            Rigidbody node = Instantiate(MeshNode, gameObject.transform);
            node.transform.position += relativePositons[i];
            nodes[i] = node;
            for (int j = 0; j < i; j++)
            {
                AddSpringJoint(node, nodes[j]);
            }
        }
    }

    // Update is called once per frame
	void Update ()
	{
	    foreach (SpringJoint joint in GetComponentsInChildren<SpringJoint>())
	    {
	        joint.connectedAnchor *= growthFactor;
	    }
	}
}
