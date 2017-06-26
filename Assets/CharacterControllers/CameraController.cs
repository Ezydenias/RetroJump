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


    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;

        [HideInInspector] public bool colliding = false;
        [HideInInspector] public Vector3[] adjustedCameraClipPoints;
        [HideInInspector] public Vector3[] desiredCameraClipPoints;

        private Camera camera;

        public void Initialize(Camera cam)
        {
            camera = cam;
            adjustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];
        }

        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera)
                return;

            //clear the contens of the intoArray
            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f);
            float y = x / camera.aspect;

            //top left
            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
            //top right
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
            //bottom left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
            //bottom right
            intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
            //Camera's position
            intoArray[4] = cameraPosition - camera.transform.forward;
        }

        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if (Physics.Raycast(ray, distance, collisionLayer))
                {
                    return true;
                }
            }
            return false;
        }

        public float GetAdjustedDistanceWithRayFrom(Vector3 fromPosition)
        {
            float distance = -1f;

            for (int i = 0; i < desiredCameraClipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, desiredCameraClipPoints[i] - fromPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (distance == -1)
                    {
                        distance = hit.distance;
                    }
                    else
                    {
                        if (hit.distance < distance)
                        {
                            distance = hit.distance;
                        }
                    }
                }
            }

            if (distance == -1)
            {
                return 0;
            }
            else
            {
                return distance;
            }
        }

        public void CheckColliding(Vector3 targetPosition)
        {
            if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }
        }
    }
}