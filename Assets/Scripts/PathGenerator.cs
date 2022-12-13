using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Linq;

public class PathGenerator : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    public PathCreator pathCreator;
    private Autopilot autoPilot;
    public BezierPath bezierPath;
    public VertexPath path;
    private bool [,] roadMap = new bool[10000,10000];
    public float roadWidth = 2.0f;
    public float thickness = .15f;
    public bool flattenSurface;

    public Material roadMaterial;
    public Material undersideMaterial;
    public float textureTiling = 1;

    [SerializeField]
    GameObject meshHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    [SerializeField] private GameObject exclamationMark;
    GameObject exclamationMarkHolder;
    public float exclamationMarkTime;
    public Vector3 exclamationMarkPosition;

    // Awake is called before Start
    void Awake()
    {
        InitObjects ();
        GenerateRoad ();
        PlaceExclamationMark();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitObjects ()
    {
        pathCreator = gameObject.AddComponent<PathCreator>();
        points.Add(new Vector3(0,0,0));
        points.Add(new Vector3(3,0,3));
        bezierPath = new BezierPath(this.points.ToArray(), false, PathSpace.xyz);
    }

    void GenerateRoad ()
    {
        while(pathCreator.path.length < 1000)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 lastPoint = points.Last();
                Vector3 newPoint;
                do
                {
                    float randomPosX = Random.Range(1, 5)*3;
                    float randomPosZ = Random.Range(1, 5)*3;

                    newPoint = new Vector3(lastPoint.x + randomPosX, 0, lastPoint.z + randomPosZ);

                }while( IsPointOutBound(lastPoint) && IsLastSegementIntersecting(lastPoint, newPoint));

                if(!IsPointOutBound(lastPoint) && !IsPointOutBound(newPoint))
                {
                    bezierPath.AddSegmentToEnd(newPoint);

                    AddSegmentToRoadMap (lastPoint, newPoint);
                    
                    points.Add(newPoint);
                }
            }
            bezierPath = new BezierPath(this.points.ToArray(), false, PathSpace.xyz);
            path = new VertexPath(bezierPath, transform, 1f, 1f);
            pathCreator.bezierPath = bezierPath;
        }

        if (pathCreator != null) 
        {
            AssignMeshComponents ();
            AssignMaterials ();
            CreateRoadMesh ();
        }
    }

    bool IsLastSegementIntersecting(Vector3 lastPoint, Vector3 lastSegement)
    {
        //Vector3 [] lastSegmentPoints = {offset-lastPoint, offset-lastSegement};
        Vector3 [] lastSegmentPoints = {lastPoint, lastSegement};
        BezierPath bez = new BezierPath(lastSegmentPoints, false, PathSpace.xyz);
        VertexPath pathSegment = new VertexPath(bez, transform);
        Vector3 p;
        for(float steps = 0.0f; steps <= 1.0f; steps+=0.01f)
        {
            p = pathSegment.GetPointAtTime(steps);

            if( 
                !IsPointOutBound (p) ||
                roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Ceil(p.z)] || 
                roadMap[(int) Mathf.Floor(p.x), (int) Mathf.Floor(p.z)] ||
                roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Floor(p.z)] ||
                roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Ceil(p.z)] 
            ) 
            {
                return true;
            }
        }
        return false;
    }

    void AddSegmentToRoadMap (Vector3 lastPoint, Vector3 lastSegment)
    {
        Vector3 [] lastSegmentPoints = {lastPoint, lastSegment};
        BezierPath bez = new BezierPath(lastSegmentPoints, false, PathSpace.xyz);
        VertexPath pathSegment = new VertexPath(bez, transform);
        Vector3 tempPosition;
        for(float steps = 0.0f; steps <= 1.0f; steps+=0.01f)
        {
            tempPosition = pathSegment.GetPointAtTime(steps);
            AddPointToRoadMap(tempPosition);
        }
    }

    void AddPointToRoadMap (Vector3 point)
    {
        if(IsPointOutBound(point))
        {
            return;
        }
        //Vector3 p = offset - point;
        Vector3 p = point;
        roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Ceil(p.z)] = true;
        roadMap[(int) Mathf.Floor(p.x), (int) Mathf.Floor(p.z)] = true;
        roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Floor(p.z)] = true;
        roadMap[(int) Mathf.Ceil(p.x), (int) Mathf.Ceil(p.z)] = true;
    }

    bool IsPointOutBound (Vector3 point)
    {
        //Vector3 p = offset - point;
        Vector3 p = point;
        if(
            (int) Mathf.Floor(p.x) < 0 ||
            (int) Mathf.Floor(p.z) < 0 ||
            (int) Mathf.Floor(p.x) > roadMap.GetLength(0) ||
            (int) Mathf.Ceil(p.x) > roadMap.GetLength(0) ||
            (int) Mathf.Floor(p.z) > roadMap.GetLength(1) ||
            (int) Mathf.Ceil(p.z) > roadMap.GetLength(1)
        )
        {
            return true;
        }
        return false;
    }

    void CreateRoadMesh () {
        Vector3[] verts = new Vector3[path.NumPoints * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];
        int[] underRoadTriangles = new int[numTris * 3];
        int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

        for (int i = 0; i < path.NumPoints; i++) {
            Vector3 localUp = (usePathNormals) ? Vector3.Cross (path.GetTangent (i), path.GetNormal (i)) : path.up;
            Vector3 localRight = (usePathNormals) ? path.GetNormal (i) : Vector3.Cross (localUp, path.GetTangent (i));

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint (i) - localRight * Mathf.Abs (roadWidth);
            Vector3 vertSideB = path.GetPoint (i) + localRight * Mathf.Abs (roadWidth);

            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;
            // Add bottom of road vertices
            verts[vertIndex + 2] = vertSideA - localUp * thickness;
            verts[vertIndex + 3] = vertSideB - localUp * thickness;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2 (0, path.times[i]);
            uvs[vertIndex + 1] = new Vector2 (1, path.times[i]);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop) {
                for (int j = 0; j < triangleMap.Length; j++) {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++) {
                    sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                }

            }

            vertIndex += 8;
            triIndex += 6;
        }

        mesh.Clear ();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles (roadTriangles, 0);
        mesh.SetTriangles (underRoadTriangles, 1);
        mesh.SetTriangles (sideOfRoadTriangles, 2);
        mesh.RecalculateBounds ();
    }

    // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
    void AssignMeshComponents () {

        if (meshHolder == null) {
            meshHolder = new GameObject ("Road Mesh Holder");
        }

        meshHolder.transform.rotation = Quaternion.identity;
        meshHolder.transform.position = Vector3.zero;
        meshHolder.transform.localScale = Vector3.one;

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<MeshFilter> ()) {
            meshHolder.gameObject.AddComponent<MeshFilter> ();
        }
        if (!meshHolder.GetComponent<MeshRenderer> ()) {
            meshHolder.gameObject.AddComponent<MeshRenderer> ();
        }

        meshRenderer = meshHolder.GetComponent<MeshRenderer> ();
        meshFilter = meshHolder.GetComponent<MeshFilter> ();
        if (mesh == null) {
            mesh = new Mesh ();
        }
        meshFilter.sharedMesh = mesh;
    }

    void AssignMaterials () {
        if (roadMaterial != null && undersideMaterial != null) {
            meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
            meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
        }
    }

    void PlaceExclamationMark()
    {
        exclamationMarkTime = Random.Range(0.0f, 1.0f);
        exclamationMarkPosition = path.GetPointAtTime(exclamationMarkTime);
        exclamationMarkPosition.y += 2.0f;
        Quaternion exclamationMarkRot = Quaternion.identity;
        exclamationMarkHolder = (GameObject) Instantiate(exclamationMark, exclamationMarkPosition, exclamationMarkRot);
    }
}
