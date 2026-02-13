public interface IBindable<T>
{
    void Bind(T vm);
    void Unbind();
}