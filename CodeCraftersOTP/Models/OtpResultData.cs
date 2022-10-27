using System;

namespace CodeCraftersOTP.Models
{
    public class OtpResultData
    {
        public string UserId { get;  set; }
        public bool? WasUsed { get;  set; }
        public bool? IsExpired { get;  set; }
        public string Key { get;  set; }
        public string Message { get;  set; }
        public string Status { get;  set; }
        

        public OtpResultData(string userId, bool? wasUsed, bool? isExpired, string key, string message, string status)
        {
            this.UserId = userId;
            this.WasUsed = wasUsed;
            this.IsExpired = isExpired;
            this.Key = key;
            this.Message = message;
            this.Status = status;
        }
    }
}