namespace Honse.Resources.Interface
{
    public interface IItemResource : IResource<Item>
    {

    }

    public class Item : Entity
    {
        string Name { get; set; } = string.Empty;

        decimal Price { get; set; }


    }
}
