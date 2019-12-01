using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace SandraDCoder
{
    public class Decode
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var str = input.QueryStringParameters["string"];
            var key = input.QueryStringParameters["key"];
            var heavyDict = input.QueryStringParameters["heavy"];
            var guessKey = input.QueryStringParameters["guessKey"];

            if (string.IsNullOrEmpty(str) || (string.IsNullOrEmpty(key) && guessKey!="true"))
            {
                return CreateResponse("bad input");
            }

            if (guessKey == "true")
            {
                return CreateResponse(
                    JsonConvert.SerializeObject(new SandraDecoder().DecodeWithoutKey(str, heavyDict == "true")));
            }

            return CreateResponse(JsonConvert.SerializeObject(new SandraDecoder().Decode(str, key, heavyDict == "true")));
        }

        private APIGatewayProxyResponse CreateResponse(string result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            string body = (result != null) ?
                JsonConvert.SerializeObject(result) : string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };

            return response;
        }
    }
}
