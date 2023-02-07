namespace Apeek.Common.Configuration
{
    public interface IAppSettings
    {
        string ConnectionStringMysql { get; }
        string ConnectionStringTest { get; }
    }
}