using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheArchitect.Game
{
    public class RuntimeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            Debug.Log("RuntimeInitialize");
            #if UNITY_EDITOR
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = 60;
            #endif
            // Game game = Resources.Load<Game>("ScriptableObjects/Game");

            // SceneManager.sceneLoaded += (scene, loadMode) => {
            //     Time.timeScale = 1;
            // };

            // SceneManager.sceneLoaded += (scene, loadMode) => {
            //     Time.timeScale = 1;
                
            //     if (scene.name=="Loading")
            //     {
            //         SceneLoaderBehaviour loader = GameObject.FindObjectOfType<SceneLoaderBehaviour>();
            //         loader.Load(game.LoadingStage);
            //         game.LoadingStage = null;
            //     }
            //     else if (scene.name!="Title" && scene.name!="Settings" && scene.name!="Gallery")
            //     {
            //         GameObject console = GameObject.Instantiate(Resources.Load<GameObject>(ResourcePaths.PREFAB_CONSOLE));
            //         console.name = "Console";
            //         GameObject eventSystem = GameObject.Instantiate(Resources.Load<GameObject>(ResourcePaths.PREFAB_EVENT_SYSTEM));
            //         eventSystem.name = "EventSystem";
            //         GameObject pause = GameObject.Instantiate(Resources.Load<GameObject>(ResourcePaths.PREFAB_PAUSE_CONTROLLER));
            //         pause.name = "Pause Controller";
            //         GameObject fade = GameObject.Instantiate(Resources.Load<GameObject>(ResourcePaths.PREFAB_FADE_FROM_BLACK));
            //         fade.name = "AutoFade";
            //         GameObject.Destroy(fade, 2);
            //     }
            // };

            // #if UNITY_EDITOR
            // if (game.EditorAutoNewGame) game.NewGame();
            // #endif
        }
        
    }
}