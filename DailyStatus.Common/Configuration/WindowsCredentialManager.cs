using CredentialManagement;

namespace DailyStatus.Common.Configuration
{
    public class WindowsCredentialManager
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
