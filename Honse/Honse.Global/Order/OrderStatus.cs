
namespace Honse.Global.Order
{
    public enum OrderStatus
    {
        Unconfirmed = 0,
        New,
        Accepted,
        Delivery,
        Finished,
        Cancelled = -1
    }
}
