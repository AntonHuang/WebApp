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
using WebApp.DomainModels.Core;
using Microsoft.Data.Entity.Relational;
using System.Data.Common;

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

        [HttpGet]
        public async Task<IActionResult> ListMattressType()
        {
            if (ModelState.IsValid)
            {
                var items = await this._applicationDbContext.ProductDesc.Select(
                                      pd => new { ID = pd.ID, Name = pd.Name, Type = pd.Type }
                                    ).ToListAsync();

                //if (items.Count() > 0)
                //{
                    return Json(items);
                //}
                //else
                //{
                //    return Json(new { ID = "", Name = "", Type = "" });
                //}
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        [HttpPost]
        public async Task<IActionResult> Sell(SellToCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existMattresID = await _applicationDbContext.Mattress
                                                .Where(m => m.ID.Equals(model.MattressID, StringComparison.InvariantCultureIgnoreCase))
                                                .Take(1)
                                                .Select(m => m.ID)
                                                .FirstOrDefaultAsync();

                if (existMattresID != null)
                {
                    return ErrorMessage.BadRequestJsonResult("MattressID is Exist.");
                }

                var existMattressTypeID = await _applicationDbContext.ProductDesc
                                       .Where(m => m.ID.Equals(model.MattressTypeID, StringComparison.InvariantCultureIgnoreCase))
                                       .Take(1)
                                       .Select(m => m.ID)
                                       .FirstOrDefaultAsync();
                if (existMattressTypeID == null)
                {
                    return ErrorMessage.BadRequestJsonResult("MattressTypeID is not Exist.");
                }

                var existCustomerID = await _applicationDbContext.Members
                                       .Where(m => m.MemberID.Equals(model.CustomerID, StringComparison.InvariantCultureIgnoreCase))
                                       .Take(1)
                                       .Select(m => m.MemberID)
                                       .FirstOrDefaultAsync();
                if (existCustomerID == null)
                {
                    return ErrorMessage.BadRequestJsonResult("CustomerID is not Exist.");
                }

                return await DoSell(model);
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        private async Task<IActionResult> DoSell(SellToCustomerViewModel model)
        {
            var loginUser = await GetCurrentUserAsync();

            ProductDesc productDesc = await _applicationDbContext.ProductDesc
                                        .Where(m => m.ID.Equals(model.MattressTypeID,
                                                StringComparison.InvariantCultureIgnoreCase))
                                        .FirstOrDefaultAsync();
            Mattress mattress = new Mattress
            {
                ID = model.MattressID,
                TypeDesc = productDesc,
                RegisterDate = model.SaleDate,
                SaleDate = model.SaleDate,
            };

            SaleToCustomerDetail saleToCustomerDetail = new SaleToCustomerDetail
            {
                Gifts = model.Gifts,
                DeliveryAddress = model.DeliveryAddress,
                Prodect = mattress,
                Price = productDesc.Price

            };
            SaleToCustomer saleToCustomer = new SaleToCustomer
            {
                ID = IDGenerator.GetSaleToCustomerIDGenerator(_applicationDbContext).GetNext(),
                Customer = new Member { MemberID = model.CustomerID },
                SellingAgents = new Member { MemberID = loginUser.UserName },
                DealDate = model.SaleDate,
            };

            saleToCustomerDetail.Sale = saleToCustomer;
            saleToCustomer.DetailItems.Add(saleToCustomerDetail);

            _applicationDbContext.Mattress.Add(mattress);
            _applicationDbContext.SaleToCustomer.Add(saleToCustomer);
            _applicationDbContext.SaleToCustomeDetails.Add(saleToCustomerDetail);

            var pointItems = await AddMemberPoint(saleToCustomerDetail);

            _applicationDbContext.SaveChanges();

            return Json(new {
                    saleToCustomerID = saleToCustomer.ID,
                    memberPointItems = pointItems
            });
        }

        private async Task<SellMemberPointViewModel> AddMemberPoint(SaleToCustomerDetail saleToCustomerDetail)
        {
            MemberPointRule pointRule = await this.GetPointRule();

            // Left JOIN
            /*
            var customers = await (from m1 in _applicationDbContext.Members
                                   join m2 in _applicationDbContext.Members
                                       on m1.ReferenceMemberID equals m2.MemberID
                                    select new {
                                        MemberID = m1.MemberID,
                                        MemberName = m1.Name,
                                        MemberLevel = m1.Level,

                                        Up1ID = m2.MemberID,
                                        Up1Name = m2.Name,
                                        Up1Level = m2.Level,
                                        Up1ReferenceMemberID = m2.ReferenceMemberID
                                    } into M12
                                   join m3 in _applicationDbContext.Members 
                                       on M12.Up1ReferenceMemberID equals m3.MemberID into M23
                                   from m5 in M23.DefaultIfEmpty()
                                   where M12.MemberID.Equals(saleToCustomerDetail.Sale.Customer.MemberID, 
                                       StringComparison.InvariantCultureIgnoreCase)
                                   select new SellMemberPointViewModel
                                   {
                                       MemberID = M12.MemberID,
                                       MemberName = M12.MemberName,
                                       MemberLevel = M12.MemberLevel,

                                       Up1ID = M12.Up1ID,
                                       Up1Name = M12.Up1Name,
                                       Up1Level = M12.Up1Level,

                                       Up2ID = m5.MemberID,
                                       Up2Name = m5.Name,
                                       Up2Level = m5.Level,
                                   }).FirstOrDefaultAsync();

            */
            var customers = GetCustomersFromSqlCommand( _applicationDbContext.Database.AsRelational(), 
                 saleToCustomerDetail.Sale.Customer.MemberID);


            if (customers == null)
            {
                return new SellMemberPointViewModel
                {
                    MemberID = "",
                    MemberName = "",
                    MemberLevel = "",

                    Up1ID = "",
                    Up1Name = "",
                    Up1Level = "",

                    Up2ID = "",
                    Up2Name = "",
                    Up2Level = ""
                };
            }

            if (string.IsNullOrWhiteSpace(customers.MemberID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = new Member { MemberID = customers.MemberID };
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.MemberLevel, LevelRelation.Self, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this._applicationDbContext).GetNext();
                this._applicationDbContext.MemberPoint.Add(menberPoint);

                customers.PointCount = menberPoint.Quantity;
            }

            if (string.IsNullOrWhiteSpace(customers.Up1ID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = new Member { MemberID = customers.Up1ID };
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.Up1Level,
                    LevelRelation.Son, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this._applicationDbContext).GetNext();
                this._applicationDbContext.MemberPoint.Add(menberPoint);

                customers.Up1PointCount = menberPoint.Quantity;
            }

            if (string.IsNullOrWhiteSpace(customers.Up2ID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = new Member { MemberID = customers.Up2ID };
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.Up2Level,
                    LevelRelation.Grandson, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this._applicationDbContext).GetNext();
                this._applicationDbContext.MemberPoint.Add(menberPoint);

                customers.Up2PointCount = menberPoint.Quantity;
            }

            return customers;
        }

        public static SellMemberPointViewModel GetCustomersFromSqlCommand(RelationalDatabase database , string customerID)
        {
            var connection = database.Connection;
            var command = connection.DbConnection.CreateCommand();
            command.CommandText = @"SELECT 
                [m1].[MemberID] AS MemberID,
                [m1].[Name] AS MemberName,
                [m1].[Level]AS MemberLevel,
                [m2].[MemberID] AS Up1ID,
                [m2].[Name] AS Up1Name,
                [m2].[Level] AS Up1Level,
                [m3].[MemberID] AS Up2ID,
                [m3].[Name] AS Up2Name,
                [m3].[Level] AS Up2Level
                FROM[Member] AS[m1]
                LEFT JOIN[Member] AS[m2] ON[m1].[ReferenceMemberID] = [m2].[MemberID]
        LEFT JOIN[Member] AS[m3] ON[m2].[ReferenceMemberID] = [m3].[MemberID]
        WHERE[m1].[MemberID] = @CustomerID";

            try
            {
                DbParameter p = command.CreateParameter();
                p.DbType = System.Data.DbType.String;
                p.ParameterName = "CustomerID";
                p.Value = customerID == null ? "" : customerID;

                command.Parameters.Add(p);
                connection.Open();
                var r = command.ExecuteReader();
                if (r.Read()) {
                    var result= new SellMemberPointViewModel
                    {
                        MemberID = r.GetString(0),
                        MemberName = r.GetString(1),
                        MemberLevel = r.GetString(2),

                        Up1ID = r.GetString(3),
                        Up1Name = r.GetString(4),
                        Up1Level = r.GetString(5),

                        Up2ID = r.GetString(6),
                        Up2Name = r.GetString(7),
                        Up2Level = r.GetString(8),
                    };

                    r.Close();
                    return result;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            finally
            {
                
                connection.Close();
            }
            return null;
      }

        private async Task<MemberPointRule> GetPointRule() {
            var ruleSetting =  await(from s in this._applicationDbContext.Settings
                         where s.ID.Equals(SettingName.PointRule.ToString(), StringComparison.InvariantCultureIgnoreCase)
                         select s
                          ).FirstOrDefaultAsync();
            if (ruleSetting == null)
            {
                return MemberPointRule.Default;
            }
            else {
                return MemberPointRule.fromJson(ruleSetting.SettingValue);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }

        
    }
}
