using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Commander.Convertors
{
    /// <summary>
    /// A generic implementation of the <code>IConvertor</code> interface.
    /// </summary>
    /// <seealso cref="IConvertor{T}"/>
    public class Convertor : IConvertor<object>
    {
        private readonly Type _type;
        
        public Convertor() {}
        
        public Convertor(Type typ)
        {
            _type = typ;
        }
        
        /// <summary>
        /// Generic convertor that can handle all primitive types.
        /// </summary>
        /// <param name="typ">The type to convert to.</param>
        /// <param name="val">The string value to convert from.</param>
        /// <param name="res">The resulting value of the conversion.</param>
        /// <returns>A bool representing the success of the attempted conversion.</returns>
        /// <see cref="TryConvert(System.Type,string,out object)"/>
        protected static bool TryGenericConvert(Type typ, string val, out object res)
        {
            try
            {
                var convertor = TypeDescriptor.GetConverter(typ);
                if (convertor != null)
                {
                    res = convertor.ConvertFromString(val);
                    return true;
                }

                res = null;
                return false;
            }
            catch (Exception e)
            {
                res = null;
                return false;
            }
        }
        
        /// <summary>
        /// Attempts to convert the given string value to the given type value.
        /// </summary>
        /// <seealso cref="IConvertor{T}.TryConvert(System.Type,string,out T)"/>
        public virtual bool TryConvert(Type typ, string val, out object res)
        {
            return TryGenericConvert(typ, val, out res);
        }
        
        /// <summary>
        /// Attempts to convert the given string value to the generic type <code>T</code> which is of type <code>object</code> in  this class.
        /// </summary>
        /// <seealso cref="IConvertor{T}.TryConvert(string,out T)"/>
        public virtual bool TryConvert(string val, out object res)
        {
            return TryGenericConvert(_type, val, out res);
        }
    }
}