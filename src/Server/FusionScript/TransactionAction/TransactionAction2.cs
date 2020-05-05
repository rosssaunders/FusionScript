using System.Diagnostics;
using Python.Runtime;
using sophis.portfolio;

namespace FusionScript
{
    public class TransactionAction2 : CSMTransactionAction
    {
        public TransactionAction2()
        {
        }

        public override void VoteForModification(CSMTransaction original, CSMTransaction transaction, int event_id)
        {
            using (Py.GIL())
            {
                //dynamic test = Py.Import("Python.Test.cls.clsMyTest");

                //var type = System.Type.GetType("Python.Test.cls.clsMyTest");
                //dynamic test = System.Activator.CreateInstance(type);

                //dynamic r3 = test.Test03(); //OK
                //dynamic r4 = test.Test04();

                //dynamic test = Py.Import("Python.Test.MyTransactionAction4");
                //dynamic r1 = test.VoteForModification(new object(), new object(), 1); //OK
            }
        }
    }

    public static class CSMTransactionActionEx
    {
        public static void Register(string key, CSMTransactionAction.eMOrder e, CSMTransactionAction prototype)
        {
            var tp = prototype.GetType();

            Debug.Print(prototype.ToString());

            
            var x = tp.Assembly.CreateInstance(tp.FullName) as CSMTransactionAction;

            x.Clone(); //This works
            x.VoteForModification(new CSMTransaction(), new CSMTransaction(), 1);


        }
    }
}
