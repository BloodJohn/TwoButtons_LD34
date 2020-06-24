using HECS.Systems;
using System;

namespace HECS.Components
{
    public interface IActor : IEntity, ISaveble, IDisposable
    {
        void Pause();
        void UnPause();
    }

    public interface ISaveble
    {
        int UID { get; } //уникальный айдишник сущности
        string GID { get; } //уникальный айдишник префаба

        void Save(SaveManager saveManager);
        void Load(SaveManager saveManager);
    }

    public struct SaveGlobalCommand { }

    [System.Serializable]
    public class SaveEntityContainer
    {
        public int ID;
        public string GID;
        public string Save;
    }
}