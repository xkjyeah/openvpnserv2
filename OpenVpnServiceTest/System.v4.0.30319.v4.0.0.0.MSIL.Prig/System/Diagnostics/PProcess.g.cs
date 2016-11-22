
using System;
using System.ComponentModel;
using System.Linq;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace System.Diagnostics.Prig
{
    public class PProcess : PProcessBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionFunc<System.Diagnostics.Process, System.Boolean>> Start() 
        {
            return Stub<OfPProcess>.Setup<Urasandesu.Prig.Delegates.IndirectionFunc<System.Diagnostics.Process, System.Boolean>>(_ => _.Start());
        }
 
        public static TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionFunc<System.String, System.Diagnostics.Process>> StartString() 
        {
            return Stub<OfPProcess>.Setup<Urasandesu.Prig.Delegates.IndirectionFunc<System.String, System.Diagnostics.Process>>(_ => _.StartString());
        }
 
        public static TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>> BeginOutputReadLine() 
        {
            return Stub<OfPProcess>.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>>(_ => _.BeginOutputReadLine());
        }
 
        public static TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>> BeginErrorReadLine() 
        {
            return Stub<OfPProcess>.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>>(_ => _.BeginErrorReadLine());
        }
 
        public static TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process, System.Diagnostics.ProcessPriorityClass>> PriorityClassSetProcessPriorityClass() 
        {
            return Stub<OfPProcess>.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process, System.Diagnostics.ProcessPriorityClass>>(_ => _.PriorityClassSetProcessPriorityClass());
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPProcess>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class TypeBehaviorSetting : BehaviorSetting
        {
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PProcess.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PProcess.DefaultBehavior);
                }
            }
        }
    }
}
