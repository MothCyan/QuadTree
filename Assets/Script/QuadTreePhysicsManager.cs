using System.Collections.Generic;
using UnityEngine;



namespace PhysicsOptimization
{
    /// <summary>
    /// 四叉树物理管理器 - 管理所有物理对象的碰撞检测
    /// </summary>
    public class QuadTreePhysicsManager : MonoBehaviour
    {
        [Header("四叉树设置")]
        public Vector2 worldSize = new Vector2(20f, 20f);
        public bool enableOptimization = true;
        public bool showQuadTreeBounds = true;
        
        [Header("性能统计")]
        public int totalObjects = 0;
        public int collisionChecks = 0;
        public float updateTime = 0f;
        
        private QuadTree quadTree;
        private List<PhysicsObject> allObjects = new List<PhysicsObject>();
        private List<PhysicsObject> retrievedObjects = new List<PhysicsObject>();

        void Start()
        {
            // 初始化四叉树，覆盖整个世界空间
            Rect worldBounds = new Rect(-worldSize.x / 2f, -worldSize.y / 2f, worldSize.x, worldSize.y);
            quadTree = new QuadTree(0, worldBounds);
        }

        void Update()
        {
            float startTime = Time.realtimeSinceStartup;
            
            // 收集所有物理对象
            CollectAllObjects();
            
            // 执行碰撞检测
            if (enableOptimization)
            {
                PerformOptimizedCollisionDetection();
            }
            else
            {
                PerformBruteForceCollisionDetection();
            }
            
            updateTime = (Time.realtimeSinceStartup - startTime) * 1000f; // 转换为毫秒
        }

        /// <summary>
        /// 收集场景中的所有物理对象
        /// </summary>
        private void CollectAllObjects()
        {
            allObjects.Clear();
            PhysicsObject[] objects = FindObjectsOfType<PhysicsObject>();
            allObjects.AddRange(objects);
            totalObjects = allObjects.Count;
        }

        /// <summary>
        /// 【核心优化算法】使用四叉树优化的碰撞检测
        /// 相比暴力法，将时间复杂度从O(n²)优化到O(n log n)
        /// </summary>
        private void PerformOptimizedCollisionDetection()
        {
            // 【第一步】清空并重建四叉树
            // 每帧都重建的原因：对象在移动，空间分布在变化
            quadTree.Clear();
            
            // 【第二步】将所有对象插入四叉树
            // 这个过程会自动根据对象位置进行空间分割
            foreach (PhysicsObject obj in allObjects)
            {
                quadTree.Insert(obj);  // 每个对象会被分配到合适的四叉树节点
            }
            
            collisionChecks = 0;  // 重置碰撞检查计数器
            
            // 【第三步】对每个对象执行优化的碰撞检测
            foreach (PhysicsObject obj in allObjects)
            {
                // 清空上次查询的结果列表（复用列表以减少GC）
                retrievedObjects.Clear();
                
                // 【关键优化】只获取可能与当前对象碰撞的邻近对象
                // 而不是检查所有对象！这是性能提升的核心
                quadTree.Retrieve(retrievedObjects, obj);
                
                // 只检查邻近对象的碰撞
                foreach (PhysicsObject other in retrievedObjects)
                {
                    if (obj != other)  // 避免对象与自己碰撞检测
                    {
                        collisionChecks++;  // 统计实际进行的碰撞检查次数
                        if (obj.CheckCollision(other))
                        {
                            obj.HandleCollision(other);
                        }
                    }
                }
            }
            
            
        }

        /// <summary>
        /// 暴力法碰撞检测（用于性能对比）
        /// </summary>
        private void PerformBruteForceCollisionDetection()
        {
            collisionChecks = 0;
            
            for (int i = 0; i < allObjects.Count; i++)
            {
                for (int j = i + 1; j < allObjects.Count; j++)
                {
                    collisionChecks++;
                    if (allObjects[i].CheckCollision(allObjects[j]))
                    {
                        allObjects[i].HandleCollision(allObjects[j]);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制调试信息
        /// </summary>
        void OnDrawGizmos()
        {
            if (showQuadTreeBounds && quadTree != null)
            {
                quadTree.DrawBounds();
            }
            
            // 绘制世界边界
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(worldSize.x, worldSize.y, 0));
        }

        /// <summary>
        /// 在GUI中显示性能信息
        /// </summary>
        void OnGUI()
        {           
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 24;  
            labelStyle.fontStyle = FontStyle.Bold;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20; 
            buttonStyle.fixedHeight = 50;  
            
            GUILayout.BeginArea(new Rect(20, 20, 550, 350)); 
            
            GUILayout.Label("=== 四叉树物理优化性能监控 ===", labelStyle);
            GUILayout.Space(15);
            
            GUILayout.Label($"物理对象数量: {totalObjects}", labelStyle);
            GUILayout.Space(5);
            
            GUILayout.Label($"碰撞检查次数: {collisionChecks}", labelStyle);
            GUILayout.Space(5);
            
            GUILayout.Label($"更新时间: {updateTime:F2} ms", labelStyle);
            GUILayout.Space(5);
            
            GUILayout.Label($"优化模式: {(enableOptimization ? "四叉树优化" : "暴力法检测")}", labelStyle);
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("切换优化模式", buttonStyle))
            {
                enableOptimization = !enableOptimization;
            }            
            GUILayout.Space(10);                  
            GUILayout.EndArea();
        }
    }
}
