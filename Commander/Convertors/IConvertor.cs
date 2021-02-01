using System;

namespace Commander.Convertors
{
    /// <summary>
    /// Convertor interface to specify the TryConvert methods.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    public interface IConvertor<T>
    {
        /// <summary>
        /// Attempts to convert the given string value to the given type value.
        /// </summary>
        /// <param name="typ">The type to convert to.</param>
        /// <param name="val">The string value to convert from.</param>
        /// <param name="res">The resulting value of the conversion.</param>
        /// <returns>A bool representing the success of the attempted conversion.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool TryConvert(Type typ, string val, out T res)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Attempts to convert the given string value to the generic type <code>T</code>.
        /// </summary>
        /// <param name="val">The string value to convert from.</param>
        /// <param name="res">The resulting value of the conversion.</param>
        /// <returns>A bool representing the success of the attempted conversion.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool TryConvert(string val, out T res)
        {
            throw new NotImplementedException();
        }
    }
}