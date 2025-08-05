using Common.Libraries.EventSourcing;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class Username : Value<Username>
    {
        [JsonConstructor]
        internal Username(string value) => Value = value;

        // Satisfy the serialization requirements 
       
        protected Username() { }

        public string Value { get; }

        public static Username FromString(string title)
        {
            CheckValidity(title);
            return new Username(title);
        }

       

        public static implicit operator string(Username name) => name.Value;

        static void CheckValidity(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(
                    nameof(value),
                    "name cannot be empty");

            if (value.Length < 3)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "name cannot be shorter than 3 characters");

           
        }
    }
}
