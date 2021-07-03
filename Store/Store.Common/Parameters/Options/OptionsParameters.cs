namespace Store.Common.Parameters.Options
{
    public class OptionsParameters : IOptionsParameters
    {
        public string[] IncludeProperties { get; }

        public OptionsParameters(string[] includeProperties)
        {
            IncludeProperties = includeProperties;
        }
    }
}