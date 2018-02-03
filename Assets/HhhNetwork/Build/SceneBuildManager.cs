namespace HhhNetwork
{
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed class SceneBuildManager : SingletonMonoBehaviour<SceneBuildManager>
    {
        [Header("This component is automatically populated with the list of scenes from the custom Build Window. Should usually not be modified manually.")]
        [SerializeField]
        private List<SceneBuildItem> _scenes = new List<SceneBuildItem>();

        public List<SceneBuildItem> scenes
        {
            get { return _scenes; }
        }
    }
}