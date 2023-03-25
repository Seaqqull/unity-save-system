using UnityEngine;


namespace SaveSystem.Additional.Actions
{
    public abstract class ActionSO : ScriptableObject
    {
        public abstract void Do();
        public abstract void Do(MonoBehaviour mono);
    }
}