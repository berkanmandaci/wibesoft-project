using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Managers;
using WibeSoft.Data.Models;

namespace WibeSoft.Features.Grid
{
    /// <summary>
    /// Represents a single cell in the grid system
    /// Manages cell type, state, and visual representation
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private Material _material;
        private Mesh _waterMesh;
        private Mesh _groundMesh;
        private Mesh _farmMesh;
        
        private LogManager _logger => LogManager.Instance;

        public int X { get; private set; }
        public int Y { get; private set; }
        public CellType Type { get; private set; }
        public CellState State { get; private set; }

        private void OnValidate()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        }

        public async UniTask Initialize(int x, int y, Material material, Mesh waterMesh, Mesh groundMesh, Mesh farmMesh)
        {
            X = x;
            Y = y;
            Type = CellType.Ground;
            State = CellState.Empty;

            // Store mesh references
            _material = material;
            _waterMesh = waterMesh;
            _groundMesh = groundMesh;
            _farmMesh = farmMesh;

            await SetupComponents();
            
            _logger.LogInfo($"Cell initialized at position ({X}, {Y})", "Cell");
        }

        private async UniTask SetupComponents()
        {
            // Validate components
            if (_meshFilter == null || _meshRenderer == null)
            {
                _logger.LogError($"Required components missing on cell ({X}, {Y})", "Cell");
                throw new System.Exception("Required components missing on cell!");
            }

            // Set material
            _meshRenderer.sharedMaterial = _material;
            
            // Set default mesh
            await SwitchToGround();
            
            await UniTask.CompletedTask;
        }

        public async UniTask LoadFromData(CellSaveData data)
        {
            // Parse and set type
            if (System.Enum.TryParse(data.Type, out CellType type))
            {
                switch (type)
                {
                    case CellType.Water:
                        await SwitchToWater();
                        break;
                    case CellType.Ground:
                        await SwitchToGround();
                        break;
                    case CellType.Farm:
                        await SwitchToFarm();
                        break;
                }
            }

            // Parse and set state
            if (System.Enum.TryParse(data.State, out CellState state))
            {
                UpdateState(state);
            }

            _logger.LogInfo($"Cell ({X}, {Y}) loaded from data: Type={Type}, State={State}", "Cell");
        }

        public async UniTask SwitchToWater()
        {
            Type = CellType.Water;
            _meshFilter.sharedMesh = _waterMesh;
            _logger.LogInfo($"Cell ({X}, {Y}) switched to Water", "Cell");
            await UniTask.CompletedTask;
        }

        public async UniTask SwitchToGround()
        {
            Type = CellType.Ground;
            _meshFilter.sharedMesh = _groundMesh;
            _logger.LogInfo($"Cell ({X}, {Y}) switched to Ground", "Cell");
            await UniTask.CompletedTask;
        }

        public async UniTask SwitchToFarm()
        {
            if (Type != CellType.Ground)
            {
                _logger.LogWarning($"Cannot switch cell ({X}, {Y}) to Farm: Cell must be Ground type first", "Cell");
                return;
            }

            Type = CellType.Farm;
            _meshFilter.sharedMesh = _farmMesh;
            _logger.LogInfo($"Cell ({X}, {Y}) switched to Farm", "Cell");
            await UniTask.CompletedTask;
        }

        public void UpdateState(CellState newState)
        {
            State = newState;
            _logger.LogInfo($"Cell ({X}, {Y}) state updated to {newState}", "Cell");
        }
    }

    public enum CellType
    {
        Water,
        Ground,
        Farm
    }

    public enum CellState
    {
        Empty,
        Growing,
        ReadyToHarvest
    }
} 