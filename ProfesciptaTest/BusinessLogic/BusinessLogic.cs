using Microsoft.EntityFrameworkCore;

namespace ProfesciptaTest.BusinessLogic;

public class BusinessLogic : IBusinessLogic
{
    private readonly TestContext _context;

    public BusinessLogic(TestContext context)
    {
        _context = context;
    }
    #region :::Customer
    public async Task<List<ComCustomer>> GetAllCustomersAsync()
    {
        return await _context.ComCustomers.ToListAsync();
    }

    public async Task<ComCustomer> GetCustomerByIdAsync(int id)
    {
        return await _context.ComCustomers.FindAsync(id);
    }

    public async Task<bool> AddCustomerAsync(ComCustomer customer)
    {
        _context.ComCustomers.Add(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCustomerAsync(ComCustomer customer)
    {
        _context.ComCustomers.Update(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await GetCustomerByIdAsync(id);
        if (customer == null) return false;

        _context.ComCustomers.Remove(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion
    #region :::Order
    public async Task<List<SoOrder>> GetAllOrdersAsync()
    {
        return await _context.SoOrders.ToListAsync();
    }
    public async Task<SoOrder> GetOrderByIdAsync(long id)
    {
        return await _context.SoOrders.FindAsync(id);
    }
    public async Task<SoOrder> AddOrderAsync(SoOrder order)
    {
        _context.SoOrders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }
    public async Task<SoOrder> UpdateOrderAsync(SoOrder order)
    {
        _context.SoOrders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
    public async Task<bool> DeleteOrderAsync(long id)
    {
        var order = await GetOrderByIdAsync(id);
        if (order == null) return false;

        _context.SoOrders.Remove(order);
        return await _context.SaveChangesAsync() > 0;
    }
    #endregion
    #region :::Item
    public async Task<List<SoItem>> GetAllItemsAsync()
    {
        return await _context.SoItems.ToListAsync();
    }
    public async Task<SoItem> GetItemByIdAsync(long id)
    {
        return await _context.SoItems.FindAsync(id);
    }
    public async Task<bool> AddItemAsync(SoItem item)
    {
        _context.SoItems.Add(item);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateItemAsync(SoItem item)
    {
        _context.SoItems.Update(item);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteItemAsync(long id)
    {
        var item = await GetItemByIdAsync(id);
        if (item == null) return false;

        _context.SoItems.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteItemsByOrderIdAsync(long orderId)
    {
        var items = await _context.SoItems.Where(i => i.SoOrderId == orderId).ToListAsync();
        if (items.Count == 0) return false;

        _context.SoItems.RemoveRange(items);
        return await _context.SaveChangesAsync() > 0;
    }
    #endregion
}
