using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ListPositionProvider : PositionProvider
    {
        public List<Transform> Spawns = new List<Transform>();
        private int current;
        
        
        private void Awake()
        {
            Spawns.Shuffle();
            current = 0;
        }

        public override void Generate()
        {
            current++;
            if (current > Spawns.Count)
            {
                current = 0;
            }
        }
        
        public override Vector3 GetPosition()
        {
            return Spawns[current].position;
        }

        public override Vector3 GetRotation()
        {
            return Spawns[current].rotation.eulerAngles;
        }


        private void OnDrawGizmos()
        {
            foreach (var spawn in Spawns)
            {
                Gizmos.DrawSphere(spawn.position, 1);
            }
        }
    }

}

