using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = UnityEngine.CharacterController;

public class CameraController : MonoBehaviour
{
    public Transform target;

    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
        public float lookSmooth = 100f;
        public float distanceFromTarget = -8f;
        public float zoomSmooth = 10f;
        public float maxZoom = -2f;
        public float minZoom = -15f;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = 20f;
        public float yRotation = -180f;
        public float maxXRotation = 25f;
        public float minXRotation = -85f;
        public float vOrbitSmooth = 150f;
        public float hOrbitSmooth = 150f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
        public string ORBIT_HORIZONTAL = "OrbitHorizontal";
        public string ORBIT_VERTICAL = "OrbitVertical";
        public string ZOOM = "Mouse ScrollWheel";
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public bool inverseX = false;
    public bool inverseY = false;

    Vector3 targetPosition = Vector3.zero;
    Vector3 destination = Vector3.zero;
    CharacterControl charController;
    float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;


    void Start()
    {
        SetCameraTarget(target);

        //MoveToTarget();
    }

    public void SetCameraTarget(Transform t)
    {
        target = t;

        if (target != null)
        {
            if (target.GetComponent<CharacterControl>())
            {
                charController = target.GetComponent<CharacterControl>();
            }
            else
            {
                Debug.LogError("Camera needs a Character Controller");
            }
        }
        else
        {
            Debug.LogError("nothing to look at");
        }
    }

    void GetInput()
    {
        vOrbitInput = Input.GetAxisRaw(input.ORBIT_VERTICAL);
        hOrbitInput = Input.GetAxis(input.ORBIT_HORIZONTAL);
        hOrbitSnapInput = Input.GetAxis(input.ORBIT_HORIZONTAL_SNAP);
        zoomInput = Input.GetAxis(input.ZOOM);
    }

    void Update()
    {
        GetInput();
        OrbitTarget();
        ZoomInOnTarget();
    }


    void LateUpdate()
    {
        //moving
        MoveToTarget();
        //rotating   
        LookAtTarget();
    }

    void MoveToTarget()
    {
        targetPosition = target.position + position.targetPosOffset;
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward *
                      position.distanceFromTarget;
        destination += targetPosition;
        transform.position = destination;
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
    }

    void OrbitTarget()
    {
        if (hOrbitSnapInput > 0)
        {
            orbit.yRotation = -180;
        }

        if (inverseX)
        {
            orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
        }
        else
        {
            orbit.xRotation += vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
        }
        if (inverseY)
        {
            orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;
        }
        else
        {
            orbit.yRotation += hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;
        }

        if (orbit.xRotation > orbit.maxXRotation)
        {
            orbit.xRotation = orbit.maxXRotation;
        }
        if (orbit.xRotation < orbit.minXRotation)
        {
            orbit.xRotation = orbit.minXRotation;
        }
    }

    void ZoomInOnTarget()
    {
        position.distanceFromTarget += zoomInput * position.zoomSmooth;
        if (position.distanceFromTarget > position.maxZoom)
        {
            position.distanceFromTarget = position.maxZoom;
        }
        if (position.distanceFromTarget < position.minZoom)
        {
            position.distanceFromTarget = position.minZoom;
        }
    }
}