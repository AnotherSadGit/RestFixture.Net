namespace RestClient
{
    public class ProxyInfo
    {
        public string Address { get; private set; }
        public int Port { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }

        public ProxyInfo(string address, int port, string userName, string password, string domain)
        {
            Address = address;
            Port = port;
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public ProxyInfo(string address, int port, string userName, string password) 
            : this(address, port, userName, password, string.Empty)
        { }

        public ProxyInfo(string address, int port)
            : this(address, port, string.Empty, string.Empty, string.Empty)
        { }

        public ProxyInfo(string address)
            : this(address, 0, string.Empty, string.Empty, string.Empty)
        { }
    }
}