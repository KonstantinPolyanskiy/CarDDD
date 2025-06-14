﻿using Serilog.Context;

namespace CarDDD.Api.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        const string header = "X-Correlation-Id";

        if (!ctx.Request.Headers.TryGetValue(header, out var id))
            id = Guid.NewGuid().ToString();

        ctx.Response.Headers[header] = id;

        using (LogContext.PushProperty("CorrelationId", id))
            await next(ctx);
    }
}