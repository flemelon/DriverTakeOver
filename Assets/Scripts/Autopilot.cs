using UnityEngine;
using PathCreation;

public class Autopilot : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 3;
    float distanceTravelled;

    void Update() 
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

    }

}
