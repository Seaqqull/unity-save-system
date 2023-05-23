using SaveSystem.Data;
using UnityEngine;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "SceneSaveAction", menuName = "Actions/Simple/SaveScene", order = 0)]
    public class SaveSceneActionSO : ActionSO
    {
        [SerializeField] private SaveType _saveType = SaveType.Ordinal;
        
        public override void Do()
        {
            SaveManager.Instance.Save(_saveType);
        }

        public override void Do(MonoBehaviour mono)
        {
            Do();
        }
    }
}