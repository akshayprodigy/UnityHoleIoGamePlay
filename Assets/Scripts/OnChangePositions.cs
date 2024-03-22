using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnChangePositions : MonoBehaviour
{
    // Start is called before the first frame update
    public PolygonCollider2D hole2dCollider;
    public PolygonCollider2D ground2dCollider;
    public MeshCollider generatedMeshCollider;
    public Collider groundCollider;
    public float initialScale = 0.5f;
    Mesh generatedMesh;

    private void Start()
    {
        GameObject[] allObject = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach(var ga in allObject)
        {
            if(ga.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Physics.IgnoreCollision(ga.GetComponent<Collider>(), generatedMeshCollider, true);
            }
        }
    }

    private void FixedUpdate()
    {
        if(transform.hasChanged == true)
        {
            transform.hasChanged = false;
            hole2dCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
            hole2dCollider.transform.localScale = transform.localScale * initialScale;
            MakeHole2D();
            Make3DMeshCollider();
        }
    }

    public void Move(BaseEventData baseEventData)
    {
        if (((PointerEventData)baseEventData).pointerCurrentRaycast.isValid)
        {
            transform.position = ((PointerEventData)baseEventData).pointerCurrentRaycast.worldPosition;
        }
    }

    private void MakeHole2D()
    {
        Vector2[] PointPositions = hole2dCollider.GetPath(0);
        for(int i =0;i<PointPositions.Length; i++)
        {
            //PointPositions[i] += (Vector2)hole2dCollider.transform.position;
            PointPositions[i] = hole2dCollider.transform.TransformPoint(PointPositions[i]);
        }

        ground2dCollider.pathCount = 2;
        ground2dCollider.SetPath(1, PointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (generatedMesh != null)
            Destroy(generatedMesh);

        generatedMesh = ground2dCollider.CreateMesh(true, true);
        generatedMeshCollider.sharedMesh = generatedMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(other, groundCollider,true);
        Physics.IgnoreCollision(other, generatedMeshCollider, false);

    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(other, groundCollider, false);
        Physics.IgnoreCollision(other, generatedMeshCollider, true);
    }
}
