using RonnieTest.Model;
using Newtonsoft.Json.Linq;

namespace RonnieTest.APIHandler
{
    public class UserHandler
    {
        private  string json;
        private  int sourceid;

        public UserHandler(string json, int sourceid)
        {
            this.json = json;
            this.sourceid = sourceid;
        }

        public List<User> ParseUsersFromJson()
        {
            var result = new List<User>();
            var root = JToken.Parse(this.json);
            Traverse(root, result);
            return result;
        }

        private void Traverse(JToken token, List<User> result)
        {
            if (token.Type == JTokenType.Object)
            {
                string firstName = "", lastName = "", email = "";

                foreach (var prop in token.Children<JProperty>())
                {
                    var key = prop.Name.ToLower();
                    var value = prop.Value;

                    if (key.Contains("name"))
                    {
                        (firstName, lastName) = ParseName(value);
                    }
                    else if (key.Contains("email"))
                    {
                        email = ParseEmail(value);
                    }
                    else
                    {
                        // Continue traversing other properties
                        Traverse(value, result);
                    }
                }

                if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName) || !string.IsNullOrWhiteSpace(email))
                {
                    result.Add(new User
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        SourceId = this.sourceid
                    });
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                    Traverse(item, result);
            }
        }

        private (string FirstName, string LastName) ParseName(JToken nameToken)
        {
            string firstName = "", lastName = "";

            if (nameToken.Type == JTokenType.String)
            {
                var parts = nameToken.ToString().Trim().Split(" ", 2);
                firstName = parts.ElementAtOrDefault(0) ?? "";
                lastName = parts.ElementAtOrDefault(1) ?? "";
            }
            else if (nameToken.Type == JTokenType.Object)
            {
                foreach (var prop in nameToken.Children<JProperty>())
                {
                    var key = prop.Name.ToLower();
                    var value = prop.Value?.ToString()?.Trim() ?? "";

                    if (key.Contains("first") && string.IsNullOrWhiteSpace(firstName))
                        firstName = value;
                    if (key.Contains("last") && string.IsNullOrWhiteSpace(lastName))
                        lastName = value;
                }
            }

            return (firstName, lastName);
        }

        private string ParseEmail(JToken emailToken)
        {
            return emailToken.Type == JTokenType.String ? emailToken.ToString().Trim() : "";
        }
    }
}
