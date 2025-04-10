using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfesciptaTest.BusinessLogic;

namespace ProfesciptaTest.Controllers;
public class SalesOrderController : Controller
{
    private readonly IBusinessLogic _businessLogic;

    public SalesOrderController(IBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }
    #region :::view
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        ViewBag.SoOrderId = id;
        return View();
    }
    #endregion

    [HttpGet("SalesOrder/GetSalesOrder")]

    public async Task<JsonResult> GetSalesOrder(string search = "", DateTime? dateTime = null)
    {
        var orders = await _businessLogic.GetAllOrdersAsync();
        var customers = await _businessLogic.GetAllCustomersAsync();
        var result = from order in orders
                     join customer in customers on order.ComCustomerId equals customer.ComCustomerId
                     where (
                        (string.IsNullOrEmpty(search)
                        || order.OrderNo.Contains(search)
                         || customer.CustomerName.Contains(search))
                         &&
                           (dateTime == null || order.OrderDate == dateTime)
                           )
                     select new SalesOrderView
                     {
                         SoOrderId = order.SoOrderId,
                         OrderNo = order.OrderNo,
                         OrderDate = order.OrderDate,
                         CustomerName = customer.CustomerName
                     };

        if (result == null || !result.Any())
        {
            return Json(new { success = false, message = "No sales orders found." });
        }

        return Json(result);
    }

    [HttpGet("SalesOrder/GetSalesOrderById/{id}")]
    public async Task<JsonResult> GetSalesOrderById(int id)
    {
        var order = await _businessLogic.GetOrderByIdAsync(id);
        var items = await _businessLogic.GetAllItemsAsync();

        if (order == null || items == null)
        {
            return Json(new { success = false, message = "Sales order not found." });
        }

        var result = new SalesOrderModel
        {
            Order = order,
            OrderItem = items.Where(x => x.SoOrderId == id).ToList()
        };

        return Json(result);
    }

    [HttpPost("SalesOrder/Save")]
    public async Task<JsonResult> Save(SalesOrderModel salesOrder)
    {
        if (salesOrder.Order.SoOrderId == 0)
        {
            var result = await _businessLogic.AddOrderAsync(salesOrder.Order);
            if (result != null)
            {
                foreach (var item in salesOrder.OrderItem)
                {
                    item.SoOrderId = result.SoOrderId;
                    await _businessLogic.AddItemAsync(item);
                }
            }
            return Json(result);
        }
        else
        {
            var result = await _businessLogic.UpdateOrderAsync(salesOrder.Order);
            if (result != null)
            {
                await _businessLogic.DeleteItemsByOrderIdAsync(salesOrder.Order.SoOrderId);
                foreach (var item in salesOrder.OrderItem)
                {
                    item.SoOrderId = salesOrder.Order.SoOrderId;
                    await _businessLogic.AddItemAsync(item);
                }
            }
            return Json(result);
        }

    }
    [HttpDelete("SalesOrder/Delete/{id}")]
    public async Task<JsonResult> Delete(int id)
    {
        var result = await _businessLogic.DeleteOrderAsync(id);
        if (result)
        {
            await _businessLogic.DeleteItemsByOrderIdAsync(id);
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Failed to delete order." });
    }
}


public class SalesOrderModel
{
    public SoOrder? Order { get; set; }
    public List<SoItem>? OrderItem { get; set; }
}

public class SalesOrderView
{
    public long SoOrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = null!;
}