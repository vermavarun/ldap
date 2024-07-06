// References:
// https://ianatkinson.net/computing/adcsharp.htm

using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.DirectoryServices;

var builder = new ConfigurationBuilder().AddJsonFile($"config.json", true, true);
var config = builder.Build();

var ADIP = config["ADIP"];
var ADUser = config["ADUser"];
var ADPassword = config["ADPassword"];

DirectoryEntry myLdapConnection = createDirectoryEntry(ADIP, ADUser, ADPassword);
DirectorySearcher search = new(myLdapConnection);

var username = "pu1";
var password = "";
var updatedTitle = "Software Engineer";

//////////////////////////////////////////////////////////////
//SearchUser(search,username);
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
//UpdateUser(search,username,updatedTitle);
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
//AllUsers(search);
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
//bool isAuth = AuthenticateUser(ADIP, username, password);
//Console.WriteLine(isAuth);
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
//GetAllGroups(search);
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
bool isAdmin = IsUserAdmin(search, username);
Console.WriteLine(isAdmin);
//////////////////////////////////////////////////////////////

static void AllUsers(DirectorySearcher search)
{
    try
    {
        search.PropertiesToLoad.Add("cn");

        SearchResultCollection allUsers = search.FindAll();

        foreach (SearchResult result in allUsers)
        {
            if (result.Properties["cn"].Count > 0)
            {
                Console.WriteLine(result.Properties["cn"][0].ToString());
            }
        }
    }

    catch (Exception e)
    {
        Console.WriteLine("Exception caught:\n\n" + e.ToString());
    }

}

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

static bool AuthenticateUser(string ADIP, string userName, string password)
{
    bool ret = false;

    try
    {
        DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ADIP + ":389", userName, password);
        DirectorySearcher dsearch = new DirectorySearcher(ldapConnection);
        SearchResult results = null;

        results = dsearch.FindOne();

        ret = true;
    }
    catch
    {
        ret = false;
    }

    return ret;
}

static void GetAllGroups(DirectorySearcher ds)
{
    // Sort by name
    ds.Sort = new SortOption("name", SortDirection.Ascending);
    ds.PropertiesToLoad.Add("name");
    ds.PropertiesToLoad.Add("memberof");
    ds.PropertiesToLoad.Add("member");

    ds.Filter = "(&(objectCategory=Group))";

    var results = ds.FindAll();

    foreach (SearchResult sr in results)
    {
        if (sr.Properties["name"].Count > 0)
            Debug.WriteLine(sr.Properties["name"][0].ToString());

        if (sr.Properties["memberof"].Count > 0)
        {
            Debug.WriteLine("  Member of...");
            foreach (string item in sr.Properties["memberof"])
            {
                Console.WriteLine("    " + item);
            }
        }
        if (sr.Properties["member"].Count > 0)
        {
            Debug.WriteLine("  Members");
            foreach (string item in sr.Properties["member"])
            {
                Console.WriteLine("    " + item);
            }
        }
    }
}

static bool IsUserAdmin(DirectorySearcher search, string username)
{
    search.Filter = $"(cn={username})";
    search.PropertiesToLoad.Add("memberOf");
    List<string> groups = new List<string>();
    // create results objects from search object  

    SearchResult result = search.FindOne();

    if (result != null)
    {
        foreach (object memberOf in result.Properties["memberOf"])
        {
            groups.Add(memberOf.ToString().Split("CN=")[1].Split(",")[0]);

            // All Groups of current user.
            // Console.WriteLine(memberOf.ToString().Split("CN=")[1].Split(",")[0]);  
        }

    }
    return groups.Contains("Administrators");
}