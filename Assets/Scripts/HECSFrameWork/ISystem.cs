using HECS.Components;
using System;

namespace HECS.Systems
{
    public interface ISystem : ISimpleSystem
    {
        IEntity Entity { get; set; }
        void Command(ICommand command);
        void InitSystem();
    }

    public interface ISingleSystem : IEquatable<ISystem> 
    {
    }

    public interface ISimpleSystem : IDisposable
    {
        void UpdateLocal();
        void Pause();
        void UnPause();
    }
}