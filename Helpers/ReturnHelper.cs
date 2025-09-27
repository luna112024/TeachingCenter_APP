using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Helpers
{
    public class ReturnHelper : Controller
    {
        private readonly string _errorMessage = "បរាជ័យ! ប្រតិបត្តិការមិនអាចដំណើរការបានទេ";
        private readonly string _successMessage = "ជោគជ័យ! ប្រតិបត្តិការបានធ្វើរួចរាល់";

        private string GetDefaultMessage(bool result)
        {
            return result ? _successMessage : _errorMessage;
        }

        public JsonResult ReturnNewResult(bool flag)
        {
            return Json(new
            {
                result = flag ? "true" : "false",
                message = GetDefaultMessage(flag),
                redirect = "",
                isredirect = false,
                partialView = ""
            });
        }

        public JsonResult ReturnNewResult(bool flag, string message)
        {
            return Json(new
            {
                result = flag ? "true" : "false",
                message,
                redirect = "",
                isredirect = false,
                partialView = ""
            });
        }

        public JsonResult ReturnNewResult(bool flag, string message, string redirect)
        {
            return Json(new
            {
                result = flag ? "true" : "false",
                message,
                redirect,
                isredirect = true,
                partialView = ""
            });
        }

        public JsonResult ReturnNewResult(bool flag, string message, string redirect, bool isredirect, string partialView = "")
        {
            return Json(new
            {
                result = flag ? "true" : "false",
                message,
                redirect,
                isredirect,
                partialView
            });
        }
    }
} 