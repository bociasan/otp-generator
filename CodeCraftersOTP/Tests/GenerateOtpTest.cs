using System;
using System.Collections.Generic;
using System.Threading;
using CodeCraftersOTP.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCraftersOTP.Tests
{
    public class GenerateOtpTest
    {
        [TestClass]
        public class TestOtpService
        {
            [TestMethod]
            public void GenerateOtpKeyValid()
            {
                var service = new OtpService();
                var otpInputData = new OtpInputData("1234", DateTime.Now);

                var result = service.GenerateOtpKey(otpInputData);
                Assert.IsInstanceOfType(result, typeof(OTP));
            }

            [TestMethod]
            public void ValidateOtpKey()
            {
                var service = new OtpService();
                var otpInputData = new OtpInputData("999999", DateTime.Now);
                var otp = service.GenerateOtpKey(otpInputData);
                var otpValidateData = new OtpValidateData(otp.UserId, otp.Key);
                var result = service.ValidateOtpKey(otpValidateData);
                Assert.IsTrue(result.Status == "valid");
            }
            [TestMethod]
            public void ValidateOtpKeyInvalid()
            {
                var service = new OtpService();
                var otpInputData = new OtpInputData("999999", DateTime.Now);
                var otp = service.GenerateOtpKey(otpInputData);
                var invalidKey = otp.Key;
                otp = service.GenerateOtpKey(otpInputData);
                var otpValidateData = new OtpValidateData(otp.UserId, invalidKey);
                var result = service.ValidateOtpKey(otpValidateData);
                Assert.IsTrue(result.Status == "invalid");
            }

            [TestMethod]
            public void ValidateOtpKeyExpired()
            {
                var service = new OtpService();
                var otpInputData = new OtpInputData("666666", DateTime.Now);
                var otp = service.GenerateOtpKey(otpInputData);
                var otpValidateData = new OtpValidateData(otp.UserId, otp.Key);
                Thread.Sleep(35000);
                var result = service.ValidateOtpKey(otpValidateData);
                Assert.IsTrue(result.Status == "expired");
            }
        }
    }
}