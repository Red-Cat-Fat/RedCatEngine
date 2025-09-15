using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedCatEngine.CommonServices.Services.RandomServices
{
    public class UnityRandomService : IRandomService
    {
        public void SetSeed(int seed)
        {
            Random.InitState(seed);
        }

        public int GetRange(int minInclude, int maxExecute)
            => Random.Range(minInclude, maxExecute);

        public float GetRange(float minInclude, float maxExecute)
            => Random.Range(minInclude, maxExecute);

        public float ErrorRate(float baseValue, float error)
            => baseValue + Random.Range(-error, error);

        public float ErrorRate(
            float baseValue,
            float error,
            float min,
            float max
        )
        {
            return Mathf.Clamp(ErrorRate(baseValue, error), min, max);
        }
    }
}