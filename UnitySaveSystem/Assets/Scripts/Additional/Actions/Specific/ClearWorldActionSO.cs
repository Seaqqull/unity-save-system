using UnityEngine;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "ClearWorld", menuName = "Actions/Simple/ClearWorld", order = 0)]
    public class ClearWorldActionSO : ActionSO
    {
        public override void Do()
        {
            World.Instance.ClearAll();
        }

        public override void Do(MonoBehaviour mono)
        {
            Do();
        }
    }
}