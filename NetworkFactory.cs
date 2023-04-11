using System;
using Game.Factories;
using Game.Contexts;
using Core.ObjectsSystem;
using Game.Networks;

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