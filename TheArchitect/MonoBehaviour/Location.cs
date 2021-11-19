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

        this.m_Player.transform.localPosition = new Vector2(x, y);
    }

}
