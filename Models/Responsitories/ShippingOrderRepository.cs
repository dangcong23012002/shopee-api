using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class ShippingOrderRepository : IShippingOrderRepository
{
    private readonly DatabaseContext _context;

    public ShippingOrderRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<ShippingDelivery> getShippingDeliveredByOrderID(int orderID)
    {
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        return _context.ShippingDeliveries.FromSqlRaw("EXEC sp_GetShippingDeliveredByOrderID @FK_iOrderID", orderIDParam);
    }

    public IEnumerable<ShippingDelivery> getShippingDeliveryByDeliverID(int deliverID)
    {
        SqlParameter deliverIDParam = new SqlParameter("@FK_iUserID", deliverID);
        return _context.ShippingDeliveries.FromSqlRaw("EXEC sp_GetShippingDeliveryByDeliverID @FK_iUserID", deliverIDParam);
    }

    public IEnumerable<ShippingDelivery> getShippingDeliveryByOrderID(int orderID)
    {
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        return _context.ShippingDeliveries.FromSqlRaw("EXEC sp_GetShippingDeliveryByOrderID @FK_iOrderID", orderIDParam);
    }

    public IEnumerable<ShippingDelivery> getShippingDeliveryCompleteByDeliverID(int deliverID)
    {
        SqlParameter deliverIDParam = new SqlParameter("@FK_iUserID", deliverID);
        return _context.ShippingDeliveries.FromSqlRaw("EXEC sp_GetShippingDeliveryCompleteByDeliverID @FK_iUserID", deliverIDParam);
    }

    public IEnumerable<ShippingOrder> getShippingOrderByID(int shippingOrderID)
    {
        SqlParameter shippingOrderIDParam = new SqlParameter("@PK_iShippingOrderID", shippingOrderID);
        return _context.ShippingOrders.FromSqlRaw("EXEC sp_GetShippingOrderByID @PK_iShippingOrderID", shippingOrderIDParam);
    }

    public IEnumerable<ShippingOrder> getShippingOrderByOrderID(int orderID)
    {
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        return _context.ShippingOrders.FromSqlRaw("EXEC sp_GetShippingOrderByOrderID @FK_iOrderID", orderIDParam);
    }

    public IEnumerable<ShippingOrder> getShippingOrderByShopID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@FK_iShopID", shopID);
        return _context.ShippingOrders.FromSqlRaw("EXEC sp_GetShippingOrderByShopID @FK_iShopID", shopIDParam);
    }

    public IEnumerable<ShippingOrder> getShippingOrders()
    {
        return _context.ShippingOrders.FromSqlRaw("EXEC sp_GetShippingOrders");
    }

    public IEnumerable<ShippingPicker> getShippingPickerAboutedWarehouse()
    {
        return _context.ShippingPickers.FromSqlRaw("EXEC sp_GetShippingPickerAboutedWarehouse");
    }

    public IEnumerable<ShippingPicker> getShippingPickerByID(int shippingPickerID)
    {
        SqlParameter shippingPickerParam = new SqlParameter("@PK_iShippingPicker", shippingPickerID);
        return _context.ShippingPickers.FromSqlRaw("EXEC sp_GetShippingPickersByID @PK_iShippingPicker", shippingPickerParam);
    }

    public IEnumerable<ShippingPicker> getShippingPickerByOrderID(int orderID)
    {
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        return _context.ShippingPickers.FromSqlRaw("EXEC sp_GetShippingPickersByOrderID @FK_iOrderID", orderIDParam);
    }

    public IEnumerable<ShippingPicker> getShippingPickers()
    {
        return _context.ShippingPickers.FromSqlRaw("EXEC sp_GetShippingPickers");
    }

    public IEnumerable<ShippingPicker> getShippingPickersAboutWarehouseByOrderID(int orderID)
    {
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        return _context.ShippingPickers.FromSqlRaw("EXEC sp_GetShippingPickersAboutWarehouseByOrderID @FK_iOrderID", orderIDParam);
    }

    public bool insertShippingOrder(int shippingUnitID, int orderID)
    {
        SqlParameter shippingUnitIDParam = new SqlParameter("@FK_iShippingUnitID", shippingUnitID);
        SqlParameter orderIDParam = new SqlParameter("@FK_iOrderID", orderID);
        SqlParameter orderStatusIDParam = new SqlParameter("@FK_iOrderStatusID", 6);
        SqlParameter shippingTimeParam = new SqlParameter("@ShippingTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("SET DATEFORMAT dmy EXEC sp_InsertShippingOrder @FK_iShippingUnitID, @FK_iOrderID, @FK_iOrderStatusID, @ShippingTime", shippingUnitIDParam, orderIDParam, orderStatusIDParam, shippingTimeParam);
        return true;
    }
}