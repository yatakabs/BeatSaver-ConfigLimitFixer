using System.Threading;

namespace ConfigLimitFixer.Tests;

public class TestContainer
{
    public static int publicStaticIntField = 1;
    public static string publicStaticStringField = "test";
    public static Thread publicStaticThreadField = Thread.CurrentThread;

    public int publicInstanceIntField = 1;
    public string publicInstanceStringField = "test";
    public Thread publicInstanceThreadField = Thread.CurrentThread;

    private static int privateStaticIntField = 1;
    private static string privateStaticStringField = "test";
    private static Thread privateStaticThreadField = Thread.CurrentThread;

    private int privateInstanceIntField = 1;
    private string privateInstanceStringField = "test";
    private Thread privateInstanceThreadField = Thread.CurrentThread;

    public TestContainer()
    {
    }

    public TestContainer(
        int intFieldValue,
        string stringFieldValue,
        Thread threadFieldValue)
    {
        this.privateInstanceIntField = intFieldValue;
        this.privateInstanceStringField = stringFieldValue;
        this.privateInstanceThreadField = threadFieldValue;

        this.publicInstanceIntField = intFieldValue;
        this.publicInstanceStringField = stringFieldValue;
        this.publicInstanceThreadField = threadFieldValue;
    }

    public void SetIntValue(int intFieldValue)
    {
        this.privateInstanceIntField = intFieldValue;
        this.publicInstanceIntField = intFieldValue;
    }

    public void SetStringValue(string stringFieldValue)
    {
        this.privateInstanceStringField = stringFieldValue;
        this.publicInstanceStringField = stringFieldValue;
    }

    public void SetThreadValue(Thread threadFieldValue)
    {
        this.privateInstanceThreadField = threadFieldValue;
        this.publicInstanceThreadField = threadFieldValue;
    }

    public static void SetStaticIntValue(int intFieldValue)
    {
        publicStaticIntField = intFieldValue;
        privateStaticIntField = intFieldValue;
    }

    public static void SetStaticStringValue(string stringFieldValue)
    {
        publicStaticStringField = stringFieldValue;
        privateStaticStringField = stringFieldValue;
    }

    public static void SetStaticThreadValue(Thread threadValue)
    {
        publicStaticThreadField = threadValue;
        privateStaticThreadField = threadValue;
    }
}
