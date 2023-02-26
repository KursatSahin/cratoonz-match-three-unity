using System;
using Bootsrapper;
using Containers;
using UnityEngine;
using static Containers.ContainerFacade;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _tileSize = 1.28f;
        [SerializeField] private float _boardScreenPercent = 94.81f;
        
        private BoardSettings _boardSettings;
        
        private float BoardScreenRatio => _boardScreenPercent / 100;

        private void Awake()
        {
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;
        }

        private void Start()
        {
            AdjustOrthographicCameraSize();
        }
        
        /// <summary>
        /// Adjust orthographic camera size with fixed tile size value
        /// </summary>
        private void AdjustOrthographicCameraSize()
        {
            float boardWidthInWorldSpace = _tileSize * _boardSettings.BoardWidth;
            float calculatedScreenWidthInWorldSpace = boardWidthInWorldSpace * 100;

            float screenRatio = (float) Screen.width / Screen.height;
            float targetRatio = (_boardSettings.BoardWidth * _tileSize) / (_boardSettings.BoardHeight * _tileSize); // 9/16 = 0.56

            if (screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = ((_boardSettings.BoardHeight * _tileSize) / BoardScreenRatio) / 2;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                Camera.main.orthographicSize = ((_boardSettings.BoardHeight * _tileSize) / BoardScreenRatio) / 2 * differenceInSize;
            }
        }
    }
}