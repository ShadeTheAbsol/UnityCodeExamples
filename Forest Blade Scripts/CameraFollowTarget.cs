using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform followTarget;
    public Vector3 followOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Camera Follows Assigned Target
    private void LateUpdate()
    {
        Vector3 targetPos = followTarget.position;
        targetPos.z = -10;
        transform.position = targetPos + followOffset;
    }
}
