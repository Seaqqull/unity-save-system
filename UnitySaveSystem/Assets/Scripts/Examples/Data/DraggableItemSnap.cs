using Newtonsoft.Json;
using SaveSystem.Data;
using UnityEngine;


namespace SaveSystem.Examples.Data
{
    [System.Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class DraggableItemSnap : LocationItemSnap
    {
        [JsonProperty] private float _positionX;
        [JsonProperty] private float _positionY;

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

        public DraggableItemSnap() { }
    }
}