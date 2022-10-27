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
        public OtpResultData ValidateOtpKey(OtpValidateData validateData)
        {
            String status, message;
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
                            message = "Success.";
                            status = "valid";
                            OtpResultData result = new OtpResultData(validateData.UserId, otp.WasUsed,
                                !(otp.ValidDateTime > DateTime.UtcNow), otp.Key, message ,status);
                            var filter = Builders<OTP>.Filter.Eq("_id", otp.Id);
                            var update = Builders<OTP>.Update.Set("WasUsed", true);
                            _collection.UpdateOne(filter, update);
                            return result;
                        }
                        else
                        {
                            status = "expired";
                            message = "This OTP code is expired.";
                            OtpResultData result = new OtpResultData(validateData.UserId, otp.WasUsed,
                                !(otp.ValidDateTime > DateTime.UtcNow), validateData.Key, message ,status);
                            return result;
                        }
                    }
                    else
                    {
                        status = "used";
                        message = "This OTP code was already used.";
                        OtpResultData result = new OtpResultData(validateData.UserId, otp.WasUsed,
                            !(otp.ValidDateTime > DateTime.UtcNow), validateData.Key, message ,status);
                        return result;
                    }
                }
                else
                {
                    status = "invalid";
                    message = "OTP code is wrong.";
                    OtpResultData result = new OtpResultData(validateData.UserId, otp.WasUsed,
                        !(otp.ValidDateTime > DateTime.UtcNow), validateData.Key, message ,status);
                    return result;
                }
            }
            else
            {
                status = "unknown";
                message = "You should retrieve an OTP code first.";
                OtpResultData result = new OtpResultData(validateData.UserId, null,
                    null, validateData.Key, message ,status);
                return result;
            }
        }
    }

    public interface IOtpService
    {
        OTP GenerateOtpKey(OtpInputData inputData);
        OtpResultData ValidateOtpKey(OtpValidateData validateData);
    }
}