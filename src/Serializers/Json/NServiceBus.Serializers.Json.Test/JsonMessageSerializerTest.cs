using System;
using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NUnit.Framework;

namespace NServiceBus.Serializers.Json.Test
{
  public class A : IMessage
  {
    public byte[] Data;
    public string S;
    public int I { get; set; }
  }

  public interface IA : IMessage
  {
    byte[] Data { get; set; }
    string S { get; set; }
    int I { get; set; }
    B B { get; set; }
  }

  public class B
  {
    public string BS { get; set; }
  }

  [TestFixture]
  public class JsonMessageSerializerTest
  {
    [Test]
    public void Test()
    {
      var messageMapper = new MessageMapper();
      messageMapper.Initialize(new[] { typeof(IA), typeof(A) });

      var serializer = new JsonMessageSerializer(messageMapper);

      var obj = new A() { Data = new byte[32], I = 23, S = "Foo" };

      new Random().NextBytes(obj.Data);

      var output = new MemoryStream();

      serializer.Serialize(new IMessage[] { obj }, output);

      output.Position = 0;

      File.WriteAllBytes("json.txt", output.ToArray());

      output.Position = 0;

      var result = serializer.Deserialize(output);

      Assert.IsNotEmpty(result);
      Assert.That(result, Has.Length.EqualTo(1));

      Assert.That(result[0], Is.TypeOf(typeof(A)));
      var a = ((A)result[0]);

      Assert.AreEqual(a.Data, obj.Data);
      Assert.AreEqual(23, a.I);
      Assert.AreEqual("Foo", a.S);
    }

    [Test]
    public void TestInterfaces()
    {
      var messageMapper = new MessageMapper();
      messageMapper.Initialize(new[] {typeof (IA), typeof (A)});

      var serializer = new JsonMessageSerializer(messageMapper);

      var output = new MemoryStream();

      var obj = messageMapper.CreateInstance<IA>(
        x =>
          {
            x.S = "kalle";
            x.I = 42;
            x.Data = new byte[23];
            x.B = new B {BS = "BOO"};
          }
        );

      new Random().NextBytes(obj.Data);

      serializer.Serialize(new IMessage[] { obj }, output);

      output.Position = 0;

      File.WriteAllBytes("json2.txt", output.ToArray());

      output.Position = 0;

      var result = serializer.Deserialize(output);

      Assert.IsNotEmpty(result);
      Assert.That(result, Has.Length.EqualTo(1));

      Assert.That(result[0], Is.AssignableTo(typeof(IA)));
      var a = ((IA)result[0]);

      Assert.AreEqual(a.Data, obj.Data);
      Assert.AreEqual(42, a.I);
      Assert.AreEqual("kalle", a.S);
      Assert.IsNotNull(a.B);
      Assert.AreEqual("BOO", a.B.BS);
    }
  }
}
