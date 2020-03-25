using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.WebMvc.Common
{
    /// <summary>
    /// This input formatter bypasses the <see cref="BodyModelBinder"/> by returning a null result, when the request has a form content type.
    /// When registered, both <see cref="FromBodyAttribute"/> and <see cref="FromFormAttribute"/> can be used in the same method.
    /// </summary>
    public class BypassFormDataInputFormatter : IInputFormatter
    {
        public bool CanRead(InputFormatterContext context)
        {
            return context.HttpContext.Request.HasFormContentType;
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            return InputFormatterResult.SuccessAsync(null);
        }
    }
}
