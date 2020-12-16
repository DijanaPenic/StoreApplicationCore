namespace Store.EmailTemplate.Models
{
    public class ConfirmAccountEmailViewModel
    {
        public ConfirmAccountEmailViewModel(string confirmUrl)
        {
            ConfirmUrl = confirmUrl;
        }

        public string ConfirmUrl { get; set; }
    }
}