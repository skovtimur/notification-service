namespace NotificationService.Domain.Validators;

public interface IValidator<ValiableType>
{
    public bool Validate(ValiableType telegramIdString);
}

public interface IValidatorAsync<ValiableType>
{
    public Task<bool> ValidateAsync(ValiableType value);
}