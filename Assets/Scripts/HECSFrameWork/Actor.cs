using GlobalCommander;
using HECS.Systems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HECS.Components
{
    [DefaultExecutionOrder(-800)]
    public class Actor : MonoBehaviour, IActor, IUpdatable
    {
        [SerializeField] private int uID = 0;
        [SerializeField] private string gID = string.Empty;

        private HashSet<ISystem> systems = new HashSet<ISystem>();
        private HashSet<IComponent> components = new HashSet<IComponent>();
        public HashSet<IComponent> GetAllComponents => components;
        public ComponentID[] ComponentsMask { get; protected set; } = HECSHelper.Components();
        public int UID { get => uID; protected set => uID = value; }
        public string GID => gID;
        public Queue<ICommand> commands = new Queue<ICommand>(50);

        public void AddHecsComponent(IComponent component)
        {
            if (component == null)
            {
                Debug.LogAssertion($"пустой компонент прилетел " + gameObject.name);
                return;
            }

            if (IsHaveComponents(component.TypeID))
            {
                if (component is ISingleComponent)
                {
                    Debug.LogAssertion("добвлаяем дважды компонент " + component.TypeID);
                    return;
                }
            }
            else
                ComponentsMask[(int)component.TypeID] = component.TypeID;


            component.Owner = this;
            components.Add(component);
        }

        public bool TryGetHecsComponent<T>(ComponentID iD, out T component) where T : IComponent
        {
            foreach (var c in components)
            {
                if (c.TypeID == iD)
                {
                    if (c is T need)
                    {
                        component = need;
                        return true;
                    }
                    Debug.LogAssertion("неправильный айди у компонента " + c.ToString());
                    component = default;
                    return false;
                }
            }

            component = default;
            return false;
        }

        public void GetHecsComponents<T>(ref List<T> outComponents) where T : IComponent
        {
            outComponents.Clear();
            foreach (var c in components)
            {
                if (c is T needed)
                    outComponents.Add(needed);
            }
        }

        private void Awake()
        {
            Commander.RegisterObjectByEvent<ISaveble>(this, true);
            var currentComponents = GetComponents<IComponentContainer>();
            var currentPureComponents = GetComponents<IComponent>();

            foreach (var c in currentPureComponents)
                AddHecsComponent(c);

            foreach (var c in currentComponents)
            {
                AddHecsComponent(c.GetHECSComponent);
                c.GetHECSComponent.Owner = this;
            }
        }

        protected virtual void Start()
        {
            Commander.RegisterObjectByEvent<IUpdatable>(this, true);
            Commander.RegisterObjectByEvent<IEntity>(this, true);

            RegisterAll();
        }

        public void RegisterAll()
        {
            var currentSystems = GetComponents<ISystem>();

            foreach (var b in currentSystems)
            {
                b.Entity = this;
                AddHecsSystem(b);
            }

            Init();
        }

        protected virtual void Init()
        {
        }

        private void OnDestroy()
        {
            Commander.RegisterObjectByEvent<IUpdatable>(this, false);
            Commander.RegisterObjectByEvent<IEntity>(this, false);
            Commander.RegisterObjectByEvent<ISaveble>(this, false);

            Dispose();
        }

        public virtual void SetCommand(ICommand command)
        {
            commands.Enqueue(command);
        }

        public virtual void UpdateLocal()
        {
            DispatchCommads();
        }

        private void DispatchCommads()
        {
            if (commands.Count == 0)
                return;

            var command = commands.Dequeue();

            foreach (var sys in systems)
                sys.Command(command);
        }

        public void RemoveHecsComponent(IComponent component)
        {
            components.AddOrRemoveElement(component, false);
            if (components.Any(x => x.TypeID == component.TypeID))
                return;
            else
                ComponentsMask[(int)component.TypeID] = ComponentID.Default;
        }

        public bool IsHaveComponents(params ComponentID[] componentIDs)
        {
            if (componentIDs == null || componentIDs.Length == 0)
                return false;

            for (var x = 0; x < componentIDs.Length; ++x)
            {
                if (ComponentsMask[(int)componentIDs[x]] == ComponentID.Default)
                    return false;
            }

            return true;
        }

        private void Reset()
        {
            uID = Mathf.Abs(GetHashCode() * UnityEngine.Random.Range(0, 100000));
            gID = gameObject.name;
        }

        public bool IsHaveComponents(ComponentID mask)
        {
            return ComponentsMask.Contains(mask);
        }

        public void Save(SaveManager saveManager)
        {
            saveManager.SaveEntity(this);
        }

        public void Load(SaveManager saveManager)
        {
            //if (saveManager.TryGetSavedEntity(UID, out var save))
            //{
            //    var components = JsonConvert.DeserializeObject<List<IComponent>>(save, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            //    this.components.Clear();
            //    Array.Clear(ComponentsMask, 0, ComponentsMask.Length);

            //    foreach (var c in components)
            //        AddHecsComponent(c);
            //}

            //RegisterAll();
        }

        public void AddHecsSystem<T>(T system) where T : ISystem
        {
            system.Entity = this;

            var count = systems.Count;

            if (system is ISingleSystem singleSystem)
            {
                if (systems.Any(x => singleSystem.Equals(x)))
                {
                    Debug.LogAssertion("такой тип системы уже есть " + singleSystem.ToString());
                    return;
                }

            }
            systems.Add(system);
            system.InitSystem();
        }

        public new bool TryGetComponent<T>(out T component)
        {
            component = GetComponent<T>();
            return component != null;
        }

        public bool TryGetComponents<T>(out T[] components)
        {
            components = GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }

        public void Pause()
        {
            foreach (var s in systems)
                s.Pause();
        }

        public void UnPause()
        {
            foreach (var s in systems)
                s.UnPause();
        }

        public virtual void Dispose()
        {
            foreach (var s in systems)
                s.Dispose();
        }

        public void ReplaceSystem<T>(T system) where T : ISystem
        {
            foreach (var s in systems)
            {
                if (s is T)
                {
                    s.Dispose();
                    Commander.Invoke(new DispatchGlobalCommand { Action = () => systems.Remove(s) });
                    AddHecsSystem(system);
                }
            }
        }

        public T GetSystem<T>() where T : ISystem
        {
            foreach (var s in systems)
            {
                if (s is T casted)
                    return casted;
            }
            return default;
        }

        public void GetSystems<T>(ref List<T> addSystems) where T : ISystem 
        {
            foreach (var s in systems)
            {
                if (s is T casted)
                    addSystems.Add(casted);
            }
        }
    }
}