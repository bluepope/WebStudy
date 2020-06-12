using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.WebMvc.Filters
{
    public class RequestFromBodyToFormDataFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public async void OnResourceExecuting(ResourceExecutingContext context)
        {
            var request = context.HttpContext.Request;

            var accept = request.GetTypedHeaders().Accept.FirstOrDefault(p => p.MediaType == "application/json");

            if (accept != null && context.HttpContext.Request.HasFormContentType == false)
            {
                var list = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
                var bodyObj = JsonConvert.DeserializeObject(await new StreamReader(request.Body).ReadToEndAsync());

                foreach (var item in ConvertFormData(bodyObj))
                {
                    list.Add(item.Key, item.Value);
                }

                context.HttpContext.Request.Body = new MemoryStream();
                context.HttpContext.Request.Form = new Microsoft.AspNetCore.Http.FormCollection(list);
                //filterContext.HttpContext.Items
            }

        }

        public List<KeyValuePair<string, string>> ConvertFormData(object obj, string prefix = "")
        {
            var list = new List<KeyValuePair<string, string>>();
            string name;

            if (obj is null)
                return list;

            if (obj.GetType() == typeof(JArray))
            {
                int idx = 0;
                foreach (var item in (obj as JArray).Children())
                {
                    foreach (JProperty prop in item)
                    {
                        if (string.IsNullOrWhiteSpace(prefix))
                        {
                            name = $"{prop.Name}[{idx}]";
                        }
                        else
                        {
                            name = $"{prefix}[{idx}][{prop.Name}]";
                        }

                        if (prop.Value.GetType() == typeof(JArray))
                        {
                            list.AddRange(ConvertFormData(prop.Value, name));
                        }
                        else
                        {
                            list.Add(new KeyValuePair<string, string>(name, prop.Value?.ToString()));
                        }

                    }
                    idx++;
                }
            }
            else
            {
                foreach (JProperty prop in (obj as JObject).Properties())
                {
                    if (string.IsNullOrWhiteSpace(prefix))
                    {
                        name = prop.Name;
                    }
                    else
                    {
                        name = $"{prefix}[{prop.Name}]";
                    }

                    if (prop.Value.GetType() == typeof(JArray))
                    {
                        list.AddRange(ConvertFormData(prop.Value, name));
                    }
                    else
                    {
                        list.Add(new KeyValuePair<string, string>(name, prop.Value?.ToString()));
                    }
                }
            }

            return list;
        }
    }
}
