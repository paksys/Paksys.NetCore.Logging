namespace Paksys.NetCore.Logging
{
    public interface ILogger
    {
        void Perf(LogDetail logDetail);
        void Usage(LogDetail logDetail);
        void Verbose(LogDetail logDetail);
        void Debug(LogDetail logDetail);
        void Info<T>(T propertyValue);
        void Warn(LogDetail logDetail);
        void Error(LogDetail logDetail);
        void Fatal(LogDetail logDetail);
    }
}
