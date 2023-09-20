using FluentValidation;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.BLL.Services;
using System.Reflection;

namespace FriendBook.GroupService.API
{
    public static class ReflectionInjector
    {
        public static void AddValidators(this IServiceCollection services)
        {
            var validatorsTypes = Assembly.Load(Assembly.GetExecutingAssembly().GetReferencedAssemblies().First(x => x.Name == "FriendBook.GroupService.API.BLL"))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

            foreach (var validatorType in validatorsTypes)
            {
                var validatorBase = validatorType.BaseType;

                services.AddTransient(typeof(IValidator<>).MakeGenericType(validatorBase!.GenericTypeArguments), validatorType);

                services.AddTransient(typeof(IValidationService<>).MakeGenericType(validatorBase!.GenericTypeArguments), 
                                      typeof(ValidationService<>).MakeGenericType(validatorBase!.GenericTypeArguments));
            }
        }
    }
}
