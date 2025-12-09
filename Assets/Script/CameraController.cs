using UnityEngine;

namespace PhysicsOptimization
{
    /// <summary>
    /// 相机控制器 - 提供简单的相机控制功能
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("相机控制")]
        public float moveSpeed = 10f;
        public float zoomSpeed = 5f;
        public float minZoom = 2f;
        public float maxZoom = 20f;

        private Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                cam = Camera.main;
            }
        }

        void Update()
        {
            HandleMovement();
            HandleZoom();
        }

        /// <summary>
        /// 处理相机移动
        /// </summary>
        private void HandleMovement()
        {
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                movement += Vector3.up;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                movement += Vector3.down;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                movement += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                movement += Vector3.right;

            transform.position += movement * moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 处理相机缩放
        /// </summary>
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            
            if (scroll != 0f)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
        }

        /// <summary>
        /// 重置相机位置
        /// </summary>
        public void ResetCamera()
        {
            transform.position = new Vector3(0, 0, -10);
            cam.orthographicSize = 10f;
        }

        
    }
}
