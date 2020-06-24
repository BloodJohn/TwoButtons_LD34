using HECS.Components;
using UnityEngine;

namespace HECS.Systems
{
    [DefaultExecutionOrder(300)]
    public abstract class SystemContainer<T> : MonoBehaviour where T: ISystem 
    {
        [SerializeField] protected T GetSystem;

        private void Start()
        {
            var actor = GetComponent<IActor>();
            actor.AddHecsSystem(GetSystem);
        }
    }
}
