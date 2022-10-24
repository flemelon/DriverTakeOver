using UnityEngine;
using PathCreation;

public class Autopilot : MonoBehaviour
{
    public PathGenerator pathGenerator;
    private PathCreator pathCreator;
    public float speed = 3;
    public float distanceTravelled;

    void Start()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        pathCreator = pathGenerator.pathCreator;
    }

    void Update() 
    {
        pathCreator = pathGenerator.pathCreator;
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

    }

}
