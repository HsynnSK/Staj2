using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BOMManagement.Components.Shared;

/// <summary>
/// Blazor EditForm ile FluentValidation entegrasyonunu sağlayan custom validator bileşeni.
/// EditContext üzerinden OnValidationRequested event'ini dinleyerek ilgili AbstractValidator'ü DI'dan alır.
/// </summary>
public class FluentValidationValidator : ComponentBase, IDisposable
{
    [Inject] private IServiceProvider ServiceProvider { get; set; } = null!;

    [CascadingParameter] private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(FluentValidationValidator)} requires a cascading parameter of type {nameof(EditContext)}.");
        }

        CurrentEditContext.OnValidationRequested += HandleValidationRequested;
        CurrentEditContext.OnFieldChanged += HandleFieldChanged;
    }

    private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
    {
        if (CurrentEditContext is null) return;

        var validationMessageStore = new ValidationMessageStore(CurrentEditContext);
        validationMessageStore.Clear();

        var model = CurrentEditContext.Model;
        var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
        var validator = ServiceProvider.GetService(validatorType) as IValidator;

        if (validator is null) return;

        var context = new ValidationContext<object>(model);
        var result = validator.Validate(context);

        foreach (var error in result.Errors)
        {
            var fieldIdentifier = new FieldIdentifier(model, error.PropertyName);
            validationMessageStore.Add(fieldIdentifier, error.ErrorMessage);
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs args)
    {
        if (CurrentEditContext is null) return;

        var validationMessageStore = new ValidationMessageStore(CurrentEditContext);
        validationMessageStore.Clear(args.FieldIdentifier);

        var model = CurrentEditContext.Model;
        var validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
        var validator = ServiceProvider.GetService(validatorType) as IValidator;

        if (validator is null) return;

        var context = new ValidationContext<object>(model);
        var result = validator.Validate(context);

        foreach (var error in result.Errors.Where(e => e.PropertyName == args.FieldIdentifier.FieldName))
        {
            validationMessageStore.Add(args.FieldIdentifier, error.ErrorMessage);
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    public void Dispose()
    {
        if (CurrentEditContext is not null)
        {
            CurrentEditContext.OnValidationRequested -= HandleValidationRequested;
            CurrentEditContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}
