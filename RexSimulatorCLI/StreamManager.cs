using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Timers;

namespace RexSimulatorCLI
{
    internal class StreamManager
    {
        private Dictionary<string, Stream> _streams;
        private string _activeStream;
        private Timer _updateTimer;
        private string _previousHash;

        public StreamManager()
        {
            _streams = new Dictionary<string, Stream>();
            _updateTimer = new Timer( 1000 / 30 );
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Enabled = true;
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DrawActiveStream();
        }

        public void SetActiveStream(string name)
        {
            if (_streams.ContainsKey(name))
                _activeStream = name;
            else
                throw new Exception("Stream does not exist");
        }

        public Stream CreateStream(string name)
        {
            var s = new MemoryStream();
            _streams.Add(name, s);
            if (_streams.Count == 1)
                _activeStream = name;
            return s;
        }

        public void RemoveStream(string name)
        {
            if (!_streams.ContainsKey(name))
                throw new Exception("Stream does not exist");

            if (_activeStream == name)
                _activeStream = null;
            _streams.Remove(name);
        }

        public void DrawActiveStream()
        {
            if (string.IsNullOrEmpty(_activeStream)) return;

            var s = _streams[_activeStream];
            s.Seek(0, SeekOrigin.Begin);

            TextReader tr = new StreamReader(s);
            var data = tr.ReadToEnd();
            var hash = CalculateMD5Hash(data);

            if (hash != _previousHash)
            {
                Console.Clear();
                Console.Write(data);
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
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
