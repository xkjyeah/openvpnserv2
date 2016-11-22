
using System;
using System.ComponentModel;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace System.Diagnostics.Prig
{
    public class PProxyProcess 
    {
        Proxy<OfPProxyProcess> m_proxy = new Proxy<OfPProxyProcess>();

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionFunc<System.Diagnostics.Process, System.Boolean>> Start() 
        {
            return m_proxy.Setup<Urasandesu.Prig.Delegates.IndirectionFunc<System.Diagnostics.Process, System.Boolean>>(_ => _.Start());
        }
 
        public TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>> BeginOutputReadLine() 
        {
            return m_proxy.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>>(_ => _.BeginOutputReadLine());
        }
 
        public TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>> BeginErrorReadLine() 
        {
            return m_proxy.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process>>(_ => _.BeginErrorReadLine());
        }
 
        public TypedBehaviorPreparable<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process, System.Diagnostics.ProcessPriorityClass>> PriorityClassSetProcessPriorityClass() 
        {
            return m_proxy.Setup<Urasandesu.Prig.Delegates.IndirectionAction<System.Diagnostics.Process, System.Diagnostics.ProcessPriorityClass>>(_ => _.PriorityClassSetProcessPriorityClass());
        }


        public static implicit operator System.Diagnostics.Process(PProxyProcess @this)
        {
            return (System.Diagnostics.Process)@this.m_proxy.Target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            return m_proxy.ExcludeGeneric(new InstanceBehaviorSetting(this));
        }

        public class InstanceBehaviorSetting : BehaviorSetting
        {
            private PProxyProcess m_this;

            public InstanceBehaviorSetting(PProxyProcess @this)
            {
                m_this = @this;
            }
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    m_this.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(m_this.DefaultBehavior);
                }
            }
        }
    }
}
