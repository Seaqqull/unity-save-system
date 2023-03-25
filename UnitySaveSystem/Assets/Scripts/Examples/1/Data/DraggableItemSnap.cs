using SaveSystem.Data;
using UnityEngine;


namespace SaveSystem.Examples._1.Data
{
    [System.Serializable]
    public class DraggableItemSnap : LocationItemSnap
    {
        private float _positionX;
        private float _positionY;

        public Vector2 Position => new (_positionX, _positionY);


        public DraggableItemSnap(MonoBehaviour behaviour, Vector2 position, bool exists = true) : base(behaviour, exists)
        {
            _positionX = position.x;
            _positionY = position.y;
        }
        
        public DraggableItemSnap(string id, Vector2 position, bool exists = true) : base(id, exists)
        {
            _positionX = position.x;
            _positionY = position.y;
        }
    }
}