namespace Toumash.DailyStatus
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CredentialManagement;

    public class ApiKeyRepository
    {
        private const string PasswordName = "toggl-api-key";

        public void Save(string password, string key = PasswordName)
        {
            using (var cred = new Credential())
            {
                cred.Password = password;
                cred.Target = key;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        public string Get(string key = PasswordName)
        {
            using (var cred = new Credential())
            {
                cred.Target = key;
                cred.Load();
                return cred.Password;
            }
        }
    }
}
