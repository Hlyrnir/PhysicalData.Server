namespace PhysicalData.Api.Endpoint
{
    public static class EndpointRoute
    {
        public const string Exception = "/exception";

        private const string EndpointBase = "/api";

        internal static class PhysicalDimension
        {
            public const string Base = $"{EndpointBase}/physical_dimension";
            public const string Create = Base;
            public const string Delete = Base;
            public const string GetById = $"{Base}/{{guId:Guid}}";
            public const string GetUnspecific = Base;
            public const string Update = Base;
        }

        internal static class TimePeriod
        {
            public const string Base = $"{EndpointBase}/time_period";
            public const string Create = Base;
            public const string Delete = Base;
            public const string GetById = $"{Base}/{{guId:Guid}}";
            public const string GetUnspecific = Base;
            public const string Update = Base;
        }
    }
}
