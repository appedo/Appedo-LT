using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;



namespace AppedoLT.Core
{
   public class ConnectionManager
    {
        private int _maxConnction;
        private List<Connection> connections = new List<Connection>();
        public ConnectionManager(int maxConnction)
        {
            _maxConnction = maxConnction;
        }
        public Connection GetConnection(string host, int port)
        {
            Connection connection = null;
            if (connections.Exists(f => f.Host == host && f.Port == port) == true)
            {
                if (connections.Find(f => f.Host == host && f.Port == port).IsHold == false)
                {
                    connection = connections.Find(f => f.Host == host && f.Port == port);
                    connection.IsHold = true;
                }
                else
                {
                    if (connections.FindAll(f => f.Host == host && f.Port == port).Count < _maxConnction)
                    {
                        Connection newConnection = new Connection(host, port);
                        connections.Add(newConnection);
                        connection = newConnection;
                        connection.IsHold = true;
                    }
                    else
                    {
                        while (connections.Find(f => f.Host == host && f.Port == port && f.IsHold == false) == null)
                        {
                            Thread.Sleep(100);
                        }
                        connection = connections.Find(f => f.Host == host && f.Port == port && f.IsHold == false);
                        connection.IsHold = true;
                    }
                }
            }
            else
            {
                Connection newConnection = new Connection(host, port);
                connections.Add(newConnection);
                connection = newConnection;
                connection.IsHold = true;
            }
            
            return connection;

        }
        public void CloseAllConnetions()
        {
            foreach (Connection con in connections)
            {
                try
                {
                    con.client.Close();
                }
                catch
                {

                }
            }
        }
    }
}