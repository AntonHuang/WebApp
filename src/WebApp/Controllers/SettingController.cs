using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WebApp.Common;
using WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using WebApp.DomainModels.Core;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using WebApp.DomainModels.Customer;
using Microsoft.AspNet.Http;
using System.IO;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{


    [Authorize()]
    public class SettingController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingController(ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PointRule()
        {
            if (ModelState.IsValid)
            {
                SettingViewModel pointRuleSetting = await GetSetting(SettingName.PointRule.ToString());

                if (pointRuleSetting == null)
                {
                    var defaultRule = MemberPointRule.fromJson(null);
                    await PointRule(defaultRule);
                    return Json(defaultRule);
                }
                else
                {
                    return Json(MemberPointRule.fromJson(pointRuleSetting.Value));
                }
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        [HttpPost]
        public async Task<IActionResult> PointRule(MemberPointRule pointRule)
        {
            if (ModelState.IsValid)
            {
                //MemberPointRule pointRule = null;
               if (pointRule.Level0 == null && Request.Form.Count > 0) {
                    string v = Request.Form["pointRule"];
                    pointRule = MemberPointRule.fromJson(v);
                }

                if (pointRule == null) {
                    return ErrorMessage.BadRequestJsonResult("Cannot parse request context.");
                }

                //MemberPointRule r = JsonConvert.<MemberPointRule>()
                // string bodyStr = GetFromBodyString(Request);
                //MemberPointRule pointRule = MemberPointRule.fromJson(pointRuleValue);
                await AddOrUpdateSetting(SettingName.PointRule.ToString(), "json", pointRule.toJson());
                return Json("OK");
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }


        private async Task AddOrUpdateSetting(string sName, string sValueT, string sValue)
        {
            Setting pointRuleSetting = await GetSettingEntity(sName);

            if (pointRuleSetting == null)
            {
                this._applicationDbContext.Settings.Add(new Setting
                {
                    ID = sName,
                    Name = sName,
                    SettingValue = sValue,
                    SettingValueType = sValueT,
                    CreateBy = this.GetCurrentUserName()
                });
                await this._applicationDbContext.SaveChangesAsync();
            }
            else
            {
                SettingHistory history = new SettingHistory(pointRuleSetting);
                history.CreateBy = this.GetCurrentUserName();

                pointRuleSetting.SettingValue = sValue;

                this._applicationDbContext.SettingHistorys.Add(history);

                await this._applicationDbContext.SaveChangesAsync();
            }
        }

        private async Task<SettingViewModel> GetSetting(string id)
        {
            return await (from s in this._applicationDbContext.Settings
                          where s.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                          select new SettingViewModel
                          {
                              ID = s.ID,
                              Name = s.Name,
                              Value = s.SettingValue,
                              ValueType = s.SettingValueType,
                              Desc = s.Desc
                          }).FirstOrDefaultAsync();
        }

        private async Task<Setting> GetSettingEntity(string id)
        {
            return await (from s in this._applicationDbContext.Settings
                          where s.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                          select s
                          ).FirstOrDefaultAsync();
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }

        private string GetCurrentUserName()
        {
            return Context.User.GetUserName();
        }

}
}
