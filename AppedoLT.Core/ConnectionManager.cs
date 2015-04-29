using System.Collections.Generic;
using System.Threading;

namespace AppedoLT.Core
{
    public class ConnectionManager
    {

        #region The private fields

        private int _maxConnction;
        private List<Connection> _connections = new List<Connection>();

        #endregion

        #region The constructor

        public ConnectionManager(int maxConnction)
        {
            _maxConnction = maxConnction;
        }

        #endregion

        #region The public methods

        public Connection GetConnection(string host, int port)
        {
            Connection connection = null;
            if (_connections.Exists(f => f.Host == host && f.Port == port) == true)
            {
                if (_connections.Find(f => f.Host == host && f.Port == port).IsHold == false)
                {
                    connection = _connections.Find(f => f.Host == host && f.Port == port);
                    connection.IsHold = true;
                }
                else
                {
                    if (_connections.FindAll(f => f.Host == host && f.Port == port).Count < _maxConnction)
                    {
                        Connection newConnection = new Connection(host, port);
                        _connections.Add(newConnection);
                        connection = newConnection;
                        connection.IsHold = true;
                    }
                    else
                    {
                        while (_connections.Find(f => f.Host == host && f.Port == port && f.IsHold == false) == null)
                        {
                            Thread.Sleep(100);
                        }
                        connection = _connections.Find(f => f.Host == host && f.Port == port && f.IsHold == false);
                        connection.IsHold = true;
                    }
                }
            }
            else
            {
                Connection newConnection = new Connection(host, port);
                _connections.Add(newConnection);
                connection = newConnection;
                connection.IsHold = true;
            }

            return connection;

        }

        public void CloseAllConnetions()
        {
            foreach (Connection con in _connections)
            {
                try
                {
                    con.Client.Close();
                }
                catch
                {

                }
            }
        }

        #endregion

    }
}