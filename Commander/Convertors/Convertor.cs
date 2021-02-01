using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Commander.Convertors
{
    public class Convertor : IConvertor<object>
    {
        private readonly Type _type;
        
        public Convertor() {}
        
        public Convertor(Type typ)
        {
            _type = typ;
        }
        
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
        
        // attempts to convert the string value to typ
        public virtual bool TryConvert(Type typ, string val, out object res)
        {
            return TryGenericConvert(typ, val, out res);
        }
        
        // attempts to convert the string value to T
        public virtual bool TryConvert(string val, out object res)
        {
            return TryGenericConvert(_type, val, out res);
        }
    }
}