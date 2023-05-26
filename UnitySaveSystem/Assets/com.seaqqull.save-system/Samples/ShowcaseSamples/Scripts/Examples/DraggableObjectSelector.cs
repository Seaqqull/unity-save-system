using System.Collections.Generic;
using SaveSystem.Additional;
using UnityEngine;


namespace SaveSystem.Examples
{
    [CreateAssetMenu(fileName = "DraggableSelector", menuName = "Selector/Draggable", order = 0)]
    public class DraggableObjectSelector : ObjectSelector
    {
        public override IEnumerable<LocationItem>Select()
        {
            return FindObjectsOfType<DraggableEntity>();
        }
    }
}