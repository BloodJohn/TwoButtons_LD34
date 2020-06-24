namespace HECS.Components
{
    public interface ICommand
    {
        IEntity Owner { get; }
        IEntity Target { get; }
    }

    public interface IMoveCommand:ICommand { }

    public struct MoveCommand : IMoveCommand
    {
        public IEntity Owner { get; set; }
        public IEntity Target { get; set; }
    }
}