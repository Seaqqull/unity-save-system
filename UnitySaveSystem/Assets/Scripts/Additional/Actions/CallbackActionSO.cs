using UnityEngine;
using System;


namespace SaveSystem.Additional.Actions
{
    public abstract class CallbackActionSO : ScriptableObject
    {
        public abstract void Do(MonoBehaviour mono, Action response);
    }
}