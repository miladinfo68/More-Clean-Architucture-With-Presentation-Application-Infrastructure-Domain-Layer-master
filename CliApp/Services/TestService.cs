namespace CliApp.Services;

public class TestService : ITestService
{
    public void SayHello() => Console.WriteLine("Hello World! I am a Service Only For Test");
}
