using Containers;
using Core.Events;
using Core.Services;
using UnityEngine;

namespace Bootsrapper
{
    public class AppBootstrapper
    {
        private const int TweenersCapacity = 1024;
        private const int SequencesCapacity = 256;
        
        private static readonly ServiceLocator _serviceLocator = ServiceLocator.Instance;
        public static ContainerProvider Containers { get; private set; }
        
        /// <summary>
        /// This function is called AfterAssembliesLoaded automatically with this attribute
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void AfterAssembliesLoaded()
        {
            DG.Tweening.DOTween.defaultRecyclable = true;
            DG.Tweening.DOTween.SetTweensCapacity(TweenersCapacity, SequencesCapacity);

            Containers = new ContainerProvider();
            
            RegisterEventDispatcher();
        }
        
        private static void RegisterEventDispatcher()
        {
            var eventDispatcher = new EventDispatcher();
            _serviceLocator.RegisterService<IEventDispatcher>(eventDispatcher);
        }
    }
}