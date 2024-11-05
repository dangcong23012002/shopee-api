
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class AdminResponsitory : IAdminResponsitory
{
    private readonly DatabaseContext _context;
    public AdminResponsitory(DatabaseContext context)
    {
        _context = context;
    }
    public IEnumerable<Order> getOrdersWaitSettlment()
    {
        return _context.Orders.FromSqlRaw("EXEC sp_GetOrderWaitSettlement");
    }

    public IEnumerable<Order> getOrdersPicking()
    {
        return _context.Orders.FromSqlRaw("EXEC sp_GetOrderPickingUp");
    }

    public IEnumerable<Order> getOrderDelivering()
    {
        return _context.Orders.FromSqlRaw("EXEC sp_GetOrderDelivering");
    }

    public IEnumerable<Order> getOrderCompleted()
    {
        return _context.Orders.FromSqlRaw("sp_GetOrderCompleted");
    }

    public IEnumerable<Order> getOrderWaitPickup()
    {
        return _context.Orders.FromSqlRaw("EXEC sp_GetOrderWaitPickup");
    }
}