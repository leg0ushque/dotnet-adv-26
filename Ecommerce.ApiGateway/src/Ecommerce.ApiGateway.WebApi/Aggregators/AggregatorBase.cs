using System.Net;
using System.Text;
using Ecommerce.ApiGateway.WebApi.Models.GatewayModels;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace Ecommerce.ApiGateway.WebApi.Aggregators;

public abstract class AggregatorBase<TAggregationResponse>(ILogger logger) : IDefinedAggregator
    where TAggregationResponse : AggregationResponseBase
{
    protected const string GatewayErrorReason = "GatewayError";
    private const string ApplicationJsonContentType = "application/json";
    private const string OkReason = "OK";

    protected ILogger _logger = logger;

    protected abstract string[] RequiredRouteKeys { get; }

    protected abstract Task<DownstreamResponse> AggregateSuccessfulResponses(
        Dictionary<string, DownstreamResponse> responses);

    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        var contexts = ResolveContexts(responses);
        if (contexts is null)
        {
            return BuildGatewayError(
                $"Missing one or more required downstream contexts: {string.Join(", ", RequiredRouteKeys)}",
                HttpStatusCode.BadGateway);
        }

        var downstreamResponses = GetDownstreamResponses(contexts);
        if (downstreamResponses is null)
        {
            return BuildGatewayError(
                "One or more downstream responses are null",
                HttpStatusCode.BadGateway);
        }

        var failedResponse = ValidateAndReturnFirstError(downstreamResponses);
        if (failedResponse is not null)
        {
            return failedResponse;
        }

        try
        {
            return await AggregateSuccessfulResponses(downstreamResponses);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize downstream response");
            return BuildGatewayError(
                $"Failed to deserialize downstream response: {ex.Message}",
                HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aggregation failed");
            return BuildGatewayError(
                $"Aggregation failed: {ex.Message}",
                HttpStatusCode.InternalServerError);
        }
    }

    protected static bool IsSuccessStatusCode(HttpStatusCode statusCode) =>
        statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.MultipleChoices;

    protected virtual HttpStatusCode MapDownstreamStatus(HttpStatusCode downstreamStatus)
    {
        return downstreamStatus switch
        {
            HttpStatusCode.NotFound => HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest => HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.BadGateway
        };
    }

    protected DownstreamResponse BuildSuccessResponse(
        TAggregationResponse response,
        HttpStatusCode code = HttpStatusCode.OK,
        string reason = OkReason,
        IReadOnlyCollection<Header>? headers = null) =>
        BuildResponse(response, code, reason, headers);

    protected DownstreamResponse BuildErrorResponse(
        TAggregationResponse response,
        HttpStatusCode code,
        string reason,
        IReadOnlyCollection<Header>? headers = null) =>
        BuildResponse(response, code, reason, headers);

    protected DownstreamResponse BuildResponse(
        TAggregationResponse response,
        HttpStatusCode code = HttpStatusCode.OK,
        string reason = OkReason,
        IReadOnlyCollection<Header>? headers = null)
    {
        var body = new StringContent(
            JsonConvert.SerializeObject(response),
            Encoding.UTF8,
            ApplicationJsonContentType);

        return new DownstreamResponse(
            body,
            code,
            headers?.ToList() ?? [],
            reason);
    }

    protected DownstreamResponse BuildGatewayError(string message, HttpStatusCode code)
    {
        var body = new StringContent(
            JsonConvert.SerializeObject(new { error = message }),
            Encoding.UTF8,
            ApplicationJsonContentType);

        return new DownstreamResponse(
            body,
            code,
            new List<Header>(),
            GatewayErrorReason);
    }

    protected static HttpContext? FindByKey(List<HttpContext> responses, string key) =>
        responses.FirstOrDefault(r => r.Items.DownstreamRoute()?.Key == key);

    protected static DownstreamResponse? GetDownstreamResponse(HttpContext? context) =>
        context?.Items.DownstreamResponse();

    private Dictionary<string, HttpContext>? ResolveContexts(List<HttpContext> responses)
    {
        var contexts = new Dictionary<string, HttpContext>();

        foreach (var key in RequiredRouteKeys)
        {
            var ctx = FindByKey(responses, key);
            if (ctx is null)
            {
                _logger.LogDebug("Missing context for required route key: {RouteKey}", key);
                return null;
            }
            contexts[key] = ctx;
        }

        return contexts;
    }

    private Dictionary<string, DownstreamResponse>? GetDownstreamResponses(Dictionary<string, HttpContext> contexts)
    {
        var responses = new Dictionary<string, DownstreamResponse>();

        foreach (var (key, ctx) in contexts)
        {
            var response = GetDownstreamResponse(ctx);
            if (response is null)
            {
                _logger.LogDebug("Downstream response is null for route key: {RouteKey}", key);
                return null;
            }
            responses[key] = response;
        }

        return responses;
    }

    private DownstreamResponse? ValidateAndReturnFirstError(Dictionary<string, DownstreamResponse> responses)
    {
        foreach (var (key, response) in responses)
        {
            _logger.LogDebug("{RouteKey} Status: {StatusCode}", key, response.StatusCode);

            if (!IsSuccessStatusCode(response.StatusCode))
            {
                _logger.LogDebug("Returning error response from {RouteKey}: {StatusCode}", key, response.StatusCode);
                return response;
            }
        }

        _logger.LogDebug("All responses succeeded, proceeding with aggregation");
        return null;
    }
}
