namespace Store.Common.Parameters.Options
{
    public class OptionsFactory : IOptionsFactory
    {
        public IOptionsParameters Create(string[] includeProperties)
        {
            return new OptionsParameters(includeProperties);
        }
    }
}