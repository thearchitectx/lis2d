using UnityEngine;
using UnityEngine.AddressableAssets;
using TheArchitect.XMLScript;

namespace TheArchitect.Game
{

    public class XMLScriptBootstrap : UnityEngine.MonoBehaviour
    {

        [SerializeField] private AssetReferenceGameContext m_Context;

        void Start()
        {
            this.m_Context.LoadAssetAsync<GameContext>().Completed += (handle) => {
                var gameContext = handle.Result;
                
                string startScriptPath = gameContext.GetVariable(
                    GameState.SYSTEM_SCRIPT_PATH_VARIABLE,
                    gameContext.DefaultStartScript
                );
                string startNode = gameContext.GetVariable(
                    GameState.SYSTEM_SCRIPT_NODE_VARIABLE,
                    ""
                );

                Debug.Log($"Bootstraping ({startScriptPath}:{startNode})");
                
                GameObject startScriptObject = new GameObject("BootstrapScript");
                startScriptObject.SetActive(false);
                XMLScriptController controller = startScriptObject.AddComponent<XMLScriptController>();
                controller.PreStartConfigure(startScriptPath, startNode, gameContext);
                startScriptObject.SetActive(true);

                Destroy(this.gameObject);
            };
        }

    }

}
