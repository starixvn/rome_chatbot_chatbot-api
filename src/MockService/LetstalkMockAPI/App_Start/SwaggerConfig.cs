using System.Web.Http;
using LetstalkMockAPI;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace LetstalkMockAPI
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "LetstalkMockAPI");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\LetstalkMockAPI.XML",
                           System.AppDomain.CurrentDomain.BaseDirectory));
                })
                .EnableSwaggerUi(c => c.DocExpansion(DocExpansion.List));
        }
    }
}
