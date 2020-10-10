using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using SA51_CA_Project_Team10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.DBs
{
    public class Verify
    {
        public bool VerifySession(string SessionId, DbT10Software _db) {
            if (SessionId.IsNullOrEmpty())
            {
                return false;
            } else
            {                
                return _db.Sessions.Where(x => x.Id == SessionId).ToList().Count != 0;
            }
        }
    }
}
