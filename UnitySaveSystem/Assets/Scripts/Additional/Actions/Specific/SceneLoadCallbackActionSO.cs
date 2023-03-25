using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;


namespace SaveSystem.Additional.Actions.Specific
{
    [CreateAssetMenu(fileName = "SceneLoadCallbackAction", menuName = "Actions/Callback/LoadScene", order = 0)]
    public class SceneLoadCallbackActionSO : CallbackActionSO
    {
        [SerializeField] private int _sceneId;
        
        
        private IEnumerator LoadSceneAsync(int id, Action onLoad)
        {
            var loadingOperation = SceneManager.LoadSceneAsync(id);

            while (!loadingOperation.isDone)
                yield return null;
            
            onLoad?.Invoke();
        }

        
        public override void Do(MonoBehaviour mono, Action response)
        {
            mono.StartCoroutine(LoadSceneAsync(_sceneId, response));
        }
    }
}