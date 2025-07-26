namespace Presentation.Models.Payment
{
    public class RedirectToGatewayModel
    {
        public string Token { get; set; }

        public string CallbackUrl { get; set; }
    }
}
