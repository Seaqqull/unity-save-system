using UnityEditor;
using UnityEngine;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "CloseApplication", menuName = "Actions/Simple/CloseApplication", order = 0)]
    public class CloseApplicationSO : ActionSO
    {
        public override void Do()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public override void Do(MonoBehaviour mono)
        {
            Do();
        }
    }
}