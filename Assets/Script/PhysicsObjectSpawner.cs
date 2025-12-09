using UnityEngine;

namespace PhysicsOptimization
{
    /// <summary>
    /// 物理对象生成器 - 用于创建测试用的物理对象
    /// </summary>
    public class PhysicsObjectSpawner : MonoBehaviour
    {
        [Header("生成设置")]
        public GameObject physicsObjectPrefab;
        public int objectCount = 50;
        public float spawnRadius = 8f;
        public Vector2 velocityRange = new Vector2(1f, 5f);
        public Vector2 massRange = new Vector2(0.5f, 2f);
        public Vector2 sizeRange = new Vector2(0.2f, 0.8f);
        
        [Header("静态对象")]
        public int staticObjectCount = 10;
        public Color staticObjectColor = Color.blue;

        void Start()
        {
            SpawnObjects();
        }

        /// <summary>
        /// 生成物理对象
        /// </summary>
        public void SpawnObjects()
        {
            // 清除现有对象
            ClearExistingObjects();
            
            // 生成动态对象
            for (int i = 0; i < objectCount; i++)
            {
                SpawnRandomObject(false);
            }
            
            // 生成静态对象
            for (int i = 0; i < staticObjectCount; i++)
            {
                SpawnRandomObject(true);
            }
        }

        /// <summary>
        /// 生成随机对象
        /// </summary>
        private void SpawnRandomObject(bool isStatic)
        {
            GameObject obj;
            
            if (physicsObjectPrefab != null)
            {
                obj = Instantiate(physicsObjectPrefab);
            }
            else
            {
                // 如果没有预制体，创建默认对象
                obj = CreateDefaultPhysicsObject();
            }
            
            // 设置随机位置
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            obj.transform.position = randomPos;
            
            // 设置物理属性
            PhysicsObject physicsObj = obj.GetComponent<PhysicsObject>();
            if (physicsObj == null)
            {
                physicsObj = obj.AddComponent<PhysicsObject>();
            }
            
            physicsObj.isStatic = isStatic;
            physicsObj.mass = Random.Range(massRange.x, massRange.y);
            physicsObj.radius = Random.Range(sizeRange.x, sizeRange.y);
            
            if (isStatic)
            {
                physicsObj.velocity = Vector2.zero;
                physicsObj.objectColor = staticObjectColor;
            }
            else
            {
                // 设置随机速度
                float speed = Random.Range(velocityRange.x, velocityRange.y);
                Vector2 direction = Random.insideUnitCircle.normalized;
                physicsObj.velocity = direction * speed;
                physicsObj.objectColor = GetRandomColor();
            }
        }

        /// <summary>
        /// 创建默认物理对象
        /// </summary>
        private GameObject CreateDefaultPhysicsObject()
        {
            GameObject obj = new GameObject("PhysicsObject");
            
            // 添加视觉表示
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateCircleSprite();
            
            return obj;
        }

        /// <summary>
        /// 创建圆形精灵
        /// </summary>
        private Sprite CreateCircleSprite()
        {
            int size = 32;
            Texture2D texture = new Texture2D(size, size);
            Color[] colors = new Color[size * size];
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f - 1;
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    colors[x + y * size] = distance <= radius ? Color.white : Color.clear;
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 获取随机颜色
        /// </summary>
        private Color GetRandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        /// <summary>
        /// 清除现有对象
        /// </summary>
        private void ClearExistingObjects()
        {
            PhysicsObject[] existingObjects = FindObjectsOfType<PhysicsObject>();
            foreach (PhysicsObject obj in existingObjects)
            {
                if (obj.gameObject != gameObject)
                {
                    DestroyImmediate(obj.gameObject);
                }
            }
        }

        /// <summary>
        /// GUI控制面板
        /// </summary>
        void OnGUI()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 30;  
            labelStyle.fontStyle = FontStyle.Bold;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 24;  
            buttonStyle.fixedHeight = 60;  
            
            GUILayout.BeginArea(new Rect(Screen.width - 350, 20, 320, 400)); 
            
            GUILayout.Label("=== 对象生成器 ===", labelStyle);
            GUILayout.Space(15);
            
            GUILayout.Label($"动态对象: {objectCount}", labelStyle);
            objectCount = (int)GUILayout.HorizontalSlider(objectCount, 10, 200);
            GUILayout.Space(15);
            
            GUILayout.Label($"静态对象: {staticObjectCount}", labelStyle);
            staticObjectCount = (int)GUILayout.HorizontalSlider(staticObjectCount, 0, 50);
            GUILayout.Space(20);
            
            if (GUILayout.Button("重新生成对象", buttonStyle))
            {
                SpawnObjects();
            }
            GUILayout.Space(10);
            
            if (GUILayout.Button("清除所有对象", buttonStyle))
            {
                ClearExistingObjects();
            }
            
            GUILayout.EndArea();
        }
    }
}
