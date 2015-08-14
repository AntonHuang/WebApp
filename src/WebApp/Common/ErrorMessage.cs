using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Common
{
    public class ErrorMessage
    {
        public static JsonResult BadRequestJsonResult(object value)
        {
            JsonResult r = new JsonResult(value);
            r.StatusCode = (int)HttpStatusCode.BadRequest;
            return r;
        }
    }
}
