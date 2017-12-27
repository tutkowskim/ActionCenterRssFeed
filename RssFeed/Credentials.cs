using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RssFeed
{
    /// <summary>
    /// Credential information.
    /// </summary>
    /// <remarks>
    /// Passwords are stored in 'Data Protection API (DPAPI)' so that they
    /// can be safely stored until they are needed to be passed to
    /// an external source.
    /// </remarks>
    public class Credentials
    {
        [XmlElement("Username")]
        public string Username { get; set; } = string.Empty;

        [XmlElement("Entropy")]
        public byte[] Entropy { get; set; } = new byte[0];

        [XmlElement("CipherText")]
        public byte[] Ciphertext { get; set; } = new byte[0];

        [XmlIgnore]
        public string Password
        {
            get
            {
                if (Entropy.Length == 0 || Ciphertext.Length == 0)
                {
                    // The password was never set. Returing an empty string.
                    return string.Empty;
                }

                return Encoding.UTF8.GetString(ProtectedData.Unprotect(Ciphertext, Entropy, DataProtectionScope.CurrentUser));
            }
            set
            {
                // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
                byte[] plaintext = Encoding.UTF8.GetBytes(value);

                // Generate additional entropy (will be used as the Initialization vector)
                Entropy = new byte[20];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(Entropy);
                }

                Ciphertext = ProtectedData.Protect(plaintext, Entropy, DataProtectionScope.CurrentUser);
            }
        }

        public SecureString GetSecurePassword()
        {
            if (Entropy.Length == 0 || Ciphertext.Length == 0)
            {
                // The password was never set. Returing an empty string.
                return new SecureString();
            }

            SecureString securePassword = new SecureString();
            byte[] bytes = ProtectedData.Unprotect(Ciphertext, Entropy, DataProtectionScope.CurrentUser);
            foreach (byte b in bytes)
            {
                securePassword.AppendChar(Convert.ToChar(b));
            }

            return securePassword;
        }
    }
}
