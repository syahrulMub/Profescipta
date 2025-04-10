namespace ProfesciptaTest.BusinessLogic;

public interface IBusinessLogic
{
    Task<List<ComCustomer>> GetAllCustomersAsync();
    Task<ComCustomer> GetCustomerByIdAsync(int id);
    Task<bool> AddCustomerAsync(ComCustomer customer);
    Task<bool> UpdateCustomerAsync(ComCustomer customer);
    Task<bool> DeleteCustomerAsync(int id);

    Task<List<SoOrder>> GetAllOrdersAsync();
    Task<SoOrder> GetOrderByIdAsync(long id);
    Task<SoOrder> AddOrderAsync(SoOrder order);
    Task<SoOrder> UpdateOrderAsync(SoOrder order);
    Task<bool> DeleteOrderAsync(long id);

    Task<List<SoItem>> GetAllItemsAsync();
    Task<SoItem> GetItemByIdAsync(long id);
    Task<bool> AddItemAsync(SoItem item);
    Task<bool> UpdateItemAsync(SoItem item);
    Task<bool> DeleteItemAsync(long id);
    Task<bool> DeleteItemsByOrderIdAsync(long orderId);
}
