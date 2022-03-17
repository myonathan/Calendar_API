using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using AppointmentAPI.MiddleWare.ServiceResponseMiddleWare.Extension;
using AppointmentAPI.MiddleWare.ServiceResponseMiddleWare.wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Appointment.Utilities;
using Appointment.Resources;

namespace AppointmentAPI.MiddleWare.ServiceResponseMiddleWare
{
    public class ServiceResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiServiceResponseWrapperOptions _options;
        private readonly ILogger<ServiceResponseMiddleware> _logger;
        public ServiceResponseMiddleware(RequestDelegate next, ApiServiceResponseWrapperOptions options, ILogger<ServiceResponseMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsSwagger(context))
                await this._next(context);
            else
            {
                var stopWatch = Stopwatch.StartNew();

                var request = await FormatRequest(context.Request);

                var originalBodyStream = context.Response.Body;

                using (var bodyStream = new MemoryStream())
                {
                    try
                    {
                        context.Response.Body = bodyStream;

                        await _next.Invoke(context);

                        context.Response.Body = originalBodyStream;
                        if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            var bodyAsText = await FormatResponse(bodyStream);
                            await HandleSuccessRequestAsync(context, bodyAsText, context.Response.StatusCode);
                        }
                        else
                        {
                            await HandleNotSuccessRequestAsync(context, context.Response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        await HandleExceptionAsync(context, ex);
                        bodyStream.Seek(0, SeekOrigin.Begin);
                        await bodyStream.CopyToAsync(originalBodyStream);
                    }
                    finally
                    {
                        stopWatch.Stop();
                        _logger.Log(LogLevel.Information,
                                    $@"Request: {request} Responded with [{context.Response.StatusCode}] in {stopWatch.ElapsedMilliseconds}ms");
                    }
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new ServiceResponse(false);
            if (exception is ExpectedException)
            {
                var exp = exception as ExpectedException;

                response.Message = new { code = exp.ErrorCode, Message = exp.Message };
                _logger.Log(LogLevel.Warning, exception, $"[{exp.ErrorCode}]: {exp.Message}");
            }
            else
            {
                var message = $"{ exception.GetBaseException().Message }";
                string stackTrace = exception.StackTrace;

                response.Message = new { code = "ERUKN", Message = "UnKnown Error" };
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //   response.Message = new { code = exp.ErrorCode, Message = exp.Message };
                _logger.Log(LogLevel.Error, exception, $"[ERUKN]: {stackTrace}");
            }

            var jsonString = ConvertToJSONString(GetErrorResponse(response));

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(jsonString);
        }

        private ServiceResponse GetErrorResponse(ServiceResponse response)
        {
            response.Version = GetApiVersion();
            return response;
        }
        private string ConvertToJSONString(object rawJSON)
        {
            return JsonConvert.SerializeObject(rawJSON, JSONSettings());
        }
        private JsonSerializerSettings JSONSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
        }
        private Task HandleNotSuccessRequestAsync(HttpContext context, int code)
        {
            ServiceResponse apiError = new ServiceResponse(false);
            context.Response.StatusCode = code;

            var jsonString = ConvertToJSONString(GetErrorResponse(apiError));

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(jsonString);
        }

        private Task HandleSuccessRequestAsync(HttpContext context, string body, int code)
        {
            ServiceResponse response = new ServiceResponse(true);

            string jsonString = string.Empty;

            var bodyText = !body.ToString().IsValidJson() ? ConvertToJSONString(body) : body.ToString();

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText);
            Type type = bodyContent?.GetType();
            response.Result = bodyContent;

            jsonString = ConvertToJSONString(GetSucessResponse(response));
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(jsonString);
        }
        private ServiceResponse GetSucessResponse(ServiceResponse apiResponse)
        {
            if (apiResponse.Version.Equals("1.0.0.0"))
                apiResponse.Version = GetApiVersion();

            return apiResponse;
        }
        private string GetApiVersion()
        {
            return string.IsNullOrEmpty(_options.ApiVersion) ? "1.0.0.0" : _options.ApiVersion;
        }
        private async Task<string> FormatRequest(HttpRequest request)
        {

            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Method} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(Stream bodyStream)
        {
            bodyStream.Seek(0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader(bodyStream).ReadToEndAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);

            return plainBodyText;
        }

        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");

        }
    }
}
