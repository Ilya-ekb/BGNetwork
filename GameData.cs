using System;
using Game.Contexts;
using Core;
using Events;

namespace Game.Networks
{
    [Serializable]
    public class GameData
    {
        public int scope;
        public int stationCount;
        public bool isGameplay;
        public bool isGameOver;
        
        private readonly IContext context;
        
        public GameData(IContext context)
        {
            this.context = context;
            GEvent.Attach(LocationEvents.GameOver, _ => isGameOver = true);
        }

        public GameData()
        {
            scope = 0;
            stationCount = 0;
        }
        
        public GameData Get()
        {
            return this;
        }

        public override string ToString()
        {
            return $"Scope: {scope} | Stations: {stationCount} | GameOver: {isGameOver} | GamePlay: {isGameplay}";
        }
    }
}