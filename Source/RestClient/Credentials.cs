namespace RestClient
{
    public class Credentials
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }

        public Credentials(string userName, string password, string domain)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public Credentials(string userName, string password) 
            : this(userName, password, string.Empty)
        { }
    }
}