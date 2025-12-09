using UnityEngine;

namespace PhysicsOptimization
{
    /// <summary>
    /// 物理对象类，代表可以进行碰撞检测的对象
    /// </summary>
    public class PhysicsObject : MonoBehaviour
    {
        [Header("物理属性")]
        public Vector2 velocity = Vector2.zero;
        public float mass = 1f;
        public float radius = 0.5f;
        public bool isStatic = false;
        
        [Header("显示设置")]
        public Color objectColor = Color.red;
        
        private Vector2 position;
        private Vector2 lastPosition;
        
        void Start()
        {
            position = transform.position;
            lastPosition = position;
        }

        void Update()
        {
            if (!isStatic)
            {
                // 简单的物理运动
                lastPosition = position;
                position += velocity * Time.deltaTime;
                transform.position = position;
                
                // 简单的边界检查
                CheckBounds();
            }
        }

        /// <summary>
        /// 检查边界并反弹
        /// </summary>
        private void CheckBounds()
        {
            float boundsSize = 10f; // 假设场景边界
            
            if (position.x - radius < -boundsSize || position.x + radius > boundsSize)
            {
                velocity.x = -velocity.x;
                position.x = Mathf.Clamp(position.x, -boundsSize + radius, boundsSize - radius);
            }
            
            if (position.y - radius < -boundsSize || position.y + radius > boundsSize)
            {
                velocity.y = -velocity.y;
                position.y = Mathf.Clamp(position.y, -boundsSize + radius, boundsSize - radius);
            }
        }

        /// <summary>
        /// 获取对象的边界矩形
        /// </summary>
        public Rect GetBounds()
        {
            return new Rect(position.x - radius, position.y - radius, radius * 2f, radius * 2f);
        }

        /// <summary>
        /// 检查与另一个对象是否碰撞
        /// </summary>
        public bool CheckCollision(PhysicsObject other)
        {
            float distance = Vector2.Distance(position, other.position);
            return distance < (radius + other.radius);
        }

        /// <summary>
        /// 处理碰撞
        /// </summary>
        public void HandleCollision(PhysicsObject other)
        {
            if (isStatic && other.isStatic) return;
            
            Vector2 collisionNormal = (other.position - position).normalized;
            
            if (!isStatic && !other.isStatic)
            {
                // 两个动态对象的碰撞
                Vector2 relativeVelocity = other.velocity - velocity;
                float separatingVelocity = Vector2.Dot(relativeVelocity, collisionNormal);
                
                if (separatingVelocity > 0) return; // 对象正在分离
                
                float totalMass = mass + other.mass;
                float impulse = 2 * separatingVelocity / totalMass;
                
                velocity += impulse * other.mass * collisionNormal;
                other.velocity -= impulse * mass * collisionNormal;
            }
            else if (!isStatic)
            {
                // 动态对象与静态对象碰撞
                velocity = Vector2.Reflect(velocity, collisionNormal);
            }
            else if (!other.isStatic)
            {
                // 静态对象与动态对象碰撞
                other.velocity = Vector2.Reflect(other.velocity, -collisionNormal);
            }
            
            // 分离重叠的对象
            float overlap = (radius + other.radius) - Vector2.Distance(position, other.position);
            if (overlap > 0)
            {
                Vector2 separation = collisionNormal * (overlap / 2f);
                if (!isStatic) position -= separation;
                if (!other.isStatic) other.position += separation;
            }
        }

        /// <summary>
        /// 绘制对象（用于调试）
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = objectColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
