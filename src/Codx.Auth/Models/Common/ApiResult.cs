using System;
using System.Collections.Generic;

namespace Codx.Auth.Models.Common
{
    /// <summary>
    /// Standard API result wrapper for ensuring consistent response structure
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResult<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResult(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
            StatusCode = 200;
            Errors = null;
        }

        public ApiResult(string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = default;
            StatusCode = 200;
            Errors = null;
        }

        public ApiResult(string errorMessage, int statusCode)
        {
            Succeeded = false;
            Message = errorMessage;
            StatusCode = statusCode;
            Errors = new List<string> { errorMessage };
        }

        public ApiResult(List<string> errors, int statusCode)
        {
            Succeeded = false;
            Errors = errors;
            StatusCode = statusCode;
        }

        public static ApiResult<T> Success(T data, string message = null)
        {
            return new ApiResult<T>(data, message);
        }

        public static ApiResult<T> Fail(string errorMessage, int statusCode = 400)
        {
            return new ApiResult<T>(errorMessage, statusCode);
        }

        public static ApiResult<T> Fail(List<string> errors, int statusCode = 400)
        {
            return new ApiResult<T>(errors, statusCode);
        }
    }
}
