using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Timers;

namespace RexSimulatorCLI
{
    internal class PanelManager
    {
        private Dictionary<string, Panel> _panels;
        private string _activePanel;
        private Timer _updateTimer;
        private string _previousHash;

        public PanelManager()
        {
            _panels = new Dictionary<string, Panel>();
            _updateTimer = new Timer( 1000 / 30 );
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Enabled = true;
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DrawActivePanel();
        }

        public string ActivePanel
        {
            get { return _activePanel; }
            set
            {
                if (_panels.ContainsKey(value))
                    _activePanel = value;
                else
                    throw new Exception(string.Format("Panel \"{0}\" does not exist", value));
            }
        }

        public void MoveToNextStream()
        {
            int i = _panels.Keys.ToList().IndexOf(_activePanel);
            if (_panels.Keys.Count > i + 1) //If there is a next value
            {
                _activePanel = _panels.Keys.ToList()[i + 1];
            }
            else if (_panels.Keys.Count == i + 1 && _panels.Keys.Count > 1)
            {
                _activePanel = _panels.Keys.ToList()[0];
            }
        }

        public void MoveToPrevStream()
        {
            int i = _panels.Keys.ToList().IndexOf(_activePanel);
            if (_panels.Keys.Count <= i + 1) //If there is a prev value
            {
                _activePanel = _panels.Keys.ToList()[i - 1];
            }
            else if (i == 0 && _panels.Keys.Count > 1)
            {
                _activePanel = _panels.Keys.ToList()[_panels.Keys.Count - 1];
            }
        }

        public void AddPanel(string name, Panel panel)
        {
            _panels.Add(name, panel);
            if (_panels.Count == 1)
                _activePanel = name;
        }

        public void RemovePanel(string name)
        {
            if (!_panels.ContainsKey(name))
                throw new Exception("Panel does not exist");

            if (_activePanel == name)
                _activePanel = null;
            _panels.Remove(name);
        }

        public void DrawActivePanel()
        {
            if (string.IsNullOrEmpty(_activePanel)) return;

            var s = _panels[_activePanel].BaseStream;
            s.Seek(0, SeekOrigin.Begin);

            TextReader tr = new StreamReader(s);
            var data = tr.ReadToEnd();
            var hash = CalculateMD5Hash(data);

            if (hash != _previousHash)
            {
                Console.Clear();
                Console.Write(data.Replace("\n", "").Replace("\r", "\r\n"));
                _previousHash = hash;
            }
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public void SendInputToActiveStream(ConsoleKeyInfo info)
        {
            _panels[_activePanel].SendInput(info);
        }
    }
}
