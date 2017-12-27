using System;
using System.Security;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using RssFeed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT_RssFeed
{
    [TestClass]
    public class CredentialsTest
    {
        [TestMethod]
        public void TestUserNameAndPasswordCanBeSetAndRetrieved()
        {
            const string username = "UsErNamE_001";
            const string password = "P@$$W0rd_12345";

            Credentials credentials = new Credentials();
            credentials.Username = username;
            credentials.Password = password;

            Assert.AreEqual(username, credentials.Username);
            Assert.AreEqual(password, credentials.Password);
            Assert.AreEqual(password, ExtractStringFromSecureString(credentials.GetSecurePassword()));
        }

        [TestMethod]
        public void TestUserNameAndPasswordCanBeSetSerilizedAndRetrieved()
        {
            
            const string username = "UsErNamE_001";
            const string password = "P@$$W0rd_12345";

            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Credentials));

                // Create the credentials
                {
                    Credentials credentials = new Credentials();
                    credentials.Username = username;
                    credentials.Password = password;

                    // Serialize the credentials
                    serializer.Serialize(stream, credentials);
                }

                // Set the strean to the starting position before reading it
                stream.Position = 0;

                // Deserialize the credentials
                {
                    Credentials credentials = serializer.Deserialize(stream) as Credentials;

                    Assert.AreEqual(username, credentials.Username);
                    Assert.AreEqual(password, credentials.Password);
                    Assert.AreEqual(password, ExtractStringFromSecureString(credentials.GetSecurePassword()));
                }
            }
        }

        [TestMethod]
        public void TestPasswordIsNotSerialized()
        {
            const string username = "UsErNamE_001";
            const string password = "P@$$W0rd_12345";

            // Create the credentials
            Credentials credentials = new Credentials();
            credentials.Username = username;
            credentials.Password = password;

            using (MemoryStream stream = new MemoryStream())
            {
                // Serialize the credentials
                XmlSerializer serializer = new XmlSerializer(typeof(Credentials));
                serializer.Serialize(stream, credentials);

                // Verify password is not in the stream
                string xml = Encoding.UTF8.GetString(stream.GetBuffer(), 0, stream.GetBuffer().Length);

                // Verify the password is not in the xml
                Assert.IsFalse(xml.Contains(password));

                // Verify that the password property is not in the xml
                Assert.IsFalse(xml.ToLower().Contains("password"));
            }
        }

        private string ExtractStringFromSecureString(SecureString secureString)
        {
            return (new System.Net.NetworkCredential(string.Empty, secureString)).Password;
        }
    }
}
