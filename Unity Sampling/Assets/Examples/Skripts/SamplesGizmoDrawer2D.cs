using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling.Examples
{
    public class SamplesGizmoDrawer2D : MonoBehaviour
    {
        [SerializeField]
        private int _samplesAmount = 10;
        [SerializeField]
        private BoundsSettings2D _boundsSettings;
        [SerializeField]
        private Vector2 _startPosition = new Vector2(5, 5);
        [SerializeField]
        private float _minimalDistance = 0.5f;
        [SerializeField]
        private float _maximalDistance = 0.7f;
        [SerializeField]
        private GameObject _sampleObject;
        [SerializeField]
        private GameObject _connectionObject;
        [SerializeField]
        private int _retries = 10;
        [SerializeField]
        private int _seed = 0;
        [SerializeField]
        private float _secondsTillSeedIncrease = 2;
        [SerializeField]
        private Color _gizmoColor = new Color(1, 0, 0, 0.25f);

        private List<Vector2> _samples = new List<Vector2>();
        private List<GameObject> _sampleObjects = new List<GameObject>();
        private List<GameObject> _connections = new List<GameObject>();
        private Bounds2D _bounds;

        private void Start()
		{
            _bounds = _boundsSettings.GetBounds();
            Create();
            InvokeRepeating(nameof(Recreate), _secondsTillSeedIncrease, _secondsTillSeedIncrease);
        }

        private void OnDrawGizmos()
        {
            if (_boundsSettings == null)
                return;

            Gizmos.color = _gizmoColor;
            DrawBounds();
        }

        private void DrawBounds()
		{
            Vector3 center = _boundsSettings.GetCenter();
            switch(_boundsSettings.BoundsType)
			{
                case BoundsSettings2D.Type.Rectangle:
                    Gizmos.DrawCube(_boundsSettings.GetCenter(), (_boundsSettings as RectangleBoundsSettings).Size);
                    break;
                default:
                    throw new NotImplementedException();
			}
        }

        private void Recreate()
		{
            _seed++;
            Clear();
            Create();
		}

        private void Create()
		{
            CreateSamples();
            CreateConnections();
        }

        private void CreateSamples()
		{
            _samples = new PoissonDiskSampling2D(new System.Random(_seed), _retries, new PoissonDiskSampling2DParameterValidator()).
                Sample(new PoissonDiskSampling2D.Parameters(_samplesAmount, _minimalDistance, _bounds, _startPosition));
            foreach (Vector3 sample in _samples)
            {
                _sampleObjects.Add(Instantiate(_sampleObject, sample, Quaternion.identity, transform));
            }
        }

		private void CreateConnections()
		{
            int amount = _samples.Count;
            for (int i = 0; i < amount; i++)
            {
                for (int j = i + 1; j < amount; j++)
                {
                    Vector2 first = _samples[i];
                    Vector2 second = _samples[j];
                    float distance = (first - second).magnitude;
                    if (distance > _maximalDistance)
                        continue;
                    Transform connection = Instantiate(_connectionObject, first, Quaternion.identity, transform).transform;
                    connection.LookAt(second);
                    connection.localScale = new Vector3(1, 1, distance);
                    _connections.Add(connection.gameObject);
                }
            }
        }

        private void Clear()
		{
            foreach (GameObject gO in _connections)
                Destroy(gO);
            foreach (GameObject gO in _sampleObjects)
                Destroy(gO);
            _connections.Clear();
            _sampleObjects.Clear();
        }
	}
}