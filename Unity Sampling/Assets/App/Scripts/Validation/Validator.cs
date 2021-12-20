using UnityEngine;

namespace SBaier.Sampling
{
    public interface Validator<T>
    {
        void Validate(T obj);
    }
}