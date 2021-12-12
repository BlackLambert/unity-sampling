using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling.Examples
{
    public class SamplesGizmoDrawer : MonoBehaviour
    {
        [SerializeField]
        private int _samplesAmount = 10;
        [SerializeField]
        private Vector3 _bounds = new Vector3(10, 10, 10);
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

        private List<Vector3> _samples = new List<Vector3>();


        private void Start()
		{
            _samples = new PoissonDiskSampling3D(new System.Random(0), _retries).Sample(new PoissonDiskSampling3D.Parameters(_samplesAmount, _minimalDistance, _bounds, _bounds/2));
            foreach (Vector3 sample in _samples)
            {
                Instantiate(_sampleObject, sample, Quaternion.identity);
            }

            CreateConnections();
        }

		private void CreateConnections()
		{
            int amount = _samples.Count;
            for (int i = 0; i < amount; i++)
            {
                for (int j = i + 1; j < amount; j++)
                {
                    Vector3 first = _samples[i];
                    Vector3 second = _samples[j];
                    float distance = (first - second).magnitude;
                    if (distance > _maximalDistance)
                        continue;
                    Transform connection = Instantiate(_connectionObject, first, Quaternion.identity).transform;
                    connection.LookAt(second);
                    connection.localScale = new Vector3(1, 1, distance);
                }
            }
        }

		private void OnDrawGizmos()
		{
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(_bounds / 2, _bounds);
		}
	}
}