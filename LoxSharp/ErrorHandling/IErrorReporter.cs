namespace LoxSharp.ErrorHandling
{
    public interface IErrorReporter
    {
        bool HadError { get; }
        void Error(int line, string message);
        void Reset();
    }

    public sealed class ConsoleErrorReporter : IErrorReporter
    {
        public void Error(int line, string message)
        {
            Console.Error.WriteLine($"[Line {line}] Error: {message}");
            HadError = true;
        }

        public void Reset()
        {
            HadError = false;
        }

        public bool HadError { get; private set; }
    }
}