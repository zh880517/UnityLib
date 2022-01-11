using System.Collections.Generic;

namespace LiteECS
{
    public class Feature
    {
        protected List<IInitializeSystem> initializes = new List<IInitializeSystem>();
        protected List<IExecuteSystem> executes = new List<IExecuteSystem>();
        protected List<ICleanupSystem> cleanups = new List<ICleanupSystem>();
        protected List<ITearDownSystem> tearDowns = new List<ITearDownSystem>();

        public void Add(ISystem system)
        {
            if (system is IInitializeSystem initialize)
            {
                initializes.Add(initialize);
            }
            if (system is IExecuteSystem execute)
            {
                executes.Add(execute);
            }
            if (system is ICleanupSystem cleanup)
            {
                cleanups.Add(cleanup);
            }
            if (system is ITearDownSystem tearDown)
            {
                tearDowns.Add(tearDown);
            }
        }

        public void Initialize()
        {
            for (int i=0; i<initializes.Count; ++i)
            {
                initializes[i].OnInitialize();
            }
        }

        public void Execute()
        {
            for (int i = 0; i < executes.Count; ++i)
            {
                executes[i].OnExecute();
            }
        }

        public void Cleanup()
        {
            for (int i = 0; i < cleanups.Count; ++i)
            {
                cleanups[i].OnCleanup();
            }
        }

        public void TearDown()
        {
            for (int i = 0; i < tearDowns.Count; ++i)
            {
                tearDowns[i].OnTearDown();
            }
        }

    }


}