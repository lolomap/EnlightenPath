using System;
using Events;
using NLog;
using UnityEngine;
using Zenject;

namespace Utilities
{
    public class Logger : IInitializable, IDisposable
    {
        public NLog.Logger Events { get; private set; }
        public NLog.Logger Systems { get; private set; }
        
        [Inject] private EventBus _eventBus;
        
        public void Initialize()
        {
            LogManager.Setup().LoadConfigurationFromFile(Application.streamingAssetsPath + "/NLog.config", false);
            LogManager.ReconfigExistingLoggers();

            Events = LogManager.GetLogger("events");
            Systems = LogManager.GetLogger("systems");
            Events.Trace($"{Application.productName} {Application.version}");
            
            _eventBus.SubscribeAll(LogEvent);
        }
    
        public void Dispose()
        {
            _eventBus.UnsubscribeAll(LogEvent);
            LogManager.Shutdown();
        }

        private void LogEvent(string sender, object payload, Type payloadType)
        {
            Events.Info("Event: {sender}. Data: {payload}", sender, payload);
        }
    }
}
