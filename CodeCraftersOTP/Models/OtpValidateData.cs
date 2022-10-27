using System;

namespace CodeCraftersOTP.Models
{
    public class OtpValidateData
    {

        public string UserId { get; set; }
        public string Key { get; set; }

        public OtpValidateData(string userId, string key)
        {
            this.UserId = userId;
            this.Key = key;
        }
    }
}