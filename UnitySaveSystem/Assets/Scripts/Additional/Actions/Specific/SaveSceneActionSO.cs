using UnityEngine;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "SceneSaveAction", menuName = "Actions/Simple/SaveScene", order = 0)]
    public class SceneSaveActionSO : ActionSO
    {
        public override void Do()
        {
            SaveManager.Instance.Save();
        }

        public override void Do(MonoBehaviour mono)
        {
            Do();
        }
    }
}