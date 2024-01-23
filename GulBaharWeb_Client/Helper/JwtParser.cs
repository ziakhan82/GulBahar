using System.Security.Claims;
using System.Text.Json;

namespace GulBaharWeb_Client.Helper
{
 
        public static class JwtParser
        {
        // jwt parser method to consume the jwt token, token will be send from API to client in all subsequent req, extraxt claim from that
            public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
            {
                var claims = new List<Claim>();
                var payload = jwt.Split('.')[1]; // spliting jwt and first piece is paylaod

                var jsonBytes = ParseBase64WithoutPadding(payload); // parsing based on payload
                // extracting key value
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
                return claims; // adding key value pairs to cliams and returing back ienum of claims
            }
            private static byte[] ParseBase64WithoutPadding(string base64)
            {
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }
                return Convert.FromBase64String(base64);
            }
        }
    }

