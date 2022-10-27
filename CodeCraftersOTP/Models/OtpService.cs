using System;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CodeCraftersOTP.Models
{
    public class OtpService : IOtpService
    {
        private MongoClient _dbClient;
        private IMongoDatabase _database;
        private IMongoCollection<OTP> _collection;
        public OtpService()
        {
            _dbClient = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            _database = _dbClient.GetDatabase("OTPGeneratorDB");
            _collection = _database.GetCollection<OTP>("OTPs");
        }
        public OTP GenerateOtpKey(OtpInputData inputData)
        {
            var key = new Random().Next(100000, 999999).ToString();
            var userId = inputData.UserId;
            var requestDateTime = inputData.RequestDateTime;
            var validDateTime = (DateTime.UtcNow + TimeSpan.FromSeconds(30));
            var otp = new OTP(userId, requestDateTime, validDateTime, key, false);

            _collection.DeleteMany(_ => _.UserId == inputData.UserId);
            _collection.InsertOne(otp);
            return otp;
        }
        public Object ValidateOtpKey(OtpValidateData validateData)
        {
            String status = "";
            var otp = _collection.Find(_ => _.UserId == validateData.UserId)
                .SortByDescending(e => e.ValidDateTime)
                .FirstOrDefault();
            if (otp != null)
            {
                if (otp.Key == validateData.Key)
                {
                    if (!otp.WasUsed)
                    {
                        if (otp.ValidDateTime > DateTime.UtcNow)
                        {
                            OtpResultData result = new OtpResultData(validateData.UserId, otp.WasUsed,
                                !(otp.ValidDateTime > DateTime.UtcNow), otp.Key);
                            var filter = Builders<OTP>.Filter.Eq("_id", otp.Id);
                            var update = Builders<OTP>.Update.Set("WasUsed", true);
                            _collection.UpdateOne(filter, update);
                            status = "valid";
                            return new { message = "Success.", result, status};
                        }
                        else
                        {
                            status = "expired";
                            return new { message = "This OTP code is expired.", status};
                        }
                    }
                    else
                    {
                        status = "used";
                        return new { message = "This OTP code was already used.", status};
                    }
                }
                else
                {
                    status = "invalid";
                    return new { message = "OTP code is wrong.", status};
                }
            }
            else
            {
                status = "unknown";
                return new { message = "You should retrieve an OTP code first.", status};
            }
        }
    }

    public interface IOtpService
    {
        OTP GenerateOtpKey(OtpInputData inputData);
        Object ValidateOtpKey(OtpValidateData validateData);
    }
}