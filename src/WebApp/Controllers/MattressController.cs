using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using WebApp.Common;
using WebApp.Models;
using WebApp.DomainModels.Product;
using WebApp.DomainModels.Customer;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Authorize()]
    public class MattressController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MattressController(ApplicationDbContext applicationDbContext, 
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Sell(SellToCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExistMattresID = await _applicationDbContext.Mattress
                                       .AnyAsync(m => m.ID.Equals(model.MattressID, 
                                       StringComparison.InvariantCultureIgnoreCase));
                if (isExistMattresID) {
                    return  ErrorMessage.BadRequestJsonResult("MattressID is Exist.");
                }

                bool isExistMattressTypeID = await _applicationDbContext.ProductDesc
                                       .AnyAsync(m => m.ID.Equals(model.MattressTypeID,
                                       StringComparison.InvariantCultureIgnoreCase));
                if (isExistMattressTypeID == false)
                {
                    return ErrorMessage.BadRequestJsonResult("MattressTypeID is not Exist.");
                }

                bool isExistCustomerID = await _applicationDbContext.Members
                                       .AnyAsync(m => m.MemberID.Equals(model.CustomerID,
                                       StringComparison.InvariantCultureIgnoreCase));
                if (isExistCustomerID == false)
                {
                    return ErrorMessage.BadRequestJsonResult("CustomerID is not Exist.");
                }

                var loginUser = await GetCurrentUserAsync();

                ProductDesc productDesc = await _applicationDbContext.ProductDesc
                                            .Where(m => m.ID.Equals(model.MattressID,
                                                    StringComparison.InvariantCultureIgnoreCase))
                                            .FirstOrDefaultAsync();
                Mattress mattress = new Mattress {
                    ID = model.MattressID,
                    TypeDesc = productDesc,
                    RegisterDate = model.SaleDate,
                    SaleDate = model.SaleDate,
                };

                SaleToCustomerDetail saleToCustomerDetail = new SaleToCustomerDetail
                {
                    Gifts = model.Gifts,
                    DeliveryAddress = model.DeliveryAddress,
                    Prodect = mattress
                };
                SaleToCustomer saleToCustomer = new SaleToCustomer {
                    Customer = new Member {  MemberID = model.CustomerID },
                    SellingAgents = new Member { MemberID = loginUser.UserName },
                    DueDate =  model.SaleDate,
    
                };

                saleToCustomerDetail.Sale = saleToCustomer;
                saleToCustomer.DetailItems.Add(saleToCustomerDetail);

                _applicationDbContext.Mattress.Add(mattress);
                _applicationDbContext.SaleToCustomer.Add(saleToCustomer);

                try
                {
                    _applicationDbContext.SaveChanges();
                    return Json("OK");
                }
                catch (Exception e)
                {
                    return ErrorMessage.BadRequestJsonResult(e);
                }
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }
    }
}
