using Bootsrapper;
using Containers;
using Core.Services;
using UnityEngine;
using static Containers.ContainerFacade;

namespace Game
{
    public class BoardDrawHelper : IBoardDrawHelper
    {
        private BoardSettings _boardSettings;
        private Vector3 _originPosition;
        
        // [Space]
        // [SerializeField]
        // [SerializeField] private float _tileSize = 1.28f;

        public BoardDrawHelper()
        {
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;
            _originPosition = GetOriginPosition(_boardSettings.BoardHeight, _boardSettings.BoardWidth);
        }

        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _boardSettings.TileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _boardSettings.TileSize;

            if (rowCount % 2 == 0)
            {
                offsetY -= _boardSettings.TileSize / 2;
            }
            
            if (columnCount % 2 == 0)
            {
                offsetX -= _boardSettings.TileSize / 2;
            }
            
            return new Vector3(-offsetX, -offsetY);
        }
        
        public Vector3 GetWorldPosition(int rowIndex, int columnIndex)
        {
            return new Vector3(columnIndex, rowIndex) * _boardSettings.TileSize + _originPosition;
        }
    }

    public interface IBoardDrawHelper : IService

    {
    /// <summary>
    /// Get world position of gem according to board pivot point and tile size
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    /// <returns></returns>
    Vector3 GetWorldPosition(int rowIndex, int columnIndex);
    }

}