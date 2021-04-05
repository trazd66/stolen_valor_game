using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    public Transform boss;

    public float smoothSpeed = 0.075f;

    public float minDistance;

    public float maxDistance; 

    public Vector3 offset;

    public Vector3 velocity=Vector3.one;

    
    void LateUpdate()
    {
        if(boss != null)
        {
            offset.z = -(Vector3.Distance(boss.position, target.position) + 2);
            if (offset.z > -minDistance)
                offset.z = -minDistance;
            if (offset.z < -maxDistance)
                offset.z = -maxDistance;
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
            transform.position = smoothedPosition;
        }
        



        
    }
}

