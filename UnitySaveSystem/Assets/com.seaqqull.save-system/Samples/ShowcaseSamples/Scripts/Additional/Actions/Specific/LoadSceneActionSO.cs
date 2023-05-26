using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "SceneLoadAction", menuName = "Actions/Simple/LoadScene", order = 0)]
    public class LoadSceneActionSO : ActionSO
    {
        [SerializeField] private int _sceneId;

        
        private IEnumerator LoadSceneAsync(int id, Action onLoad)
        {
            var loadingOperation = SceneManager.LoadSceneAsync(id);

            while (!loadingOperation.isDone)
                yield return null;
            onLoad?.Invoke();
        }
        
        
        public override void Do()
        {
            SceneManager.LoadScene(_sceneId);
        }
        
        public override void Do(MonoBehaviour mono) =>
            Do();
        
        public void Do(MonoBehaviour mono, Action onLoad) =>
            mono.StartCoroutine(LoadSceneAsync(_sceneId, onLoad));
    }
}