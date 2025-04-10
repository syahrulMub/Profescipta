using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfesciptaTest.BusinessLogic;

namespace ProfesciptaTest.Controllers;
public class ComCustomerController : Controller
{
    private readonly IBusinessLogic _businessLogic;

    public ComCustomerController(IBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet("ComCustomer/GetComCustomer")]
    public async Task<JsonResult> GetComCustomer()
    {
        var result = await _businessLogic.GetAllCustomersAsync();
        if (result == null)
        {
            return null;
        }
        return Json(result);
    }
    [HttpPost("ComCustomer/Save")]
    public async Task<JsonResult> Save(ComCustomer comCustomer)
    {
        if (comCustomer == null || string.IsNullOrEmpty(comCustomer.CustomerName))
        {
            return Json(new { success = false, message = "Invalid customer data." });
        }

        var result = await _businessLogic.AddCustomerAsync(comCustomer);
        if (result != null)
        {
            return Json(new { success = true, data = result });
        }

        return Json(new { success = false, message = "Failed to save customer." });
    }
    [HttpDelete("ComCustomer/Delete/{id}")]
    public async Task<JsonResult> Delete(int id)
    {
        var result = await _businessLogic.DeleteCustomerAsync(id);
        if (result)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Failed to delete customer." });
    }
}
