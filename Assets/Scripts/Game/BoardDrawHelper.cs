using UnityEngine;

namespace Game
{
    public class BoardDrawHelper : MonoBehaviour, IBoardDrawHelper
    {
        [Header("Board Settings Dependencies")]
        [SerializeField] private BoardSettings _boardSettings;
        [SerializeField] private float _tileSize = 1.28f;
        
        private Vector3 _originPosition;
        
        // [Space]
        // [SerializeField]
        // [SerializeField] private float _tileSize = 1.28f;

        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _tileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _tileSize;

            return new Vector3(-offsetX, offsetY);
        }
        
        public Vector3 GetWorldPosition(int rowIndex, int columnIndex)
        {
            return new Vector3(columnIndex, -rowIndex) * _tileSize + _originPosition;
        }
    }

    public interface IBoardDrawHelper

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