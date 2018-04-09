using DailyStatus.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.CommonTests
{
    [TestClass]
    public class TogglProxyTests
    {
        const string API_KEY = "2a0d2a5553f4ad84ed025cdc4d39bb27";
        [TestMethod]
        public async Task ShouldCreateNewTask()
        {
            var proxy = new TogglProxy();
            proxy.Configure(API_KEY);
            await proxy.StartTimer();
        }
    }
}
