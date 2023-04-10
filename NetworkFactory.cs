using System;
using BGCore.Game.Factories;
using Contexts;
using Core.ObjectsSystem;
using GameLogic.Networks;
using Networks;

namespace Factories
{
    public class NetworkFactory : IFactory
    {
        public Type SettingType => typeof(NetworkSetting);
        public IDroppable CreateItem<TConfig>(TConfig config, IContext context)
        {
            return config switch
            {
                EventSenderSetting setting => new EventSender(setting, context),
                HttpSetting setting => new HttpServer(setting, context),
                WebSetting setting => new WebServer(setting, context),
                _ => null,
            };
        }
    }
}