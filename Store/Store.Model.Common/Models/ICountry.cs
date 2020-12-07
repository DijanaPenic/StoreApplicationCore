namespace Store.Model.Common.Models
{
    public interface ICountry
    {
        string Name { get; set; }

        string AlphaTwoCode { get; set; }

        string AlphaThreeCode { get; set; }

        string[] CallingCodes { get; set; }

        string Region { get; set; }

        string NumericCode { get; set; }
    }
}