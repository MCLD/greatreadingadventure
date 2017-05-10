using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GRA.CommandLine.FakeWeb
{
    public class FakeSession : ISession
    {
        private Dictionary<string, byte[]> _values = new Dictionary<string, byte[]>();
        private string _id;

        public FakeSession()
        {
            _id = Guid.NewGuid().ToString();
        }
        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public string Id {
            get
            {
                return _id;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _values.Keys;
            }
        }

        public void Clear()
        {
            _values = new Dictionary<string, byte[]>();
        }

        public async Task CommitAsync()
        {
            return;
        }

        public async Task LoadAsync()
        {
            return;
        }

        public void Remove(string key)
        {
            _values.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _values[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (!_values.ContainsKey(key))
            {
                value = null;
                return false;
            }
            value = _values[key];
            return true;
        }
    }
}
