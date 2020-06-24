
using HECS.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HECS.Components
{
    public interface IEntity
    {
        HashSet<IComponent> GetAllComponents { get; }
        
        bool TryGetHecsComponent<T>(ComponentID iD, out T component) where T: IComponent;
        bool TryGetComponent<T>(out T component);
        bool TryGetComponents<T>(out T[] components);
        void GetHecsComponents<T>(ref List<T> component) where T : IComponent;
        
        void AddHecsComponent(IComponent component);
        void AddHecsSystem<T>(T system) where T : ISystem;
        
        void RemoveHecsComponent(IComponent component);

        void SetCommand(ICommand command);
        T GetSystem<T>() where T : ISystem;
        void GetSystems<T>(ref List<T> addSystemHere) where T : ISystem;

        void ReplaceSystem<T>(T system) where T : ISystem;

        ComponentID[] ComponentsMask { get; }
        bool IsHaveComponents(params ComponentID[] componentIDs);
    }
}