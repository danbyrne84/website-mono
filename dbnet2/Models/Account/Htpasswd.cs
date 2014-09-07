using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CryptSharp;

namespace dbnet2.Models.Account
{
    public class Htpasswd
    {
        protected Dictionary<string,string> UserHashes = new Dictionary<string, string>();
        public Htpasswd(string passwordFile)
        {
            UserHashes = ParseUserHashesFromFile(passwordFile);
        }

        public bool Validate(string username, string password)
        {
            var userRecord = UserHashes.Where(x => x.Key == username);
            if (!userRecord.Any() || userRecord.Count() > 1)
            {
                return false;
            }

            return Crypter.CheckPassword(password, userRecord.First().Value);
        }

        protected Dictionary<string, string> ParseUserHashesFromFile(string passwordFile)
        {
            var userHashes = new Dictionary<string, string>();
            if (!System.IO.File.Exists(passwordFile))
            {
                return new Dictionary<string, string>();
            }

            // parse password file into a dictionary of users/hashes
            var lines = System.IO.File.ReadAllLines(passwordFile);
            foreach (var line in lines)
            {
                var lineData = line.Split(':');
                if (lineData.Count() != 2)
                {
                    continue;
                }

                var username = lineData[0];
                var password = lineData[1];
                userHashes.Add(username, password);
            }

            return userHashes;
        }
    }
}