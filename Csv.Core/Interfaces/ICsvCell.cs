namespace Csv.Core.Interfaces
{
    public interface ICsvCell
    {
        object Value { get; }
        ICsvRow Row { get; }
        ICsvColumn Column { get; }
    }
}
