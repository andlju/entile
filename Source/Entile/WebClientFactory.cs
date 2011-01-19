namespace Entile
{
    public class WebClientFactory : IWebClientFactory
    {
        public IWebClient CreateWebClient()
        {
            return new WebClientAdapter();
        }
    }
}