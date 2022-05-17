using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;

namespace Application.ViewModels
{
    public class ApiResponse
    {
        public string MessageCode { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public int? Total { get; set; }

        public ApiResponse()
        {
        }

        #region Unauthorized

        public static ObjectResult Unauthorized() => new(new ApiResponse()) { StatusCode = StatusCodes.Status401Unauthorized };

        #endregion

        #region OK

        public static ObjectResult Ok() => new(new ApiResponse()) { StatusCode = StatusCodes.Status200OK };

        public static ObjectResult Ok(object data, int? total = null)
        {
            if (data is ICollection d && total == null) total = d.Count;

            return new ObjectResult(new ApiResponse { Data = data, Total = total })
            { StatusCode = StatusCodes.Status200OK };
        }

        #endregion

        #region NotFound

        public static ObjectResult NotFound() => new(new ApiResponse()) { StatusCode = StatusCodes.Status404NotFound };

        public static ObjectResult NotFound(ResponseMessage message) =>
            new(new ApiResponse { MessageCode = message.Code, Message = message.Value })
            { StatusCode = StatusCodes.Status404NotFound };

        #endregion

        #region BadRequest

        public static ObjectResult BadRequest() => new(new ApiResponse()) { StatusCode = StatusCodes.Status400BadRequest };

        public static ObjectResult BadRequest(ResponseMessage message) =>
            new(new ApiResponse { MessageCode = message.Code, Message = message.Value })
            { StatusCode = StatusCodes.Status400BadRequest };

        public static ObjectResult BadRequest(ResponseMessage message, object data) =>
            new(new ApiResponse { MessageCode = message.Code, Message = message.Value, Data = data })
            { StatusCode = StatusCodes.Status400BadRequest };

        #endregion

        #region Forbidden

        public static ObjectResult Forbidden() => new(new ApiResponse()) { StatusCode = StatusCodes.Status403Forbidden };

        public static ObjectResult Forbidden(ResponseMessage message) =>
            new(new ApiResponse { MessageCode = message.Code, Message = message.Value })
            { StatusCode = StatusCodes.Status403Forbidden };

        #endregion

        #region InternalServerError

        public static ObjectResult InternalServerError() => new(new ApiResponse()) { StatusCode = StatusCodes.Status500InternalServerError };

        public static ObjectResult InternalServerError(ResponseMessage message) =>
            new(new ApiResponse {MessageCode = message.Code, Message = message.Value})
            {StatusCode = StatusCodes.Status500InternalServerError};

        #endregion
    }
}