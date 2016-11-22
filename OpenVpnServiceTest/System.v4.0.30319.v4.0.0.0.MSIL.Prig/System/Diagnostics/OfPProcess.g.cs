
using System;
using System.ComponentModel;
using System.Linq;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace System.Diagnostics.Prig
{
    public class OfPProcess : PProcessBase, IPrigTypeIntroducer 
    {
        public virtual ImplForStart Start() 
        {
            return new ImplForStart();
        }

        static IndirectionStub ms_stubStart = NewStubStart();
        static IndirectionStub NewStubStart()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""Start"" alias=""Start"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Start</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Boolean Start()</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Boolean Start()</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplForStart : TypedBehaviorPreparableImpl 
        {
            public ImplForStart()
                : base(ms_stubStart, new Type[] {  }, new Type[] {  })
            { }
        }
 
        public virtual ImplForStartString StartString() 
        {
            return new ImplForStartString();
        }

        static IndirectionStub ms_stubStartString = NewStubStartString();
        static IndirectionStub NewStubStartString()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""StartString"" alias=""StartString"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Start</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process Start(System.String)</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process Start(System.String)</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplForStartString : TypedBehaviorPreparableImpl 
        {
            public ImplForStartString()
                : base(ms_stubStartString, new Type[] {  }, new Type[] {  })
            { }
        }
 
        public virtual ImplForBeginOutputReadLine BeginOutputReadLine() 
        {
            return new ImplForBeginOutputReadLine();
        }

        static IndirectionStub ms_stubBeginOutputReadLine = NewStubBeginOutputReadLine();
        static IndirectionStub NewStubBeginOutputReadLine()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""BeginOutputReadLine"" alias=""BeginOutputReadLine"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">BeginOutputReadLine</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Void BeginOutputReadLine()</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Void BeginOutputReadLine()</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplForBeginOutputReadLine : TypedBehaviorPreparableImpl 
        {
            public ImplForBeginOutputReadLine()
                : base(ms_stubBeginOutputReadLine, new Type[] {  }, new Type[] {  })
            { }
        }
 
        public virtual ImplForBeginErrorReadLine BeginErrorReadLine() 
        {
            return new ImplForBeginErrorReadLine();
        }

        static IndirectionStub ms_stubBeginErrorReadLine = NewStubBeginErrorReadLine();
        static IndirectionStub NewStubBeginErrorReadLine()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""BeginErrorReadLine"" alias=""BeginErrorReadLine"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">BeginErrorReadLine</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Void BeginErrorReadLine()</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Void BeginErrorReadLine()</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplForBeginErrorReadLine : TypedBehaviorPreparableImpl 
        {
            public ImplForBeginErrorReadLine()
                : base(ms_stubBeginErrorReadLine, new Type[] {  }, new Type[] {  })
            { }
        }
 
        public virtual ImplForPriorityClassSetProcessPriorityClass PriorityClassSetProcessPriorityClass() 
        {
            return new ImplForPriorityClassSetProcessPriorityClass();
        }

        static IndirectionStub ms_stubPriorityClassSetProcessPriorityClass = NewStubPriorityClassSetProcessPriorityClass();
        static IndirectionStub NewStubPriorityClassSetProcessPriorityClass()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""PriorityClassSetProcessPriorityClass"" alias=""PriorityClassSetProcessPriorityClass"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">set_PriorityClass</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Diagnostics.Process</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Void set_PriorityClass(System.Diagnostics.ProcessPriorityClass)</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Void set_PriorityClass(System.Diagnostics.ProcessPriorityClass)</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplForPriorityClassSetProcessPriorityClass : TypedBehaviorPreparableImpl 
        {
            public ImplForPriorityClassSetProcessPriorityClass()
                : base(ms_stubPriorityClassSetProcessPriorityClass, new Type[] {  }, new Type[] {  })
            { }
        }


        public static Type Type
        {
            get { return ms_stubStart.GetDeclaringTypeInstance(new Type[] {  }); }
        }
    }
}
