using System;

namespace Commander.Convertors
{
    public interface IConvertor<T>
    {
        public virtual bool TryConvert(Type typ, string val, out T res)
        {
            throw new NotImplementedException();
        }
        
        public virtual bool TryConvert(string val, out T res)
        {
            throw new NotImplementedException();
        }
    }
}