using Polly;
using Polly.Timeout;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace IdealAPI.Utility
{
    public class ResiliencyPolly
    {
        private readonly IAsyncPolicy<RestResponse> _retryPolicy;
        private readonly IAsyncPolicy<RestResponse> _timeoutPolicy;
        private readonly IAsyncPolicy<RestResponse> _circuiteBreaker;

        public ResiliencyPolly()
        {
            // Retry policy for handling failed requests
            _retryPolicy = Policy
                .HandleResult<RestResponse>(response => !response.IsSuccessful)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (response, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"Error: {response.Result?.ErrorMessage ?? "Unknown error"}.. Retry Count: {retryCount}");
                    });
            _circuiteBreaker = Policy
                .HandleResult<RestResponse>(response => !response.IsSuccessful)
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

        }

        public async Task<RestResponse> ConnectApi()
        {
            var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";
            var client = new RestClient();
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("x-rapidapi-key", "Sign Up for Key"); // Replace with your actual API key
            request.AddHeader("x-rapidapi-host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");
            request.AddHeader("accept", "application/json");

            var policyWrap = Policy.WrapAsync(_retryPolicy,_circuiteBreaker);

            var response = await policyWrap.ExecuteAsync(() => client.ExecuteAsync(request));

            if (response.IsSuccessful)
            {
                Console.Out.WriteLine(response.Content);
            }
            else
            {
                Console.WriteLine($"Request failed: {response.ErrorMessage}");
            }

            return response;
        }
    }
}
