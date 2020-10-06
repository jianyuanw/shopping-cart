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

        public bool VerifySession(String SessionId, DbT10Software _db) {
            List<Session> sessions = _db.Sessions.Where(x => x.Id == SessionId).ToList();
            if (sessions.Count != 0) return true;
            else return false;
        }
    }
}
