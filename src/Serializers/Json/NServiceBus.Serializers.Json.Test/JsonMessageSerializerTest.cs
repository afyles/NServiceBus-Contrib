using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InExchange.NServiceBus.Serializers;
using InExchange.NServiceBus.Serializers.DoesNotWorkYet;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NUnit.Framework;
//using SharpTestsEx;

namespace InExchange.NServiceBus.Test
{
  public class A : IMessage
  {
    public byte[] Data;
  }

  public interface IA : IMessage
  {
    string S { get; set; }
  }

  [TestFixture]
  public class JsonMessageSerializerTest
  {
    [Test]
    public void Test()
    {
      var serializer = new JsonMessageSerializer();

      var obj = new A() { Data = new byte[32] };

      new Random().NextBytes(obj.Data);

      var output = new MemoryStream();

      serializer.Serialize(new IMessage[] { obj }, output);

      output.Position = 0;

      File.WriteAllBytes("json.txt", output.ToArray());

      output.Position = 0;

      var result = serializer.Deserialize(output);

      //result.Should()
      //  .Not.Be.Null()
      //  .And.Have.Count.EqualTo(1);

      //result[0].Should().Be.OfType<A>();
    }

    [Test]
    public void TestInterfaces()
    {
      var serializer = new JsonMessageSerializer();

      var output = new MemoryStream();

      var messageMapper = new MessageMapper();
      messageMapper.Initialize(new [] { typeof(IA), typeof(A) });

      var obj = messageMapper.CreateInstance<IA>(x => x.S = "kalle");

      var mappedType = messageMapper.GetMappedTypeFor(typeof (A));
      var typeName = messageMapper.GetNewTypeName(typeof (A));

      serializer.Serialize(new IMessage[] { obj }, output);

      output.Position = 0;

      File.WriteAllBytes("json.txt", output.ToArray());

      output.Position = 0;

      var result = serializer.Deserialize(output);

      //result.Should()
      //  .Not.Be.Null()
      //  .And.Have.Count.EqualTo(1);

      //result[0].Should().Be.OfType<IA>();
      //((IA) result[0]).S.Should().Be.EqualTo("kalle");
    }
  }
}
