using GlobalCommander;
using HECS;
using HECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;

public class EntityManager : IDisposable
{
    private static HashSet<IEntity> entities;
    private static List<IEntity> selection = new List<IEntity>(500);
    private static List<INeedActualEntities> actualEntities = new List<INeedActualEntities>();
    private static List<IEntity> garbage = new List<IEntity>(200);

    public static IReadOnlyCollection<IEntity> Entities => entities;

    public static void SendCommand(ICommand command)
    {
        entities.FirstOrDefault(x => x == command.Target).SetCommand(command);
    }

    public static IEnumerable<T> GetEntities<T>() where T : IEntity
    {
        return entities.OfType<T>();
    }

    public static void GetAllComponentsByType<T>(ref List<T> components, ComponentID componentID) where T : IComponent
    {
        components.Clear();
        selection = GetAllEntitiesByComponents(componentID);

        foreach (var e in selection)
        {
            if (e.TryGetHecsComponent<T>(componentID, out var component))
                components.Add(component);
        }
    }

    public static void GetAllComponentsWithMask<T>(ref List<T> components, ComponentID componentID, params ComponentID[] mask) where T : IComponent
    {
        components.Clear();
        selection = GetAllEntitiesByComponents(mask);

        foreach (var e in selection)
        {
            if (e.TryGetHecsComponent<T>(componentID, out var component))
                components.Add(component);
        }
    }

    public static void GetAllComponentsWithMask<T>(ref List<T> components, ComponentID componentID, ComponentID[] additiveMask, ComponentID[] exceptionMask) where T : IComponent
    {
        components.Clear();
        selection = GetAllEntitiesByComponents(componentID, additiveMask, exceptionMask);

        foreach (var e in selection)
        {
            if (e.TryGetHecsComponent<T>(componentID, out var component))
                components.Add(component);
        }
    }

    public static void GetAllComponentsWithExceptionMask<T>(ref List<T> components, ComponentID componentID, params ComponentID[] mask) where T : IComponent
    {
        components.Clear();
        selection = GetAllEntitiesByComponentsExceptMask(new ComponentID[]{componentID}, mask);

        foreach (var e in selection)
        {
            if (e.TryGetHecsComponent<T>(componentID, out var component))
                components.Add(component);
        }
    }

    public static void GetEntityByComponents(out IEntity outEntity, params ComponentID[] componentIDs)
    {
        garbage.Clear();

        foreach (var e in entities)
        {
            if (e.IsHaveComponents(componentIDs))
                garbage.Add(e);
        }

        outEntity = garbage.FirstOrDefault();
    }

    public static List<IEntity> GetAllEntitiesByComponents(params ComponentID[] componentIDs)
    {
        garbage.Clear();

        foreach (var e in entities)
        {
            if (e.IsHaveComponents(componentIDs))
                garbage.Add(e);
        }

        return garbage;
    }

    public static List<IEntity> GetAllEntitiesByComponents(ComponentID neededComponent, ComponentID[] additiveMask, ComponentID[] exceptionMask)
    {
        garbage.Clear();

        foreach (var e in entities)
        {
            if (e.IsHaveComponents(neededComponent) && e.IsHaveComponents(additiveMask) && !e.IsHaveComponents(exceptionMask))
                garbage.Add(e);
        }

        return garbage;
    }

    public static List<IEntity> GetAllEntitiesByComponentsExceptMask(ComponentID[] componentIDs, ComponentID[] mask)
    {
        garbage.Clear();

        foreach (var e in entities)
        {
            if (e.IsHaveComponents(componentIDs) && !e.IsHaveComponents(mask))
                garbage.Add(e);
        }

        return garbage;
    }

    public EntityManager()
    {
        entities = new HashSet<IEntity>(new List<IEntity>(500));
        Commander.AddListener<Register<IEntity>>(this, RegisterReact);
        Commander.RecieveRegisterObject(this, actualEntities);
    }

    private void RegisterReact(Register<IEntity> obj)
    {
        entities.AddOrRemoveElement(obj.RegisterObject, obj.Add);

        foreach (var e in actualEntities)
            e.UpdateEntities();
    }

    public void Dispose()
    {
        Commander.ReleaseListener(this);
        entities.Clear();
        selection.Clear();
        garbage.Clear();
    }
}