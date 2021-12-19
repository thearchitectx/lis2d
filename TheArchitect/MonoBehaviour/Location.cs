using UnityEngine;
using TheArchitect.Game;
using TheArchitect.SceneObjects;

public class Location : SceneObject
{
    [SerializeField] private GameContext m_GameContext;
    [SerializeField] private PlayerController m_Player;

    void Start()
    {

        float x = this.m_GameContext.GetVariable(GameState.SYSTEM_PLAYER_X, this.m_Player.transform.localPosition.x);
        float y = this.m_GameContext.GetVariable(GameState.SYSTEM_PLAYER_Y, this.m_Player.transform.localPosition.y);
        var rot = Quaternion.identity;

        string spawn = this.m_GameContext.GetVariable(GameState.SYSTEM_PLAYER_SPAWN, "");
        if (!string.IsNullOrEmpty(spawn))
        {
            this.m_GameContext.UnsetVariable(GameState.SYSTEM_PLAYER_SPAWN);
            var t = this.transform.Find(spawn);
            if (t != null)
            {
                x = t.position.x;
                y = t.position.y;
                rot = t.rotation;
            }
        }

        this.m_Player.transform.localPosition = new Vector2(x, y);
        this.m_Player.LookAt(rot);
    }

}
