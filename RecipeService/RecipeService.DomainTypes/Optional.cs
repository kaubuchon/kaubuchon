namespace RecipeService
{
    public class Optional<T>
    {
        T t;
        Optional()
        {

        }
        Optional(T tee)
        {
            t = tee;
        }
        #region statics
        /// <summary>
        /// Returns an empty Optional instance.
        /// </summary>
        /// <returns></returns>
        public static Optional<T> empty()
        {
            return new Optional<T>();
        }
        /// <summary>
        /// returns an Optional with the specified present non-null value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> of(T value)
        {
            if (value == null)
                throw new NullReferenceException();
            return new Optional<T>(value);
        }
        /// <summary>
        /// Returns an Optional describing the specified value, if non-null, otherwise returns an empty Optional.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> ofNullable(T value)
        {
            return new Optional<T>(value);
        }
        #endregion

        public Optional<U> map<U>(Func<T,U> mapper)
        {
            var u = mapper(t);
            return new Optional<U>(u);
        }
        
        public void ifPresent(Action<T> action)
        {
            if (t != null)
            {
                action(t);
            }
        }
        public T get()
        {
            return t;
        }
        public bool isPresent()
        {
            return t != null;
        }
    }
}
