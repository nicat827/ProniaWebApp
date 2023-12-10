namespace Pronia.ViewModels
{
    public class CheckoutVM
    {
        public string  Address { get; set; }

        public List<CheckoutItemVM>? CheckoutItems { get; set; }
    }
}
