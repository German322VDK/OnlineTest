using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class SceneTranspositionHandler : MonoBehaviour
    {
        public static SceneTranspositionHandler SceneTranspositionHandlerInst {  get; internal set; }

        [SerializeField] private string defaultMainMenu = "StartMenu";

        public enum SceneStates
        {
            Init,
            Start,
            Lobby,
            InGame
        }

        private SceneStates _sceneState;

        [HideInInspector]
        public delegate void SceneStateChangedDelegateHandler(SceneStates newState);

        [HideInInspector]
        public event SceneStateChangedDelegateHandler OnSceneStateChanged;

        [HideInInspector]
        public delegate void ClientLoadedSceneDelegateHandler(ulong clientId);

        [HideInInspector]
        public event ClientLoadedSceneDelegateHandler OnClientLoadedScene;

        private int _numberOfClientLoaded;

        private void Awake()
        {
            if(SceneTranspositionHandlerInst != null && SceneTranspositionHandlerInst != this)
            {
                Destroy(SceneTranspositionHandlerInst.gameObject);
            }

            SceneTranspositionHandlerInst = this;

            SetSceneState(SceneStates.Init);
            DontDestroyOnLoad(this);
        }


        private void Start()
        {
            if(_sceneState == SceneStates.Init)
            {
                SceneManager.LoadScene(defaultMainMenu);
            }
        }


        private void SetSceneState(SceneStates sceneState)
        {
            _sceneState = sceneState;

            if(OnSceneStateChanged != null)
            {
                OnSceneStateChanged.Invoke(_sceneState);
            }
        }

        public SceneStates GetCurrnetSceneState()
        {
            return _sceneState;
        }

        public void RegisterCallBacks()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            _numberOfClientLoaded += 1;
            print(clientId);
            OnClientLoadedScene?.Invoke(clientId);
        }

        public bool AllClientAreLoaded()
        {
            return _numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count;
        }

        public void SwitchScene(string sceneName)
        {
            if(NetworkManager.Singleton.IsListening)
            {
                _numberOfClientLoaded = 0;

                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName);
            }
        }

        public void ExitAndLoadStartMenu()
        {
            if(NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
                SetSceneState(SceneStates.Start);
                SceneManager.LoadScene(1);
            }
        }
    }
}