namespace HhhNetwork
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class AdditiveLevelLoader : SingletonMonoBehaviour<AdditiveLevelLoader>
    {
        [SerializeField]
        private string _sceneName = string.Empty;

        [SerializeField]
        private float _pollFrequency = 0.1f;

        public string sceneName
        {
            get { return _sceneName; }
            set { _sceneName = value; }
        }

        private IEnumerator Start()
        {
            var scene = SceneManager.GetSceneByName(_sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                Debug.Log(this.ToString() + " did not additively load a new scene, since the scene is already loaded == " + _sceneName);
                Destroy(this.gameObject, 0.1f);
            }
            else
            {
                var op = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
                if (op != null)
                {
                    op.allowSceneActivation = true;
                    while (!op.isDone)
                    {
                        yield return new WaitForSeconds(_pollFrequency);
                    }

                    Debug.Log(this.ToString() + " additively loaded new scene == " + _sceneName);
                    Destroy(this.gameObject, 0.1f);
                }
            }
        }
    }
}