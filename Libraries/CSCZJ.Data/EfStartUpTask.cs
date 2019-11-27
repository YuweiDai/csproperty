using CSCZJ.Core;
using CSCZJ.Core.Data;
using CSCZJ.Core.Infrastructure;

namespace CSCZJ.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var provider = EngineContext.Current.Resolve<IDataProvider>();
            if (provider == null)
                throw new CSCZJException("No IDataProvider found");
            provider.SetDatabaseInitializer();
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
