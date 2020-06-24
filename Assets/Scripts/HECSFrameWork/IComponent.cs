using System;

namespace HECS.Components
{
    public interface IComponent
    {
        IEntity Owner { get; set; }
        ComponentID TypeID { get; }
    }

    public interface IExecutableComponent
    {
        void Execute();
    }

    public interface ISerializeData
    {
        string Serialize();
        string Desirialize();
    }

    public enum ComponentID
    {
        Default = 0,
        MoveSpeedComponent = 1,
        PlayerTagID = 2,
        HealthComponentID = 3,
        EnemyTagComponentID = 4,
        TransformComponentID = 5,
        TargetComponentID = 6,
        DamageComponentID = 7,
        AttackComponentID = 8,
        CurrentAttackComponentID = 9,
        DamagableComponentID = 10,
        SoundComponentID = 11,
        AfterDeathComponentID = 12,
        SlowComponentID = 13,
        MultipleAttackID = 14,
        StopSpawnComponentID = 15,
        CollisionComponentID = 16,
    }
}