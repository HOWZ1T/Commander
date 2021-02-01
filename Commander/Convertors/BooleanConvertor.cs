using System;

namespace Commander.Convertors
{
    public class BooleanConvertor: Convertor
    {
        public override bool TryConvert(string val, out object res)
        {
            var result = base.TryConvert(typeof(bool), val, out res);
            if (result) return true;

            switch (val.ToLower().Trim())
            {
                case "yes":
                case "y":
                case "t":
                case "true":
                    res = true;
                    return true;
                
                case "no": 
                case "n":
                case "f":
                case "false":
                    res = false;
                    return true;
                
                default:
                    res = null;
                    return false;
            }
        }
    }
}