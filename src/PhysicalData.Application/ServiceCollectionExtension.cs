using Microsoft.Extensions.DependencyInjection;
using Passport.Application;
using PhysicalData.Application.Command.PhysicalDimension.Create;
using PhysicalData.Application.Command.PhysicalDimension.Delete;
using PhysicalData.Application.Command.PhysicalDimension.Update;
using PhysicalData.Application.Command.TimePeriod.Create;
using PhysicalData.Application.Command.TimePeriod.Delete;
using PhysicalData.Application.Command.TimePeriod.Update;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Query.PhysicalDimension.ByFilter;
using PhysicalData.Application.Query.PhysicalDimension.ById;
using PhysicalData.Application.Query.TimePeriod.ByFilter;
using PhysicalData.Application.Query.TimePeriod.ById;
using PhysicalData.Application.Validation;

namespace PhysicalData.Application
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServiceCollection(this IServiceCollection cltService)
        {
            // Add mediator
            cltService.AddMediator(
                mdtOptions =>
                {
                    mdtOptions.ServiceLifetime = ServiceLifetime.Scoped;
                    mdtOptions.GenerateTypesAsInternal = true;
                    //mdtOptions.Assemblies = [typeof(Passport.Application.ServiceCollectionExtension)];
                });

            cltService.AddScoped<IMessageValidation, MessageValidation>();

            #region PhysicalDimension - Command
            cltService.AddAuthorizationBehaviour<CreatePhysicalDimensionCommand, Guid, CreatePhysicalDimensionAuthorization>();
            cltService.AddValidationBehaviour<CreatePhysicalDimensionCommand, Guid, CreatePhysicalDimensionValidation>();

            cltService.AddAuthorizationBehaviour<DeletePhysicalDimensionCommand, bool, DeletePhysicalDimensionAuthorization>();
            cltService.AddValidationBehaviour<DeletePhysicalDimensionCommand, bool, DeletePhysicalDimensionValidation>();

            cltService.AddAuthorizationBehaviour<UpdatePhysicalDimensionCommand, bool, UpdatePhysicalDimensionAuthorization>();
            cltService.AddValidationBehaviour<UpdatePhysicalDimensionCommand, bool, UpdatePhysicalDimensionValidation>();
            #endregion

            #region PhysicalDimension - Query
            cltService.AddAuthorizationBehaviour<PhysicalDimensionByFilterQuery, PhysicalDimensionByFilterResult, PhysicalDimensionByFilterAuthorization>();
            cltService.AddValidationBehaviour<PhysicalDimensionByFilterQuery, PhysicalDimensionByFilterResult, PhysicalDimensionByFilterValidation>();

            cltService.AddAuthorizationBehaviour<PhysicalDimensionByIdQuery, PhysicalDimensionByIdResult, PhysicalDimensionByIdAuthorization>();
            cltService.AddValidationBehaviour<PhysicalDimensionByIdQuery, PhysicalDimensionByIdResult, PhysicalDimensionByIdValidation>();
            #endregion

            #region TimePeriod - Command
            cltService.AddAuthorizationBehaviour<CreateTimePeriodCommand, Guid, CreateTimePeriodAuthorization>();
            cltService.AddValidationBehaviour<CreateTimePeriodCommand, Guid, CreateTimePeriodValidation>();

            cltService.AddAuthorizationBehaviour<DeleteTimePeriodCommand, bool, DeleteTimePeriodAuthorization>();
            cltService.AddValidationBehaviour<DeleteTimePeriodCommand, bool, DeleteTimePeriodValidation>();

            cltService.AddAuthorizationBehaviour<UpdateTimePeriodCommand, bool, UpdateTimePeriodAuthorization>();
            cltService.AddValidationBehaviour<UpdateTimePeriodCommand, bool, UpdateTimePeriodValidation>();
            #endregion

            #region TimePeriod - Query
            cltService.AddAuthorizationBehaviour<TimePeriodByFilterQuery, TimePeriodByFilterResult, TimePeriodByFilterAuthorization>();
            cltService.AddValidationBehaviour<TimePeriodByFilterQuery, TimePeriodByFilterResult, TimePeriodByFilterValidation>();

            cltService.AddAuthorizationBehaviour<TimePeriodByIdQuery, TimePeriodByIdResult, TimePeriodByIdAuthorization>();
            cltService.AddValidationBehaviour<TimePeriodByIdQuery, TimePeriodByIdResult, TimePeriodByIdValidation>();
            #endregion

            return cltService;
        }
    }
}
