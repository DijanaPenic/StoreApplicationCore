namespace Store.Common.Parameters.Options
{
    public interface IOptionsFactory
    {
        IOptionsParameters Create(string[] properties = null);
    }
}