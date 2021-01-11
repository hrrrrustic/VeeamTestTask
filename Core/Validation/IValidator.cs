namespace Core.Validation
{
    public interface IValidator<T>
    {
        void Validate(T source);
    }
}