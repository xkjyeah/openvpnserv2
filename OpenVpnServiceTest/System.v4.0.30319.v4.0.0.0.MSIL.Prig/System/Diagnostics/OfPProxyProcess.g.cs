
using System;
using System.ComponentModel;
using Urasandesu.NAnonym;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace System.Diagnostics.Prig
{
    public class OfPProxyProcess : OfPProcess, IPrigProxyTypeIntroducer 
    {
        object m_target;
        
        Type IPrigProxyTypeIntroducer.Type
        {
            get { return Type; }
        }

        void IPrigProxyTypeIntroducer.Initialize(object target)
        {
            m_target = target;
        }

        public override OfPProcess.ImplForStart Start()
        {
            return new ImplForStart(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplForStart : OfPProcess.ImplForStart 
        {
            object m_target;

            public ImplForStart(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplForStart>(m_target);
                    else
                        SetTargetInstanceBody<ImplForStart>(m_target, value);
                }
            }
        }
 
        public override OfPProcess.ImplForBeginOutputReadLine BeginOutputReadLine()
        {
            return new ImplForBeginOutputReadLine(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplForBeginOutputReadLine : OfPProcess.ImplForBeginOutputReadLine 
        {
            object m_target;

            public ImplForBeginOutputReadLine(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplForBeginOutputReadLine>(m_target);
                    else
                        SetTargetInstanceBody<ImplForBeginOutputReadLine>(m_target, value);
                }
            }
        }
 
        public override OfPProcess.ImplForBeginErrorReadLine BeginErrorReadLine()
        {
            return new ImplForBeginErrorReadLine(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplForBeginErrorReadLine : OfPProcess.ImplForBeginErrorReadLine 
        {
            object m_target;

            public ImplForBeginErrorReadLine(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplForBeginErrorReadLine>(m_target);
                    else
                        SetTargetInstanceBody<ImplForBeginErrorReadLine>(m_target, value);
                }
            }
        }
 
        public override OfPProcess.ImplForPriorityClassSetProcessPriorityClass PriorityClassSetProcessPriorityClass()
        {
            return new ImplForPriorityClassSetProcessPriorityClass(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplForPriorityClassSetProcessPriorityClass : OfPProcess.ImplForPriorityClassSetProcessPriorityClass 
        {
            object m_target;

            public ImplForPriorityClassSetProcessPriorityClass(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplForPriorityClassSetProcessPriorityClass>(m_target);
                    else
                        SetTargetInstanceBody<ImplForPriorityClassSetProcessPriorityClass>(m_target, value);
                }
            }
        }

    }
}
