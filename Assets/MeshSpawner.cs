using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

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
        MakeSphere();
    }

    private void MakeSphere()
    {
        // Ode to My Code, by Steven Weiss
        DedupedMesh mesh = new DedupedMesh(sphereMesh);
        Rigidbody[] nodes = new Rigidbody[mesh.vertices.Length];

        // Some have said code is less science than art
        for (int i = 0; i < mesh.vertices.Length; i++) {
            // Code can be beautiful, elegant, smart
            nodes [i] = Instantiate (MeshNode, gameObject.transform);
            // This is not that code.
            nodes [i].transform.position = mesh.vertices [i] * 3f;
        }

        // In some of your source, no doubt you've been proud
        for (int i = 0; i < sphereMesh.triangles.Length; i += 3) {
            // You'd happily share it, read it aloud
            AddSpringJoint(nodes[mesh.triangles[i + 1]], nodes[mesh.triangles[i + 0]]);
            // No doubt your professors would all have been wowed
            AddSpringJoint(nodes[mesh.triangles[i + 2]], nodes[mesh.triangles[i + 0]]);
            // This code is not that code.
            AddSpringJoint(nodes[mesh.triangles[i + 2]], nodes[mesh.triangles[i + 1]]);
        }

        nodes[0].useGravity = true;            // And yet there is something there in its refrain
        nodes[0].mass = 50f;                // For problems were solved, and demons were slain
        Debug.Log("Created " + nodes.Length // And sure it's not great, I'll be quick to admit
                  + " nodes from a mesh."); // But I still love this code, even if it is shit.
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
