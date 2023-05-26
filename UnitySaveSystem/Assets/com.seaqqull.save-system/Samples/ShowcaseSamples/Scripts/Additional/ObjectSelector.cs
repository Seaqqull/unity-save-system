using System.Collections.Generic;
using UnityEngine;


namespace SaveSystem.Additional
{
    public abstract class ObjectSelector : ScriptableObject
    {
        public abstract IEnumerable<LocationItem> Select();
    }
}