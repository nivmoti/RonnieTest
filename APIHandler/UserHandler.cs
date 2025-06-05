using RonnieTest.Model;
using Newtonsoft.Json.Linq;

namespace RonnieTest.APIHandler
{
    public class UserHandler
    {
        private readonly string json;
        private readonly int sourceid;

        public UserHandler(string json, int sourceid)
        {
            this.json = json;
            this.sourceid = sourceid;
        }

        public List<User> ParseUsersFromJson()
        {
            var result = new List<User>();
            var token = JToken.Parse(this.json);

            TraverseAndExtractUsers(token, result);
            return result;
        }

        private void TraverseAndExtractUsers(JToken token, List<User> result)
        {
            if (token.Type == JTokenType.Object)
            {
                // Try to parse this token as a user
                var parsed = ParseSingleUser(token);
                if (parsed != null)
                    result.Add(parsed);

                foreach (var child in token.Children())
                    TraverseAndExtractUsers(child, result);
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                    TraverseAndExtractUsers(item, result);
            }
        }

        private User? ParseSingleUser(JToken token)
        {
            if (token.Type != JTokenType.Object) return null;

            string firstName = "";
            string lastName = "";
            string email = "";

            // Case 1: name is an object
            if (token["name"] is JObject nameObj)
            {
                foreach (var prop in nameObj.Properties())
                {
                    var key = prop.Name.ToLower();
                    var value = prop.Value?.ToString()?.Trim() ?? "";

                    if (key.Contains("first") && string.IsNullOrEmpty(firstName))
                        firstName = value;
                    if (key.Contains("last") && string.IsNullOrEmpty(lastName))
                        lastName = value;
                }
            }

            // Case 2: name is a full string
            else if (token["name"]?.Type == JTokenType.String)
            {
                var parts = token["name"]!.ToString().Trim().Split(" ", 2);
                firstName = parts.ElementAtOrDefault(0) ?? "";
                lastName = parts.ElementAtOrDefault(1) ?? "";
            }

            // Case 3: flat properties
            foreach (var prop in token.Children<JProperty>())
            {
                var key = prop.Name.ToLower();
                var value = prop.Value?.ToString()?.Trim() ?? "";

                if (key.Contains("first") && key.Contains("name") && string.IsNullOrEmpty(firstName))
                    firstName = value;

                if (key.Contains("last")  && key.Contains("name") && string.IsNullOrEmpty(lastName))
                    lastName = value;

                if (key.Contains("email") && string.IsNullOrEmpty(email))
                    email = value;
            }

            // Valid user must have at least email or name
            if (string.IsNullOrWhiteSpace(email) &&
                string.IsNullOrWhiteSpace(firstName) &&
                string.IsNullOrWhiteSpace(lastName))
            {
                return null;
            }

            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                SourceId = this.sourceid
            };
        }
    }
}
