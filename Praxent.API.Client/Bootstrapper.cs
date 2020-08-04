using Autofac;

namespace Praxent.API.Client
{
    public static class Bootstrapper
    {
        public static ContainerBuilder AddAPIClient(this ContainerBuilder builder)
        {
            builder.RegisterType<URIBuilder>().As<IURIBuilder>();
            builder.RegisterType<RESTAPIClient>().As<IRESTAPIClient>();

            return builder;
        }
    }
}