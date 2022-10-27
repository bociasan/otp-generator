using System;

namespace CodeCraftersOTP.Models
{
    public class OtpInputData
    {
        public string UserId { get; set; }
        public DateTime RequestDateTime { get; set; }

        public OtpInputData(string userId, DateTime requestDateTime)
        {
            this.UserId = userId;
            this.RequestDateTime = requestDateTime;
        }
    }
}