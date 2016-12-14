namespace Tailspin.Web.Survey.Shared.Models
{
    using System;

    public enum SubscriptionKind
    {
        Standard,
        Premium
    }

    [Serializable]
    public class Tenant
    {
        public string IssuerValue { get; set; }
        public string TenantId { get; set; }

        public string ClaimType { get; set; }
        
        public string ClaimValue { get; set; }
        
        public string HostGeoLocation { get; set; }
        
        public string IssuerThumbPrint { get; set; }
        
        public string IssuerUrl { get; set; }

        public string IssuerIdentifier { get; set; }
        
        public string Logo { get; set; }

        public string WelcomeText { get; set; }

        public SubscriptionKind SubscriptionKind { get; set; }

        public string ModelExtensionAssembly { get; set; }

        public string ModelExtensionNamespace { get; set; }

        public string SqlAzureConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseUserName { get; set; }

        public string DatabasePassword { get; set; }
        
        public string SqlAzureFirewallIpStart { get; set; }

        public string SqlAzureFirewallIpEnd { get; set; }
    }
}