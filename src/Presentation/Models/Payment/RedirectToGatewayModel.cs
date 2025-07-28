namespace Presentation.Models.Payment
{
    public record RedirectToGatewayModel
    {
        public string Token { get; set; }

        public string RedirctToGatewayUrl { get; set; }
    }
}
