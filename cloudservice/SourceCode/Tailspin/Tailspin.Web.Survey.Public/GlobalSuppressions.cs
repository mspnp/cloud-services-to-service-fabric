using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("BEST.Security.Config", "ASPCONFIG:SecureCookie", Justification = "The Anti-forgery token requires a cookie and this site is not SSL.")]
[assembly: SuppressMessage("Microsoft.WebConfigurationSecurity", "CA3119:HttpCookiesRequireSslRule", Justification = "The Anti-forgery token requires a cookie and this site is not SSL.")]
[assembly: SuppressMessage("Microsoft.WebConfigurationSecurity", "CA3128:MachineKeyValidationKeyRule", Justification = "Need this to support Azure. Need the same key for each machine to support validation token.")]
[assembly: SuppressMessage("Microsoft.WebConfigurationSecurity", "CA3126:MachineKeyDecryptionKeyRule", Justification = "Need this to support Azure. Need the same key for each machine to support validation token.")]