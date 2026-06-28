using System.Collections.Generic;
using UnityEngine;
using SliceAndStack.Core;

namespace SliceAndStack.Stacking
{
    public class TowerManager : MonoBehaviour
    {
        [SerializeField] private StackingConfig _config;
        [SerializeField] private Transform _towerBase;

        public float CurrentTowerHeight { get; private set; }

        private readonly List<StackedPiece> _stackedPieces = new();
        private TowerStabilityMonitor _stabilityMonitor;
        private float _snapshotTimer;
        private TowerSnapshot _lastSnapshot;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _stabilityMonitor = GetComponent<TowerStabilityMonitor>();
            if (_stabilityMonitor == null)
                _stabilityMonitor = gameObject.AddComponent<TowerStabilityMonitor>();
        }

        private void Start()
        {
            if (_towerBase != null)
            {
                _stabilityMonitor.Initialize(_stackedPieces, _towerBase.position.x);
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;

            UpdateTowerHeight();
            UpdateSnapshot();
        }

        private void UpdateTowerHeight()
        {
            float maxY = Utils.Constants.TOWER_BASE_Y;
            foreach (var piece in _stackedPieces)
            {
                if (piece != null && piece.transform.position.y > maxY)
                    maxY = piece.transform.position.y;
            }
            CurrentTowerHeight = maxY - Utils.Constants.TOWER_BASE_Y;
        }

        private void UpdateSnapshot()
        {
            _snapshotTimer += Time.deltaTime;
            if (_snapshotTimer >= _config.snapshotInterval)
            {
                _snapshotTimer = 0f;
                TakeSnapshot();
            }
        }

        public void RegisterPiece(StackedPiece piece)
        {
            if (!_stackedPieces.Contains(piece))
            {
                _stackedPieces.Add(piece);
            }
        }

        public void UnregisterPiece(StackedPiece piece)
        {
            _stackedPieces.Remove(piece);
        }

        public void ResetTower()
        {
            foreach (var piece in _stackedPieces)
            {
                if (piece != null)
                {
                    if (ServiceLocator.TryGet<Spawning.ObjectPool>(out var pool))
                        pool.Return(piece.gameObject);
                    else
                        Destroy(piece.gameObject);
                }
            }
            _stackedPieces.Clear();
            CurrentTowerHeight = 0f;
            _snapshotTimer = 0f;
            _lastSnapshot = null;

            if (_stabilityMonitor != null)
                _stabilityMonitor.Reset();
        }

        private void TakeSnapshot()
        {
            var snapshot = new TowerSnapshot();
            foreach (var piece in _stackedPieces)
            {
                if (piece != null)
                {
                    snapshot.Pieces.Add(new TowerSnapshot.PieceEntry
                    {
                        Piece = piece,
                        Snapshot = piece.TakeSnapshot()
                    });
                }
            }
            _lastSnapshot = snapshot;
        }

        public void RestorePreCollapseState()
        {
            if (_lastSnapshot == null) return;

            // Remove pieces that aren't in the snapshot
            var snapshotPieces = new HashSet<StackedPiece>();
            foreach (var entry in _lastSnapshot.Pieces)
            {
                if (entry.Piece != null)
                    snapshotPieces.Add(entry.Piece);
            }

            for (int i = _stackedPieces.Count - 1; i >= 0; i--)
            {
                if (_stackedPieces[i] != null && !snapshotPieces.Contains(_stackedPieces[i]))
                {
                    if (ServiceLocator.TryGet<Spawning.ObjectPool>(out var pool))
                        pool.Return(_stackedPieces[i].gameObject);
                    else
                        Destroy(_stackedPieces[i].gameObject);
                    _stackedPieces.RemoveAt(i);
                }
            }

            // Restore snapshot state
            foreach (var entry in _lastSnapshot.Pieces)
            {
                if (entry.Piece != null)
                {
                    entry.Piece.RestoreSnapshot(entry.Snapshot);
                }
            }

            if (_stabilityMonitor != null)
                _stabilityMonitor.Reset();
        }

        public float GetStability()
        {
            return _stabilityMonitor != null ? _stabilityMonitor.CurrentStability : 1f;
        }

        public StabilityLevel GetStabilityLevel()
        {
            return _stabilityMonitor != null ? _stabilityMonitor.CurrentLevel : StabilityLevel.Normal;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<TowerManager>();
        }
    }

    public class TowerSnapshot
    {
        public List<PieceEntry> Pieces = new();

        public struct PieceEntry
        {
            public StackedPiece Piece;
            public PieceSnapshot Snapshot;
        }
    }
}
