namespace Commander.Convertors
{
    /// <summary>
    ///     An extension of the Convertor class to handle converting bool values.
    /// </summary>
    /// <see cref="Convertor" />
    public class BooleanConvertor : Convertor
    {
        /// <summary>
        ///     Attempts to convert the given string value to a bool.
        /// </summary>
        /// <seealso cref="Convertor.TryConvert(string,out object)" />
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