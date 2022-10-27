using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeCraftersOTP.Models
{
    public class OTP
    {
        [BsonId]
        public ObjectId Id { get; private set; }
        public string UserId { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime ValidDateTime { get; set; }
        public string Key { get; set; }
        public bool WasUsed { get; set; }

        public OTP(string userId, DateTime requestDateTime, DateTime validDateTime,string key, bool wasUsed)
        {
            this.Id = ObjectId.GenerateNewId();
            this.UserId = userId;
            this.RequestDateTime = requestDateTime;
            this.ValidDateTime = validDateTime;
            this.Key = key;
            this.WasUsed = wasUsed;
            
        }
    }
}