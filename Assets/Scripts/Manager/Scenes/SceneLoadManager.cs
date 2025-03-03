using System;
using System.Collections;
using System.Collections.Generic;
using TANK3.Data.StageManagement;
using TANK3.Manager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TANK3.Manager.Scene
{
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        //---PRIVATE---//
        Stack<string> sceneStack = new();
        Coroutine handleSceneTransitionCoroutine = null;

        //---PUBLIC---//
        public StageDataStore StageDataStore;
        public string currentStageName = null;
        public string currentSceneName = null;
        public StageData currentStageData;

        readonly public string FrontSceneName = "FrontScene";
        readonly public string BackSceneName = "BackScene";
        readonly public string FisrstSceneName = "FirstScene";
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { }; 

        //---METHODS---\//
        protected override void Awake()
        {
            base.Awake();
            currentStageData = StageDataStore.dataList[0];
            currentStageName = currentStageData.stageName;
            currentSceneName = FisrstSceneName;
        }

        protected override void Start()
        {
            base.Start();
        }
        public void GoBackScene()
        {
            if (string.IsNullOrEmpty(currentSceneName))
            {
                Debug.LogWarning("現在のシーン名が空です。戻れません。");
                return;
            }
            if (sceneStack.Count > 1)
            {
                sceneStack.Pop();
                string previousScene = sceneStack.Peek();
                if (string.IsNullOrEmpty(previousScene)) return;
                handleSceneTransitionCoroutine = StartCoroutine(HandleSceneLoad(previousScene));
            }
        }

        public Coroutine LoadAsync(string sceneName, StageData stageData, LoadSceneMode mode = LoadSceneMode.Single)
        {
            handleSceneTransitionCoroutine = StartCoroutine(HandleSceneLoad(sceneName, mode));
            currentStageName = stageData.stageName;
            currentSceneName = sceneName;
            sceneStack.Push(currentSceneName);

            return handleSceneTransitionCoroutine;
        }

        IEnumerator HandleSceneLoad(string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode = LoadSceneMode.Single)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            yield return loadOperation;

            if (loadOperation.isDone)
            {
                OnSceneLoaded?.Invoke(sceneName);
            }
            else
            {
                Debug.LogError("シーンロード中にエラーが発生しました: " + sceneName);
            }
        }

        public void ResetStack()
        {
            if (string.IsNullOrEmpty(currentStageName)) return;
            sceneStack.Clear();
            sceneStack.Push(currentStageName);
        }

        protected override void OnAwake()
        {
            string name = "SceneLoadManager";
            InitializeSingletonObject(name);
        }

        protected override void OnDestroy()
        {
            OnSceneLoaded -= HandleSceneLoaded;
            OnSceneUnloaded -= HandleSceneUnloaded;
            base.OnDestroy();
        }

        private void OnEnable()
        {
            OnSceneLoaded += HandleSceneLoaded;
            OnSceneUnloaded += HandleSceneUnloaded;
        }

        public void HandleSceneLoaded(string sceneName)
        {
            // シーンロード後に行いたい処理
            Debug.Log($"Scene loaded: {sceneName}");
        }

        public void HandleSceneUnloaded(string sceneName)
        {
            // シーンアンロード後に行いたい処理
            Debug.Log($"Scene unloaded: {sceneName}");
            // ここでUIの更新処理などを行う
        }

        
    }
}
