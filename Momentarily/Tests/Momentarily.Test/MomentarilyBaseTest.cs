using System;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Core.IocRegistry;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.Test.Common;
using Momentarily.Common;
namespace Momentarily.Test
{
    public class MomentarilyBaseTest : BaseTest
    {
        public MomentarilyBaseTest(bool useProfiler = false)
            :base(useProfiler)
        {
        }
        public override void SetUp()
        {
            Ioc.Add(x =>
            {
                x.AddRegistry(new IocScanerRegistry(AppDomain.CurrentDomain.BaseDirectory, new string[] { "Momentarily" }));
            });
            base.SetUp();
        }
    }
}