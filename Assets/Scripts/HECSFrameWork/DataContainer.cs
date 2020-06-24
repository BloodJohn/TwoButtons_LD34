using UnityEngine;

namespace HECS.Components
{
    public abstract class DataContainer<T> : ScriptableObject
    {
        [SerializeField] protected T data;
        public T Data => data;
    }
}
