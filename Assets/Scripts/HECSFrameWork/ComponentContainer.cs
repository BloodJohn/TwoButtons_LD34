using UnityEngine;

namespace HECS.Components
{
    public abstract class ComponentContainer<T> : MonoBehaviour, IComponentContainer where T: IComponent
    {
        [SerializeField] protected T Value = default; 
        public IComponent GetHECSComponent => Value;

        private void Awake()
        {
            if (Value != null)
            {
                if (Value is INeedInit component)
                    component.Init();
            }
        }
    }
}