using System;
using UnityEngine;

namespace Grid.Agent
{
    public class AgentColourChanger : MonoBehaviour
    {
        public GridAgent agent;
        public SpriteRenderer sprite;
        public Color uninitialisedColor = Color.red;
        public Color readyColor = Color.green;
        public Color movingColor = Color.blue;

        private void Start()
        {
            agent.OnStateChange += SetColour;
            SetColour(agent.State, agent.State);
        }

        public void SetColour(AgentState prev, AgentState current)
        {
            switch (current)
            {
                case AgentState.Uninitialised:
                    sprite.color = uninitialisedColor;
                    break;
                case AgentState.Ready:
                    sprite.color = readyColor;
                    break;
                case AgentState.Moving:
                    sprite.color = movingColor;
                    break;
            }
        }
    }
}
