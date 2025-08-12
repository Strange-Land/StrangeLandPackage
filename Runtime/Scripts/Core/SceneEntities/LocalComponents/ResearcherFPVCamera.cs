using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.SceneEntities
{
    public class FreeCameraController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float speedMultiplier = 2f;

        public float rotationSpeed = 5f;
        public float minPitch = -80f;
        public float maxPitch = 80f;

        public float zoomSpeed = 10f;
        public float zoomMultiplier = 2f;
        public float minFOV = 20f;
        public float maxFOV = 90f;

        float _yaw;
        float _pitch;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            _yaw = angles.y;
            _pitch = angles.x;
        }

        void Update()
        {
            Move();
            Look();
            Zoom();
        }

        void Move()
        {
            float currentSpeed = moveSpeed;

            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
                currentSpeed *= speedMultiplier;

            float h = 0f;
            float v = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) h = -1f;
                if (Keyboard.current.dKey.isPressed) h = 1f;
                if (Keyboard.current.wKey.isPressed) v = 1f;
                if (Keyboard.current.sKey.isPressed) v = -1f;
            }

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 direction = (right * h + forward * v).normalized;
            Vector3 move = direction * currentSpeed * Time.deltaTime;

            float up = 0f;
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) up += 1f;
            if (Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed) up -= 1f;
            move += Vector3.up * up * currentSpeed * Time.deltaTime;

            transform.position += move;
        }

        void Look()
        {
            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                float mouseX = Mouse.current.delta.x.ReadValue() * rotationSpeed * Time.deltaTime;
                float mouseY = Mouse.current.delta.y.ReadValue() * rotationSpeed * Time.deltaTime;

                _yaw += mouseX;
                _pitch -= mouseY;
                _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

                transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
            }
        }

        void Zoom()
        {
            float currentZoomSpeed = zoomSpeed;
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
                currentZoomSpeed *= zoomMultiplier;

            float scroll = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Camera cam = GetComponent<Camera>();
                if (cam)
                {
                    float fov = cam.fieldOfView;
                    fov -= scroll * currentZoomSpeed * Time.deltaTime;
                    fov = Mathf.Clamp(fov, minFOV, maxFOV);
                    cam.fieldOfView = fov;
                }
            }
        }
    }
}
