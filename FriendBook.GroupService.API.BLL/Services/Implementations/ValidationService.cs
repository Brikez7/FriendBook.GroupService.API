using FluentValidation;
using FluentValidation.Results;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class ValidationService<T> : IValidationService<T>
    {
        private readonly IValidator<T> Validator;

        public ValidationService(IValidator<T> validator)
        {
            Validator = validator;
        }

        public async Task<BaseResponse<List<Tuple<string, string>>?>> ValidateAsync(T dto)
        {
            var validationResult = await Validator.ValidateAsync(dto);
            return GetErrors(validationResult);
        }
        public BaseResponse<List<Tuple<string, string>>?> Validate(T dto)
        {
            var validationResult = Validator.Validate(dto);
            return GetErrors(validationResult);
        }

        private static BaseResponse<List<Tuple<string, string>>?> GetErrors(ValidationResult validationResult)
        {
            var reponse = new StandardResponse<List<Tuple<string, string>>?>();
            if (!validationResult.IsValid)
            {
                reponse.ServiceCode = ServiceCode.EntityIsNotValidated;
                reponse.Message = $"Error validation: {validationResult.Errors.First().ErrorMessage}";
                reponse.Data = validationResult.Errors.Select(x => new Tuple<string, string>(x.PropertyName, x.ErrorMessage)).ToList();
                return reponse;
            }
            reponse.ServiceCode = ServiceCode.EntityIsValidated;
            return reponse;
        }
    }
}
