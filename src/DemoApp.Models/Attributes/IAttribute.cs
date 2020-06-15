namespace DemoApp.Models.Attributes
{
    public interface IAttribute<T>
    {
        T Value { get; }
    }
}
