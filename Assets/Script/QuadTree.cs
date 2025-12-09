using System.Collections.Generic;
using UnityEngine;

namespace PhysicsOptimization
{
    
    public class QuadTree
    {
        private const int MAX_OBJECTS = 10;  
        private const int MAX_LEVELS = 5;    
        
        private int level;
        private List<PhysicsObject> objects;
        private Rect bounds;
        private QuadTree[] nodes;

        public QuadTree(int pLevel, Rect pBounds)
        {
            level = pLevel;
            objects = new List<PhysicsObject>();
            bounds = pBounds;
            nodes = new QuadTree[4];
        }
       
        public void Clear()
        {
            objects.Clear();
            
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }
        
        private void Split()
        {
            
            float subWidth = bounds.width / 2f;
            float subHeight = bounds.height / 2f;
            float x = bounds.x;  
            float y = bounds.y;  
            
            nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));        
            nodes[1] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));                      
        }

        
        private int GetIndex(Rect rect)
        {
            int index = -1;
            float verticalMidpoint = bounds.x + (bounds.width / 2f);
            float horizontalMidpoint = bounds.y + (bounds.height / 2f);
            bool topQuadrant = (rect.y < horizontalMidpoint && rect.y + rect.height < horizontalMidpoint);
            bool bottomQuadrant = (rect.y > horizontalMidpoint);
            if (rect.x < verticalMidpoint && rect.x + rect.width < verticalMidpoint)
            {
                if (topQuadrant)
                    index = 1;     
                else if (bottomQuadrant)
                    index = 2;      
            }
            else if (rect.x > verticalMidpoint)
            {
                if (topQuadrant)
                    index = 0;      
                else if (bottomQuadrant)
                    index = 3;      
            }
            return index;
            
        }

        
        public void Insert(PhysicsObject physicsObject)
        {
            
            if (nodes[0] != null)
            {
                int index = GetIndex(physicsObject.GetBounds());

                if (index != -1)  
                {
                    nodes[index].Insert(physicsObject);  
                    return; 
                }
            }
           
            objects.Add(physicsObject);
            
            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                
                if (nodes[0] == null)
                {
                    Split();  
                }
                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i].GetBounds());
                    if (index != -1)  
                    {                        
                        nodes[index].Insert(objects[i]);
                        objects.RemoveAt(i);    
                    }
                    else
                    {
                        i++;  
                    }
                }
            }           
        }

        
        public List<PhysicsObject> Retrieve(List<PhysicsObject> returnObjects, PhysicsObject physicsObject)
        {
            
            int index = GetIndex(physicsObject.GetBounds());
            
            
            if (index != -1 && nodes[0] != null)
            {
                
                nodes[index].Retrieve(returnObjects, physicsObject);
            }
         
            returnObjects.AddRange(objects);

            return returnObjects;
            
        
        }

        
        public void DrawBounds()
        {
            Debug.DrawLine(new Vector3(bounds.x, bounds.y, 0), new Vector3(bounds.x + bounds.width, bounds.y, 0), Color.white);
            Debug.DrawLine(new Vector3(bounds.x + bounds.width, bounds.y, 0), new Vector3(bounds.x + bounds.width, bounds.y + bounds.height, 0), Color.white);
            Debug.DrawLine(new Vector3(bounds.x + bounds.width, bounds.y + bounds.height, 0), new Vector3(bounds.x, bounds.y + bounds.height, 0), Color.white);
            Debug.DrawLine(new Vector3(bounds.x, bounds.y + bounds.height, 0), new Vector3(bounds.x, bounds.y, 0), Color.white);

            if (nodes[0] != null)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].DrawBounds();
                }
            }
        }
    }
}
