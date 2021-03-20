namespace Store.Common.Parameters.Options
{
    public class OptionsFactory : IOptionsFactory
    {
        public OptionsFactory()
        {
        }

        public IOptionsParameters Create(string[] properties = null)
        {
            return new OptionsParameters(properties);
        }
    }
}