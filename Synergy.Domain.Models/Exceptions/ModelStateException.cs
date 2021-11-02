using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Synergy.Domain.Models.Exceptions
{
    public class ModelStateException : ApplicationException
    {
        public IEnumerable<KeyValuePair<string, string>> State { get; }

        public ModelStateException(IEnumerable<KeyValuePair<string, string>> state)
        {
            this.State = state;
        }

        public ModelStateException(string propertyName, string message)
        {
            this.State = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(propertyName, message)
            };
        }

        public ModelStateException(string message)
            : base(message)
        {
        }

        public ModelStateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public override string Message => JsonConvert.SerializeObject(this.State);
    }
}
