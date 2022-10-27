using System;
using System.Web.Mvc;
using CodeCraftersOTP.Models;

namespace CodeCraftersOTP.Controllers
{
    public class OtpController : Controller
    {
        private IOtpService _service;
        public OtpController(IOtpService service)
        {
            _service = service;
        }
        public OtpController()
        {
            _service = new OtpService();
        }
        [HttpPost]
        public JsonResult Generate(string userId, DateTime requestDateTime)
        {
            var inputData = new OtpInputData(userId, requestDateTime);
            return Json(_service.GenerateOtpKey(inputData));
        }
        [HttpPost]
        public JsonResult Validate(string userId, string key)
        {
            var validateData = new OtpValidateData(userId, key);
            return Json(_service.ValidateOtpKey(validateData));
        }
    }
}