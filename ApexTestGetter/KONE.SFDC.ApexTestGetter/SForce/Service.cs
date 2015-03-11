using Salesforce.Partner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KONE.SFDC.ApexTestGetter.SForce
{
    class Service
    {
        private readonly static Service _service = new Service();
        private SforceService partnerService = new SforceService();

        public static Service Instance
        {
            get { return _service; }
        }

        public SforceService SalesForce
        {
            get { return partnerService; }
        }

        // Constructor
        protected Service() { }

        public bool SfLogin(string username, string password) 
        {
            LoginResult loginResult = new LoginResult();

            try
            {
                loginResult = partnerService.login(username, password);
                partnerService.Url = loginResult.serverUrl;
                partnerService.SessionHeaderValue = new SessionHeader();

                if (loginResult.sessionId != null)
                {
                    partnerService.SessionHeaderValue.sessionId = loginResult.sessionId;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
