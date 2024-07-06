using Microsoft.Extensions.Configuration;

using System.DirectoryServices;

var builder = new ConfigurationBuilder() 
                 .AddJsonFile($"config.json", true, true);

var config = builder.Build();

var ADIP = config["ADIP"];
var ADUser = config["ADUser"];
var ADPassword = config["ADPassword"];


DirectoryEntry myLdapConnection = createDirectoryEntry(ADIP,ADUser,ADPassword);
DirectorySearcher search = new DirectorySearcher(myLdapConnection);
var username = "pu1";
search.Filter = "(cn=" + username + ")";

// create results objects from search object  

SearchResult result = search.FindOne();

if (result != null)
{
    // user exists, cycle through LDAP fields (cn, telephonenumber etc.)  

    ResultPropertyCollection fields = result.Properties;

    foreach (String ldapField in fields.PropertyNames)
    {
        // cycle through objects in each field e.g. group membership  
        // (for many fields there will only be one object such as name)  

        foreach (Object myCollection in fields[ldapField])
            Console.WriteLine(String.Format("{0,-20} : {1}",
                          ldapField, myCollection.ToString()));
    }
}

else
{
    // user does not exist  
    Console.WriteLine("User not found!");
}


static DirectoryEntry createDirectoryEntry(string ADIP,string ADUser,string ADPassword)
{
    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://"+ ADIP + ":389", ADUser, ADPassword);
    //ldapConnection.Path = "LDAP://CN-Users,DC=varun,DC=com";
    ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
    return ldapConnection;
}