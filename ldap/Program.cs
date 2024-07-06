using Microsoft.Extensions.Configuration;
using System.DirectoryServices;

var builder = new ConfigurationBuilder().AddJsonFile($"config.json", true, true);
var config = builder.Build();

var ADIP = config["ADIP"];
var ADUser = config["ADUser"];
var ADPassword = config["ADPassword"];

DirectoryEntry myLdapConnection = createDirectoryEntry(ADIP, ADUser, ADPassword);
DirectorySearcher search = new(myLdapConnection);

var username = "pu1";
var updatedTitle = "Software Engineer";

SearchUser(search,username);
UpdateUser(search,username,updatedTitle);

static DirectoryEntry createDirectoryEntry(string ADIP, string ADUser, string ADPassword)
{
    DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ADIP + ":389", ADUser, ADPassword);
    //ldapConnection.Path = "LDAP://CN-Users,DC=varun,DC=com";
    ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
    return ldapConnection;
}

static void SearchUser(DirectorySearcher search,string username)
{
    search.Filter = $"(cn={username})";

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
}

static void UpdateUser(DirectorySearcher search, string username, string newTitle)
{
    search.Filter = $"(cn={username})";

    // create results objects from search object  

    SearchResult result = search.FindOne();

    if (result != null)
    {
        // create new object from search result  

        DirectoryEntry entryToUpdate = result.GetDirectoryEntry();

        // show existing title  

        Console.WriteLine("Current title   : " + entryToUpdate.Properties["title"].Value);

    
        entryToUpdate.Properties["title"].Value = newTitle;
        entryToUpdate.CommitChanges();

        Console.WriteLine("\n\n...new title saved");
    }

    else Console.WriteLine("User not found!");
}  
